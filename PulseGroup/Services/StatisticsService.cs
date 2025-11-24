using PulseGroup.Models;
using PulseGroup.Handlers;

namespace PulseGroup.Services;

/// <summary>
/// Service for tracking bot statistics
/// </summary>
public class StatisticsService
{
    private int _totalCalculations;
    private decimal _totalAmount;
    private decimal _minAmount;
    private decimal _maxAmount;
    private readonly DateTime _startTime;
    private readonly ConfigurationService _configurationService;

    public StatisticsService(ConfigurationService configurationService)
    {
        _configurationService = configurationService;

        // Load existing statistics
        var stats = _configurationService.LoadStatistics();
        _totalCalculations = stats.TotalCalculations;
        _totalAmount = stats.TotalAmount;
        _minAmount = stats.MinAmount;
        _maxAmount = stats.MaxAmount;
        _startTime = stats.StartTime;
    }

    /// <summary>
    /// Records a new calculation
    /// </summary>
    public void RecordCalculation(decimal totalPrice)
    {
        _totalCalculations++;
        _totalAmount += totalPrice;

        if (totalPrice < _minAmount)
            _minAmount = totalPrice;

        if (totalPrice > _maxAmount)
            _maxAmount = totalPrice;

        Console.WriteLine(string.Format(Localization.Messages.ConsoleStatsUpdate, _totalCalculations, _totalAmount));

        // Save statistics after each calculation
        SaveStatistics();
    }

    /// <summary>
    /// Gets formatted statistics text
    /// </summary>
    public string GetStatisticsText()
    {
        var uptime = DateTime.Now - _startTime;
        var avgAmount = _totalCalculations > 0 ? _totalAmount / _totalCalculations : 0;

        return $"{Localization.Messages.StatsTitle}\n\n" +
               $"{string.Format(Localization.Messages.StatsTotalCalculations, _totalCalculations)}\n" +
               $"{string.Format(Localization.Messages.StatsTotalAmount, _totalAmount)}\n" +
               $"{string.Format(Localization.Messages.StatsAverageAmount, avgAmount)}\n" +
               $"{string.Format(Localization.Messages.StatsMinAmount, _minAmount == decimal.MaxValue ? 0 : _minAmount)}\n" +
               $"{string.Format(Localization.Messages.StatsMaxAmount, _maxAmount)}\n\n" +
               $"{string.Format(Localization.Messages.StatsUptime, uptime.Days, uptime.Hours, uptime.Minutes)}\n" +
               $"{string.Format(Localization.Messages.StatsStarted, _startTime)}";
    }

    /// <summary>
    /// Resets statistics
    /// </summary>
    public void Reset()
    {
        _totalCalculations = 0;
        _totalAmount = 0;
        _minAmount = decimal.MaxValue;
        _maxAmount = 0;

        // Save reset statistics
        SaveStatistics();
    }

    /// <summary>
    /// Saves current statistics to file
    /// </summary>
    private void SaveStatistics()
    {
        var stats = new BotStatistics
        {
            TotalCalculations = _totalCalculations,
            TotalAmount = _totalAmount,
            MinAmount = _minAmount,
            MaxAmount = _maxAmount,
            StartTime = _startTime,
            LastSaved = DateTime.Now
        };

        _configurationService.SaveStatistics(stats);
    }
}
