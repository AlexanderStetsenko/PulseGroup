namespace PulseGroup.Models;

/// <summary>
/// Represents a car calculation session for a user
/// </summary>
public class CarCalculation
{
    public string? Country { get; set; }
    public decimal? CarPrice { get; set; }
    public string? DeliveryType { get; set; }
    public string CurrentStep { get; set; } = "start";
}
