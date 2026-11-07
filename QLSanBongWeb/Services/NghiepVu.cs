namespace QLSanBongWeb.Services;

public static class NghiepVu
{
    public static bool GiaoNhau(DateTime bdMoi, DateTime ktMoi, DateTime bdCu, DateTime ktCu)
        => bdMoi < ktCu && ktMoi > bdCu;
    public static decimal TinhTien(DateTime batDau, DateTime ketThuc, decimal donGia)
    {
        if (ketThuc <= batDau || donGia <= 0) throw new ArgumentException("Thời gian hoặc đơn giá không hợp lệ.");
        decimal soGio = (decimal)(ketThuc.Ticks - batDau.Ticks) / TimeSpan.TicksPerHour;
        return Math.Round(soGio * donGia, 0, MidpointRounding.AwayFromZero);
    }
}
