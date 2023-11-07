using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using eStore.Data.Entity;
using eStore.Service.Models.ResponseModels;
using System.Net.Http.Headers;
using System.Text.Json;

namespace eStore.Client.Pages.Products
{
    public class IndexModel : PageModel
    {
        private readonly HttpClient client = null;
        private string ProductApiUrl = "";

        public string CurrentFilter { get; set; }

        public IList<ProductResponseModel> Product { get; set; } = default!;
        public IndexModel()
        {
            client = new HttpClient();
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            client.DefaultRequestHeaders.Accept.Add(contentType);
            ProductApiUrl = "https://localhost:7248/api/products";
        }
        public async Task<IActionResult> OnGetProductsAsync(int currentPage, int pageSize, string search)
        {
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            string apiUrl = ProductApiUrl;

            if (!String.IsNullOrEmpty(search))
            {
                decimal price = 0;
                bool isNumber = decimal.TryParse(search, out price);

                apiUrl +=
                    $"?filter=contains(productName,'{search}'){(isNumber ? $" or unitPrice eq {price}" : "")}";
            }

            HttpResponseMessage response = await client.GetAsync(apiUrl);

            if (response.IsSuccessStatusCode)
            {
                string strData = await response.Content.ReadAsStringAsync();
                var products = JsonSerializer.Deserialize<List<ProductResponseModel>>(strData, options);
                int totalItems = products.Count();
                var pagedProducts = products
                    .OrderByDescending(a => a.ProductId)
                    .Skip((currentPage - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                return new JsonResult(new { products = pagedProducts, totalItems });
            }
            else
            {
                return new JsonResult(null);
            }
        }


        //public async Task OnGetAsync()
        //{
        //    HttpResponseMessage response = await client.GetAsync(ProductApiUrl);
        //    string strData = await response.Content.ReadAsStringAsync();

        //    var options = new JsonSerializerOptions
        //    {
        //        PropertyNameCaseInsensitive = true,
        //    };
        //    Product = JsonSerializer.Deserialize<List<ProductResponseModel>>(strData, options);
        //}
    }
}
