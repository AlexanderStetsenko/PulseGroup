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
               $"{string.Format(Messages.PricingImportPreparation, _pricingConfig.ImportPreparation)}\n" +
               $"{string.Format(Messages.PricingLandSeaDelivery, _pricingConfig.LandSeaDelivery)}\n" +
               $"{string.Format(Messages.PricingBroker, _pricingConfig.Broker)}\n" +
               $"{string.Format(Messages.PricingTransportFromPort, _pricingConfig.TransportFromPort)}\n" +
               $"{string.Format(Messages.PricingCustoms, _pricingConfig.CustomsPercent * 100)}\n" +
               $"{string.Format(Messages.PricingImportServices, _pricingConfig.ImportServices)}";
    }

    /// <summary>
    /// Gets detailed calculation result text
    /// </summary>
    public string GetCalculationResultText(CarCalculation calculation, decimal carPrice, decimal total)
    {
        var calculator = _calculatorFactory.GetCalculator(calculation.Country!);
        var breakdown = calculator.GetBreakdown(carPrice, calculation.DeliveryType!, _pricingConfig);

        var result = $"{Messages.ResultTitle}\n\n" +
               $"Машина сама {breakdown.CarPrice:N0}$\n" +
               $"Доля по расходам\n" +
               $"-подготовка импорта {breakdown.ImportPreparation:N0}$\n" +
               $"-доставка суша + море {breakdown.LandSeaDelivery:N0}$\n" +
               $"-брокер {breakdown.Broker:N0}$\n" +
               $"-транспорт от порта {breakdown.TransportFromPort:N0}$\n" +
               $"-таможня {breakdown.Customs:N0}$ ({_pricingConfig.CustomsPercent * 100:N0}%)\n" +
               $"-услуги за привоз {breakdown.ImportServices:N0}$\n\n" +
               $"━━━━━━━━━━━━━━━━━━━━\n" +
               $"Всего: {breakdown.Total:N0}$\n" +
               $"━━━━━━━━━━━━━━━━━━━━\n\n" +            
               $"📝 Для нового расчета используйте /calculate";

        return result;
    }

    /// <summary>
    /// Gets example calculation text
    /// </summary>
    public string GetExampleText()
    {
        decimal examplePrice = 2000m;
        var calculator = _calculatorFactory.GetCalculator("china");
        var breakdown = calculator.GetBreakdown(examplePrice, "train", _pricingConfig);

        return $"{Messages.ExampleTitle}\n\n" +
               $"Машина сама {breakdown.CarPrice:N0}$\n" +
               $"Доля по расходам\n" +
               $"-подготовка импорта {breakdown.ImportPreparation:N0}$\n" +
               $"-доставка суша + море {breakdown.LandSeaDelivery:N0}$\n" +
               $"-брокер {breakdown.Broker:N0}$\n" +
               $"-транспорт от порта {breakdown.TransportFromPort:N0}$\n" +
               $"-таможня {breakdown.Customs:N0}$ ({_pricingConfig.CustomsPercent * 100:N0}%)\n" +
               $"-услуги за привоз {breakdown.ImportServices:N0}$\n\n" +
               $"━━━━━━━━━━━━━━━━━━━━\n" +
               $"Всего {breakdown.Total:N0}$\n" +
               $"━━━━━━━━━━━━━━━━━━━━\n\n" +
               $"💡 Используйте /calculate для своего расчета!";
    }
}
