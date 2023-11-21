using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TechStore.Models;

namespace TechStore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        private TechStoreContext _context = new TechStoreContext();
        public RoleController(TechStoreContext context) 
        {
            _context = context;
        }

        [Route("GetAll_Role")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Role>>> GetAll()
        {
            try
            {
                var query = await (from x in _context.Roles
                                   select new
                                   {
                                       id = x.Id,
                                       tenRole = x.TenRole,
                                       trangThai = x.TrangThai
                                   }).Where(x => x.trangThai == true).ToListAsync();
                return Ok(query);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [Route("GetById_Role/{id}")]
        [HttpGet]
        public async Task<ActionResult<Role>> GetById(int id)
        {
            try
            {
                var query = await (from x in _context.Roles
                                   where x.Id == id
                                   select new
                                   {
                                       id = x.Id,
                                       tenRole = x.TenRole,
                                       trangThai = x.TrangThai
                                   }).FirstOrDefaultAsync();

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

        [Route("Create_Role")]
        [HttpPost]
        public async Task<ActionResult<Role>> CreateRole([FromBody] Role model)
        {
            try
            {
                _context.Roles.Add(model);
                await _context.SaveChangesAsync();
                return Ok(new
                {
                    message = "Thêm quyền thành công!"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Route("Update_Role")]
        [HttpPut]
        public async Task<ActionResult<Role>> UpdateRole([FromBody] Role model)
        {
            try
            {
                var query = await (from x in _context.Roles 
                                   where x.Id == model.Id
                                   select x).FirstOrDefaultAsync();
                if(query == null)
                {
                    return NotFound();
                }

                query.TenRole = model.TenRole;
                query.TrangThai = model.TrangThai;
                query.UpdateDate = DateTime.Now;

                await _context.SaveChangesAsync();
                return Ok(new
                {
                    message = "Sửa quyền thành công!"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Route("Update_Role_TrangThai/{id}")]
        [HttpPut]
        public async Task<ActionResult<Role>> UpdateRoleTrangThai(int id)
        {
            try
            {
                var query = await (from x in _context.Roles
                                   where x.Id == id
                                   select x).FirstOrDefaultAsync();
                if( query == null)
                {
                    return NotFound();
                }
                query.TrangThai = !query.TrangThai;
                query.UpdateDate = DateTime.Now;

                await _context.SaveChangesAsync();

                return Ok(new
                {
                    message = "Sửa trạng thái thành công!"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Route("Delete_Role/{id}")]
        [HttpDelete]
        public async Task<ActionResult<Role>> DeleteRole(int id)
        {
            try
            {
                var query = await (from x in _context.Roles
                                   where x.Id == id
                                   select x).FirstOrDefaultAsync();
                if( query == null)
                {
                    return NotFound();
                }

                _context.Remove(query);
                await _context.SaveChangesAsync();
                return Ok(new
                {
                    message = "Xóa quyền thành công!"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Route("DeleteMany_Role")]
        [HttpDelete]
        public IActionResult DeleteMany([FromBody] List<int> listId)
        {
            try
            {
                var query = _context.Roles.Where(i => listId.Contains(i.Id)).ToList();

                if (query.Count == 0)
                {
                    return NotFound("Không tìm thấy bất kỳ mục nào để xóa.");
                }

                _context.Roles.RemoveRange(query);
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

        [Route("Search_Role")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Role>>> Search(
            [FromQuery] string? Keywork)
        {
            IQueryable<Role> query = _context.Roles;

            if (!string.IsNullOrEmpty(Keywork))
            {
                query = query.Where(dc => dc.TenRole.Contains(Keywork));
            }

            return Ok(query);
        }
    }
}
