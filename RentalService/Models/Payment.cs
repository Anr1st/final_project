using System.ComponentModel.DataAnnotations;

namespace RentalService.Models;

public enum PaymentStatus { pending, succeeded, failed, refunded }

public class Payment
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    public Guid BookingId { get; set; }

    [Required]
    public decimal Amount { get; set; }

    public PaymentStatus Status { get; set; } = PaymentStatus.pending;

    public DateTime TransactionDate { get; set; } = DateTime.UtcNow;

    public Booking? Booking { get; set; }
}