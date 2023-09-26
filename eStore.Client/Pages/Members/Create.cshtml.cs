using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using eStore.Data.Entity;
using eStore.Service.Models.RequestModels;
using System.Net.Http.Headers;
using eStore.Client.Helpers;
using eStore.Service.Models.ResponseModels;

namespace eStore.Client.Pages.Members
{
    public class CreateModel : PageModel
    {
		private readonly HttpClient client = null;
		private string MemberApiUrl = "";

		[BindProperty]
		public CreateMemberRequestModel Member { get; set; } = default!;
		public CreateModel()
        {
			client = new HttpClient();
			var contentType = new MediaTypeWithQualityHeaderValue("application/json");
			client.DefaultRequestHeaders.Accept.Add(contentType);
			MemberApiUrl = "https://localhost:7248/api/members";
		}

        public IActionResult OnGet()
        {
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
			if (ModelState.IsValid)
			{
				var jwtToken = SessionHelper.GetObjectFromJson<string>(HttpContext.Session, "JWTToken");
				client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);
				HttpResponseMessage response = await client.PostAsJsonAsync(MemberApiUrl, Member);
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
