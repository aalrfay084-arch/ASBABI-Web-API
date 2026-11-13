using System.ComponentModel.DataAnnotations;
using LibaryManagement.Identity;

namespace LibaryManagement.Models
{
    public class Purchase
    {
        public int PurchaseId { get; set; }

        [Required]
        public string UserName { get; set; }
        public string? UserId { get; set; }

        public int BookId { get; set; }

        public int Quantity { get; set; }

        public decimal UnitPrice { get; set; }

        public decimal TotalAmount { get; set; }

        public DateTime PurchaseDate { get; set; }

        public DateTime? ApprovalDate { get; set; }

        public DateTime? ExpiryDate { get; set; }

        public DateTime? CompletionDate { get; set; }

        public string Status { get; set; } = "Pending";

        public string PaymentStatus { get; set; } = "Unpaid";

        public Book? Book { get; set; }
        public ApplicationUser? User { get; set; }

    }

}

