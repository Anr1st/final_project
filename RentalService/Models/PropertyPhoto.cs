using System.ComponentModel.DataAnnotations;

namespace RentalService.Models;

public class PropertyPhoto
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    public Guid PropertyId { get; set; }

    [Required]
    public string Url { get; set; } = string.Empty;

    public bool IsCover { get; set; } = false;

    public Property? Property { get; set; }
}