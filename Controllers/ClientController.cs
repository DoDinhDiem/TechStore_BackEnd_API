using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TechStore.Models;

namespace TechStore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientController : ControllerBase
    {
        private TechStoreContext _context;
        public ClientController(TechStoreContext context)
        {
            _context = context;
        }

        [Route("SanPhamMoi")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SanPham>>> SanPhamMoi()
        {
            try
            {
                var query = await (from x in _context.SanPhams
                                   where x.TrangThaiSanPham == "Sản phẩm mới"
                                   where x.TrangThaiHoatDong == true
                                   orderby x.CreateDate descending
                                   select new
                                   {
                                       id = x.Id,
                                       tenSanPham = x.TenSanPham,
                                       giaBan = x.GiaBan,
                                       khuyenMai = x.KhuyenMai,
                                       avatar = _context.AnhSanPhams.Where(a => a.SanPhamId == x.Id && a.TrangThai == true).Select(a => a.DuongDanAnh).FirstOrDefault()
                                   }).Take(10).ToListAsync();
                return Ok(query);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Route("SanPhamKhuyenMai")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SanPham>>> SanPhamKhuyenMai()
        {
            try
            {
                var query = await (from x in _context.SanPhams
                                   where x.TrangThaiSanPham == "Sản phẩm Khuyến mại"
                                   where x.TrangThaiHoatDong == true
                                   orderby x.CreateDate descending
                                   select new
                                   {
                                       id = x.Id,
                                       tenSanPham = x.TenSanPham,
                                       giaBan = x.GiaBan,
                                       khuyenMai = x.KhuyenMai,
                                       avatar = _context.AnhSanPhams.Where(a => a.SanPhamId == x.Id && a.TrangThai == true).Select(a => a.DuongDanAnh).FirstOrDefault()
                                   }).Take(10).ToListAsync();
                return Ok(query);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Route("SanPhamBanChay")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SanPham>>> SanPhamBanChay()
        {
            try
            {
                var query = await (from x in _context.SanPhams
                                   join ct in _context.ChiTietHoaDonBans on x.Id equals ct.SanPhamId
                                   where x.TrangThaiHoatDong == true
                                   group x by new
                                   {
                                       x.Id,
                                       x.TenSanPham,
                                       x.GiaBan,
                                       x.KhuyenMai
                                   } into g
                                   select new
                                   {
                                       SanPhamId = g.Key.Id,
                                       TenSanPham = g.Key.TenSanPham,
                                       GiaBan = g.Key.GiaBan,
                                       KhuyenMai = g.Key.KhuyenMai,
                                       Avatar = _context.AnhSanPhams
                                           .Where(a => a.SanPhamId == g.Key.Id && a.TrangThai == true)
                                           .Select(a => a.DuongDanAnh)
                                           .FirstOrDefault(),
                                       Total = g.Count()
                                   }).Take(10).ToListAsync();
                return Ok(query);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Route("ChiTietSanPham/{id}")]
        [HttpGet]
        public async Task<ActionResult<SanPham>> GetById(int id)
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
                                       trangThaiSanPham = x.TrangThaiSanPham,
                                       trangThaiHoatDong = x.TrangThaiHoatDong,
                                       createDate = x.CreateDate,
                                       updateDate = x.UpdateDate
                                   }).Where(x => x.id == id).FirstOrDefaultAsync();
                return Ok(query);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Route("AnhSanPham/{id}")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AnhSanPham>>> GetByIdAnh(int id)
        {
            try
            {
                var query = await _context.AnhSanPhams
                                .Where(x => x.SanPhamId == id)
                                 .OrderByDescending(x => x.TrangThai)
                                .Select(x => x.DuongDanAnh)
                                .ToListAsync();
                return Ok(query);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Route("ThongSo/{id}")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ThongSo>>> GetByIdThongSo(int id)
        {
            try
            {
                var query = await (from thongso in _context.ThongSos
                                   select new
                                   {
                                       sanPhamId = thongso.SanPhamId,
                                       tenThongSo = thongso.TenThongSo,
                                       moTa = thongso.MoTa,
                                       trangThai = thongso.TrangThai
                                   }).Where(x => x.sanPhamId == id && x.trangThai == true).ToListAsync();
                return Ok(query);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Route("LoaiSanPham")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Loai>>> GetAll()
        {
            try
            {
                var query = await (from loai in _context.Loais
                                   select new
                                   {
                                       id = loai.Id,
                                       tenLoai = loai.TenLoai,
                                       trangThai = loai.TrangThai
                                   }).Where(x => x.trangThai == true).ToListAsync();
                return Ok(query);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Route("SanPhamTheoLoai/{id}")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SanPham>>> GetSanPhamByLoaiID(int id)
        {
            try
            {
                var query = await (from x in _context.SanPhams
                                   where x.LoaiId == id
                                   where x.TrangThaiHoatDong == true
                                   orderby x.CreateDate descending
                                   select new
                                   {
                                       id = x.Id,
                                       tenSanPham = x.TenSanPham,
                                       giaBan = x.GiaBan,
                                       khuyenMai = x.KhuyenMai,
                                       avatar = _context.AnhSanPhams.Where(a => a.SanPhamId == x.Id && a.TrangThai == true).Select(a => a.DuongDanAnh).FirstOrDefault()
                                   }).ToListAsync();
                return Ok(query);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Route("GetAll_Slide")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Slider>>> GetAllSlide()
        {
            try
            {
                var query = await (from x in _context.Sliders
                                   where x.Status == true
                                   select new
                                   {
                                       id = x.Id,
                                       anhSlide = x.AnhSlide,
                                       status = x.Status
                                   }).ToListAsync();
                return Ok(query);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}
