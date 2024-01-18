using ShoesApi.DbContextFile.DBFiles;

namespace ShoesApi.Models.ProductModel
{
    public class ProductInfo
    {
        public AddProductTable? ProductDetails { get; set; }
        public List<string>? images { get; set; }

        public string? ProductImgId { get; set; }

    }
}
