using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SukotSystemCore.DTOs.AdminDTOs.Request;
using SukotSystemCore.DTOs.AdminDTOs.Response;
using SukotSystemCore.Services;
using System.Security.Claims;
using System.Threading;

namespace SukotSystemApi.Controllers
{
    // Self-service only, by explicit design choice: Admin is 3 fixed/seeded accounts (Part A),
    // so there is deliberately no list/create/delete action here - only an Admin viewing or
    // editing their own profile. "me" is resolved from the JWT's NameIdentifier claim, never
    // from the client.
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminsController : ControllerBase
    {
        private readonly IAdminService _adminService;

        public AdminsController(IAdminService adminService)
        {
            _adminService = adminService;
        }

        [HttpGet("me")]
        public async Task<ActionResult<AdminResponseDTO>> GetMyProfile(CancellationToken cancellationToken)
        {
            var res = await _adminService.GetAdminById(GetCurrentAdminId(), cancellationToken);
            return Ok(res);
        }

        [HttpPut("me")]
        public async Task<ActionResult<AdminResponseDTO>> UpdateMyProfile([FromBody] AdminUpdateDTO admin, CancellationToken cancellationToken)
        {
            var res = await _adminService.UpdateAdmin(GetCurrentAdminId(), admin, cancellationToken);
            return Ok(res);
        }

        private int GetCurrentAdminId()
        {
            var idClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.Parse(idClaim!);
        }
    }
}
