using System.ComponentModel.DataAnnotations;

namespace RentalService.Models;

public enum PropertyType { apartment, house, room, studio }
public enum PropertyStatus { draft, active, inactive, blocked }

public class Property
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    public Guid OwnerId { get; set; }

    [Required]
    public string Title { get; set; } = string.Empty;

    [Required]
    public string City { get; set; } = string.Empty;

    [Required]
    public string Address { get; set; } = string.Empty;

    public PropertyType PropertyType { get; set; }

    public int Rooms { get; set; }

    [Required]
    public decimal BasePrice { get; set; }

    public PropertyStatus Status { get; set; } = PropertyStatus.draft;

    public string Description { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public User? Owner { get; set; }
    public List<Booking> Bookings { get; set; } = new();
    public List<PropertyPhoto> Photos { get; set; } = new();
    public List<PriceRate> PriceRates { get; set; } = new();
}