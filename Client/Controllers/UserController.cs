using Client.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Newtonsoft.Json;
using Serilog;
using System.Security.Claims;
using System.Text;

namespace Client.Controllers
{
    public class UserController : Controller
    {
        public static string URL = "https://localhost:7257/";

        public IActionResult Index()
        {
            return View();
        }
        public IActionResult AddUserShippingInfo()
        {
            return View();
        }

        public async Task<IActionResult> UserInfoUpdate(AppUser appUser)
        {
            var user = HttpContext.User;
            var Uid = user.FindFirst(ClaimTypes.PrimarySid)?.Value;

            // Serialize appUser to JSON
            var appUserJson = JsonConvert.SerializeObject(appUser);
            var content = new StringContent(appUserJson, Encoding.UTF8, "application/json");
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(URL);
                var requestUrl = $"Admin/UserInfoUpdate?Uid={Uid}";

                var response = await client.PutAsync(requestUrl, content);

                var responseContent = await response.Content.ReadAsStringAsync();
                ProductInfo users = JsonConvert.DeserializeObject<ProductInfo>(responseContent);
                if (response.IsSuccessStatusCode)
                {
                    return View(users);
                }
            }
            return RedirectToAction("LoginPage", "Authenticate");
        }

        [Route("~/ProductDetail/{ProductId}")]
        [HttpGet]
        public async Task<IActionResult> ProductDetail(string ProductId)
        {
            try
            {
                var user = HttpContext.User;
                var userId = user.FindFirst(ClaimTypes.PrimarySid)?.Value;
                if (userId != null)
                {
                    using (var client = new HttpClient())
                    {
                        client.BaseAddress = new Uri(URL);
                        var response = await client.GetAsync("Product/InfoById?ProductId=" + ProductId); // using concatenation
                        var responseContent = await response.Content.ReadAsStringAsync();
                        ProductInfo users = JsonConvert.DeserializeObject<ProductInfo>(responseContent);
                        if (response.IsSuccessStatusCode)
                        {
                            return View(users);
                        }
                    }
                }
                return RedirectToAction("LoginPage", "Authenticate"); ; // Return an empty list if the user is not an admin or an error occurs
            }
            catch (Exception ex)
            {
                Log.Error(ex.InnerException != null ? string.Format("Inner Exception: {0} --- Exception: {1}", ex.InnerException.Message, ex.Message) : ex.Message, ex);

            }
            return View(new ProductInfo());
        }
        #region GetCartCounter
        [HttpGet]
        public async Task<IActionResult> GetCartCounter()
        {
            try
            {

                using (var client = new HttpClient())
                {
                    var user = HttpContext.User;
                    var Uid = user.FindFirst(ClaimTypes.PrimarySid)?.Value;
                    if (Uid != null)
                    {
                        client.BaseAddress = new Uri(URL);
                        var response = await client.GetAsync("Product/GetCartCounter?Uid=" + Uid);
                        var responseContent = await response.Content.ReadAsStringAsync();
                        if (response.IsSuccessStatusCode)
                        {
                            return Json(responseContent);
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                Log.Error(ex.InnerException != null ? string.Format("Inner Exception: {0} --- Exception: {1}", ex.InnerException.Message, ex.Message) : ex.Message, ex);

            }
            return Json(0);
        }
        #endregion

        // start
        #region functionSearchResults
        [HttpGet]
        public async Task<IActionResult> functionSearchResults(string input)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(URL);
                    var response = await client.GetAsync("Product/functionSearchResults?input=" + input);
                    var responseContent = await response.Content.ReadAsStringAsync();
                    if (response.IsSuccessStatusCode)
                    {
                        return Json(responseContent);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.InnerException != null ? string.Format("Inner Exception: {0} --- Exception: {1}", ex.InnerException.Message, ex.Message) : ex.Message, ex);

            }
            return Json(0);
        }
        #endregion
        // ends
        #region Add Product to User Cart
        [HttpPost]
        public async Task<IActionResult> ProductDetail(AddToCart cart)
        {
            if (ModelState.IsValid)
            {
                var user = HttpContext.User;
                var Uid = user.FindFirst(ClaimTypes.PrimarySid)?.Value;
                if (Uid == null)
                {
                    return Json(new { success = false, redirectUrl = Url.Action("LoginPage", "Authenticate") });
                    //return RedirectToAction("LoginPage", "Authenticate");
                }
                using (var client = new HttpClient())
                {
                    cart.UserId = Guid.Parse(Uid);
                    client.BaseAddress = new Uri(URL + "Product/ProductDetail");
                    var response = await client.PostAsJsonAsync("", cart);
                    if (response.IsSuccessStatusCode)
                    {
                        var responseContent = await response.Content.ReadAsStringAsync();
                        return Json(new { success = true, redirectUrl = Url.Action("UserCartDetails", "User") });
                        //return RedirectToAction("UserCartDetails", "User");
                    }
                }
            }
            return Json(new { success = false });
        }
        #endregion

        #region DeleteProduct
        [HttpGet]
        public async Task<IActionResult> DeleteProduct(string UserCartTableId)
        {
            try
            {

                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(URL);
                    var response = await client.DeleteAsync("Product/DeleteProduct?UserCartTableId=" + UserCartTableId);
                    var responseContent = await response.Content.ReadAsStringAsync();
                    if (response.IsSuccessStatusCode)
                    {
                        return RedirectToAction("UserCartDetails", "User");
                    }
                }

            }
            catch (Exception ex)
            {
                Log.Error(ex.InnerException != null ? string.Format("Inner Exception: {0} --- Exception: {1}", ex.InnerException.Message, ex.Message) : ex.Message, ex);

            }
            return RedirectToAction("UserCartDetails", "User");
        }
        #endregion

