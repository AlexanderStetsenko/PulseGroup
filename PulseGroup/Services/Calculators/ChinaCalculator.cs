using PulseGroup.Models;
using static PulseGroup.Handlers.Localization;

namespace PulseGroup.Services.Calculators;

/// <summary>
/// Calculator for cars from China
/// </summary>
public class ChinaCalculator : ICountryCalculator
{
    public string CountryCode => "china";
    public string CountryName => Messages.CountryChina;
    public string CountryFlag => Messages.CountryChinaFlag;

    public decimal CalculateTotalPrice(decimal carPrice, string deliveryType, PricingConfig config)
    {
        var breakdown = GetBreakdown(carPrice, deliveryType, config);
        return breakdown.Total;
    }

    public CalculationBreakdown GetBreakdown(decimal carPrice, string deliveryType, PricingConfig config)
    {
        Console.WriteLine($"🔍 DEBUG ChinaCalculator.GetBreakdown:");
        Console.WriteLine($"   carPrice: {carPrice}");
        Console.WriteLine($"   config.ImportPreparation: {config.ImportPreparation}");
        Console.WriteLine($"   config.LandSeaDelivery: {config.LandSeaDelivery}");
        Console.WriteLine($"   config.Broker: {config.Broker}");
        Console.WriteLine($"   config.TransportFromPort: {config.TransportFromPort}");
        Console.WriteLine($"   config.CustomsPercent: {config.CustomsPercent}");
        Console.WriteLine($"   config.ImportServices: {config.ImportServices}");
        
        // Calculate customs as percentage of (car price + import services)
        decimal customs = (carPrice + config.ImportServices) * config.CustomsPercent;
        Console.WriteLine($"   calculated customs: {customs}");

        var breakdown = new CalculationBreakdown
        {
            CarPrice = carPrice,
            ImportPreparation = config.ImportPreparation,
            LandSeaDelivery = config.LandSeaDelivery,
            Broker = config.Broker,
            TransportFromPort = config.TransportFromPort,
            Customs = customs,
            ImportServices = config.ImportServices,
            DeliveryType = deliveryType
        };

        // Total calculation: Car + all expenses
        breakdown.Total = breakdown.CarPrice + 
                         breakdown.ImportPreparation + 
                         breakdown.LandSeaDelivery + 
                         breakdown.Broker + 
                         breakdown.TransportFromPort + 
                         breakdown.Customs + 
                         breakdown.ImportServices +
                         1600;
        
        Console.WriteLine($"   total: {breakdown.Total}");
        Console.WriteLine();

        return breakdown;
    }

    public string? GetCountrySpecificNotes()
    {
        return Messages.CountryChinaNotes;
    }
}
