using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Nimapinfotech.Models
{
    public class Product
    {
        [Key]
        public int ProductId { get; set; }

        [Column("ProductName", TypeName = "varchar(100)")]
        [Required]
        public string ProductName { get; set; }

       
        public int CategoryId { get; set; }
    }
}