        [HttpGet]
        public async Task<IActionResult> UserCartDetails()
        {
            if (ModelState.IsValid)
            {
                var user = HttpContext.User;
                var Uid = user.FindFirst(ClaimTypes.PrimarySid)?.Value;
                if (Uid != null)
                {
                    using (var client = new HttpClient())
                    {
                        client.BaseAddress = new Uri(URL);
                        var response = await client.GetAsync("Product/UserCartDetails?Uid=" + Uid);
                        var responseContent = await response.Content.ReadAsStringAsync();
                        UserCartDetails? users = JsonConvert.DeserializeObject<UserCartDetails>(responseContent);
                        if (response.IsSuccessStatusCode)
                        {
                            return View(users);
                        }
                    }
                }
                else
                {
                    return RedirectToAction("LoginPage", "Authenticate");
                }
                return View(new List<UserCartDetails>());
            }
            return View(new List<UserCartDetails>());
        }

        #region Contact Info
        [HttpGet]
        public async Task<IActionResult> Contact()
        {
            return View();
        }
        #endregion

        #region Quantity Updated 
        [HttpGet]
        public async Task<bool> ProductQuantity(string ProductId, string Quantity)
        {

            var user = HttpContext.User;
            var Uid = user.FindFirst(ClaimTypes.PrimarySid)?.Value;
            if (Uid != null)
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(URL);
                    var response = await client.GetAsync("Product/ProductQuantity?ProductId=" + ProductId + "&Quantity=" + Quantity + "&UserId=" + Uid);
                    var responseContent = await response.Content.ReadAsStringAsync();
                    UserCartDetails? users = JsonConvert.DeserializeObject<UserCartDetails>(responseContent);
                    if (response.IsSuccessStatusCode)
                    {
                        return true;
                    }
                }
            }
            else
            {
                return false;
            }
            return true;
        }
        #endregion 

        #region user checkout 
        [HttpGet]
        public async Task<IActionResult> Checkout()
        {
            if (ModelState.IsValid)
            {
                var user = HttpContext.User;
                var Uid = user.FindFirst(ClaimTypes.PrimarySid)?.Value;
                if (Uid != null)
                {
                    using (var client = new HttpClient())
                    {
                        client.BaseAddress = new Uri(URL);
                        var response = await client.GetAsync("Product/Checkout?Uid=" + Uid);
                        var responseContent = await response.Content.ReadAsStringAsync();
                        UserCartDetails? users = JsonConvert.DeserializeObject<UserCartDetails>(responseContent);
                        if (response.IsSuccessStatusCode)
                        {
                            return View(users);
                        }
                    }
                }
                else
                {
                    return RedirectToAction("LoginPage", "Authenticate");
                }
                return View(new List<UserCartDetails>());
            }
            return View(new List<UserCartDetails>());
        }
        #endregion

        #region fetch all the category data like mens cloth 
        //[Route("~/Categories/{category}")]
        [HttpGet]
        public async Task<IActionResult> Categories(string category, int pageNumber = 1, int pageSize = 3)
        {
            try
            {
                var user = HttpContext.User;

                TempData["filter"] = category;
                TempData["pageNumber"] = pageNumber;

                // Access the desired claims by their claim type
                //var Uid = user.FindFirst(ClaimTypes.PrimarySid)?.Value;

                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(URL);
                    var response = await client.GetAsync("Product/Categories?category=" + category + "&pageNumber="+ pageNumber + "&pageSize="+ pageSize); // using concatenation
                    var responseContent = await response.Content.ReadAsStringAsync();
                    CategoriesModel users = JsonConvert.DeserializeObject<CategoriesModel>(responseContent);
                    if (response.IsSuccessStatusCode)
                    {
                        return View(users);
                    }
                }
                return View(new CategoriesModel()); // Return an empty list if the user is not an admin or an error occurs
            }
            catch (Exception ex)
            {
                Log.Error(ex.InnerException != null ? string.Format("Inner Exception: {0} --- Exception: {1}", ex.InnerException.Message, ex.Message) : ex.Message, ex);

            }
            return View();
        }
        #endregion
    }
}
