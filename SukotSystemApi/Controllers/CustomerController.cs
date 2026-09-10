using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using SukotSystemCore.DTOs.CustomerDTOs.Request;
using SukotSystemCore.DTOs.CustomerDTOs.Response;
using SukotSystemCore.DTOs.RabbiDTOs;
using SukotSystemCore.Models;
using SukotSystemCore.Services;
using SukotSystemService;
using System.Threading;

namespace SukotSystemApi.Contorollers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerService _customerService;

        public CustomerController(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<ActionResult<CustomerResponseDTO>> AddCustomer([FromBody] CustomerRegisterDTO customer, CancellationToken cancellationToken)
        {
            var res = await _customerService.AddCustomer(customer, cancellationToken);
            return CreatedAtAction(nameof(GetCustomerById), new { id = res.Id }, res);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<CustomerResponseDTO>> UpdateCustomer([FromRoute] int id, [FromBody] CustomerRegisterDTO customer, CancellationToken cancellationToken)
        {
            var res = await _customerService.UpdateCustomer(id, customer, cancellationToken);
            return Ok(res);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<CustomerResponseDTO>> DeleteCustomer([FromRoute] int id, CancellationToken cancellationToken)
        {
            var res = await _customerService.DeleteCustomer(id, cancellationToken);
            return Ok(res);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CustomerResponseDTO>> GetCustomerById([FromRoute] int id, CancellationToken cancellationToken)
        {
            var res = await _customerService.GetCustomerById(id, cancellationToken);
            return Ok(res);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CustomerResponseDTO>>> GetAllCustomers(CancellationToken cancellationToken)
        {
            var res = await _customerService.GetAllCustomers(cancellationToken);
            return Ok(res);
        }

        [HttpGet("{id}/rabbi-view")]
        public async Task<ActionResult<CustomerRabbiDTO>> GetCustomerRaabiDTOById([FromRoute] int id, CancellationToken cancellationToken)
        {
            var res = await _customerService.GetCustomerRaabiDTOById(id, cancellationToken);
            return Ok(res);
        }
    }
}
