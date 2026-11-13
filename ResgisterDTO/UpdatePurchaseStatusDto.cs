using System.ComponentModel.DataAnnotations;

namespace LibaryManagement.ResgisterDTO
{
    public class UpdatePurchaseStatusDto
    {
        [Required]
        public string Status { get; set; } = string.Empty;
    }
}