using PulseGroup.Models;
using static PulseGroup.Handlers.Localization;

namespace PulseGroup.Services.Calculators;

/// <summary>
/// Calculator for cars from USA
/// </summary>
public class USACalculator : ICountryCalculator
{
    public string CountryCode => "usa";
    public string CountryName => Messages.CountryUSA;
    public string CountryFlag => Messages.CountryUSAFlag;

    public decimal CalculateTotalPrice(decimal carPrice, string deliveryType, PricingConfig config)
    {
        var breakdown = GetBreakdown(carPrice, deliveryType, config);
        return breakdown.Total;
    }

    public CalculationBreakdown GetBreakdown(decimal carPrice, string deliveryType, PricingConfig config)
    {
        // Calculate customs as percentage of car price
        decimal customs = carPrice * config.CustomsPercent;

        var breakdown = new CalculationBreakdown
        {
            CarPrice = carPrice,
            ImportPreparation = config.ImportPreparation,
            LandSeaDelivery = config.LandSeaDelivery,
            Broker = config.Broker,
            TransportFromPort = config.TransportFromPort,
            Customs = customs,
            ImportServices = config.ImportServices,
            DeliveryType = "ship" // USA typically uses ship delivery
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
        return Messages.CountryUSANotes;
    }
}
