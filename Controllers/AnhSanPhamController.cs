using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TechStore.Models;

namespace TechStore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AnhSanPhamController : ControllerBase
    {
        private TechStoreContext _context = new TechStoreContext();
        public AnhSanPhamController(TechStoreContext context)
        {
            _context = context;
        }

        [Route("GetAll_AnhSanPham/{id}")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AnhSanPham>>> GetAll(int id)
        {
            try
            {
                var query = await (from anh in _context.AnhSanPhams
                                   select new
                                   {
                                       id = anh.Id,
                                       sanPhamName = _context.SanPhams.Where(l => l.Id == anh.SanPhamId).Select(x => x.TenSanPham).FirstOrDefault(),
                                       sanPhamId = anh.SanPhamId,
                                       duongDanAnh = anh.DuongDanAnh,
                                       trangThai = anh.TrangThai
                                   }).Where(x => x.sanPhamId == id).ToListAsync();
                return Ok(query);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Route("GetById_AnhSanPham/{id}")]
        [HttpGet]
        public async Task<ActionResult<AnhSanPham>> GetById(int id)
        {
            try
            {
                var query = await (from anh in _context.AnhSanPhams
                                   select new
                                   {
                                       id = anh.Id,
                                       sanPhamId = _context.SanPhams.Where(l => l.Id == anh.SanPhamId).Select(x => x.TenSanPham).FirstOrDefault(),
                                       trangThai = anh.TrangThai
                                   }).Where(x => x.id == id).FirstOrDefaultAsync();
                if (query == null)
                {
                    return NotFound();
                }
                return Ok(query);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Route("Create_AnhSanPham")]
        [HttpPost]
        public async Task<ActionResult<AnhSanPham>> CreateAnhOnly([FromBody] AnhSanPham model)
        {
            try
            {
                var img = new AnhSanPham
                {
                    SanPhamId = model.SanPhamId,
                    DuongDanAnh = model.DuongDanAnh,
                    TrangThai = true
                };
                _context.AnhSanPhams.Add(img);
                await _context.SaveChangesAsync();
                return Ok(new
                {
                    message = "Thêm ảnh sản phẩm thành công"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        [Route("Create_AnhSanPhamList")]
        [HttpPost]
        public async Task<ActionResult<AnhSanPham>> CreateAnh([FromBody] List<AnhSanPham> danhSachAnh)
        {
            try
            {
                foreach (var anh in danhSachAnh)
                {
                    var img = new AnhSanPham
                    {
                        DuongDanAnh = anh.DuongDanAnh,
                        TrangThai = false
                    };
                    _context.AnhSanPhams.Add(img);
                }
                await _context.SaveChangesAsync();
                return Ok(new
                {
                    message = "Thêm ảnh sản phẩm thành công"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        [Route("Update_AnhSanPham")]
        [HttpPut]
        public async Task<ActionResult<AnhSanPham>> UpdateSanPham([FromBody] AnhSanPham anh)
        {
            try
            {
                var query = await (from img in _context.AnhSanPhams
                                   where img.Id == anh.Id
                                   select anh).FirstOrDefaultAsync();
                if (query == null)
                {
                    return NotFound();
                }

                query.DuongDanAnh = anh.DuongDanAnh;
                query.TrangThai = anh.TrangThai;
                query.UpdateDate = DateTime.Now;

                await _context.SaveChangesAsync();

                return Ok(new
                {
                    message = "Sửa ảnh sản phẩm thành công!"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Route("Update_AnhSanPham_TrangThai/{id}")]
        [HttpPut]
        public async Task<ActionResult<AnhSanPham>> UpdateAnhSanPham(int id)
        {
            try
            {
                var query = await (from anh in _context.AnhSanPhams
                                   where anh.Id == id
                                   select anh).FirstOrDefaultAsync();
                if (query == null)
                {
                    return NotFound();
                }

                query.TrangThai = !query.TrangThai;
                query.UpdateDate = DateTime.Now;

                await _context.SaveChangesAsync();

                return Ok(new
                {
                    message = "Sửa trạng thái và UpdateDate của ảnh sản phẩm thành công!"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Route("Delete_AnhSanPham/{id}")]
        [HttpDelete]
        public async Task<ActionResult<AnhSanPham>> DeleteAnhSanPham(int id)
        {
            try
            {
                var query = await (from anh in _context.AnhSanPhams
                                   where anh.Id == id
                                   select anh).FirstOrDefaultAsync();

                if (query == null)
                {
                    return NotFound();
                }

                _context.Remove(query);
                await _context.SaveChangesAsync();
                return Ok(new
                {
                    message = "Xóa ảnh sản phẩm thành công!"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Route("DeleteMany_AnhSanPham")]
        [HttpDelete]
        public IActionResult DeleteMany([FromBody] List<int> listId)
        {
            try
            {
                var query = _context.Loais.Where(i => listId.Contains(i.Id)).ToList();

                if (query.Count == 0)
                {
                    return NotFound("Không tìm thấy bất kỳ mục nào để xóa.");
                }

                _context.Loais.RemoveRange(query);
                _context.SaveChanges();

                return Ok(new
                {
                    message = "Danh sách đã được xóa thành công."
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
