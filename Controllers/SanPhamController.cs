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
        private string _path;
        public SanPhamController(TechStoreContext context, IConfiguration configuration)
        {
            _context = context;
            _path = configuration["AppSettings:UrlImage"];
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

        [Route("Search_SanPham")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SanPham>>> Search(
            [FromQuery] string? Keywork,
            [FromQuery] decimal? MinGiaBan,
            [FromQuery] decimal? MaxGiaBan,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            var query = _context.SanPhams
                .Select(x => new
                {
                    id = x.Id,
                    tenSanPham = x.TenSanPham,
                    giaBan = x.GiaBan,
                    khuyenMai = x.KhuyenMai,
                    soLuonTon = x.SoLuongTon,
                    baoHanh = x.BaoHanh,
                    moTa = x.MoTa,
                    loaiId = _context.Loais.Where(l => l.Id == x.LoaiId).Select(l => l.TenLoai).FirstOrDefault(),
                    hangSanXuatId = _context.HangSanXuats.Where(h => h.Id == x.HangSanXuatId).Select(h => h.TenHang).FirstOrDefault(),
                    trangThaiSanPham = x.TrangThaiSanPham,
                    trangThaiHoatDong = x.TrangThaiHoatDong,
                    createDate = x.CreateDate,
                    updateDate = x.UpdateDate
                });

            if (!string.IsNullOrEmpty(Keywork))
            {
                query = query.Where(dc => dc.tenSanPham.Contains(Keywork));
            }

            if (MinGiaBan.HasValue)
            {
                query = query.Where(dc => dc.giaBan >= MinGiaBan.Value);
            }

            if (MaxGiaBan.HasValue)
            {
                query = query.Where(dc => dc.giaBan <= MaxGiaBan.Value);
            }

            var result = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
            return Ok(result);
        }

        [Route("upload")]
        [HttpPost, DisableRequestSizeLimit]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            try
            {
                if (file.Length > 0)
                {
                    string filePath = $"products/{file.FileName}";
                    var fullPath = CreatePathFile(filePath);
                    using (var fileStream = new FileStream(fullPath, FileMode.Create))
                    {
                        await file.CopyToAsync(fileStream);
                    }
                    return Ok(new { filePath });
                }
                else
                {
                    return BadRequest();
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Not found!");
            }
        }

        [NonAction]
        private string CreatePathFile(string RelativePathFileName)
        {
            try
            {
                string serverRootPathFolder = _path;
                string fullPathFile = $@"{serverRootPathFolder}\{RelativePathFileName}";
                string fullPathFolder = System.IO.Path.GetDirectoryName(fullPathFile);
                if (!Directory.Exists(fullPathFolder))
                    Directory.CreateDirectory(fullPathFolder);
                return fullPathFile;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
    }
}