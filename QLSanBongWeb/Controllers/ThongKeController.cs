using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QLSanBongMini.Models;
using Microsoft.Data.SqlClient;

namespace QLSanBongMini.Controllers
{
    [Authorize(Roles = "Quản lý")]
    public class ThongKeController : Controller
    {
        private readonly IConfiguration _configuration;

        public ThongKeController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IActionResult Index(DateTime? tuNgay, DateTime? denNgay)
        {
            // Mặc định từ đầu tháng đến hôm nay.
            DateTime fromDate = (tuNgay ?? new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1)).Date;
            DateTime toDate = (denNgay ?? DateTime.Today).Date;

            // Dùng cận trên là đầu ngày tiếp theo.
            if (fromDate > toDate || toDate >= DateTime.MaxValue.Date)
            {
                ViewBag.TuNgay = fromDate.ToString("yyyy-MM-dd");
                ViewBag.DenNgay = toDate.ToString("yyyy-MM-dd");
                ViewBag.TongDoanhThu = 0m; ViewBag.SoHoaDon = 0;
                ModelState.AddModelError("", "Khoảng ngày thống kê không hợp lệ.");
                return View(new List<ThongKeModel>());
            }
            DateTime queryToDate = toDate.Date.AddDays(1);

            List<ThongKeModel> listHoaDon = new List<ThongKeModel>();
            decimal tongDoanhThu = 0;
            string connectionString = _configuration.GetConnectionString("DefaultConnection")!;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = @"
                    SELECT hd.MaHD, 
                           hd.ThoiDiemThanhToan, 
                           s.TenSan, 
                           hd.TongTien 
                    FROM tblHoaDon hd
                    JOIN tblDatSan ds ON hd.MaDatSan = ds.MaDatSan
                    JOIN tblSan s ON ds.MaSan = s.MaSan
                    WHERE hd.TrangThaiThanhToan = 1 
                      AND hd.ThoiDiemThanhToan >= @TuNgay 
                      AND hd.ThoiDiemThanhToan < @DenNgay
                    ORDER BY hd.ThoiDiemThanhToan DESC";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@TuNgay", fromDate.Date);
                    cmd.Parameters.AddWithValue("@DenNgay", queryToDate);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var item = new ThongKeModel
                            {
                                MaHD = reader["MaHD"].ToString()!,
                                NgayThanhToan = Convert.ToDateTime(reader["ThoiDiemThanhToan"]),
                                TenSan = reader["TenSan"].ToString()!,
                                TongTien = Convert.ToDecimal(reader["TongTien"])
                            };
                            listHoaDon.Add(item);
                            tongDoanhThu += item.TongTien;
                        }
                    }
                }
            }

            ViewBag.TuNgay = fromDate.ToString("yyyy-MM-dd");
            ViewBag.DenNgay = toDate.ToString("yyyy-MM-dd");
            ViewBag.TongDoanhThu = tongDoanhThu;
            ViewBag.SoHoaDon = listHoaDon.Count;

            return View(listHoaDon);
        }
    }
}