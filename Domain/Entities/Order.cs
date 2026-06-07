using System.ComponentModel.DataAnnotations;

public class Order
{
    public int Id { get; set; }
    [Required]
    [MaxLength(100)]
    public string ProductName { get; set; } = string.Empty;
    [Required]
    public decimal Amount { get; set; }
}