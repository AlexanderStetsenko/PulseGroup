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
        // TODO: Implement USA calculator
        var breakdown = GetBreakdown(carPrice, deliveryType, config);
        return breakdown.Total;
    }

    public CalculationBreakdown GetBreakdown(decimal carPrice, string deliveryType, PricingConfig config)
    {
        // PLACEHOLDER: Not implemented yet - using China calculator logic temporarily
        decimal delivery = config.DeliveryShip; // USA uses ship only
        decimal customs = carPrice * config.CustomsPercent;

        var breakdown = new CalculationBreakdown
        {
            CarPrice = carPrice,
            DocsPrice = config.Docs,
            DeliveryPrice = delivery,
            PortFee = config.PortFee,
            Customs = customs,
            Evacuator = config.Evacuator,
            EuroRegistration = config.EuroRegistration,
            ServicesFee = config.ServicesFee,
            DeliveryType = "ship"
        };

        breakdown.Total = breakdown.CarPrice + breakdown.DocsPrice + breakdown.DeliveryPrice + 
                         breakdown.PortFee + breakdown.Customs + breakdown.Evacuator + 
                         breakdown.EuroRegistration + breakdown.ServicesFee;

        return breakdown;
    }

    public string? GetCountrySpecificNotes()
    {
        return Messages.CountryUSANotes;
    }
}
