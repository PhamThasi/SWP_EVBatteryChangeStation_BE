using EV_BatteryChangeStation_Common.DTOs.RoleDTO;
using EV_BatteryChangeStation_Service.InternalService.IService;
using Microsoft.AspNetCore.Mvc;

namespace EV_BatteryChangeStation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RoleController : ControllerBase
    {
        private readonly IRoleService _roleService;

        public RoleController(IRoleService roleService)
        {
            _roleService = roleService ?? throw new ArgumentNullException(nameof(roleService));
        }

        /// <summary>
        /// Tạo mới role
        /// </summary>
        /// <param name="dto">Thông tin vai trò cần tạo</param>
        /// <returns>Thông tin vai trò đã được tạo</returns>
        /// <response code="200">Tạo vai trò thành công</response>
        /// <response code="400">Dữ liệu đầu vào không hợp lệ</response>
        /// <response code="500">Lỗi server khi xử lý yêu cầu</response>
        [HttpPost("Create")]
        public async Task<IActionResult> Create([FromBody] CreateRoleDTO dto)
        {
            if (dto == null)
                return BadRequest("Invalid role data");
            var result = await _roleService.CreateRoleAsync(dto);
            if (result.Status == 200)
                return Ok(result);
            return StatusCode(result.Status, result.Message);
        }

        /// <summary>
        /// Lấy thông tin tất cả vai trò
        /// </summary>
        /// <param name="dto">Thông tin vai trò</param>
        /// <returns>Thông tin vai trò </returns>
        /// <response code="200">lấy thông tin thành công</response>
        /// <response code="400">Dữ liệu đầu vào không hợp lệ</response>
        /// <response code="500">Lỗi server khi xử lý yêu cầu</response>
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            var result = await _roleService.GetAllRolesAsync();
            if (result.Status == 200)
                return Ok(result);
            return StatusCode(result.Status, result.Message);
        }

        /// <summary>
        /// Lấy thông tin tất cả vai trò với Id đã được mã hóa
        /// </summary>
        /// <param name="dto">Lấy thông tin tất cả vai trò với Id đã được mã hóa</param>
        /// <returns>Lấy thông tin tất cả vai trò với Id đã được mã hóa</returns>
        /// <response code="200">lấy thông tin thành công</response>
        /// <response code="400">Dữ liệu đầu vào không hợp lệ</response>
        /// <response code="500">Lỗi server khi xử lý yêu cầu</response>
        //[HttpGet("GetAllByEncodedId")]
        //public async Task<IActionResult> GetAllByEncodedId()
        //{
        //    var result = await _roleService.GetAllRoleByIdDecodeAsync();
        //    if (result.Status == 200)
        //        return Ok(result);
        //    return StatusCode(result.Status, result.Message);
        //}

        /// <summary>
        /// lấy thông tin vai trò theo tên
        /// </summary>
        /// <param name="dto">lấy thông tin vai trò theo tên</param>
        /// <returns>lấy thông tin vai trò theo tên</returns>
        /// <response code="200">lấy vai trò thành công</response>
        /// <response code="400">Dữ liệu đầu vào không hợp lệ</response>
        /// <response code="500">Lỗi server khi xử lý yêu cầu</response>
        [HttpGet("GetByName")]
        public async Task<IActionResult> GetByName([FromQuery] string roleName)
        {
            if (string.IsNullOrEmpty(roleName))
                return BadRequest("RoleName cannot be empty");

            var result = await _roleService.GetRoleByNameAsync(roleName);
            if (result.Status == 200)
                return Ok(result);
            return StatusCode(result.Status, result.Message);
        }

        /// <summary>
        /// Cập nhật vai trò
        /// </summary>
        /// <param name="dto">Cập nhật thông tin vai trò</param>
        /// <returns>Cập nhật thông tin vai trò</returns>
        /// <response code="200">Tạo vai trò thành công</response>
        /// <response code="400">Dữ liệu đầu vào không hợp lệ</response>
        /// <response code="500">Lỗi server khi xử lý yêu cầu</response>
        [HttpPut("Update")]
        public async Task<IActionResult> Update([FromBody] UpdateRoleDTO dto)
        {
            if (dto == null || dto.RoleId == Guid.Empty)
                return BadRequest("Invalid role data");

            var result = await _roleService.UpdateRoleAsync(dto);
            if (result.Status == 200)
                return Ok(result);
            return StatusCode(result.Status, result.Message);
        }

        /// <summary>
        /// xóa vai trò
        /// </summary>
        /// <param name="dto">xóa vai trò thông tin vai trò</param>
        /// <returns>xóa vai trò thông tin vai trò</returns>
        /// <response code="200">Tạo vai trò thành công</response>
        /// <response code="400">Dữ liệu đầu vào không hợp lệ</response>
        /// <response code="500">Lỗi server khi xử lý yêu cầu</response>
        [HttpDelete("Delete")]
        public async Task<IActionResult> Delete([FromQuery] Guid encodedId)
        {
            if (encodedId == Guid.Empty)
                return BadRequest("Invalid RoleId");

            var result = await _roleService.DeleteRoleAsync(encodedId);
            if (result.Status == 200)
                return Ok(result);
            return StatusCode(result.Status, result.Message);
        }

        /// <summary>
        /// xóa vai trò bằng cách thay đổi trạng thái
        /// </summary>
        /// <param name="dto">thay đổi thông tin trạng thái vai trò</param>
        /// <returns>xóa vai trò thông tin vai trò</returns>
        /// <response code="200">Tạo vai trò thành công</response>
        /// <response code="400">Dữ liệu đầu vào không hợp lệ</response>
        /// <response code="500">Lỗi server khi xử lý yêu cầu</response>
        [HttpDelete("SoftDelete")]
        public async Task<IActionResult> SoftDelete([FromQuery] Guid encodedId)
        {
            if (encodedId == Guid.Empty)
                return BadRequest("Invalid RoleId");
            var result = await _roleService.SoftDeleteAsync(encodedId);
            if (result.Status == 200)
                return Ok(result);
            return StatusCode(result.Status, result.Message);
        }
    }
}
