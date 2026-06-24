using RentalService.Models;

namespace RentalService.Services;

public class MockPaymentService
{
    public bool ProcessPayment(Payment payment)
    {
        return true;
    }
}
