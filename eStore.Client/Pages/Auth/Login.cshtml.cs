using eStore.Client.Helpers;
using eStore.Service.Models.RequestModels;
using eStore.Service.Models.ResponseModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Json;

namespace eStore.Client.Pages.Auth
{
    public class LoginModel : PageModel
    {
        private readonly HttpClient client = null;
        private string AuthApiUrl = "";

        [BindProperty]
        public LoginRequestModel LoginRequest { get; set; }

        public LoginResponseModel LoginResponse { get; set; }

        public LoginModel()
        {
            client = new HttpClient();
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            client.DefaultRequestHeaders.Accept.Add(contentType);
            AuthApiUrl = "https://localhost:7248/api/authuser/login";

		}
        public async Task<IActionResult> OnPostAsync()
        {
            HttpResponseMessage response = await client.PostAsJsonAsync(AuthApiUrl, LoginRequest);
            if(response.IsSuccessStatusCode)
            {
                string strData = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                };
                LoginResponse = JsonSerializer.Deserialize<LoginResponseModel>(strData, options);
                if(LoginResponse != null)
                {
                    var handler = new JwtSecurityTokenHandler();
                    var jwt = handler.ReadJwtToken(LoginResponse.Token);

                    var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
                    identity.AddClaim(new Claim(ClaimTypes.Email, jwt.Claims.FirstOrDefault(x => x.Type == "email").Value));
                    identity.AddClaim(new Claim(ClaimTypes.Role, jwt.Claims.FirstOrDefault(x => x.Type == "role").Value));
                    var principal = new ClaimsPrincipal(identity);
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
                    SessionHelper.SetObjectAsJson(HttpContext.Session, "JWTToken", LoginResponse.Token);

                    //check role
                    var checkRole = jwt.Claims.FirstOrDefault(x => x.Type == "role").Value;
                    if (checkRole.Equals("Admin"))
                    {
                        return RedirectToPage("/Products/Index");
                    }
                    else
                    {
                        return RedirectToPage("/Home/Index");
                    }
                }
            }
            return Page();
        }

        public async Task<IActionResult> OnPostLogoutAsync()
        {
            var scheme = CookieAuthenticationDefaults.AuthenticationScheme;
            await HttpContext.SignOutAsync(scheme);
            return RedirectToPage("/Index");
        }
    }
}
