using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using eStore.Data.Entity;
using eStore.Service.Models.ResponseModels;
using System.Net.Http.Headers;
using System.Text.Json;
using eStore.Client.Helpers;
using eStore.Service.Models.RequestModels;

namespace eStore.Client.Pages.Products
{
    public class CreateModel : PageModel
    {
        private readonly HttpClient client = null;
        private string ProductApiUrl = "";
        private string CategoryApiUrl = "";

        public List<SelectListItem> Categories { get; set; } = default!;

        [BindProperty]
        public CreateProductRequestModel Product { get; set; } = default!;



        public CreateModel()
        {
            client = new HttpClient();
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            client.DefaultRequestHeaders.Accept.Add(contentType);
            ProductApiUrl = "https://localhost:7248/api/products";
            CategoryApiUrl = "https://localhost:7248/api/categories";
        }

        public async Task<IActionResult> OnGetAsync()
        {
            HttpResponseMessage response = await client.GetAsync(CategoryApiUrl);
            string strData = await response.Content.ReadAsStringAsync();

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            List<CategoryResponseModel> cateList = JsonSerializer.Deserialize<List<CategoryResponseModel>>(strData, options);
            Categories = cateList.Select(x => new SelectListItem
            {
                Text = x.CategoryName,
                Value = x.CategoryId.ToString(),
            }).ToList();

            SessionHelper.SetObjectAsJson(HttpContext.Session, "CateList", this.Categories);
            return Page();
        }



        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
				var jwtToken = SessionHelper.GetObjectFromJson<string>(HttpContext.Session, "JWTToken");
				client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);
				HttpResponseMessage response = await client.PostAsJsonAsync(ProductApiUrl, Product);
                if(response.IsSuccessStatusCode)
                {
                    var rs = await response.Content.ReadFromJsonAsync<ProductResponseModel>();
                    return RedirectToPage("./Index");
                }
            }
            return Page();
        }
    }
}
