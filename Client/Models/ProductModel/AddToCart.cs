using System.ComponentModel.DataAnnotations.Schema;

namespace Client.Models
{
    public class AddToCart
    {
        //public Guid Id { get; set; }
        ////[ForeignKey("AspNetUsers")]
        //public Guid? UserId { get; set; }
        //public Guid? ProductId { get; set; }
        ////public string? ProuctColor { get; set; }
        //public string? ProductSize { get; set; }
        //public string? ProductSum { get; set; }

        public Guid Id { get; set; }
        //[ForeignKey("AspNetUsers")]
        public Guid? UserId { get; set; }
        [ForeignKey("AddProductTable")]
        public Guid? ProductId { get; set; }
        public string? ProuctColor { get; set; }
        public string? ProductSize { get; set; }
        public string? ProductSum { get; set; }
        public string? Quantity { get; set; }
        public AddProductTable? addProductTables { get; set; }

    }
}
