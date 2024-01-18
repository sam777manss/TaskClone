using CommonModel;
using Microsoft.AspNetCore.Mvc;
using ShoesApi.DbContextFile.DBFiles;
using ShoesApi.Interfaces;
using ShoesApi.Models;
using ShoesApi.Models.ProductModel;

namespace ShoesApi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProduct product;
        public ProductController(IProduct product)
        {
            this.product = product;
        }
        #region fetch category related products
        [HttpGet]
        [Route("Categories")]
        public async Task<IActionResult> Categories(string category, int pageNumber, int pageSize)
        {
            CategoriesModel adminIndexes = new CategoriesModel();
            adminIndexes = await product.Categories(category, pageNumber, pageSize);

            return Ok(adminIndexes);
        }
        #endregion

        #region Product Quantity
        [HttpGet]
        [Route("ProductQuantity")]
        public async Task<bool> ProductQuantity(string ProductId, string Quantity, string UserId)
        {
            bool flag = await product.ProductQuantity(ProductId, Quantity, UserId);
            return flag;
        }
        #endregion

        #region fetch product info by id
        [HttpGet]
        [Route("InfoById")]
        public async Task<IActionResult> InfoById(string ProductId)
        {
            ProductInfo ProductInfo = new ProductInfo();
            ProductInfo = await product.InfoById(ProductId);
            return Ok(ProductInfo);
        }
        #endregion

        #region add new product
        [HttpPost]
        [Route("AddProduct")]
        public async Task<IActionResult> AddProduct( AddProduct addProductdata)
        {
            IActionResult result = await product.AddProduct(addProductdata);
            // Return a response indicating success
            return new StatusCodeResult(200);
        }
        #endregion
        #region add new product
        [HttpPost]
        [Route("UpdateProduct")]
        public async Task<ApiResponseModel> UpdateProduct(AddProduct addProductdata)
        {
            try
            {
                ApiResponseModel result = await product.UpdateProduct(addProductdata);
                // Return a response indicating success
                return result;
            }
            catch
            {
                throw;
            }
        }
        #endregion

        [HttpPost]
        [Route("ProductDetail")]
        public async Task<IActionResult> ProductDetail(UserCart cart)
        {
            IActionResult result = await product.ProductDetail(cart);
            if (result is StatusCodeResult statusCodeResult)
            {
                if (statusCodeResult.StatusCode == 406)
                {
                    var errorResponse = new
                    {
                        Message = "The requested content type is not supported."
                    };

                    return BadRequest(errorResponse);
                }
            }
            return Ok();
        }

        #region user searched
        [HttpGet]
        [Route("functionSearchResults")]
        public async Task<IActionResult> functionSearchResults(string input)
        {
            List<AddProductTable> result = await product.functionSearchResults(input);
            return Ok(result);
        }
        #endregion

        #region Items in Cart 
        [HttpGet]
        [Route("GetCartCounter")]
        public async Task<int> GetCartCounter(string Uid)
        {
            int result = await product.GetCartCounter(Uid);
            return result;
        }
        #endregion

        #region fetch products in user card
        [HttpGet]
        [Route("UserCartDetails")]
        public async Task<IActionResult> UserCartDetails(string Uid)
        {
            UserCardModel userCartDetails = new UserCardModel();
            return Ok(await product.UserCartDetails(Uid));
        }
        #endregion

        #region fetch user Checkout
        [HttpGet]
        [Route("Checkout")]
        public async Task<IActionResult> Checkout(string Uid)
        {
            UserCardModel userCartDetails = new UserCardModel();
            return Ok(await product.Checkout(Uid));
        }
        #endregion


        #region DeleteProduct
        [HttpDelete]
        [Route("DeleteProduct")]
        public async Task<IActionResult> DeleteProduct(string UserCartTableId)
        {
            bool flag = await product.DeleteProduct(UserCartTableId);
            return Ok(true);
        }
        #endregion
    }
}
