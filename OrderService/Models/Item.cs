using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OrderService.Models;

[Table("order_items")]
public class Item
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("id")]
    public long Id { get; set; }

    [Column("order_id")]
    public long OrderId { get; set; }

    [ForeignKey("OrderId")]
    public Order Order { get; set; }

    [Column("name")]
    public string Name { get; set; }

    [Column("quantity")]
    public long Quantity { get; set; }

    [Column("unit_price")]
    public decimal UnitPrice { get; set; }
    
    [Column("created_at")]
    public DateTime CreatedAt { get; set; }

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; }
}