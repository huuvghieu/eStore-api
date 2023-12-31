﻿using eStore.Service.Models.RequestModels;
using eStore.Service.Models.ResponseModels;
using eStore.Service.Service.ImplementService;
using eStore.Service.Service.InterfaceService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using System.Net;

namespace eStore.API.Controllers
{
    [Route("api/products")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _service;

        public ProductsController(IProductService service)
        {
            _service = service;
        }

        [AllowAnonymous]
        [HttpGet]
        [EnableQuery]
        public async Task<ActionResult<IEnumerable<ProductResponseModel>>> GetProducts()
        {
            var rs = await _service.GetProducts();
            return Ok(rs);
        }

        [AllowAnonymous]
        [HttpGet("{id}")]
        [EnableQuery]
        public async Task<ActionResult<ProductResponseModel>> GetProductById(int id)
        {
            var rs = await _service.GetProductById(id);
            return Ok(rs);
        }

        /// <summary>
        /// Search Product
        /// </summary>
        /// <param name="searchString"></param>
        /// <returns></returns>
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<ProductResponseModel>>> SearchProduct([FromQuery] string searchString)
        {
            var rs = await _service.SearchProduct(searchString);
            return Ok(rs);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ProductResponseModel>> CreateProduct([FromBody] CreateProductRequestModel request)
        {
            var rs = _service.InsertProduct(request);
            return Ok(rs);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ProductResponseModel>> UpdateProduct(int id, [FromBody] UpdateProductRequestModel request)
        {
            var rs = (await _service.UpdateProduct(id, request));
            return Ok(rs);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ProductResponseModel>> DeleteProduct(int id)
        {
            var rs = (await _service.DeleteProduct(id));
            return Ok(rs);
        }

        [AllowAnonymous]
        [HttpGet("{id}/cart")]
        public async  Task<ActionResult<OrderDetailRequestModel>> GetCartItemsByProductId(int id)
        {
            var cart = _service.GetCartItemsByProductId(id);
            if (cart != null)
            {
                return Ok(cart);
            }
            return NotFound("Can not find this product with id " + id);
        }
    }
}
