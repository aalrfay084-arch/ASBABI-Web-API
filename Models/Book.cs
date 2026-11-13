using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace LibaryManagement.Models
{
    public class Book
    {
        public int BookId { get; set; }

        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [Range(typeof(decimal), "0", "10000")]
        public decimal Price { get; set; }

        [Range(0, 100)]
        public int Quantity { get; set; }

        // Foreign Keys
        public int AuthorId { get; set; }
        public int CategoryId { get; set; }

        // Navigation Properties
        public Author? Author { get; set; } = null!;
        public Category? Category { get; set; } = null!;
        [JsonIgnore]

        public ICollection<Borrowing> Borrowings { get; set; }
            = new List<Borrowing>();
    }
}
