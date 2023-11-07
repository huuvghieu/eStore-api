using eStore.Service.Models.RequestModels;
using eStore.Service.Models.ResponseModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;
using static System.Net.WebRequestMethods;

namespace eStore.Client.Pages.Carts
{
    public class IndexModel : PageModel
    {
        private readonly HttpClient client = null;
        private string ProductApiUrl = "";
        public List<OrderDetailRequestModel> CartItems { get; set; }
        public List<ProductResponseModel> Product { get; set; }

        public IndexModel()
        {
            client = new HttpClient();
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            client.DefaultRequestHeaders.Accept.Add(contentType);
            ProductApiUrl = "https://localhost:7248/api/products";
            this.CartItems = new List<OrderDetailRequestModel>();
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            HttpResponseMessage response = await client.GetAsync(ProductApiUrl);
            if (response.IsSuccessStatusCode)
            {
                string strData = await response.Content.ReadAsStringAsync();
                var products = JsonSerializer.Deserialize<List<ProductResponseModel>>(strData, options);
                Product = products;
            }

            return Page();
        }

        public async Task<IActionResult> OnGetAddToCartAsync(int productId, int quantity)
        {
            OrderDetailRequestModel cartItemsModel = new OrderDetailRequestModel();
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            ProductApiUrl = ProductApiUrl + "/" + productId + "/cart";
            HttpResponseMessage response = await client.GetAsync(ProductApiUrl);
            if (response.IsSuccessStatusCode)
            {
                var strData = await response.Content.ReadAsStringAsync();
                cartItemsModel = JsonSerializer.Deserialize<OrderDetailRequestModel>(strData, options);
                try
                {
                    //Get the cart from the session and if exist cart items already
                    var sessionCart = HttpContext.Session.GetString("CART");

                    //Change the json string to <List<CartItemsDTO>
                    List<OrderDetailRequestModel> cartItems = JsonConvert
                        .DeserializeObject<List<OrderDetailRequestModel>>(sessionCart);

                    //Searches the list of products in the cart for any product
                    //has the same id as the product requested to be added to the cart 
                    var checkExistProduct = cartItems
                        .Find(x => x.ProductId == cartItemsModel.ProductId);

                    //If found this product
                    if (checkExistProduct is not null)
                    {
                        //Check the unit in stock of this product
                        if (checkExistProduct.Quantity > cartItemsModel.Quantity)
                        {
                            var exceedQuantity = new
                            {
                                success = true,
                                message = "Do not have enough product in stock."
                            };
                            return new JsonResult(exceedQuantity);
                        }
                        //If enough the quantity
                        else
                        {
                            checkExistProduct.Quantity += 1;
                        }
                    }

                    //If not found the product, check if the remaining quantity of
                    //product in stock is greater than 0
                    else if (cartItemsModel.Quantity > 0)
                    {
                        //The product in the cart will be set with a quantity of 1
                        //and added to the list of items in the cart
                        cartItemsModel.Quantity = 1;
                        cartItems.Add(cartItemsModel);
                    }

                    //Case do not enough product in stock
                    else
                    {
                        var exceedQuantity = new
                        {
                            success = true,
                            message = "Do not have enough product in stock."
                        };
                    }

                    //Update the cart session
                    string saveCart = JsonConvert.SerializeObject(cartItems);
                    HttpContext.Session.SetString("CART", saveCart);

                }
                catch
                {
                    //if do not exist the cart before, new List<OrderDetailCreateModel>
                    List<OrderDetailRequestModel> carts = new List<OrderDetailRequestModel>();
                    if (cartItemsModel.Quantity > 0)
                    {
                        cartItemsModel.Quantity = 1;
                        carts.Add(cartItemsModel);
                    }
                    else
                    {
                        var exceedQuantity = new
                        {
                            success = true,
                            message = "Do not have enough product in stock."
                        };
                    }

                    string saveCart = JsonConvert.SerializeObject(carts);
                    HttpContext.Session.SetString("CART", saveCart);
                }
                //Notify that add product successfully
                var addSuccess = new { success = true, message = "Add product " + cartItemsModel.ProductId + " to cart successfully." };
                return new JsonResult(addSuccess);
            }
            else
            {
                var addFail = new { success = true, message = "Fail!!!" };
                return new JsonResult(addFail);
            }
        }
    }
}
