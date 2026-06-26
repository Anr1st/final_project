using RentalService.Models;

namespace RentalService.Services;

public class MockVerificationService
{
    public bool VerifyIdentity(User user)
    {
        Console.WriteLine($"[MockVerificationService] Верификация личности для пользователя {user.Id} выполнена успешно.");
        return true;
    }
}