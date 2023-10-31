using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TechStore.Models;

namespace TechStore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SanPhamController : ControllerBase
    {
        private TechStoreContext _context = new TechStoreContext();
        public SanPhamController(TechStoreContext context)
        {
            _context = context;
        }

        [Route("GetAll_SanPham")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SanPham>>> GetAll()
        {
            try
            {
                var query = await (from x in _context.SanPhams
                                   select new
                                   {
                                       id = x.Id,
                                       tenSanPham = x.TenSanPham,
                                       giaBan = x.GiaBan,
                                       khuyenMai = x.KhuyenMai,
                                       soLuonTon = x.SoLuongTon,
                                       baoHanh = x.BaoHanh,
                                       moTa = x.MoTa,
                                       loaiId = _context.Loais.Where(l=> l.Id == x.LoaiId).Select(x=>x.TenLoai).FirstOrDefault(),
                                       hangSanXuatId = _context.HangSanXuats.Where(h => h.Id == x.HangSanXuatId).Select(x=> x.TenHang).FirstOrDefault(),
                                       trangThaiSanPham = x.TrangThaiSanPham,
                                       trangThaiHoatDong = x.TrangThaiHoatDong,
                                       createDate = x.CreateDate,
                                       updateDate = x.UpdateDate
                                   }).ToListAsync();
                return Ok(query);
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
