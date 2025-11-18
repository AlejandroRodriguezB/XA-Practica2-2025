using System.ComponentModel.DataAnnotations.Schema;

namespace WebApi.Dto
{
    [Table("products")]
    public class Product
    {
        [Column("id")]
        public int Id { get; set; }
        [Column("name")]
        public string Name { get; set; } = string.Empty;
        [Column("price")]
        public decimal Price { get; set; }
        [Column("amount")]
        public int Amount { get; set; }
    }
}
