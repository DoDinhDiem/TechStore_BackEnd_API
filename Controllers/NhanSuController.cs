using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TechStore.Models;
using TechStore.Helper;
using Microsoft.AspNetCore.SignalR;

namespace TechStore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NhanSuController : ControllerBase
    {
        private TechStoreContext _context = new TechStoreContext();
        public NhanSuController(TechStoreContext context)
        {
            _context = context;
        }

        [Route("GetAll_NhanSu")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<NhanSu>>> GetAll()
        {
            try
            {
                var query = await (from x in _context.NhanSus
                                   select new
                                   {
                                       id = x.Id,
                                       taikhoan = _context.Users.Where(u => u.Id == x.UserId).Select(u => u.UserName).FirstOrDefault(),
                                       firstName = x.FirstName,
                                       lastName = x.LastName,
                                       email = x.Email,
                                       soDienThoai = x.SoDienThoai,
                                       diaChi = x.DiaChi,
                                       ngaySinh = x.NgaySinh,
                                       gioiTinh = x.GioiTinh,
                                       ngayVaoLam = x.NgayVaoLam,
                                       chucVu = _context.ChucVus.Where(u => u.Id == x.ChucVuId).Select(u => u.TenChucVu).FirstOrDefault(),
                                       avartar = x.Avatar
                                   }).ToListAsync();
                return Ok(query);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Route("GetById_NhanSu/{id}")]
        [HttpGet]
        public async Task<ActionResult<NhanSu>> GetById(int id)
        {
            try
            {
                var query = await (from x in _context.NhanSus
                                   where x.Id == id
                                   select new
                                   {
                                       id = x.Id,
                                       taikhoan = _context.Users.Where(u => u.Id == x.UserId).Select(u => u.UserName).FirstOrDefault(),
                                       firstName = x.FirstName,
                                       lastName = x.LastName,
                                       email = x.Email,
                                       soDienThoai = x.SoDienThoai,
                                       diaChi = x.DiaChi,
                                       ngaySinh = x.NgaySinh,
                                       gioiTinh = x.GioiTinh,
                                       ngayVaoLam = x.NgayVaoLam,
                                       chucVu = _context.ChucVus.Where(u => u.Id == x.ChucVuId).Select(u => u.TenChucVu).FirstOrDefault(),
                                       avartar = x.Avatar
                                   }).FirstOrDefaultAsync();
                if (query != null)
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

        [Route("Create_NhanSu")]
        [HttpPost]
        public async Task<ActionResult<string>> Create([FromBody] NhanSu model)
        {
            try
            {
                if (_context.Users.Any(u => u.UserName == model.User.UserName))
                {
                    return BadRequest(new 
                    { 
                        message = "UserName đã tồn tại! Vui lòng nhập UserName khác."
                    });
                }
                if(_context.Users.Any(u => u.Email == model.Email))
                    {
                    return BadRequest(new
                    {
                        message = "Email đã tồn tại! Vui lòng nhập Email khác."
                    });
                }

                var user = new User
                {
                    UserName = model.User.UserName,
                    PassWord = PasswordHasher.HashPassword(model.User.PassWord),
                    Email = model.Email,
                    RoleId = model.User.RoleId
                };

                _context.Users.Add(user);

                model.UserId = user.Id;
                _context.NhanSus.Add(model);
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    message = "Thêm nhân sự thành công!"
                });
                
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Route("Update_NhanSu")]
        [HttpPut]
        public async Task<ActionResult<string>> Update([FromBody] NhanSu model)
        {
            try
            {
                var user = await (from us in  _context.Users
                                   where us.Id == model.UserId
                                   select us
                                   ).FirstOrDefaultAsync();
                var nhansu = await (from ns in _context.NhanSus
                                    where ns.Id == model.Id
                                    select ns
                                    ).FirstOrDefaultAsync();
                if(nhansu == null || user == null)
                {
                    return NotFound();
                }

                user.RoleId = model.User.RoleId;
                user.UpdateDate = DateTime.Now;

                nhansu.FirstName = model.FirstName;
                nhansu.LastName = model.LastName;
                nhansu.SoDienThoai = model.SoDienThoai;
                nhansu.DiaChi = model.DiaChi;
                nhansu.NgaySinh = model.NgaySinh;
                nhansu.GioiTinh = model.GioiTinh;
                nhansu.NgayVaoLam = model.NgayVaoLam;
                nhansu.Avatar = model.Avatar;
                nhansu.TrangThai = model.TrangThai;
                nhansu.ChucVuId = model.ChucVuId;
                nhansu.UpdateDate = DateTime.Now;

                await _context.SaveChangesAsync();

                return Ok(new
                {
                    message = "Sửa nhân sự thành công!"
                });

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message); 
            }
        }

        [Route("Update_TrangThai_NhanSu/{id}")]
        [HttpPut]
        public async Task<ActionResult<NhanSu>> UpdateTrangThai(int id)
        {
            try
            {

            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }

            var nhansu = await (from ns in _context.NhanSus
                                where ns.Id == id
                                select ns).FirstOrDefaultAsync();
            if(nhansu == null)
            {
                return NotFound();
            }

            nhansu.TrangThai = !nhansu.TrangThai;
            nhansu.UpdateDate = DateTime.Now;
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Sửa trạng thái thành công!"
            });
        }

        [Route("Search_NhanSu")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<NhanSu>>> Search(
           [FromQuery] string? Keywork,
           [FromQuery] string? Email,
           [FromQuery] string? DiaChi)
        {
            var query = _context.NhanSus
                .Select(x => new
                {
                    id = x.Id,
                    taikhoan = _context.Users.Where(u => u.Id == x.UserId).Select(u => u.UserName).FirstOrDefault(),
                    firstName = x.FirstName,
                    lastName = x.LastName,
                    email = x.Email,
                    soDienThoai = x.SoDienThoai,
                    diaChi = x.DiaChi,
                    ngaySinh = x.NgaySinh,
                    gioiTinh = x.GioiTinh,
                    ngayVaoLam = x.NgayVaoLam,
                    chucVu = _context.ChucVus.Where(u => u.Id == x.ChucVuId).Select(u => u.TenChucVu).FirstOrDefault(),
                    avartar = x.Avatar,
                    trangThai = x.TrangThai, 
                    createDate = x.CreateDate
                });

            if (!string.IsNullOrEmpty(Keywork))
            {
                query = query.Where(dc => dc.lastName.Contains(Keywork));
            }

            if (!string.IsNullOrEmpty(Email))
            {
                query = query.Where(dc => dc.email.Contains(Email));
            }

            if (!string.IsNullOrEmpty(DiaChi))
            {
                query = query.Where(dc => dc.diaChi.Contains(DiaChi));
            };

            query = query.OrderByDescending(dc => dc.createDate);
            return Ok(query);
        }
    }
}
