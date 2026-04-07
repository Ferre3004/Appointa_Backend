namespace Agendamiento.ViewModel;

public class CheckoutRequest
{
    public string Plan { get; set; } = null!; // "Basic" | "Individual" | "Premium"
}