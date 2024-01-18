using ShoesApi.Models.ProductModel;

namespace ShoesApi.Models
{
    public class UserCardModel
    {
        public AspUsersTable? aspUsersTable = new AspUsersTable();
        public List<ProductTable>? productTable { get; set; } = new List<ProductTable>();
        //public List<ProductImgTable>? productImageTables { get; set; } = new List<ProductImgTable>();
        public List<UserCartTable>? userCartTables { get; set; } = new List<UserCartTable>();
    }
}
