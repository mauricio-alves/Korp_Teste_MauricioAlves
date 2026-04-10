using System.ComponentModel.DataAnnotations;

namespace InventoryService.Models;

public class Product
{
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    [MaxLength(50)]
    public string Code { get; set; } = string.Empty;

    [Required]
    [MaxLength(200)]
    public string Description { get; set; } = string.Empty;

    [Range(0, int.MaxValue)]
    public int Balance { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
