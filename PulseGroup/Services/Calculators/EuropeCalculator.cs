using PulseGroup.Models;
using static PulseGroup.Handlers.Localization;

namespace PulseGroup.Services.Calculators;

/// <summary>
/// Calculator for cars from Europe
/// </summary>
public class EuropeCalculator : ICountryCalculator
{
    public string CountryCode => "europe";
    public string CountryName => Messages.CountryEurope;
    public string CountryFlag => Messages.CountryEuropeFlag;

    public decimal CalculateTotalPrice(decimal carPrice, string deliveryType, PricingConfig config)
    {
        var breakdown = GetBreakdown(carPrice, deliveryType, config);
        return breakdown.Total;
    }

    public CalculationBreakdown GetBreakdown(decimal carPrice, string deliveryType, PricingConfig config)
    {
        // PLACEHOLDER: Not implemented yet - using simplified logic temporarily
        decimal customs = 0; // No customs within EU typically

        var breakdown = new CalculationBreakdown
        {
            CarPrice = carPrice,
            ImportPreparation = config.ImportPreparation * 0.5m, // Reduced for EU
            LandSeaDelivery = config.TransportFromPort, // Just road transport
            Broker = 0,
            TransportFromPort = 0,
            Customs = customs,
            ImportServices = config.ImportServices,
            DeliveryType = "road"
        };

        // Total calculation: Car + all expenses
        breakdown.Total = breakdown.CarPrice + 
                         breakdown.ImportPreparation + 
                         breakdown.LandSeaDelivery + 
                         breakdown.Broker + 
                         breakdown.TransportFromPort + 
                         breakdown.Customs + 
                         breakdown.ImportServices;

        return breakdown;
    }

    public string? GetCountrySpecificNotes()
    {
        return Messages.CountryEuropeNotes;
    }
}
