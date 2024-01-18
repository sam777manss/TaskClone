
using ShoesApi.Models.ProductModel;

namespace ShoesApi.Models
{
    public class CommonIndex
    {
        public AspUsersTable? User { get; set; }
        public AspUsersTable? Admin { get; set; }
        public IList<string>? Roles { get; set; }
        public Response? response { get; set; }
        public string? UserId { get; set; }

        public string? imageUrl { get; set; }
        public string? cart { get; set;}

    }
}
