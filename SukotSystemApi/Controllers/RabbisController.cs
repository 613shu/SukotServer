using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SukotSystemCore.DTOs.AdminDTOs;
using SukotSystemCore.DTOs.Common;
using SukotSystemCore.DTOs.RabbiDTOs.Request;
using SukotSystemCore.DTOs.RabbiDTOs.Response;
using SukotSystemCore.Services;
using System.Security.Claims;
using System.Threading;

namespace SukotSystemApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class RabbisController : ControllerBase
    {
        private readonly IRabbiService _rabbiService;

        public RabbisController(IRabbiService rabbiService)
        {
            _rabbiService = rabbiService;
        }

        // Admin-only (Part A: "Admin is the only role that adds/removes rabbis"). RabbiRegisterDTO
        // is reused here as the request body an Admin submits - there is deliberately no
        // [AllowAnonymous] self-registration action on this controller.
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult<RabbiResponseDTO>> AddRabbi([FromBody] RabbiRegisterDTO rabbi, CancellationToken cancellationToken)
        {
            var res = await _rabbiService.AddRabbi(rabbi, cancellationToken);
            return CreatedAtAction(nameof(GetRabbiById), new { id = res.Id }, res);
        }

        // Admin roster view - real pagination (requirement 6), not ToList+in-memory slicing.
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<ActionResult<PagedResultDTO<RabbiAdminDTO>>> GetAllRabbis(
            [FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken cancellationToken = default)
        {
            var res = await _rabbiService.GetAllRabbisPaged(page, pageSize, cancellationToken);
            return Ok(res);
        }

        // Admin detail view - includes every Order this Rabbi has handled.
        [Authorize(Roles = "Admin")]
        [HttpGet("{id}")]
        public async Task<ActionResult<RabbiAdminDTO>> GetRabbiById([FromRoute] int id, CancellationToken cancellationToken)
        {
            var res = await _rabbiService.GetRabbiAdminViewById(id, cancellationToken);
            return Ok(res);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<ActionResult<RabbiResponseDTO>> UpdateRabbi([FromRoute] int id, [FromBody] RabbiUpdateDTO rabbi, CancellationToken cancellationToken)
        {
            var res = await _rabbiService.UpdateRabbi(id, rabbi, cancellationToken);
            return Ok(res);
        }

        // Soft "remove" (Part A) - flips IsActive to false. Never a physical DELETE, so this
        // Rabbi's HandledOrders history is never at risk.
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeactivateRabbi([FromRoute] int id, CancellationToken cancellationToken)
        {
            await _rabbiService.DeactivateRabbi(id, cancellationToken);
            return NoContent();
        }

        [Authorize(Roles = "Admin")]
        [HttpPatch("{id}/reactivate")]
        public async Task<ActionResult<RabbiResponseDTO>> ReactivateRabbi([FromRoute] int id, CancellationToken cancellationToken)
        {
            var res = await _rabbiService.ReactivateRabbi(id, cancellationToken);
            return Ok(res);
        }

        // A Rabbi viewing/editing their own profile. "me" is resolved from the JWT's
        // NameIdentifier claim (see AuthHelper.CreateToken) - never trusted from the client.
        [Authorize(Roles = "Rabbi")]
        [HttpGet("me")]
        public async Task<ActionResult<RabbiResponseDTO>> GetMyProfile(CancellationToken cancellationToken)
        {
            var res = await _rabbiService.GetRabbiById(GetCurrentRabbiId(), cancellationToken);
            return Ok(res);
        }

        [Authorize(Roles = "Rabbi")]
        [HttpPut("me")]
        public async Task<ActionResult<RabbiResponseDTO>> UpdateMyProfile([FromBody] RabbiUpdateDTO rabbi, CancellationToken cancellationToken)
        {
            var res = await _rabbiService.UpdateRabbi(GetCurrentRabbiId(), rabbi, cancellationToken);
            return Ok(res);
        }

        private int GetCurrentRabbiId()
        {
            var idClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.Parse(idClaim!);
        }
    }
}
