using eStore.Service.Models.RequestModels;
using eStore.Service.Models.ResponseModels;
using eStore.Service.Service.InterfaceService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace eStore.API.Controllers
{
    [Route("api/orders")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _service;

        public OrdersController(IOrderService service)
        {
            _service = service;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<OrderResponseModel>>> GetOrders()
        {
            var rs = await _service.GetOrders();
            return Ok(rs);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,Customer")]
        public async Task<ActionResult<OrderResponseModel>> GetOrderById(int id)
        {
            var rs = await _service.GetOrderById(id);
            return Ok(rs);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Customer")]
        public async Task<ActionResult<OrderResponseModel>> CreateOrder([FromBody] CreateOrderRequestModel request)
        {
            var rs = await _service.CreateOrder(request);
            return Ok(rs);
        }
    }
}
