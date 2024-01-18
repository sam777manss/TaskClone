using Client.Models;
using CommonModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Serilog;
using System.Net;
using System.Reflection;
using System.Security.Claims;
using static Microsoft.AspNetCore.Razor.Language.TagHelperMetadata;

namespace Client.Controllers
{
    public class AdminController : Controller
    {
        public static string URL = "https://localhost:7257/";

        private readonly IWebHostEnvironment _webHostEnvironment;

        public AdminController(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }
        //[Authorize(Roles = "Admin")]
        #region Admin Landing Page 
        public IActionResult Index()
        {
            //AdminIndex adminIndex = new AdminIndex();
            return View();
        }
        #endregion

        #region fetch all users 
        public async Task<IActionResult> AdminTables()
        {
            try
            {
                var user = HttpContext.User;

                // Access the desired claims by their claim type
                var Uid = user.FindFirst(ClaimTypes.PrimarySid)?.Value;

                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(URL);
                    var response = await client.GetAsync("Admin/AdminTables?Uid=" + Uid); // using concatenation
                    var responseContent = await response.Content.ReadAsStringAsync();
                    List<AspUsersTable>? users = JsonConvert.DeserializeObject<List<AspUsersTable>>(responseContent);
                    if (response.IsSuccessStatusCode)
                    {
                        return View(users);
                    }
                }
                return View(new List<AspUsersTable>()); // Return an empty list if the user is not an admin or an error occurs
            }
            catch (Exception ex)
            {
                Log.Error(ex.InnerException != null ? string.Format("Inner Exception: {0} --- Exception: {1}", ex.InnerException.Message, ex.Message) : ex.Message, ex);

            }
            return View(new List<AspUsersTable>());
        }
        #endregion

