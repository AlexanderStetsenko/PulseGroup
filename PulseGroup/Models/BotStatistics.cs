namespace PulseGroup.Models;

/// <summary>
/// Model for bot statistics
/// </summary>
public class BotStatistics
{
    public int TotalCalculations { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal MinAmount { get; set; }
    public decimal MaxAmount { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime LastSaved { get; set; }

    /// <summary>
    /// Creates default statistics
    /// </summary>
    public static BotStatistics GetDefault()
    {
        return new BotStatistics
        {
            TotalCalculations = 0,
            TotalAmount = 0,
            MinAmount = decimal.MaxValue,
            MaxAmount = 0,
            StartTime = DateTime.Now,
            LastSaved = DateTime.Now
        };
    }
}
