using Microsoft.AspNetCore.Identity;

namespace LibaryManagement.Identity
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; }
    }
}
