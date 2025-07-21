namespace BookStoreAPI.DTOs;

public class BookDto
{
    public string Name { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public decimal Price { get; set; } = decimal.Zero;
    public string Description { get; set; } = string.Empty;
    public string? CoverUrl { get; set; }
}
