using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Nimapinfotech.Models
{
    public class Category
    {
        [Key]
        public int CategoryId { get; set; }

        [Column("CategoryName", TypeName = "varchar(100)")]
        [Required]
        public string CategoryName { get; set; }

    }
}
