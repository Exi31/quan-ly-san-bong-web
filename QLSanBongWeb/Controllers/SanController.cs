using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QLSanBongMini.Models;
using Microsoft.Data.SqlClient;

namespace QLSanBongMini.Controllers
{
    public class SanController : Controller
    {
        private readonly IConfiguration _configuration;

        public SanController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // GET: San/Index
        public IActionResult Index(string? keyword)
        {
            List<SanModel> listSan = new List<SanModel>();
            string connectionString = _configuration.GetConnectionString("DefaultConnection")!;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string sql = "SELECT MaSan, TenSan, LoaiSan, GiaThueGio, TrangThaiHoatDong FROM tblSan";
                if (!string.IsNullOrEmpty(keyword))
                {
                    sql += " WHERE MaSan LIKE @kw OR TenSan LIKE @kw";
                }

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    if (!string.IsNullOrEmpty(keyword))
                    {
                        cmd.Parameters.AddWithValue("@kw", "%" + keyword + "%");
                    }

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            listSan.Add(new SanModel
                            {
                                MaSan = reader["MaSan"].ToString()!,
                                TenSan = reader["TenSan"].ToString()!,
                                LoaiSan = reader["LoaiSan"].ToString()!,
                                GiaThueGio = Convert.ToDecimal(reader["GiaThueGio"]),
                                TrangThaiHoatDong = Convert.ToBoolean(reader["TrangThaiHoatDong"])
                            });
                        }
                    }
                }
            }
            ViewData["Keyword"] = keyword;
            return View(listSan);
        }

        // GET: San/Create
        [Authorize(Roles = "Quản lý")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: San/Create
        [HttpPost, Authorize(Roles = "Quản lý")]
        public IActionResult Create(SanModel model)
        {
            model.MaSan = model.MaSan?.Trim() ?? "";
            model.TenSan = model.TenSan?.Trim() ?? "";
            ModelState.Clear();
            TryValidateModel(model);
            if (ModelState.IsValid)
            {
                string connectionString = _configuration.GetConnectionString("DefaultConnection")!;
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    // Kiểm tra trùng khóa chính MaSan
                    string checkSql = "SELECT COUNT(*) FROM tblSan WHERE MaSan = @MaSan";
                    using (SqlCommand cmdCheck = new SqlCommand(checkSql, conn))
                    {
                        cmdCheck.Parameters.AddWithValue("@MaSan", model.MaSan);
                        if ((int)cmdCheck.ExecuteScalar() > 0)
                        {
                            ModelState.AddModelError("MaSan", "Mã sân này đã tồn tại trong hệ thống!");
                            return View(model);
                        }
                    }

                    string insertSql = "INSERT INTO tblSan (MaSan, TenSan, LoaiSan, GiaThueGio, TrangThaiHoatDong) VALUES (@MaSan, @TenSan, @LoaiSan, @GiaThueGio, @TrangThai)";
                    using (SqlCommand cmd = new SqlCommand(insertSql, conn))
                    {
                        cmd.Parameters.AddWithValue("@MaSan", model.MaSan);
                        cmd.Parameters.AddWithValue("@TenSan", model.TenSan);
                        cmd.Parameters.AddWithValue("@LoaiSan", model.LoaiSan);
                        cmd.Parameters.AddWithValue("@GiaThueGio", model.GiaThueGio);
                        cmd.Parameters.AddWithValue("@TrangThai", model.TrangThaiHoatDong);
                        try { cmd.ExecuteNonQuery(); }
                        catch (SqlException ex) when (ex.Number is 2627 or 2601)
                        {
                            ModelState.AddModelError("MaSan", "Mã sân này vừa được tạo bởi yêu cầu khác. Vui lòng chọn mã khác.");
                            return View(model);
                        }
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        // GET: San/Edit/S001
        [Authorize(Roles = "Quản lý")]
        public IActionResult Edit(string id)
        {
            if (string.IsNullOrEmpty(id)) return NotFound();

            SanModel? san = null;
            string connectionString = _configuration.GetConnectionString("DefaultConnection")!;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string sql = "SELECT MaSan, TenSan, LoaiSan, GiaThueGio, TrangThaiHoatDong FROM tblSan WHERE MaSan = @MaSan";
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@MaSan", id);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            san = new SanModel
                            {
                                MaSan = reader["MaSan"].ToString()!,
                                TenSan = reader["TenSan"].ToString()!,
                                LoaiSan = reader["LoaiSan"].ToString()!,
                                GiaThueGio = Convert.ToDecimal(reader["GiaThueGio"]),
                                TrangThaiHoatDong = Convert.ToBoolean(reader["TrangThaiHoatDong"])
                            };
                        }
                    }
                }
            }
            if (san == null) return NotFound();
            return View(san);
        }

        // POST: San/Edit
        [HttpPost, Authorize(Roles = "Quản lý")]
        public IActionResult Edit(SanModel model)
        {
            model.MaSan = model.MaSan?.Trim() ?? "";
            model.TenSan = model.TenSan?.Trim() ?? "";
            ModelState.Clear();
            TryValidateModel(model);
            if (ModelState.IsValid)
            {
                string connectionString = _configuration.GetConnectionString("DefaultConnection")!;
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string updateSql = "UPDATE tblSan SET TenSan = @TenSan, LoaiSan = @LoaiSan, GiaThueGio = @GiaThueGio, TrangThaiHoatDong = @TrangThai WHERE MaSan = @MaSan";
                    using (SqlCommand cmd = new SqlCommand(updateSql, conn))
                    {
                        cmd.Parameters.AddWithValue("@MaSan", model.MaSan);
                        cmd.Parameters.AddWithValue("@TenSan", model.TenSan);
                        cmd.Parameters.AddWithValue("@LoaiSan", model.LoaiSan);
                        cmd.Parameters.AddWithValue("@GiaThueGio", model.GiaThueGio);
                        cmd.Parameters.AddWithValue("@TrangThai", model.TrangThaiHoatDong);
                        if (cmd.ExecuteNonQuery() == 0) return NotFound();
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }
    }
}