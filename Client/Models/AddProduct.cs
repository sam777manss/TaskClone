using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Client.Models
{
    public class AddProduct
    {
        public Guid? ProductId { get; set; }
        public string? ProductName { get; set; }
        public string? ProductDescription { get; set; }
        public string? ProductType { get; set; }
        public string? ProductCategory { get; set; }
        public string? ProductCategoryDescription { get; set; }
        public string? ProductCategoryType { get; set; }
        //public string? ProductCategoryName { get; set;}
        //[Required(ErrorMessage = "Please select files")]
        //[NotMapped]
        //public List<IFormFile>? Files { get; set; }
        public string? MainImage { get; set; }
        public IList<string>? ImageUrl { get; set; } = new List<string>();
        public string? VendorEmail { get; set; }

        public string? Price { get;set; }
        public string? Small { get; set; }
        public string? Medium { get; set; }
        public string? Large { get; set; }
        public string? XL { get; set; }
        public string? XXL { get; set; }

    }
    public class ImagesModel
    {
        public IList<string>? Images { get; set; } = new List<string>();

    }
}
