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

namespace eStore.Client.Pages.Orders
{
    public class DetailsModel : PageModel
    {
        private HttpClient client = null;
        private string OrderApiUrl = "";

        public OrderResponseModel Order { get; set; }
        public DetailsModel()
        {
            client = new HttpClient();
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            client.DefaultRequestHeaders.Accept.Add(contentType);
            OrderApiUrl = "https://localhost:7248/api/orders";
        }

      

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            HttpResponseMessage response = await client.GetAsync($"{OrderApiUrl}/{id}");

            string strData = await response.Content.ReadAsStringAsync();

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
            };

            Order = JsonSerializer.Deserialize<OrderResponseModel>(strData, options);

            if(Order == null)
            {
                return NotFound();
            }
            return Page();
        }
    }
}
