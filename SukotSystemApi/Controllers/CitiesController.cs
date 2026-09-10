using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SukotSystemCore.DTOs.AdminDTOs.Request;
using SukotSystemCore.DTOs.Common;
using SukotSystemCore.Services;
using System.Collections.Generic;
using System.Threading;

namespace SukotSystemApi.Controllers
{
    // City is shared lookup data - CityDTO's own comment explains why it lives in Common:
    // Customer (picking a city for an Order), Rabbi (filtering/browsing available Orders by
    // city) and Admin (managing the list) all read the exact same shape. Any authenticated
    // role can read it; only an Admin writes to it - the same split Part A gives Rabbi
    // ("only an Admin adds/removes rabbis") extended to the reference data Rabbis/Orders hang
    // off of.
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CitiesController : ControllerBase
    {
        private readonly ICityService _cityService;

        public CitiesController(ICityService cityService)
        {
            _cityService = cityService;
        }

        // No pagination here on purpose (unlike Rabbi/Order's real Skip/Take pagination) -
        // City is seeded reference data every role needs in full at once, for a dropdown, not
        // a paged list. Requirement 6's pagination is already demonstrated elsewhere.
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CityDTO>>> GetAllCities(CancellationToken cancellationToken)
        {
            var res = await _cityService.GetAllCities(cancellationToken);
            return Ok(res);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CityDTO>> GetCityById([FromRoute] int id, CancellationToken cancellationToken)
        {
            var res = await _cityService.GetCityById(id, cancellationToken);
            return Ok(res);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult<CityDTO>> AddCity([FromBody] CityCreateDTO city, CancellationToken cancellationToken)
        {
            var res = await _cityService.AddCity(city, cancellationToken);
            return CreatedAtAction(nameof(GetCityById), new { id = res.Id }, res);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<ActionResult<CityDTO>> UpdateCity([FromRoute] int id, [FromBody] CityCreateDTO city, CancellationToken cancellationToken)
        {
            var res = await _cityService.UpdateCity(id, city, cancellationToken);
            return Ok(res);
        }

        // Blocked with 409 (ConflictException, translated from the database's own FK-restrict
        // violation) if any Rabbi or Order still references this City - see
        // CityRepository.DeleteCity for exactly which relationships are checked.
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCity([FromRoute] int id, CancellationToken cancellationToken)
        {
            await _cityService.DeleteCity(id, cancellationToken);
            return NoContent();
        }
    }
}
