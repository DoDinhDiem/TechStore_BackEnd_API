using System;
using System.Collections.Generic;

namespace TechStore.Models
{
    public partial class Loai
    {
        public Loai()
        {
            SanPhams = new HashSet<SanPham>();
        }

        public int Id { get; set; }
        public string? TenLoai { get; set; }
        public bool? TrangThai { get; set; }
        public int? MaCha { get; set; }
        public int? SapXep { get; set; }
        public DateTime? CreateDate { get; set; }
        public DateTime? UpdateDate { get; set; }

        public virtual ICollection<SanPham> SanPhams { get; set; }
    }
}
