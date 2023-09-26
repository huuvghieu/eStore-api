using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using eStore.Data.Entity;
using eStore.Service.Models.ResponseModels;
using System.Net.Http.Headers;
using eStore.Client.Helpers;
using System.Text.Json;

namespace eStore.Client.Pages.Members
{
    public class EditModel : PageModel
    {
		private readonly HttpClient client = null;
		private string MemberApiUrl = "";

		[BindProperty]
		public MemberReponseModel Member { get; set; }
		public EditModel()
        {
			client = new HttpClient();
			var contentType = new MediaTypeWithQualityHeaderValue("application/json");
			client.DefaultRequestHeaders.Accept.Add(contentType);
			MemberApiUrl = "https://localhost:7248/api/members";
		}


        public async Task<IActionResult> OnGetAsync(int? id)
        {
			var jwtToken = SessionHelper.GetObjectFromJson<string>(HttpContext.Session, "JWTToken");
			client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);
			HttpResponseMessage response = await client.GetAsync($"{MemberApiUrl}/{id}");
			string strData = await response.Content.ReadAsStringAsync();

			var options = new JsonSerializerOptions
			{
				PropertyNameCaseInsensitive = true,
			};
		
			this.Member = JsonSerializer.Deserialize<MemberReponseModel>(strData, options);
			if (Member == null)
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
				HttpResponseMessage response = await client.PutAsJsonAsync($"{MemberApiUrl}/{Member.MemberId}", Member);
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