        #region fetch user data using id and return data to partial view
        [HttpGet]
        public async Task<ActionResult> Edit(string Id)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(URL);
                var response = await client.GetAsync("Admin/Edit?Id=" + Id); // using concatenation
                var responseContent = await response.Content.ReadAsStringAsync();
                AppUser? user = JsonConvert.DeserializeObject<AppUser>(responseContent);
                if (response.IsSuccessStatusCode)
                {
                    return PartialView("_UserModal", user);
                }
            }
            return PartialView("_UserModal", new AppUser());
        }
        #endregion


        #region fetch all product data
        //[Route("~/Categories/{category}")]
        [HttpGet]
        public async Task<IActionResult> ProductTables()
        {
            try
            {
                var user = HttpContext.User;

                // Access the desired claims by their claim type
                //var Uid = user.FindFirst(ClaimTypes.PrimarySid)?.Value;

                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(URL);
                    var response = await client.GetAsync("Admin/ProductTables"); // using concatenation
                    var responseContent = await response.Content.ReadAsStringAsync();
                    List<AddProductTable> users = JsonConvert.DeserializeObject<List<AddProductTable>>(responseContent);
                    if (response.IsSuccessStatusCode)
                    {
                        return View(users);
                    }
                }
                return View(new List<AddProductTable>()); // Return an empty list if the user is not an admin or an error occurs
            }
            catch (Exception ex)
            {
                Log.Error(ex.InnerException != null ? string.Format("Inner Exception: {0} --- Exception: {1}", ex.InnerException.Message, ex.Message) : ex.Message, ex);

            }
            return View();
        }
        #endregion

        #region fetch user data using id and return data to partial view
        [HttpGet]
        public async Task<ActionResult> EditProduct(string Id)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(URL);
                var response = await client.GetAsync("Admin/EditProduct?Id=" + Id); // using concatenation
                var responseContent = await response.Content.ReadAsStringAsync();
                AddProductTable? product = JsonConvert.DeserializeObject<AddProductTable>(responseContent);
                if (response.IsSuccessStatusCode)
                {
                    return PartialView("_UserProductModal", product);
                }
            }
            return PartialView("_UserProductModal", new AddProductTable());
        }
        #endregion




        #region Delete product using id
        [HttpGet]
        public async Task<IActionResult> DeleteProduct(string Id)
        {
            ApiResponseModel response = new(); 
            ImagesModel imagesModel = new ImagesModel();
            if (Id != null)
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(URL);
                    var responseData = await client.DeleteAsync("Admin/DeleteProduct?Id=" + Id);
                    var responseContent = await responseData.Content.ReadAsStringAsync();
                    response = JsonConvert.DeserializeObject<ApiResponseModel>(responseContent);

                    if (response.Code == (int)HttpStatusCode.OK)
                    {
                        imagesModel = JsonConvert.DeserializeObject<ImagesModel>(response.Data.ToString());
                        foreach(var Img in imagesModel.Images)
                        {
                            string imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "ImagesFolder", Img);
                            FileInfo file = new FileInfo(imagePath);
                            if(file.Exists)
                            {
                                file.Delete();

                            }
                        }

                    }
                    //if (response.IsSuccessStatusCode)
                    //{
                    //    // TempData flow bidirectional but ViewBag and ViewData flow from controller to view only
                    //    TempData["DeletionMessage"] = "Deletion completed successfully";
                       return RedirectToAction("ProductTables");
                    //}
                }
            }
            return View();
        }
        #endregion

        #region updates user details and profile photo
        [HttpPost]
        public async Task<ActionResult> SaveEdits([FromForm] AppUser? appUser)
        {
            try
            {
                if (ModelState.IsValid) // true when all values included or when user images exits
                {
                    using (var client = new HttpClient())
                    {
                        var appUser1 = HttpContext.User;
                        var ImageUrl = appUser1.FindFirst(ClaimTypes.Uri)?.Value;
                        if (appUser.imageFile == null) // user not uploading and not have any image
                        {
                            if (ImageUrl == null) // user not have any image previously
                                ModelState.AddModelError("", "Failed to upload the image");
                            else if (ImageUrl != null)
                                appUser.imageUrl = ImageUrl; 
                        }
                        else
                        {
                            appUser.imageUrl = appUser.imageFile?.FileName; // user image
                            var imagePath = Path.Combine(_webHostEnvironment.WebRootPath, "ImagesFolder", appUser.imageFile.FileName);
                            // Save the image to the specified path
                            using (var stream = new FileStream(imagePath, FileMode.Create))
                            {
                                appUser.imageFile.CopyTo(stream);
                            }
                        }
                        // Send the image name to web api

                        client.BaseAddress = new Uri(URL + "Admin/SaveEdits");
                        var response = await client.PostAsJsonAsync("", appUser);
                        // Handle the response from the Web API controller
                        if (response.IsSuccessStatusCode)
                        {

                            return PartialView("_UserModal", appUser);
                        }
                        else
                        {
                            ModelState.AddModelError("", "Failed to upload the image");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.InnerException != null ? string.Format("Inner Exception: {0} --- Exception: {1}", ex.InnerException.Message, ex.Message) : ex.Message, ex);

            }
            return PartialView("_UserModal", appUser);
        }
        #endregion

        #region updates user details and profile photo
        [HttpPost]
        public async Task<ActionResult> UpdateProduct(AddProduct addProduct)
        {
            ApiResponseModel response = new();
            if (ModelState.IsValid)
            {
                try
                {
                    using (var client = new HttpClient())
                    {
                        if(Request.Form.Files.Count > 0)
                        {
                            IFormFileCollection files = Request.Form.Files;
                            // Add the image files to the wwwroot folder
                            foreach (var file in files)
                            {
                                var imagePath = Path.Combine(_webHostEnvironment.WebRootPath, "ImagesFolder", file.FileName);
                                // Save the image to the specified path
                                using (var stream = new FileStream(imagePath, FileMode.Create))
                                {
                                    await file.CopyToAsync(stream);
                                }
                                addProduct.ImageUrl.Add(file.FileName);
                            }
                            client.BaseAddress = new Uri(URL + "Product/UpdateProduct");
                            var responseData = await client.PostAsJsonAsync("", addProduct);
                            var responseContent = await responseData.Content.ReadAsStringAsync();
                            response = JsonConvert.DeserializeObject<ApiResponseModel>(responseContent);

                            ImagesModel imagesModel = new ImagesModel();
                            imagesModel = JsonConvert.DeserializeObject<ImagesModel>(response.Data.ToString());
                            foreach (var Img in imagesModel.Images)
                            {
                                string imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "ImagesFolder", Img);
                                FileInfo file = new FileInfo(imagePath);
                                if (file.Exists)
                                {
                                    file.Delete();

                                }
                            }
                        }
                        //// Set the base address of the Web API endpoint
                        //client.BaseAddress = new Uri(URL + "Admin/ProductSaveEdits");

                        //// Create a new instance of MultipartFormDataContent
                        //var multiContent = new MultipartFormDataContent();

                        //// Add the fields from the AddProduct object
                        //multiContent.Add(new StringContent(addProduct.ProductName ?? ""), "ProductName");
                        //multiContent.Add(new StringContent(addProduct.ProductDescription ?? ""), "ProductDescription");
                        //multiContent.Add(new StringContent(addProduct.ProductType ?? ""), "ProductType");
                        //multiContent.Add(new StringContent(addProduct.ProductCategory ?? ""), "ProductCategory");
                        //multiContent.Add(new StringContent(addProduct.ProductCategoryType ?? ""), "ProductCategoryType");
                        //multiContent.Add(new StringContent(addProduct.ProductCategoryDescription ?? ""), "ProductCategoryDescription");
                        //multiContent.Add(new StringContent(addProduct.VendorEmail ?? ""), "VendorEmail");

                        //multiContent.Add(new StringContent(addProduct.Price ?? ""), "Price");
                        //multiContent.Add(new StringContent(addProduct.Small ?? ""), "Small");
                        //multiContent.Add(new StringContent(addProduct.Medium ?? ""), "Medium");
                        //multiContent.Add(new StringContent(addProduct.Large ?? ""), "Large");
                        //multiContent.Add(new StringContent(addProduct.XL ?? ""), "XL");
                        //multiContent.Add(new StringContent(addProduct.XXL ?? ""), "XXL");

                        //// Convert the AddProduct object to JSON and add it as a StringContent
                        //var addProductJson = JsonConvert.SerializeObject(addProduct);
                        //multiContent.Add(new StringContent(addProductJson), "addProduct");

                        //// Add the image files
                        //foreach (var file in addProduct.Files)
                        //{
                        //    var fileContent = new StreamContent(file.OpenReadStream());
                        //    multiContent.Add(fileContent, "Files", file.FileName);
                        //}

                        //// Send the HTTP POST request to the Web API
                        //var response = await client.PostAsync("", multiContent);

                        //// Add the image files to the wwwroot folder
                        //foreach (var file in addProduct.Files)
                        //{
                        //    var imagePath = Path.Combine(_webHostEnvironment.WebRootPath, "ImagesFolder", file.FileName);
                        //    // Save the image to the specified path
                        //    using (var stream = new FileStream(imagePath, FileMode.Create))
                        //    {
                        //        await file.CopyToAsync(stream);
                        //    }
                        //}

                        //if (response.IsSuccessStatusCode)
                        //{
                        //    return PartialView("_UserProductModal", addProduct);
                        //}
                        //else
                        //{
                        //    ModelState.AddModelError("", "Failed to upload the image");
                        //}
                        return RedirectToAction("Index");
                    }
                }
                catch (Exception ex)
                {
                    Log.Error(ex.InnerException != null ? string.Format("Inner Exception: {0} --- Exception: {1}", ex.InnerException.Message, ex.Message) : ex.Message, ex);
                }
            }
            return PartialView("_UserProductModal", addProduct);
        }
        #endregion

        #region Delete user using id
        [HttpGet]
        public async Task<IActionResult> Delete(string Id)
        {
            if (Id != null)
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(URL);
                    var response = await client.DeleteAsync("Admin/Delete?Id=" + Id);
                    var responseContent = await response.Content.ReadAsStringAsync();
                    if (response.IsSuccessStatusCode)
                    {
                        // TempData flow bidirectional but ViewBag and ViewData flow from controller to view only
                        TempData["DeletionMessage"] = "Deletion completed successfully";
                        return RedirectToAction("AdminTables");
                    }
                }
            }
            return View();
        }
        #endregion
        [HttpGet]
        public IActionResult AdminAccount()
        {
            return View();
        }

        [HttpGet]
        public IActionResult AddProduct()
        {
            return View();
        }

        #region Add New Product and multi images
        [HttpPost]
        public async Task<IActionResult> AddProduct(AddProduct addProduct)
        {
            //if(ModelState.IsValid)
            //{
            try
            {
                using (var client = new HttpClient())
                {
                    // image
                    if (Request.Form.Files.Count > 0)
                    {
                        string wwwRootPath = _webHostEnvironment.WebRootPath;
                        IFormFileCollection files = Request.Form.Files;

                        // Add the image files to the wwwroot folder
                        foreach (var file in files)
                        {
                            var imagePath = Path.Combine(_webHostEnvironment.WebRootPath, "ImagesFolder", file.FileName);
                            // Save the image to the specified path
                            using (var stream = new FileStream(imagePath, FileMode.Create))
                            {
                                await file.CopyToAsync(stream);
                            }
                            addProduct.ImageUrl.Add(file.FileName);
                        }
                        client.BaseAddress = new Uri(URL+ "Product/AddProduct");

                        var response = await client.PostAsJsonAsync("", addProduct);

                        if (response.IsSuccessStatusCode)
                        {
                            ModelState.AddModelError("", "Failed to upload the image");
                        }
                        else
                        {
                            ModelState.AddModelError("", "Failed to upload the image");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.InnerException != null ? string.Format("Inner Exception: {0} --- Exception: {1}", ex.InnerException.Message, ex.Message) : ex.Message, ex);
            }
            //}
            return View();
        }
        #endregion
    }
}
