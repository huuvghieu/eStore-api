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
using eStore.Client.Helpers;

namespace eStore.Client.Pages.Orders
{
    public class IndexModel : PageModel
    {
        private HttpClient client = null;
        private string OrderApiUrl = "";

        public List<OrderResponseModel> Orders { get; set; }
        public IndexModel()
        {
            client = new HttpClient();
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            client.DefaultRequestHeaders.Accept.Add(contentType);
            OrderApiUrl = "https://localhost:7248/api/orders";
        }

        public async Task OnGetAsync()
        {
			var jwtToken = SessionHelper.GetObjectFromJson<string>(HttpContext.Session, "JWTToken");
			client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);
			HttpResponseMessage response = await client.GetAsync(OrderApiUrl);
            string strData = await response.Content.ReadAsStringAsync();

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
            };

            Orders = JsonSerializer.Deserialize<List<OrderResponseModel>>(strData, options);
        }
    }
}
