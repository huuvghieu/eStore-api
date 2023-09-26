using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using eStore.Data.Entity;
using System.Net.Http.Headers;
using eStore.Service.Models.ResponseModels;
using System.Text.Json;
using eStore.Client.Helpers;

namespace eStore.Client.Pages.Products
{
    public class DeleteModel : PageModel
    {
        private readonly HttpClient client = null;
        private string ProductApiUrl = "";

        [BindProperty]
        public ProductResponseModel Product { get; set; } = default!;
        public DeleteModel()
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

            Product = JsonSerializer.Deserialize<ProductResponseModel>(strData, options);
            if(Product == null)
            {
                return NotFound();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (ModelState.IsValid)
			{
				var jwtToken = SessionHelper.GetObjectFromJson<string>(HttpContext.Session, "JWTToken");
				client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);
				HttpResponseMessage response = await client.DeleteAsync($"{ProductApiUrl}/{id}");
                if (response.IsSuccessStatusCode)
                {
                    var rs = await response.Content.ReadFromJsonAsync<ProductResponseModel>();
                    return RedirectToPage("./Index");
                }
            }
            return Page();
        }
    }
}
