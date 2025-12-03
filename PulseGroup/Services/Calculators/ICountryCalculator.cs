using PulseGroup.Models;

namespace PulseGroup.Services.Calculators;

/// <summary>
/// Interface for country-specific calculation strategies
/// </summary>
public interface ICountryCalculator
{
    /// <summary>
    /// Country code this calculator is for
    /// </summary>
    string CountryCode { get; }
    
    /// <summary>
    /// Country display name
    /// </summary>
    string CountryName { get; }
    
    /// <summary>
    /// Country flag emoji
    /// </summary>
    string CountryFlag { get; }
    
    /// <summary>
    /// Calculates total price for a car from this country
    /// </summary>
    decimal CalculateTotalPrice(decimal carPrice, string deliveryType, PricingConfig config);
    
    /// <summary>
    /// Gets detailed breakdown of costs
    /// </summary>
    CalculationBreakdown GetBreakdown(decimal carPrice, string deliveryType, PricingConfig config);
    
    /// <summary>
    /// Gets country-specific notes or warnings
    /// </summary>
    string? GetCountrySpecificNotes();
}

/// <summary>
/// Breakdown of all calculation costs
/// </summary>
public class CalculationBreakdown
{
    public decimal CarPrice { get; set; }
    public decimal ImportPreparation { get; set; }
    public decimal LandSeaDelivery { get; set; }
    public decimal Broker { get; set; }
    public decimal TransportFromPort { get; set; }
    public decimal Customs { get; set; }
    public decimal ImportServices { get; set; }
    public decimal Total { get; set; }
    public string DeliveryType { get; set; } = string.Empty;
}
