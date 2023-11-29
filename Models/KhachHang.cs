using System;
using System.Collections.Generic;

namespace TechStore.Models
{
    public partial class KhachHang
    {
        public KhachHang()
        {
            Carts = new HashSet<Cart>();
            FeedBacks = new HashSet<FeedBack>();
            HoaDonBans = new HashSet<HoaDonBan>();
        }

        public int Id { get; set; }
        public int? UserId { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? SoDienThoai { get; set; }
        public string? DiaChi { get; set; }
        public DateTime? NgaySinh { get; set; }
        public string? GioiTinh { get; set; }
        public DateTime? CreateDate { get; set; }
        public DateTime? UpdateDate { get; set; }
        public string? Avatar { get; set; }
        public string? TrangThai { get; set; }

        public virtual User? User { get; set; }
        public virtual ICollection<Cart> Carts { get; set; }
        public virtual ICollection<FeedBack> FeedBacks { get; set; }
        public virtual ICollection<HoaDonBan> HoaDonBans { get; set; }
    }

    public class KhachHangDto
    {
        public int Id { get; set; }
        public int? UserId { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? SoDienThoai { get; set; }
        public string? DiaChi { get; set; }
        public DateTime? NgaySinh { get; set; }
        public string? GioiTinh { get; set; }
        public DateTime? CreateDate { get; set; }
        public DateTime? UpdateDate { get; set; }
        public string? Avatar { get; set; }
        public string? TrangThai { get; set; }
        public string? UserName { get; set; }
        public string? PassWord { get; set; }
        public int? RoleId { get; set; }
    }
}
