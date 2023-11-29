using System;
using System.Collections.Generic;

namespace TechStore.Models
{
    public partial class Cart
    {
        public int Id { get; set; }
        public int? KhachHangId { get; set; }
        public int? SanPhamId { get; set; }
        public string? TenSanPham { get; set; }
        public string? AnhSanPham { get; set; }
        public int? SoLuong { get; set; }
        public decimal? GiaBan { get; set; }

        public virtual KhachHang? KhachHang { get; set; }
        public virtual SanPham? SanPham { get; set; }
    }
}
