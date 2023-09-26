using eStore.Client.Helpers;
using eStore.Service.Models.RequestModels;
using eStore.Service.Models.ResponseModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Headers;

namespace eStore.Client.Pages.Auth
{
    public class RegisterModel : PageModel
    {
        private readonly HttpClient client = null;
        private string MemberApiUrl = "";

        [BindProperty]
        public CreateMemberRequestModel Member { get; set; } = default!;
        public RegisterModel()
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
                    return RedirectToPage("/Home/Index");
                }
            }
            return Page();
        }
    }
}
