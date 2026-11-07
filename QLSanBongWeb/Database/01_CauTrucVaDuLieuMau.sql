USE master;
GO
IF DB_ID(N'QLSanBong') IS NULL
    CREATE DATABASE QLSanBong;
go

USE [QLSanBong]
GO
/****** Object:  Table [dbo].[tblDatSan]    Script Date: 24/10/26 16:21:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tblDatSan](
	[MaDatSan] [varchar](50) NOT NULL,
	[MaSan] [varchar](50) NOT NULL,
	[MaKH] [varchar](50) NOT NULL,
	[GioBatDau] [datetime2](0) NOT NULL,
	[GioKetThuc] [datetime2](0) NOT NULL,
	[DonGiaApDung] [money] NOT NULL,
	[TrangThaiDat] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_tblDatSan] PRIMARY KEY CLUSTERED 
(
	[MaDatSan] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tblHoaDon]    Script Date: 24/10/26 16:21:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tblHoaDon](
	[MaHD] [varchar](50) NOT NULL,
	[MaDatSan] [varchar](50) NOT NULL,
	[NgayLap] [datetime2](0) NOT NULL,
	[TongTien] [money] NOT NULL,
	[TrangThaiThanhToan] [bit] NOT NULL,
	[ThoiDiemThanhToan] [datetime2](0) NULL,
 CONSTRAINT [PK_tblHoaDon] PRIMARY KEY CLUSTERED 
(
	[MaHD] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tblKhachHang]    Script Date: 24/10/26 16:21:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tblKhachHang](
	[MaKH] [varchar](50) NOT NULL,
	[TenKH] [nvarchar](50) NOT NULL,
	[SoDT] [varchar](50) NOT NULL,
 CONSTRAINT [PK_tblKhachHang] PRIMARY KEY CLUSTERED 
(
	[MaKH] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tblSan]    Script Date: 24/10/26 16:21:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tblSan](
	[MaSan] [varchar](50) NOT NULL,
	[TenSan] [nvarchar](50) NOT NULL,
	[LoaiSan] [nvarchar](50) NOT NULL,
	[GiaThueGio] [money] NOT NULL,
	[TrangThaiHoatDong] [bit] NOT NULL,
 CONSTRAINT [PK_tblSan] PRIMARY KEY CLUSTERED 
(
	[MaSan] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tblTaiKhoan]    Script Date: 24/10/26 16:21:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tblTaiKhoan](
	[MaTK] [varchar](50) NOT NULL,
	[TenDangNhap] [nvarchar](50) NOT NULL,
	[MatKhauHash] [varchar](512) NOT NULL,
	[VaiTro] [nvarchar](50) NOT NULL,
	[TrangThaiHoatDong] [bit] NOT NULL,
 CONSTRAINT [PK_tblTaiKhoan] PRIMARY KEY CLUSTERED 
(
	[MaTK] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
INSERT [dbo].[tblDatSan] ([MaDatSan], [MaSan], [MaKH], [GioBatDau], [GioKetThuc], [DonGiaApDung], [TrangThaiDat]) VALUES (N'DS001', N'S001', N'KH001', CAST(N'2026-10-15T17:00:00.0000000' AS DateTime2), CAST(N'2026-10-15T18:30:00.0000000' AS DateTime2), 200000.0000, N'Hoàn thành')
INSERT [dbo].[tblDatSan] ([MaDatSan], [MaSan], [MaKH], [GioBatDau], [GioKetThuc], [DonGiaApDung], [TrangThaiDat]) VALUES (N'DS002', N'S002', N'KH002', CAST(N'2026-10-15T18:00:00.0000000' AS DateTime2), CAST(N'2026-10-15T19:00:00.0000000' AS DateTime2), 220000.0000, N'Hoàn thành')
INSERT [dbo].[tblDatSan] ([MaDatSan], [MaSan], [MaKH], [GioBatDau], [GioKetThuc], [DonGiaApDung], [TrangThaiDat]) VALUES (N'DS003', N'S003', N'KH003', CAST(N'2026-10-15T19:00:00.0000000' AS DateTime2), CAST(N'2026-10-15T20:30:00.0000000' AS DateTime2), 300000.0000, N'Hoàn thành')
INSERT [dbo].[tblDatSan] ([MaDatSan], [MaSan], [MaKH], [GioBatDau], [GioKetThuc], [DonGiaApDung], [TrangThaiDat]) VALUES (N'DS004', N'S001', N'KH004', CAST(N'2026-10-16T17:00:00.0000000' AS DateTime2), CAST(N'2026-10-16T18:00:00.0000000' AS DateTime2), 200000.0000, N'Đã đặt')
INSERT [dbo].[tblDatSan] ([MaDatSan], [MaSan], [MaKH], [GioBatDau], [GioKetThuc], [DonGiaApDung], [TrangThaiDat]) VALUES (N'DS005', N'S001', N'KH002', CAST(N'2026-10-16T18:00:00.0000000' AS DateTime2), CAST(N'2026-10-16T19:30:00.0000000' AS DateTime2), 200000.0000, N'Đã đặt')
INSERT [dbo].[tblDatSan] ([MaDatSan], [MaSan], [MaKH], [GioBatDau], [GioKetThuc], [DonGiaApDung], [TrangThaiDat]) VALUES (N'DS006', N'S002', N'KH003', CAST(N'2026-10-16T18:00:00.0000000' AS DateTime2), CAST(N'2026-10-16T19:30:00.0000000' AS DateTime2), 220000.0000, N'Đã đặt')
INSERT [dbo].[tblDatSan] ([MaDatSan], [MaSan], [MaKH], [GioBatDau], [GioKetThuc], [DonGiaApDung], [TrangThaiDat]) VALUES (N'DS007', N'S003', N'KH001', CAST(N'2026-10-16T19:00:00.0000000' AS DateTime2), CAST(N'2026-10-16T20:00:00.0000000' AS DateTime2), 300000.0000, N'Đã hủy')
INSERT [dbo].[tblDatSan] ([MaDatSan], [MaSan], [MaKH], [GioBatDau], [GioKetThuc], [DonGiaApDung], [TrangThaiDat]) VALUES (N'DS008', N'S003', N'KH004', CAST(N'2026-10-16T19:00:00.0000000' AS DateTime2), CAST(N'2026-10-16T20:00:00.0000000' AS DateTime2), 300000.0000, N'Đã đặt')
GO
INSERT [dbo].[tblHoaDon] ([MaHD], [MaDatSan], [NgayLap], [TongTien], [TrangThaiThanhToan], [ThoiDiemThanhToan]) VALUES (N'HD001', N'DS001', CAST(N'2026-10-15T18:30:00.0000000' AS DateTime2), 300000.0000, 1, CAST(N'2026-10-15T18:30:00.0000000' AS DateTime2))
INSERT [dbo].[tblHoaDon] ([MaHD], [MaDatSan], [NgayLap], [TongTien], [TrangThaiThanhToan], [ThoiDiemThanhToan]) VALUES (N'HD002', N'DS002', CAST(N'2026-10-15T19:00:00.0000000' AS DateTime2), 220000.0000, 1, CAST(N'2026-10-15T19:00:00.0000000' AS DateTime2))
INSERT [dbo].[tblHoaDon] ([MaHD], [MaDatSan], [NgayLap], [TongTien], [TrangThaiThanhToan], [ThoiDiemThanhToan]) VALUES (N'HD003', N'DS003', CAST(N'2026-10-15T20:30:00.0000000' AS DateTime2), 450000.0000, 1, CAST(N'2026-10-15T20:30:00.0000000' AS DateTime2))
INSERT [dbo].[tblHoaDon] ([MaHD], [MaDatSan], [NgayLap], [TongTien], [TrangThaiThanhToan], [ThoiDiemThanhToan]) VALUES (N'HD004', N'DS004', CAST(N'2026-10-16T10:00:00.0000000' AS DateTime2), 200000.0000, 0, NULL)
INSERT [dbo].[tblHoaDon] ([MaHD], [MaDatSan], [NgayLap], [TongTien], [TrangThaiThanhToan], [ThoiDiemThanhToan]) VALUES (N'HD005', N'DS005', CAST(N'2026-10-16T10:15:00.0000000' AS DateTime2), 300000.0000, 0, NULL)
GO
INSERT [dbo].[tblKhachHang] ([MaKH], [TenKH], [SoDT]) VALUES (N'KH001', N'Nguyễn Minh', N'0901234567')
INSERT [dbo].[tblKhachHang] ([MaKH], [TenKH], [SoDT]) VALUES (N'KH002', N'Trần Hoàng', N'0912345678')
INSERT [dbo].[tblKhachHang] ([MaKH], [TenKH], [SoDT]) VALUES (N'KH003', N'Lê Tuấn', N'0923456789')
INSERT [dbo].[tblKhachHang] ([MaKH], [TenKH], [SoDT]) VALUES (N'KH004', N'Phạm Nam', N'0934567890')
GO
INSERT [dbo].[tblSan] ([MaSan], [TenSan], [LoaiSan], [GiaThueGio], [TrangThaiHoatDong]) VALUES (N'S001', N'Sân 1', N'Sân 5 người', 200000.0000, 1)
INSERT [dbo].[tblSan] ([MaSan], [TenSan], [LoaiSan], [GiaThueGio], [TrangThaiHoatDong]) VALUES (N'S002', N'Sân 2', N'Sân 5 người', 220000.0000, 1)
INSERT [dbo].[tblSan] ([MaSan], [TenSan], [LoaiSan], [GiaThueGio], [TrangThaiHoatDong]) VALUES (N'S003', N'Sân 3', N'Sân 7 người', 300000.0000, 1)
INSERT [dbo].[tblSan] ([MaSan], [TenSan], [LoaiSan], [GiaThueGio], [TrangThaiHoatDong]) VALUES (N'S004', N'Sân 4', N'Sân 7 người', 300000.0000, 0)
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [uq_tblHoaDon_MaDatSan]    Script Date: 24/10/26 16:21:18 ******/
ALTER TABLE [dbo].[tblHoaDon] ADD  CONSTRAINT [uq_tblHoaDon_MaDatSan] UNIQUE NONCLUSTERED 
(
	[MaDatSan] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [uq_tblTaiKhoan_TenDangNhap]    Script Date: 24/10/26 16:21:18 ******/
ALTER TABLE [dbo].[tblTaiKhoan] ADD  CONSTRAINT [uq_tblTaiKhoan_TenDangNhap] UNIQUE NONCLUSTERED 
(
	[TenDangNhap] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
ALTER TABLE [dbo].[tblDatSan]  WITH CHECK ADD  CONSTRAINT [FK_tblDatSan_tblKhachHang] FOREIGN KEY([MaKH])
REFERENCES [dbo].[tblKhachHang] ([MaKH])
GO
ALTER TABLE [dbo].[tblDatSan] CHECK CONSTRAINT [FK_tblDatSan_tblKhachHang]
GO
ALTER TABLE [dbo].[tblDatSan]  WITH CHECK ADD  CONSTRAINT [FK_tblDatSan_tblSan] FOREIGN KEY([MaSan])
REFERENCES [dbo].[tblSan] ([MaSan])
GO
ALTER TABLE [dbo].[tblDatSan] CHECK CONSTRAINT [FK_tblDatSan_tblSan]
GO
ALTER TABLE [dbo].[tblHoaDon]  WITH CHECK ADD  CONSTRAINT [FK_tblHoaDon_tblDatSan] FOREIGN KEY([MaDatSan])
REFERENCES [dbo].[tblDatSan] ([MaDatSan])
GO
ALTER TABLE [dbo].[tblHoaDon] CHECK CONSTRAINT [FK_tblHoaDon_tblDatSan]
GO
ALTER TABLE [dbo].[tblDatSan]  WITH CHECK ADD  CONSTRAINT [ck_tblDatSan_ThoiGian] CHECK  (([GioKetThuc]>[GioBatDau]))
GO
ALTER TABLE [dbo].[tblDatSan] CHECK CONSTRAINT [ck_tblDatSan_ThoiGian]
GO
ALTER TABLE [dbo].[tblDatSan]  WITH CHECK ADD  CONSTRAINT [ck_tblDatSan_TrangThaiDat] CHECK  (([TrangThaiDat]=N'Hoàn thành' OR [TrangThaiDat]=N'Đã hủy' OR [TrangThaiDat]=N'Đã đặt'))
GO
ALTER TABLE [dbo].[tblDatSan] CHECK CONSTRAINT [ck_tblDatSan_TrangThaiDat]
GO
ALTER TABLE [dbo].[tblDatSan]  WITH CHECK ADD  CONSTRAINT [ck_tblHoaDon_DonGiaApDung] CHECK  (([DonGiaApDung]>(0)))
GO
ALTER TABLE [dbo].[tblDatSan] CHECK CONSTRAINT [ck_tblHoaDon_DonGiaApDung]
GO
ALTER TABLE [dbo].[tblHoaDon]  WITH CHECK ADD  CONSTRAINT [ck_tblHoaDon_ThanhToan] CHECK  (([TrangThaiThanhToan]=(0) AND [ThoiDiemThanhToan] IS NULL OR [TrangThaiThanhToan]=(1) AND [ThoiDiemThanhToan] IS NOT NULL))
GO
ALTER TABLE [dbo].[tblHoaDon] CHECK CONSTRAINT [ck_tblHoaDon_ThanhToan]
GO
ALTER TABLE [dbo].[tblHoaDon]  WITH CHECK ADD  CONSTRAINT [ck_tblHoaDon_TongTien] CHECK  (([TongTien]>=(0)))
GO
ALTER TABLE [dbo].[tblHoaDon] CHECK CONSTRAINT [ck_tblHoaDon_TongTien]
GO
ALTER TABLE [dbo].[tblSan]  WITH CHECK ADD  CONSTRAINT [ck_tblSan_GiaThueGio] CHECK  (([GiaThueGio]>(0)))
GO
ALTER TABLE [dbo].[tblSan] CHECK CONSTRAINT [ck_tblSan_GiaThueGio]
GO
