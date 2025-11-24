using System.Text.Json;
using PulseGroup.Models;
using PulseGroup.Handlers;

namespace PulseGroup.Services;

/// <summary>
/// Service for loading and saving configuration to JSON file
/// </summary>
public class ConfigurationService
{
    private const string ConfigFileName = "pricing_config.json";
    private const string StatsFileName = "bot_statistics.json";
    private readonly string _configFilePath;
    private readonly string _statsFilePath;

    public ConfigurationService()
    {
        // Use /app/data in Docker, or current directory otherwise
        var dataDirectory = Directory.Exists("/app/data") 
            ? "/app/data" 
            : AppDomain.CurrentDomain.BaseDirectory;

        _configFilePath = Path.Combine(dataDirectory, ConfigFileName);
        _statsFilePath = Path.Combine(dataDirectory, StatsFileName);

        Console.WriteLine($"?? Data directory: {dataDirectory}");
    }

    /// <summary>
    /// Loads pricing configuration from JSON file or creates default if not exists
    /// </summary>
    public PricingConfig LoadConfiguration()
    {
        try
        {
            if (File.Exists(_configFilePath))
            {
                var json = File.ReadAllText(_configFilePath);
                var config = JsonSerializer.Deserialize<PricingConfig>(json);
                
                if (config != null)
                {
                    Console.WriteLine(string.Format(Localization.Messages.ConsoleConfigLoaded, ConfigFileName));
                    return config;
                }
            }

            Console.WriteLine(Localization.Messages.ConsoleConfigNotFound);
            var defaultConfig = PricingConfig.GetDefault();
            SaveConfiguration(defaultConfig);
            return defaultConfig;
        }
        catch (Exception ex)
        {
            Console.WriteLine(string.Format(Localization.Messages.ConsoleConfigLoadError, ex.Message));
            Console.WriteLine(Localization.Messages.ConsoleUsingDefaultConfig);
            return PricingConfig.GetDefault();
        }
    }

    /// <summary>
    /// Saves pricing configuration to JSON file
    /// </summary>
    public void SaveConfiguration(PricingConfig config)
    {
        try
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true
            };

            var json = JsonSerializer.Serialize(config, options);
            File.WriteAllText(_configFilePath, json);
            
            Console.WriteLine(string.Format(Localization.Messages.ConsoleConfigSaved, ConfigFileName));
        }
        catch (Exception ex)
        {
            Console.WriteLine(string.Format(Localization.Messages.ConsoleConfigSaveError, ex.Message));
        }
    }

    /// <summary>
    /// Loads bot statistics from JSON file or creates default if not exists
    /// </summary>
    public BotStatistics LoadStatistics()
    {
        try
        {
            if (File.Exists(_statsFilePath))
            {
                var json = File.ReadAllText(_statsFilePath);
                var stats = JsonSerializer.Deserialize<BotStatistics>(json);
                
                if (stats != null)
                {
                    Console.WriteLine(string.Format(Localization.Messages.ConsoleStatsLoaded, StatsFileName));
                    Console.WriteLine(string.Format(Localization.Messages.ConsoleStatsLoadedInfo, stats.TotalCalculations, stats.TotalAmount));
                    return stats;
                }
            }

            Console.WriteLine(Localization.Messages.ConsoleStatsNotFound);
            var defaultStats = BotStatistics.GetDefault();
            SaveStatistics(defaultStats);
            return defaultStats;
        }
        catch (Exception ex)
        {
            Console.WriteLine(string.Format(Localization.Messages.ConsoleStatsLoadError, ex.Message));
            Console.WriteLine(Localization.Messages.ConsoleUsingDefaultStats);
            return BotStatistics.GetDefault();
        }
    }

    /// <summary>
    /// Saves bot statistics to JSON file
    /// </summary>
    public void SaveStatistics(BotStatistics stats)
    {
        try
        {
            stats.LastSaved = DateTime.Now;
            
            var options = new JsonSerializerOptions
            {
                WriteIndented = true
            };

            var json = JsonSerializer.Serialize(stats, options);
            File.WriteAllText(_statsFilePath, json);
            
            Console.WriteLine(string.Format(Localization.Messages.ConsoleStatsSaved, StatsFileName));
        }
        catch (Exception ex)
        {
            Console.WriteLine(string.Format(Localization.Messages.ConsoleStatsSaveError, ex.Message));
        }
    }

    /// <summary>
    /// Gets the full path to the configuration file
    /// </summary>
    public string GetConfigFilePath()
    {
        return _configFilePath;
    }

    /// <summary>
    /// Gets the full path to the statistics file
    /// </summary>
    public string GetStatsFilePath()
    {
        return _statsFilePath;
    }
}
