using System.ComponentModel.DataAnnotations;

namespace RentalService.Models;

public enum BookingStatus { pending, confirmed, completed, cancelled }

public class Booking
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    public Guid PropertyId { get; set; }

    [Required]
    public Guid RenterId { get; set; }

    [Required]
    public DateOnly CheckIn { get; set; }

    [Required]
    public DateOnly CheckOut { get; set; }

    public decimal TotalPrice { get; set; }

    public decimal ServiceFee { get; set; }

    public BookingStatus Status { get; set; } = BookingStatus.pending;

    public string Comment { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Property? Property { get; set; }
    public User? Renter { get; set; }
    public Payment? Payment { get; set; }
}