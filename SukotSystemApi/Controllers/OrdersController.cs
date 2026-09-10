using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SukotSystemCore.DTOs.AdminDTOs;
using SukotSystemCore.DTOs.Common;
using SukotSystemCore.DTOs.CustomerDTOs.Request;
using SukotSystemCore.DTOs.CustomerDTOs.Response;
using SukotSystemCore.DTOs.RabbiDTOs;
using SukotSystemCore.DTOs.RabbiDTOs.Request;
using SukotSystemCore.Enums;
using SukotSystemCore.Services;
using System.Security.Claims;
using System.Threading;

namespace SukotSystemApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrdersController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        // ---- Customer ----

        // Customer-only (Part A: only a Customer creates requests). CustomerId comes from the
        // JWT, never the body - OrderCreateDTO deliberately has no such field.
        [Authorize(Roles = "Customer,Secretary")]
        [HttpPost]
        public async Task<ActionResult<OrderResponseDTO>> CreateOrder([FromBody] OrderCreateDTO order, CancellationToken cancellationToken)
        {
            OrderResponseDTO res;
            if(User.IsInRole("Customer"))
            {
             res = await _orderService.CreateOrder(GetCurrentUserId(), order, cancellationToken);

            }
            else
            {
                 res = await _orderService.CreateOrder(null, order, cancellationToken);

            }
            return Created("order created succes",res);
        }

        [Authorize(Roles = "Customer")]
        [HttpGet("mine")]
        public async Task<ActionResult<PagedResultDTO<OrderResponseDTO>>> GetMyOrders(
            [FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken cancellationToken = default)
        {
            var res = await _orderService.GetMyOrders(GetCurrentUserId(), page, pageSize, cancellationToken);
            return Ok(res);
        }

        [Authorize(Roles = "Customer,Secretary")]
        [HttpGet("mine/{id}")]
        public async Task<ActionResult<OrderResponseDTO>> GetMyOrderById([FromRoute] int id, CancellationToken cancellationToken)
        {
            var res = await _orderService.GetMyOrderById(GetCurrentUserId(), id, cancellationToken);
            return Ok(res);
        }

        // Only while the request is still New (enforced in the Service) - once a rabbi has
        // claimed it, the customer can no longer change it. No Cancel action in this pass.
        [Authorize(Roles = "Customer,Secretary")]
        [HttpPut("mine/{id}")]
        public async Task<ActionResult<OrderResponseDTO>> UpdateMyOrder([FromRoute] int id, [FromBody] OrderCreateDTO order, CancellationToken cancellationToken)
        {
            var res = await _orderService.UpdateMyOrder(GetCurrentUserId(), id, order, cancellationToken);
            return Ok(res);
        }

        // ---- Rabbi ----

        // Unclaimed requests, optionally narrowed to one city - "filterable either by their own
        // city or by any city they choose to browse" (Part A). No filter = every New request
        // system-wide.
        [Authorize(Roles = "Rabbi")]
        [HttpGet("available")]
        public async Task<ActionResult<PagedResultDTO<OrderRabbiDTO>>> GetAvailableOrders(
            [FromQuery] int? cityId, [FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken cancellationToken = default)
        {
            var res = await _orderService.GetAvailableOrders(cityId, page, pageSize, cancellationToken);
            return Ok(res);
        }

        [Authorize(Roles = "Rabbi")]
        [HttpGet("my-claims")]
        public async Task<ActionResult<PagedResultDTO<OrderRabbiDTO>>> GetMyHandledOrders(
            [FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken cancellationToken = default)
        {
            var res = await _orderService.GetMyHandledOrders(GetCurrentUserId(), page, pageSize, cancellationToken);
            return Ok(res);
        }

        // THE Part C action. A DbUpdateConcurrencyException from a lost race propagates all the
        // way to ExceptionHandlingMiddleware untouched -> 409, Warning-logged (see
        // OrderService.ClaimOrder for where the richer, order/rabbi-specific Warning is logged).
        [Authorize(Roles = "Rabbi")]
        [HttpPost("{id}/claim")]
        public async Task<ActionResult<OrderRabbiDTO>> ClaimOrder([FromRoute] int id, CancellationToken cancellationToken)
        {
            var res = await _orderService.ClaimOrder(GetCurrentUserId(), id, cancellationToken);
            return Ok(res);
        }

        // Only the rabbi who claimed this exact order - enforced in the Service (403 otherwise).
        [Authorize(Roles = "Rabbi")]
        [HttpPatch("{id}/complete")]
        public async Task<ActionResult<OrderRabbiDTO>> CompleteOrder([FromRoute] int id, [FromBody] OrderCompleteDTO complete, CancellationToken cancellationToken)
        {
            var res = await _orderService.CompleteOrder(GetCurrentUserId(), id, complete, cancellationToken);
            return Ok(res);
        }

        // ---- Admin ----

        [Authorize(Roles = "Admin,Secretary")]
        [HttpGet]
        public async Task<ActionResult<PagedResultDTO<OrderAdminDTO>>> GetAllOrders(
            [FromQuery] OrderStatus? status, [FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken cancellationToken = default)
        {
            var res = await _orderService.GetAllOrders(page, pageSize, status, cancellationToken);
            return Ok(res);
        }

        [Authorize(Roles = "Admin,Secretary")]
        [HttpGet("{id}")]
        public async Task<ActionResult<OrderAdminDTO>> GetOrderById([FromRoute] int id, CancellationToken cancellationToken)
        {
            var res = await _orderService.GetOrderAdminViewById(id, cancellationToken);
            return Ok(res);
        }

        // Shared across Customer/Rabbi tokens alike - both carry the caller's own id under the
        // same NameIdentifier claim (see AuthHelper.CreateToken); which table that id belongs to
        // is decided by the [Authorize(Roles = "...")] on each action above, never by this method.
        private int GetCurrentUserId()
        {
            var idClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                return int.Parse(idClaim!);
           
        }
    }
}
