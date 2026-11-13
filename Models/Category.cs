using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace LibaryManagement.Models
{
    public class Category
    {
        public int CategoryId { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Description { get; set; }
        [JsonIgnore]

        public ICollection<Book> Books { get; set; } = new List<Book>();
    }
}
