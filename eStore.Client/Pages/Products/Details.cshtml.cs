using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using eStore.Data.Entity;
using System.Net.Http.Headers;
using System.Text.Json;
using eStore.Service.Models.ResponseModels;

namespace eStore.Client.Pages.Products
{
    public class DetailsModel : PageModel
    {
        private readonly HttpClient client = null;
        private string ProductApiUrl = "";
        public ProductResponseModel Product { get; set; } = default!;

        public DetailsModel()
        {
            client = new HttpClient();
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            client.DefaultRequestHeaders.Accept.Add(contentType);
            ProductApiUrl = "https://localhost:7248/api/products";
        }


        public async Task<IActionResult> OnGetAsync(int? id)
        {
            HttpResponseMessage response = await client.GetAsync($"{ProductApiUrl}/{id}");
            string strData = await response.Content.ReadAsStringAsync();

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
            };

            this.Product = JsonSerializer.Deserialize<ProductResponseModel>(strData, options);
            if(Product == null)
            {
                return NotFound();
            }
            return Page();
        }
    }
}
