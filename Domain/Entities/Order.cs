using System.ComponentModel.DataAnnotations;

public class Order
{
    [Required]
    public int Id { get; set; }
    [Required]
    [MaxLength(100)]
    public string? ProductName { get; set; }
    [Required]
    public decimal Amount { get; set; }
}