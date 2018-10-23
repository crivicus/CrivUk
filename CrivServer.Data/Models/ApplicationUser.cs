using Microsoft.AspNetCore.Identity;

namespace CrivServer.Data.Models
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser
    {
        public int UserType { get; set; }
    }
}
