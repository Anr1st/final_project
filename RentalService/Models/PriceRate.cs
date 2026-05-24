using System.ComponentModel.DataAnnotations;

namespace RentalService.Models;

public class PriceRate
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    public Guid PropertyId { get; set; }

    [Required]
    public DateOnly StartDate { get; set; }

    [Required]
    public DateOnly EndDate { get; set; }

    [Required]
    public decimal PricePerDay { get; set; }

    public Property? Property { get; set; }
}