using System.ComponentModel.DataAnnotations;

namespace LibaryManagement.ResgisterDTO
{
    public class LoginDTO
    {
        public class LoginDto
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; }

            [Required]
            public string Password { get; set; }
        }
    }
}
