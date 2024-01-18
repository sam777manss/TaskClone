using Client.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Serilog;
using System.Security.Claims;

namespace Client.Controllers
{
    public class AuthenticateController : Controller
    {
        public static string URL = "https://localhost:7257/";
        //public IActionResult Index()
        //{
        //    return View();
        //}

        [HttpGet]
        public IActionResult LoginPage()
        {
            return View();
        }


        #region Cookie Setup (Set CartCounter, Roles, UserImage, id)
        public async Task CookiesSetUp(string UserId, List<string> roles, string username, string email, string user_image, string cart, AspUsersTable Data)
        {

            try
            {

                // cookies set start
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.PrimarySid, UserId),
                    new Claim(ClaimTypes.Email, Data.Email),
                    new Claim(ClaimTypes.Name, Data.UserName),
                    new Claim(ClaimTypes.SerialNumber, cart),
                };
                if (!string.IsNullOrEmpty(Data.PhoneNumber))
                    claims.Add(new Claim(ClaimTypes.MobilePhone, Data.PhoneNumber));
                if (!string.IsNullOrEmpty(Data.State))
                    claims.Add(new Claim(ClaimTypes.StateOrProvince, Data.State));
                if (!string.IsNullOrEmpty(Data.Address))
                    claims.Add(new Claim(ClaimTypes.StreetAddress, Data.Address));
                if (!string.IsNullOrEmpty(Data.Country))
                    claims.Add(new Claim(ClaimTypes.Country, Data.Country));
                if (!string.IsNullOrEmpty(user_image))
                    claims.Add(new Claim(ClaimTypes.Uri, user_image));
                if (!string.IsNullOrEmpty(Data.Zip_Code))
                {
                    var zipCodeClaim = new Claim("http://schemas.myapp.com/claims/zipCode", Data.Zip_Code); // Zip Code
                    claims.Add(zipCodeClaim);
                }
                if (!string.IsNullOrEmpty(Data.City))
                {
                    var cityClaim = new Claim("http://schemas.myapp.com/claims/city", Data.City);
                    claims.Add(cityClaim);
                }
                foreach (var role in roles)
                {
                    claims.Add(new Claim(ClaimTypes.Role, role));
                }
                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var authProperties = new AuthenticationProperties
                {
                    ExpiresUtc = DateTime.Now.AddMinutes(10), // user authentication timeout
                };
                await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);
            }
            catch (Exception ex)
            {
                Log.Error(ex.InnerException != null ? string.Format("Inner Exception: {0} --- Exception: {1}", ex.InnerException.Message, ex.Message) : ex.Message, ex);

            }
        }
        #endregion


        #region After successful login create cookiee and redirect
        [HttpPost]
        public async Task<IActionResult> LoginPage(Login login)
        {
            if (ModelState.IsValid)
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(URL + "Authenticate/Login");
                    var response = await client.PostAsJsonAsync("", login);
                    if (response.IsSuccessStatusCode)
                    {
                        var responseContent = await response.Content.ReadAsStringAsync();
                        CommonIndex? Data = JsonConvert.DeserializeObject<CommonIndex>(responseContent);

                        if (Data?.UserId != null)
                        {
                            var userRoles = new List<string>();
                            var user_name = string.Empty;
                            var email = string.Empty;
                            AspUsersTable? UserData;
                            if (Data.Roles.Contains("Admin"))
                            {
                                UserData = Data?.Admin;
                                userRoles.Add("Admin");
                                userRoles.Add("User");
                                user_name = Data?.Admin?.UserName;
                                email = Data?.Admin?.Email;
                            }
                            else
                            {
                                UserData = Data?.User;
                                userRoles.Add("User");
                                user_name = Data?.User?.UserName;
                                email = Data?.User?.Email;
                            }

                            await CookiesSetUp(Data.UserId, userRoles, user_name, email, Data.imageUrl, Data.cart, UserData);
                            // cookies set ends
                            if (Data.User != null && Data.Roles != null)
                            {
                                if (Data.Roles.Contains("Admin"))
                                {
                                    return RedirectToAction("Index", "User");
                                }
                                else
                                {
                                    return RedirectToAction("Index", "User"); // no need to pass values extract userdata in user index method
                                    //return RedirectToAction("Index", "User", Data.User);
                                }
                            }
                            else if (Data?.response?.Message == "Invalid Email")
                            {
                                ModelState.AddModelError("Email", "Email not found");
                            }
                            else if (Data?.response?.Message == "Invalid Password")
                            {
                                ModelState.AddModelError("Password", "password not valid");
                            }
                        }
                    }
                }
            }
            return View(login);
        }
        #endregion

        #region End Session
        public async Task<IActionResult> LogOutPage()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "User");
        }
        #endregion

        [HttpGet]
        public IActionResult RegisterPage()
        {
            return View();
        }

        #region Register new user
        [HttpPost]
        public async Task<IActionResult> RegisterPage(Register register)
        {
            if (ModelState.IsValid)
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(URL + "Authenticate/RegisterAdmin");
                    var response = await client.PostAsJsonAsync("", register);
                    if (response.IsSuccessStatusCode)
                    {
                        var responseContent = await response.Content.ReadAsStringAsync();
                        return RedirectToAction("LoginPage", "Authenticate");
                    }
                }
            }
            return View(register);
        }
        #endregion
    }
}
