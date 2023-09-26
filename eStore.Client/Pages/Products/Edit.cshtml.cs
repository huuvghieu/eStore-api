using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using eStore.Data.Entity;
using System.Net.Http.Headers;
using eStore.Service.Models.ResponseModels;
using System.Text.Json;
using eStore.Client.Helpers;

namespace eStore.Client.Pages.Products
{
    public class EditModel : PageModel
    {
        private readonly HttpClient client = null;
        private string ProductApiUrl = "";
        private string CategoryApiUrl = "";

        [BindProperty]
        public ProductResponseModel Product { get; set; } = default!;

        public List<SelectListItem> Categories { get; set; } = default!;

        public EditModel()
        {
            client = new HttpClient();
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            client.DefaultRequestHeaders.Accept.Add(contentType);
            ProductApiUrl = "https://localhost:7248/api/products";
            CategoryApiUrl = "https://localhost:7248/api/categories";
        }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            HttpResponseMessage responseProduct = await client.GetAsync($"{ProductApiUrl}/{id}");
            string strDataProduct = await responseProduct.Content.ReadAsStringAsync();

            HttpResponseMessage responseCate = await client.GetAsync(CategoryApiUrl);
            string strDataCate = await responseCate.Content.ReadAsStringAsync();

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
            };
            var cateList = JsonSerializer.Deserialize<List<CategoryResponseModel>>(strDataCate, options);
            Categories = cateList.Select(x => new SelectListItem
            {
                Text = x.CategoryName,
                Value = x.CategoryId.ToString(),
            }).ToList();
            SessionHelper.SetObjectAsJson(HttpContext.Session, "CateList", this.Categories);


            this.Product = JsonSerializer.Deserialize<ProductResponseModel>(strDataProduct, options);
            if (Product == null)
            {
                return NotFound();
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
				var jwtToken = SessionHelper.GetObjectFromJson<string>(HttpContext.Session, "JWTToken");
				client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);
				HttpResponseMessage response = await client.PutAsJsonAsync($"{ProductApiUrl}/{Product.ProductId}", Product);
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
