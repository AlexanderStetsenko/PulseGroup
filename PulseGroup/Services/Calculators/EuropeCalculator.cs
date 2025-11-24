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
        // TODO: Implement Europe calculator
        var breakdown = GetBreakdown(carPrice, deliveryType, config);
        return breakdown.Total;
    }

    public CalculationBreakdown GetBreakdown(decimal carPrice, string deliveryType, PricingConfig config)
    {
        // PLACEHOLDER: Not implemented yet - using simplified logic temporarily
        decimal customs = 0; // No customs within EU

        var breakdown = new CalculationBreakdown
        {
            CarPrice = carPrice,
            DocsPrice = config.Docs * 0.5m,
            DeliveryPrice = config.Evacuator,
            PortFee = 0,
            Customs = customs,
            Evacuator = 0,
            EuroRegistration = config.EuroRegistration,
            ServicesFee = config.ServicesFee,
            DeliveryType = "road"
        };

        breakdown.Total = breakdown.CarPrice + breakdown.DocsPrice + breakdown.DeliveryPrice + 
                         breakdown.PortFee + breakdown.Customs + breakdown.Evacuator + 
                         breakdown.EuroRegistration + breakdown.ServicesFee;

        return breakdown;
    }

    public string? GetCountrySpecificNotes()
    {
        return Messages.CountryEuropeNotes;
    }
}
