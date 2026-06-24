using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RentalService.Models;
using RentalService.Services;

namespace RentalService.Controllers;

[ApiController]
[Route("api/v1/payments")]
public class PaymentsController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly MockPaymentService _paymentService;
    private readonly MockEmailService _emailService;

    public PaymentsController(AppDbContext context, MockPaymentService paymentService, MockEmailService emailService)
    {
        _context = context;
        _paymentService = paymentService;
        _emailService = emailService;
    }

    [HttpPost("process")]
    [Authorize]
    public async Task<IActionResult> ProcessPayment([FromBody] ProcessPaymentRequest request)
    {
        var booking = await _context.Bookings
            .Include(b => b.Payment)
            .Include(b => b.Property)
            .FirstOrDefaultAsync(b => b.Id == request.BookingId);

        if (booking == null)
        {
            return NotFound("Booking not found.");
        }

        if (booking.Status != BookingStatus.pending)
        {
            return BadRequest("Only pending bookings can be processed.");
        }

        var payment = booking.Payment ?? new Payment
        {
            BookingId = booking.Id,
            Amount = booking.TotalPrice,
            Status = PaymentStatus.pending,
            TransactionDate = DateTime.UtcNow
        };

        var success = _paymentService.ProcessPayment(payment);
        payment.Status = success ? PaymentStatus.succeeded : PaymentStatus.failed;
        payment.TransactionDate = DateTime.UtcNow;

        if (booking.Payment == null)
        {
            _context.Payments.Add(payment);
        }

        booking.Status = success ? BookingStatus.confirmed : booking.Status;
        await _context.SaveChangesAsync();

        if (success)
        {
            _emailService.SendBookingConfirmedEmail(booking);
        }

        return Ok(MapPaymentResponse(payment));
    }

    [HttpGet("{id}")]
    [Authorize]
    public async Task<IActionResult> GetPayment(Guid id)
    {
        var payment = await _context.Payments
            .Include(p => p.Booking)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (payment == null)
        {
            return NotFound();
        }

        return Ok(MapPaymentResponse(payment));
    }

    [HttpPost("{id}/refund")]
    [Authorize]
    public async Task<IActionResult> RefundPayment(Guid id)
    {
        var payment = await _context.Payments
            .Include(p => p.Booking)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (payment == null)
        {
            return NotFound();
        }

        if (payment.Status == PaymentStatus.refunded)
        {
            return BadRequest("Payment is already refunded.");
        }

        if (payment.Booking == null || payment.Booking.Status != BookingStatus.cancelled)
        {
            return BadRequest("Booking must be cancelled before refund.");
        }

        payment.Status = PaymentStatus.refunded;
        await _context.SaveChangesAsync();

        return Ok(MapPaymentResponse(payment));
    }

    private static PaymentResponse MapPaymentResponse(Payment payment)
    {
        return new PaymentResponse(payment.Id, payment.BookingId, payment.Amount, payment.Status, payment.TransactionDate);
    }

    public record ProcessPaymentRequest(Guid BookingId);
    public record PaymentResponse(Guid Id, Guid BookingId, decimal Amount, PaymentStatus Status, DateTime TransactionDate);
}
