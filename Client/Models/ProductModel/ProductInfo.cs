
namespace Client.Models
{
    public class ProductInfo
    {
        public AddProductTable? ProductDetails { get; set; }
        public List<string>? images { get; set; }
        public string? ProductImgId { get; set; }
    }

    public partial class AddProductTable
    {
        public Guid? ProductId { get; set; } // primary key
        public string? ProductName { get; set; }
        public string? ProductDescription { get; set; }
        public string? ProductType { get; set; }
        public string? ProductCategory { get; set; }
        public string? ProductCategoryDescription { get; set; }
        public string? ProductCategoryType { get; set; }
        public string? VendorEmail { get; set; }
        public string? MainImage { get; set; }
        public string? Price { get; set; }
        public string? Small { get; set; }
        public string? Medium { get; set; }
        public string? Large { get; set; }
        public string? XL { get; set; }
        public string? XXL { get; set; }
        public virtual ICollection<ProductImageTable>? ProductImageTable { get; set; } // to estamblish relationship see fluent api

    }

}
