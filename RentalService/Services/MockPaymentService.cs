using RentalService.Models;

namespace RentalService.Services;

public class MockPaymentService
{
    public bool ProcessPayment(Payment payment)
    {
        Console.WriteLine($"[MockPaymentService] Платёж {payment.Id} для бронирования {payment.BookingId} выполнен успешно.");
        return true;
    }

    public string ProcessPaymentStatus(Payment payment)
    {
        Console.WriteLine($"[MockPaymentService] Платёж {payment.Id} для бронирования {payment.BookingId} выполнен успешно.");
        return "succeeded";
    }
}
