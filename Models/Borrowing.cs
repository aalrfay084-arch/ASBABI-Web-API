using LibaryManagement.Identity;
using System.ComponentModel.DataAnnotations;

namespace LibaryManagement.Models
{
    public class Borrowing
    {
        public int BorrowingId { get; set; }

        [Required]
        
        public string UserId { get; set; }

        // تاريخ تقديم طلب الاستعارة
        public DateTime RequestDate { get; set; }

        // يبدأ عند موافقة الأدمن
        public DateTime? BorrowDate { get; set; }

        // موعد الإرجاع بعد 3 أيام من الموافقة
        public DateTime? DueDate { get; set; }

        // تاريخ الإرجاع الفعلي
        public DateTime? ReturnDate { get; set; }

        [Required]
        [StringLength(20)]
        public string Status { get; set; } = "Pending";

        // رسوم الاستعارة الثابتة
        [Range(typeof(decimal), "0", "2000")]
        public decimal BorrowingFee { get; set; } = 1000m;

        // غرامة التأخير
        [Range(typeof(decimal), "0", "10000")]
        public decimal LateFee { get; set; } = 0m;

        public bool IsLateFeePaid { get; set; } = false;

        public DateTime? LateFeePaidDate { get; set; }

        public int BookId { get; set; }

        public Book? Book { get; set; }

        public ApplicationUser? User { get; set; }
    }
}
