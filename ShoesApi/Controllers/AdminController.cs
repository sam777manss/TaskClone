using CommonModel;
using Microsoft.AspNetCore.Mvc;
using ShoesApi.DbContextFile.DBFiles;
using ShoesApi.Interfaces;
using ShoesApi.Models;
using ShoesApi.Models.ProductModel;
using System.Net;

namespace ShoesApi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly IAdmin admin;
        public AdminController(IAdmin admin)
        {
            this.admin = admin;
        }

        #region fetch all user info
        [HttpGet]
        [Route("AdminTables")]
        public async Task<IActionResult> AdminTables(string Uid)
        {
            List<AspUsersTable> adminIndexes = new List<AspUsersTable>();
            adminIndexes = await admin.AdminTables(Uid);
            return Ok(adminIndexes);
        }
        #endregion

        #region fetch products
        [HttpGet]
        [Route("ProductTables")]
        public async Task<IActionResult> ProductTables()
        {
           
            List<AddProductTable> productIndexes = await admin.ProductTables();
            return Ok(productIndexes);
        }
        #endregion

        #region fetch user data using id
        [HttpGet]
        [Route("Edit")]
        public async Task<IActionResult> Edit(string Id)
        {
            AppUser adminIndexes = new AppUser();
            adminIndexes = await admin.Edit(Id);
            return Ok(adminIndexes);
        }
        #endregion

        #region fetch product data using id
        [HttpGet]
        [Route("EditProduct")]
        public async Task<IActionResult> EditProduct(string Id)
        {
            AddProductTable product = await admin.EditProduct(Id);
            return Ok(product);
        }
        #endregion

        #region Delete a user using id
        [HttpDelete]
        [Route("Delete")]
        public async Task<IActionResult> Delete(string Id)
        {
            if (Id != null)
            {
                bool flag = await admin.Delete(Id);
                if (flag)
                {
                    return new StatusCodeResult(204); // Deletion completed but return void status
                }
            }
            return new StatusCodeResult(500); // The request was not completed. The server met an unexpected condition.
        }
        #endregion

        #region Delete a user using id
        [HttpDelete]
        [Route("DeleteProduct")]
        public async Task<IActionResult> DeleteProduct(string Id)
        {
            ApiResponseModel response = new();
            try
            {
                if (Id != null)
                {
                    response = await admin.DeleteProduct(Id);

                }
            }
            catch (Exception ex)
            {
                response.Code = (int)HttpStatusCode.InternalServerError;
                response.Message = ex.Message;
            }
            return StatusCode(response.Code, response);
        }
        #endregion

        #region update user data
        [HttpPost]
        [Route("SaveEdits")]
        public async Task<IActionResult> SaveEdits(AppUser appUser)
        {
            IActionResult result = await admin.SaveEdits(appUser);
            // Return a response indicating success
            return new StatusCodeResult(200);
        }
        #endregion

        #region 
        [HttpPut]
        [Route("UserInfoUpdate")]
        public async Task<bool> UserInfoUpdate(string Uid, AppUser appUser)
        {
            bool flag = await admin.UserInfoUpdate(Uid, appUser);
            return flag;
        }
        #endregion

    }
}
