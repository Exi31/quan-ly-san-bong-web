USE QLSanBong;
GO
-- Chỉ thêm tài khoản còn thiếu, không thay đổi mật khẩu đã có.
IF NOT EXISTS (SELECT 1 FROM dbo.tblTaiKhoan WHERE TenDangNhap=N'admin_demo')
    INSERT dbo.tblTaiKhoan(MaTK,TenDangNhap,MatKhauHash,VaiTro,TrangThaiHoatDong)
    VALUES ('TK_DEMO_QL',N'admin_demo','PBKDF2-SHA256$600000$MyOqIjoEb9mD9D1P0/Oj6w==$0uCbp7mypHMawlaL8rBWjm9u9rjRPR+o1QBA+UfgVow=',N'Quản lý',1);
IF NOT EXISTS (SELECT 1 FROM dbo.tblTaiKhoan WHERE TenDangNhap=N'nhanvien_demo')
    INSERT dbo.tblTaiKhoan(MaTK,TenDangNhap,MatKhauHash,VaiTro,TrangThaiHoatDong)
    VALUES ('TK_DEMO_NV',N'nhanvien_demo','PBKDF2-SHA256$600000$gPsdvqx76YTDmQ7wVmAr3A==$wvmh6nEgfHoIbcR7GBcRQBG5uKlr6ngsglEB4XOFwc8=',N'Nhân viên',1);
GO
