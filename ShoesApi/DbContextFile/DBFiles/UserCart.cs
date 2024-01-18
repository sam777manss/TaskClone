using ShoesApi.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShoesApi.DbContextFile.DBFiles
{
    public class UserCart
    {
        public Guid Id { get; set; }
        //[ForeignKey("AspNetUsers")]
        public Guid? UserId { get; set; }
        [ForeignKey("AddProductTable")]
        public Guid? ProductId { get; set; }
        public string? ProuctColor { get; set; }
        public string? ProductSize { get; set;}
        public string? ProductSum { get; set;}
        public string? Quantity { get; set; }
        public AddProductTable? addProductTables { get; set; }

    }
}
