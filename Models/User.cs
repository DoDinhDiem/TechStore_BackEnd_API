using System;
using System.Collections.Generic;

namespace TechStore.Models
{
    public partial class User
    {
        public User()
        {
            BinhLuanSanPhams = new HashSet<BinhLuanSanPham>();
            BinhLuanTinTucs = new HashSet<BinhLuanTinTuc>();
            FeedBacks = new HashSet<FeedBack>();
            HoaDonBans = new HashSet<HoaDonBan>();
            HoaDonNhaps = new HashSet<HoaDonNhap>();
            KhachHangs = new HashSet<KhachHang>();
            NhanSus = new HashSet<NhanSu>();
            TinTucs = new HashSet<TinTuc>();
        }

        public int Id { get; set; }
        public string? UserName { get; set; }
        public string? PassWord { get; set; }
        public string? Email { get; set; }
        public int? RoleId { get; set; }
        public DateTime? CreateDate { get; set; }
        public DateTime? UpdateDate { get; set; }

        public virtual Role? Role { get; set; }
        public virtual ICollection<BinhLuanSanPham> BinhLuanSanPhams { get; set; }
        public virtual ICollection<BinhLuanTinTuc> BinhLuanTinTucs { get; set; }
        public virtual ICollection<FeedBack> FeedBacks { get; set; }
        public virtual ICollection<HoaDonBan> HoaDonBans { get; set; }
        public virtual ICollection<HoaDonNhap> HoaDonNhaps { get; set; }
        public virtual ICollection<KhachHang> KhachHangs { get; set; }
        public virtual ICollection<NhanSu> NhanSus { get; set; }
        public virtual ICollection<TinTuc> TinTucs { get; set; }
    }
}
