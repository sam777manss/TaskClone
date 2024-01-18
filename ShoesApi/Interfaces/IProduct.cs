using CommonModel;
using Microsoft.AspNetCore.Mvc;
using ShoesApi.DbContextFile.DBFiles;
using ShoesApi.Models;
using ShoesApi.Models.ProductModel;

namespace ShoesApi.Interfaces
{
    public interface IProduct
    {
        public Task<CategoriesModel> Categories(string category, int pageNumber, int pageSize);
        public Task<ProductInfo> InfoById(string ProductId);
        public Task<IActionResult> AddProduct( AddProduct addProduct);
        public Task<ApiResponseModel> UpdateProduct(AddProduct addProduct);
        public Task<IActionResult> ProductDetail(UserCart cart);
        public Task<Object> UserCartDetails(string Uid);
        public Task<bool> DeleteProduct(string UserCartTableId);
        public Task<int> GetCartCounter(string Uid);
        public Task<List<AddProductTable>> functionSearchResults(string input);
        public Task<Object> Checkout(string Uid);
        public Task<bool> ProductQuantity(string ProductId, string Quantity, string UserId);
    }
}
