namespace PulseGroup.Services.Calculators;

/// <summary>
/// Factory for creating country-specific calculators
/// </summary>
public class CalculatorFactory
{
    private readonly Dictionary<string, ICountryCalculator> _calculators;

    public CalculatorFactory()
    {
        _calculators = new Dictionary<string, ICountryCalculator>
        {
            ["china"] = new ChinaCalculator(),
            ["usa"] = new USACalculator(),
            ["europe"] = new EuropeCalculator()
        };
    }

    /// <summary>
    /// Gets calculator for specified country
    /// </summary>
    public ICountryCalculator GetCalculator(string countryCode)
    {
        if (_calculators.TryGetValue(countryCode.ToLower(), out var calculator))
        {
            return calculator;
        }

        // Default to China calculator if country not found
        return _calculators["china"];
    }

    /// <summary>
    /// Gets all available calculators
    /// </summary>
    public IEnumerable<ICountryCalculator> GetAllCalculators()
    {
        return _calculators.Values;
    }

    /// <summary>
    /// Checks if country is supported
    /// </summary>
    public bool IsCountrySupported(string countryCode)
    {
        return _calculators.ContainsKey(countryCode.ToLower());
    }
}
