using PulseGroup.Models;
using PulseGroup.Services.Calculators;
using static PulseGroup.Handlers.Localization;

namespace PulseGroup.Services;

/// <summary>
/// Service for calculation and pricing operations
/// </summary>
public class CalculationService
{
    private readonly PricingConfig _pricingConfig;
    private readonly CalculatorFactory _calculatorFactory;

    public CalculationService(PricingConfig pricingConfig)
    {
        _pricingConfig = pricingConfig;
        _calculatorFactory = new CalculatorFactory();
    }

    /// <summary>
    /// Calculates the total price for a car using country-specific calculator
    /// </summary>
    public decimal CalculateTotalPrice(decimal carPrice, string deliveryType, string countryCode)
    {
        var calculator = _calculatorFactory.GetCalculator(countryCode);
        return calculator.CalculateTotalPrice(carPrice, deliveryType, _pricingConfig);
    }

    /// <summary>
    /// Gets breakdown using country-specific calculator
    /// </summary>
    public CalculationBreakdown GetBreakdown(decimal carPrice, string deliveryType, string countryCode)
    {
        var calculator = _calculatorFactory.GetCalculator(countryCode);
        return calculator.GetBreakdown(carPrice, deliveryType, _pricingConfig);
    }

    /// <summary>
    /// Gets country-specific calculator
    /// </summary>
    public ICountryCalculator GetCalculatorForCountry(string countryCode)
    {
        return _calculatorFactory.GetCalculator(countryCode);
    }

    /// <summary>
    /// Gets formatted pricing text for display
    /// </summary>
    public string GetPricingText()
    {
        return $"{Messages.PricingTitle}\n\n" +
               $"{string.Format(Messages.PricingDocsChina, _pricingConfig.Docs)}\n" +
               $"{string.Format(Messages.PricingPort, _pricingConfig.PortFee)}\n" +
               $"{string.Format(Messages.PricingEvacuator, _pricingConfig.Evacuator)}\n" +
               $"{string.Format(Messages.PricingEuroReg, _pricingConfig.EuroRegistration)}\n" +
               $"{string.Format(Messages.PricingServices, _pricingConfig.ServicesFee)}\n" +
               $"{string.Format(Messages.PricingDeliveryShip, _pricingConfig.DeliveryShip)}\n" +
               $"{string.Format(Messages.PricingDeliveryTrain, _pricingConfig.DeliveryTrain)}\n" +
               $"{string.Format(Messages.PricingCustoms, _pricingConfig.CustomsPercent * 100)}";
    }

    /// <summary>
    /// Gets detailed calculation result text
    /// </summary>
    public string GetCalculationResultText(CarCalculation calculation, decimal carPrice, decimal total)
    {
        var calculator = _calculatorFactory.GetCalculator(calculation.Country!);
        var breakdown = calculator.GetBreakdown(carPrice, calculation.DeliveryType!, _pricingConfig);

        var deliveryText = breakdown.DeliveryType switch
        {
            "ship" => Messages.DeliveryShip,
            "train" => Messages.DeliveryTrain,
            "road" => Messages.DeliveryRoad,
            _ => Messages.DeliveryRoad
        };

        var result = $"{Messages.ResultTitle}\n\n" +
               $"{calculator.CountryFlag} Страна: *{calculator.CountryName}*\n" +
               $"{string.Format(Messages.ResultCarPrice, carPrice)}\n" +
               $"🚚 Доставка: *{deliveryText}*\n\n" +
               $"━━━━━━━━━━━━━━━━━━━━\n" +
               $"{string.Format(Messages.ResultTotal, breakdown.Total)}\n" +
               $"━━━━━━━━━━━━━━━━━━━━\n\n" +
               $"{Messages.ResultDetailsTitle}\n" +
               $"• авто ${breakdown.CarPrice:N0}\n" +
               $"• доки ${breakdown.DocsPrice:N0}\n" +
               $"• доставка ${breakdown.DeliveryPrice:N0}\n";

        // Only show port fee if applicable
        if (breakdown.PortFee > 0)
        {
            result += $"• порт ${breakdown.PortFee:N0}\n";
        }

        // Only show customs if applicable
        if (breakdown.Customs > 0)
        {
            result += $"• таможня ${breakdown.Customs:N0}\n";
        }

        // Only show evacuator if applicable
        if (breakdown.Evacuator > 0)
        {
            result += $"• эвакуатор ${breakdown.Evacuator:N0}\n";
        }

        result += $"• учёт Европы ${breakdown.EuroRegistration:N0}\n" +
                 $"• услуги за привоз ${breakdown.ServicesFee:N0}\n\n";

        // Add country-specific notes
        var notes = calculator.GetCountrySpecificNotes();
        if (!string.IsNullOrEmpty(notes))
        {
            result += $"ℹ️ {notes}\n\n";
        }

        result += $"{Messages.ResultTurnkey}\n\n" +
                 $"{Messages.ResultNewCalculation}";

        return result;
    }

    /// <summary>
    /// Gets example calculation text
    /// </summary>
    public string GetExampleText()
    {
        decimal examplePrice = 93285m;
        var calculator = _calculatorFactory.GetCalculator("china");
        var breakdown = calculator.GetBreakdown(examplePrice, "train", _pricingConfig);

        return $"{Messages.ExampleTitle}\n\n" +
               $"{Messages.ExampleCarModel}\n" +
               $"{Messages.ExampleCarDescription}\n\n" +
               $"━━━━━━━━━━━━━━━━━━━━\n" +
               $"{string.Format(Messages.ExampleTotal, breakdown.Total)}\n" +
               $"━━━━━━━━━━━━━━━━━━━━\n\n" +
               $"{Messages.ExampleIncluded}\n" +
               $"• авто ${breakdown.CarPrice:N0}\n" +
               $"• доки в Китае ${breakdown.DocsPrice:N0}\n" +
               $"• доставка суша+ЖД ${breakdown.DeliveryPrice:N0}\n" +
               $"• порт ${breakdown.PortFee:N0}\n" +
               $"• таможня ${breakdown.Customs:N0}\n" +
               $"• эвакуатор ${breakdown.Evacuator:N0}\n" +
               $"• учёт Европа ${breakdown.EuroRegistration:N0}\n" +
               $"• услуги за привоз ${breakdown.ServicesFee:N0}\n\n" +
               $"{Messages.ResultTurnkey}\n\n" +
               $"{Messages.ExampleUseCalculate}";
    }
}
