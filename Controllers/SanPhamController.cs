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
                                       loaiId = _context.Loais.Where(l => l.Id == x.LoaiId).Select(x => x.TenLoai).FirstOrDefault(),
                                       hangSanXuatId = _context.HangSanXuats.Where(h => h.Id == x.HangSanXuatId).Select(x => x.TenHang).FirstOrDefault(),
                                       trangThaiSanPham = x.TrangThaiSanPham,
                                       trangThaiHoatDong = x.TrangThaiHoatDong,
                                       createDate = x.CreateDate,
                                       updateDate = x.UpdateDate
                                   }).ToListAsync();
                return Ok(query);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Route("Create_SanPham")]
        [HttpPost]
        public async Task<ActionResult<string>> CreateSanPham([FromBody] SanPham model)
        {
            try
            {

                _context.SanPhams.Add(model);
                await _context.SaveChangesAsync();

                foreach (var img in model.AnhSanPhams)
                {
                    var image = new AnhSanPham
                    {
                        SanPhamId = model.Id,
                        DuongDanAnh = img.DuongDanAnh
                    };

                    _context.AnhSanPhams.Add(image);
                }

                await _context.SaveChangesAsync();

                return Ok(new
                {
                    message = "Thêm sản phẩm thành công"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Route("Update_SanPham")]
        [HttpPut]
        public async Task<ActionResult<string>> UpdateSanPham([FromBody] SanPham model)
        {
            try
            {
                var query = await (from sanPham in _context.SanPhams
                                   where sanPham.Id == model.Id
                                   select sanPham).FirstOrDefaultAsync(); ;

                if (query == null)
                {
                    return NotFound();
                }

                query.TenSanPham = model.TenSanPham;
                query.GiaBan = model.GiaBan;
                query.KhuyenMai = model.KhuyenMai;
                query.SoLuongTon = model.SoLuongTon;
                query.BaoHanh = model.BaoHanh;
                query.MoTa = model.MoTa;
                query.LoaiId = model.LoaiId;
                query.HangSanXuatId = model.HangSanXuatId;
                query.TrangThaiSanPham = model.TrangThaiSanPham;
                query.TrangThaiHoatDong = model.TrangThaiHoatDong;
                query.UpdateDate = DateTime.Now;


                var imgOld = _context.AnhSanPhams.Where(imgOld => imgOld.SanPhamId == query.Id).ToList();
                foreach (var img in imgOld)
                {
                    _context.AnhSanPhams.Remove(img);
                }

                foreach (var imgNew in model.AnhSanPhams)
                {
                    var image = new AnhSanPham
                    {
                        SanPhamId = model.Id,
                        DuongDanAnh = imgNew.DuongDanAnh,
                        UpdateDate = DateTime.Now
                    };

                    _context.AnhSanPhams.Add(image);
                }
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    message = "Cập nhật sản phẩm thành công"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Route("Delete_SanPham/{id}")]
        [HttpDelete]
        public async Task<ActionResult<string>> Delete(int id)
        {
            try
            {
                var productToDelete = await _context.SanPhams.FindAsync(id);

                if (productToDelete == null)
                {
                    return NotFound("Sản phẩm không tồn tại");
                }

                var productImages = _context.AnhSanPhams.Where(img => img.SanPhamId == productToDelete.Id).ToList();
                foreach (var img in productImages)
                {
                    _context.AnhSanPhams.Remove(img);
                }

                _context.SanPhams.Remove(productToDelete);
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    message = "Xóa sản phẩm thành công"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}