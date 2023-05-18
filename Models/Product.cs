using System.ComponentModel.DataAnnotations;

namespace Rest.Models
{
    public class Product
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public int Count { get; set; }
    }
}
