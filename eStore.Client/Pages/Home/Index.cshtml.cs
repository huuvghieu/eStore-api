using eStore.Service.Models.ResponseModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Headers;
using System.Text.Json;

namespace eStore.Client.Pages.Home
{
    public class IndexModel : PageModel
    {
		private readonly HttpClient client = null;
		private string ProductApiUrl = "";

		public IList<ProductResponseModel> Product { get; set; } = default!;
		public IndexModel()
		{
			client = new HttpClient();
			var contentType = new MediaTypeWithQualityHeaderValue("application/json");
			client.DefaultRequestHeaders.Accept.Add(contentType);
			ProductApiUrl = "https://localhost:7248/api/products";
		}


		public async Task OnGetAsync()
		{
			HttpResponseMessage response = await client.GetAsync(ProductApiUrl);
			string strData = await response.Content.ReadAsStringAsync();

			var options = new JsonSerializerOptions
			{
				PropertyNameCaseInsensitive = true,
			};
			Product = JsonSerializer.Deserialize<List<ProductResponseModel>>(strData, options);
		}
	}
}
