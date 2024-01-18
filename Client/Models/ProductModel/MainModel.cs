namespace Client.Models
{
    public class AspUsersTable
    {
        public string? Id { get; set; }
        public string? State { get; set; }
        public string? City { get; set; }
        public string? Zip_Code { get; set; }
        public string? Country { get; set; }
        public string? Address { get; set; }
        public string? imageUrl { get; set; }
        public string? UserSurname { get; set; }
        public string? UserName { get; set; }
        public string? NormalizedUserName { get; set; }
        public string? Email { get; set; }
        public string? NormalizedEmail { get; set; }
        public bool? EmailConfirmed { get; set; }
        public string? PasswordHash { get; set; }
        public string? SecurityStamp { get; set; }
        public string? ConcurrencyStamp { get; set; }
        public string? PhoneNumber { get; set; }
        public bool? PhoneNumberConfirmed { get; set; }
        public bool? TwoFactorEnabled { get; set; }
        public DateTimeOffset? LockoutEnd { get; set; }
        public bool? LockoutEnabled { get; set; }
        public int? AccessFailedCount { get; set; }

    }
    public class ProductTable
    {
        public ProductTable() {
            productImgTables = new List<ProductImgTable>();
        }
        public string? ProductId { get; set; }
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
        public string? UserCartTableId { get; set; }
        public string? Quantity { get; set; }
        public List<ProductImgTable> productImgTables { get; set; } = new List<ProductImgTable>();

    }

    public class ProductImgTable
    {
        public string? ProductImgId { get; set; }
        public string? ProductId { get; set; }
        public string? ImageUrl { get; set; }
    }

    public class UserCartTable
    {
        public string? Id { get; set; }
        public string? UserId { get; set; }
        public string? ProductId { get; set; }
        public string? ProuctColor { get; set; }
        public string? ProductSize { get; set; }
        public string? ProductSum { get; set; }
    }
}
