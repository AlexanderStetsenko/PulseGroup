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
        decimal delivery = deliveryType == "ship" ? config.DeliveryShip : config.DeliveryTrain;
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
            DeliveryType = deliveryType
        };

        breakdown.Total = breakdown.CarPrice + breakdown.DocsPrice + breakdown.DeliveryPrice + 
                         breakdown.PortFee + breakdown.Customs + breakdown.Evacuator + 
                         breakdown.EuroRegistration + breakdown.ServicesFee;

        return breakdown;
    }

    public string? GetCountrySpecificNotes()
    {
        return Messages.CountryChinaNotes;
    }
}
