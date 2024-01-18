namespace Client.Models
{
    public class ProductImageTable
    {
        public Guid? ProductImgId { get; set; }
        public Guid? ProductId { get; set; }
        public string? ImageUrl { get; set; }
    }
}
