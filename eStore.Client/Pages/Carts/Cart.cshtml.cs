using eStore.Client.Helpers;
using eStore.Service.Models.RequestModels;
using eStore.Service.Models.ResponseModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Json;


namespace eStore.Client.Pages.Carts
{
    public class CartModel : PageModel
    {
        private readonly HttpClient client = null;
        private string UserApiUrl = "";
        private string ProductApiUrl = "";
        private string OrderApiUrl = "";

        public List<OrderDetailRequestModel> CartItems { get; set; }
        public List<MemberReponseModel> Members { get; set; }
        public decimal Total { get; set; }

        public CartModel()
        {
            this.CartItems = new List<OrderDetailRequestModel>();

            client = new HttpClient();
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            client.DefaultRequestHeaders.Accept.Add(contentType);
            UserApiUrl = "https://localhost:7248/api/members";
            ProductApiUrl = "https://localhost:7248/api/products";
            OrderApiUrl = "https://localhost:7248/api/orders";
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var jwtToken = SessionHelper.GetObjectFromJson<string>(HttpContext.Session, "JWTToken");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            HttpResponseMessage response = await client.GetAsync(UserApiUrl);
            if (response.IsSuccessStatusCode)
            {
                string strData = await response.Content.ReadAsStringAsync();
                var members = System.Text.Json.JsonSerializer.Deserialize<List<MemberReponseModel>>(strData, options);
                Members = members;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostCheckout(int userId)
        {
            try
            {
                var jwtToken = SessionHelper.GetObjectFromJson<string>(HttpContext.Session, "JWTToken");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                //Get cart from session
                var sessionCart = HttpContext.Session.GetString("CART");
                List<OrderDetailRequestModel> cartItems = JsonConvert
                    .DeserializeObject<List<OrderDetailRequestModel>>(sessionCart);

                //When cart is empty and user click checkout button
                if (cartItems.Count <= 0)
                {
                    TempData["ErrorMessage"] = "Your cart is empty, cannot checkout!";
                    return RedirectToPage("./Cart", new { Message = "ErrorMessage" });
                }
                //When cart is not empty
                else
                {
                    decimal total = 0;
                    foreach (var item in cartItems)
                    {
                        //Sum the total based on the quantity and unit price of the cart items
                        total += (item.UnitPrice * item.Quantity);

                        ProductApiUrl = ProductApiUrl + "/" + item.ProductId;
                        HttpResponseMessage response = await client.GetAsync(ProductApiUrl);
                        if (response.IsSuccessStatusCode)
                        {
                            var strData = await response.Content.ReadAsStringAsync();
                            ProductResponseModel product = System.Text.Json.JsonSerializer.Deserialize<ProductResponseModel>(strData, options);

                            //If the user input the quantity of product items that
                            //greater than the unit in stock of this product and click update button
                            if (item.Quantity > product.UnitsInStock)
                            {
                                TempData["ErrorMessage"] = "Do not have enough product in stock, " +
                                    "try again with lower quantity";
                                return RedirectToPage("./Cart", new { Message = "ErrorMessage" });
                            }
                        }
                        var idUser = 0;

                        CreateOrderRequestModel order = new CreateOrderRequestModel();
                        order.MemberId = idUser;
                        order.RequiredDate = DateTime.Now;

                        order.OrderDetails = new List<OrderDetailRequestModel>();
                        foreach (OrderDetailRequestModel details in cartItems)
                        {
                            order.OrderDetails.Add(new OrderDetailRequestModel
                            {
                                ProductId = details.ProductId,
                                UnitPrice = details.UnitPrice,
                                Quantity = details.Quantity,
                                Discount = details.Discount,
                            });
                        }

                        HttpResponseMessage responseCreate = await client.PostAsJsonAsync(OrderApiUrl, order);
                        if (responseCreate.IsSuccessStatusCode)
                        {
                            //Update the cart session
                            cartItems = new List<OrderDetailRequestModel>();
                            string saveCart = JsonConvert.SerializeObject(cartItems);
                            HttpContext.Session.SetString("CART", saveCart);
                            if (User.IsInRole("Admin"))
                            {
                                TempData["SuccessMessage"] = "Create order successfully";
                                return RedirectToPage("./Cart", new { Message = "SuccessMessage" });
                            }
                            else
                            {
                                TempData["SuccessMessage"] = "Checkout successfully";
                                return RedirectToPage("./Cart", new { Message = "SuccessMessage" });
                            }
                        }
                    }
                }
            }
            catch
            {
                return Page();
            }
            return Page();
        }

        public async Task<IActionResult> OnPostRemoveProduct(int productId)
        {
            try
            {
                var sessionCart = HttpContext.Session.GetString("CART");
                List<OrderDetailRequestModel> cartItems = JsonConvert
                    .DeserializeObject<List<OrderDetailRequestModel>>(sessionCart);
                var checkExistProduct = cartItems
                        .Find(x => x.ProductId == productId);
                if (checkExistProduct is not null)
                {
                    cartItems.Remove(checkExistProduct);
                }
                string saveCart = JsonConvert.SerializeObject(cartItems);
                HttpContext.Session.SetString("CART", saveCart);
                return await OnGetAsync();
            }
            catch (Exception e)
            {
                throw new Exception();
            }
        }

        public async Task<IActionResult> OnPostUpdateProductQuantity(int productId, int quantity)
        {
            try
            {
                var sessionCart = HttpContext.Session.GetString("CART");
                List<OrderDetailRequestModel> cartItems = JsonConvert
                    .DeserializeObject<List<OrderDetailRequestModel>>(sessionCart);

                var checkExistProduct = cartItems
                        .Find(x => x.ProductId == productId);
                if (checkExistProduct is not null)
                {
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };
                    ProductApiUrl = ProductApiUrl + "/" + productId + "/cart";

                    HttpResponseMessage response = await client.GetAsync(ProductApiUrl);
                    if (response.IsSuccessStatusCode)
                    {
                        var strData = await response.Content.ReadAsStringAsync();
                        var cartItemModels = System.Text.Json.JsonSerializer.Deserialize<OrderDetailRequestModel>(strData, options);
                        if (quantity <= cartItemModels.Quantity)
                        {
                            checkExistProduct.Quantity = quantity;
                        }
                        else
                        {
                            TempData["ErrorMessage"] = "Do not have enough product in stock, try again with lower quantity";
                            return RedirectToPage("./Cart", new { Message = "ErrorMessage" });
                        }
                    }

                }
                string saveCart = JsonConvert.SerializeObject(cartItems);
                HttpContext.Session.SetString("CART", saveCart);
                TempData["SuccessMessage"] = "Update quantity successfully";
                return RedirectToPage("./Cart", new { Message = "SuccessMessage" });
            }
            catch (Exception e)
            {
                throw new Exception();
            }
        }
    }
}
