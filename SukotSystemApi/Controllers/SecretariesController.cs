using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SukotSystemCore.DTOs.SecretaryDTOs;
using SukotSystemCore.Services;

namespace SukotSystemApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class SecretariesController : ControllerBase
    {
        private readonly ISecreratyService _secreratyService;
        public SecretariesController(ISecreratyService secreratyService)
        {
            _secreratyService=secreratyService;
        }


        [HttpPost]
        [Authorize(Roles ="Admin")]
       public async Task<ActionResult<SecretaryResponseDTO>> AddSecretary(SecretaryRequstDTO secretary, CancellationToken cancellationToken)
        {

          var res= await _secreratyService.AddSecretary(secretary, cancellationToken);
            return Created("created secretary",res);
        }

        [HttpGet("get {id}")]
       public async Task<ActionResult<SecretaryResponseDTO?>> GetSecretaryById(int id, CancellationToken cancellationToken)
        {
          var res= await _secreratyService.GetSecretaryById(id, cancellationToken);
            return Ok(res);
        }

        // Admin-only roster listing - every secretary in the system.
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<SecretaryResponseDTO>>> GetAllSecretaries(CancellationToken cancellationToken)
        {
            var res = await _secreratyService.GetAllSecretaries(cancellationToken);
            return Ok(res);
        }


    }
}
