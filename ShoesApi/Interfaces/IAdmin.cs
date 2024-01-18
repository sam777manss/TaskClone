using CommonModel;
using Microsoft.AspNetCore.Mvc;
using ShoesApi.DbContextFile.DBFiles;
using ShoesApi.Models;
using ShoesApi.Models.ProductModel;

namespace ShoesApi.Interfaces
{
    public interface IAdmin
    {
        public Task<List<AspUsersTable>> AdminTables(string Uid);

        public Task<bool> Delete(string Id);

        public Task<ApiResponseModel> DeleteProduct(string Id);

        public Task<AppUser> Edit(string Id);
        public Task<AddProductTable> EditProduct(string Id);

        public Task<IActionResult> SaveEdits(AppUser User);

        public Task<bool> UserInfoUpdate(string Uid, AppUser appUser);

        public Task<List<AddProductTable>> ProductTables();

    }
}
