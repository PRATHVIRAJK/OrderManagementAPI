namespace Application.DTOs
{
    public class OrderResponseDto
    {
        public int Id { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public decimal Amount { get; set; }
    }
}
