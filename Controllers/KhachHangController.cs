using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TechStore.Helper;
using TechStore.Models;

namespace TechStore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class KhachHangController : ControllerBase
    {
        private TechStoreContext _context;
        public KhachHangController(TechStoreContext context)
        {
            _context = context;
        }

        [Route("GetAll_KhachHang")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<KhachHang>>> GetAll()
        {
            try
            {
                var query = await (from x in _context.KhachHangs
                                   select new
                                   {
                                       id = x.Id,
                                       userId = _context.Users.Where(us => us.Id == x.UserId).Select(us => us.UserName).FirstOrDefault(),
                                       firstName = x.FirstName,
                                       lastName = x.LastName,
                                       email = x.Email,
                                       soDienThoai = x.SoDienThoai,
                                       diaChi = x.DiaChi,
                                       ngaySinh = x.NgaySinh,
                                       gioiTinh = x.GioiTinh,
                                       avatar = x.Avatar,
                                       trangThai = x.TrangThai
                                   }).ToListAsync();
                return Ok(query);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [Route("GetById_KhachHang/{id}")]
        [HttpGet]
        public async Task<ActionResult<KhachHang>> GetById(int id)
        {
            try
            {
                var query = await (from x in _context.KhachHangs
                                   where x.Id == id
                                   select new
                                   {
                                       id = x.Id,
                                       firstName = x.FirstName,
                                       lastName = x.LastName,
                                       email = x.Email,
                                       soDienThoai = x.SoDienThoai,
                                       diaChi = x.DiaChi,
                                       ngaySinh = x.NgaySinh,
                                       gioiTinh = x.GioiTinh,
                                       avatar = x.Avatar
                                   }).FirstOrDefaultAsync();
                if(query == null) 
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

        [Route("Create_KhachHang")]
        [HttpPost]
        public async Task<ActionResult<KhachHang>> Create([FromBody] KhachHang model)
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
                if (_context.Users.Any(u => u.Email == model.Email))
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
                _context.KhachHangs.Add(model);
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    message = "Tạo tài khoản thành công thành công!"
                });

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Route("Update_KhachHang")]
        [HttpPost]
        public async Task<ActionResult<KhachHang>> Update([FromBody] KhachHang model)
        {
            try
            {
                var query = await (from x in _context.KhachHangs
                                   where x.Id == model.Id
                                   select x).FirstOrDefaultAsync();
                if(query == null)
                {
                    return NotFound();
                }
                query.FirstName = model.FirstName;
                query.LastName = model.LastName;
                query.Email = model.Email;
                query.SoDienThoai = model.SoDienThoai;
                query.DiaChi = model.DiaChi;
                query.NgaySinh = model.NgaySinh;
                query.GioiTinh = model.GioiTinh;
                query.Avatar = model.Avatar;
                query.TrangThai = model.TrangThai;

                await _context.SaveChangesAsync();
                return Ok(new
                {
                    message = "Sửa thông tin thành công!"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Route("Delete_KhachHang/{id}")]
        [HttpDelete]
        public async Task<ActionResult<KhachHang>> Delete(int id)
        {
            try
            {
                var query = await (from x in _context.KhachHangs
                                   where x.Id == id
                                   select x).FirstOrDefaultAsync();
                var user = await (from x in _context.Users
                                  where x.Id == query.UserId
                                  select x).FirstOrDefaultAsync();
                if(query == null || user == null) 
                { 
                    return NotFound();
                }
                _context.KhachHangs.Remove(query);
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    message = "Xóa tài khoản thành công"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Route("Search_KhachHang")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<KhachHang>>> Search(
           [FromQuery] string? Keywork,
           [FromQuery] string? Email,
           [FromQuery] string? DiaChi)
        {
            var query = _context.KhachHangs
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
