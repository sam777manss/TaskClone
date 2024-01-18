using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace ShoesApi.Models
{
    public class AppUser : IdentityUser
    {
        public string? State { get; set; }
        public string? City { get; set; }
        //[Required]
        [Display(Name = "Zip Code")]
        public string? Zip_Code { get; set; }
        public string? Country { get; set; }
        public string? Address { get; set; }
        public string? imageUrl { get; set; }
        [Display(Name = "Surname")]
        public string? UserSurname { get; set; }
    }
}
