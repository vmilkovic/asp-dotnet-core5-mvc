using Microsoft.AspNetCore.Identity;

namespace RockyModels
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; }
    }
}
