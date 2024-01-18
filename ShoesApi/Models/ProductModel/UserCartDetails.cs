using ShoesApi.DbContextFile.DBFiles;

namespace ShoesApi.Models.ProductModel
{
    public class UserCartDetails
    {
        public IEnumerable<AddProductTable>? addProductTable { get; set; }
        public IEnumerable<ProductImageTable>? productImageTable { get; set; }
        public IEnumerable<UserCart>? UserCart { get; set; }
    }
}
