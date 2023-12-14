﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TechStore.Models;

namespace TechStore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HoaDonNhapController : ControllerBase
    {
        private TechStoreContext _context;
        public HoaDonNhapController(TechStoreContext context)
        {
            _context = context;
        }

        [Route("GetAll_HoaDonNhap")]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var query = await (from x in _context.HoaDonNhaps
                                   select new
                                   {
                                       id = x.Id,
                                       userId = x.UserId,
                                       nhaCungCap = _context.NhaCungCaps.Where(ncc => ncc.Id == x.NhaCungCapId).Select(ncc => ncc.TenNhaCungCap),
                                       tongTien = x.TongTien,
                                       trangThaiThanhToans = x.TrangThaiThanhToan
                                   }).ToListAsync();
                return Ok(query);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Route("GetById_HoaDonNhap/{id}")]
        [HttpGet]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var query = await (from x in _context.HoaDonNhaps
                                   where x.Id == id
                                   select new
                                   {
                                       id = x.Id,
                                       userId = x.UserId,
                                       nhaCungCap =x.NhaCungCapId,
                                       tongTien = x.TongTien,
                                       trangThaiThanhToans = x.TrangThaiThanhToan,
                                       chiTietHoaDon = _context.ChiTietHoaDonNhaps.Where(u => u.HoaDonNhapId == id).Select(s => new
                                       {
                                           sanPhamId = s.SanPhamId,
                                           soLuongNhap = s.SoLuongNhap,
                                           giaNhap = s.GiaNhap,
                                           thanhTien = s.ThanhTien
                                       }).ToList(),

                                   }).FirstOrDefaultAsync();
                if (query == null)
                {
                    return BadRequest(new
                    {
                        message = "Không tìm thấy dữ liệu!"
                    });
                }

                return Ok(query);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Route("Create_HoaDonNhap")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] HoaDonNhap model)
        {
            try
            {
                _context.HoaDonNhaps.Add(model);

                var newHoaDon = new List<ChiTietHoaDonNhap>();

                foreach (var cthd in model.ChiTietHoaDonNhaps)
                {
                    var ct = new ChiTietHoaDonNhap
                    {
                        HoaDonNhapId = model.Id,
                        SanPhamId = cthd.SanPhamId,
                        SoLuongNhap = cthd.SoLuongNhap,
                        GiaNhap = cthd.GiaNhap,
                        ThanhTien = cthd.ThanhTien
                    };

                    var sanPham = await _context.SanPhams.FindAsync(ct.SanPhamId);

                    // Kiểm tra nếu sản phẩm tồn tại
                    if (sanPham != null)
                    {
                        sanPham.SoLuongTon += ct.SoLuongNhap;
                        sanPham.UpdateDate = DateTime.Now;
                        _context.SanPhams.Update(sanPham);
                    }

                    newHoaDon.Add(ct);
                }

                decimal? totalAmount = newHoaDon.Sum(ct => ct.ThanhTien);
                model.TongTien = totalAmount;

                await _context.SaveChangesAsync();

                return Ok(new
                {
                    message = "Đặt mua sản phẩm thành công!"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Route("Search_HoaDonNhap")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<HoaDonNhap>>> Search()
        {
            var query = _context.HoaDonNhaps
                .Select(x => new
                {
                    id = x.Id,
                    userId = _context.NhanSus.Where(us => us.Id == x.UserId).Select(us =>  us.FirstName + " " + us.LastName ).FirstOrDefault(),
                    nhaCungCapId = _context.NhaCungCaps.Where(ncc => ncc.Id == x.NhaCungCapId).Select(ncc => ncc.TenNhaCungCap).FirstOrDefault(),
                    tongTien = x.TongTien,
                    trangThaiThanhToan = x.TrangThaiThanhToan
                });

            var result = await query.ToListAsync();
            return Ok(result);
        }

    }
}