using eStore.Service.Models.ResponseModels;
using eStore.Service.Service.InterfaceService;
using Microsoft.AspNetCore.Mvc;

namespace eStore.API.Controllers
{
    [Route("api/categories")]  
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryService _service;

        public CategoriesController(ICategoryService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CategoryResponseModel>>> GetCategories()
        {
            var rs = await _service.GetCategories();
            return Ok(rs);
        }
    }
}
