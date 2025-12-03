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

        // Ensure data directory exists
        if (!Directory.Exists(dataDirectory))
        {
            Console.WriteLine($"?? Creating data directory: {dataDirectory}");
            Directory.CreateDirectory(dataDirectory);
        }

        _configFilePath = Path.Combine(dataDirectory, ConfigFileName);
        _statsFilePath = Path.Combine(dataDirectory, StatsFileName);

        Console.WriteLine($"?? Data directory: {dataDirectory}");
        Console.WriteLine($"?? Config file: {_configFilePath}");
        Console.WriteLine($"?? Config file exists: {File.Exists(_configFilePath)}");
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
                Console.WriteLine($"?? Loading config from: {_configFilePath}");
                var json = File.ReadAllText(_configFilePath);
                Console.WriteLine($"?? Config file content ({json.Length} chars): {json.Substring(0, Math.Min(200, json.Length))}...");
                
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    AllowTrailingCommas = true,
                    ReadCommentHandling = JsonCommentHandling.Skip
                };
                
                var config = JsonSerializer.Deserialize<PricingConfig>(json, options);
                
                if (config != null)
                {
                    // Validate that config has non-zero values
                    if (config.ImportPreparation == 0 && config.LandSeaDelivery == 0)
                    {
                        Console.WriteLine("?? Config loaded but all values are zero - using defaults");
                        config = PricingConfig.GetDefault();
                        SaveConfiguration(config);
                    }
                    
                    Console.WriteLine(string.Format(Localization.Messages.ConsoleConfigLoaded, ConfigFileName));
                    Console.WriteLine($"?? Loaded values:");
                    Console.WriteLine($"   ImportPreparation: {config.ImportPreparation}");
                    Console.WriteLine($"   LandSeaDelivery: {config.LandSeaDelivery}");
                    Console.WriteLine($"   Broker: {config.Broker}");
                    Console.WriteLine($"   TransportFromPort: {config.TransportFromPort}");
                    Console.WriteLine($"   CustomsPercent: {config.CustomsPercent}");
                    Console.WriteLine($"   ImportServices: {config.ImportServices}");
                    Console.WriteLine();
                    return config;
                }
                else
                {
                    Console.WriteLine("?? Config deserialization returned null - using defaults");
                }
            }
            else
            {
                Console.WriteLine($"?? Config file not found at: {_configFilePath}");
            }

            Console.WriteLine(Localization.Messages.ConsoleConfigNotFound);
            var defaultConfig = PricingConfig.GetDefault();
            Console.WriteLine("?? Creating default config...");
            SaveConfiguration(defaultConfig);
            Console.WriteLine($"? Default config created with values:");
            Console.WriteLine($"   ImportPreparation: {defaultConfig.ImportPreparation}");
            Console.WriteLine($"   LandSeaDelivery: {defaultConfig.LandSeaDelivery}");
            Console.WriteLine($"   Broker: {defaultConfig.Broker}");
            Console.WriteLine($"   TransportFromPort: {defaultConfig.TransportFromPort}");
            Console.WriteLine($"   CustomsPercent: {defaultConfig.CustomsPercent}");
            Console.WriteLine($"   ImportServices: {defaultConfig.ImportServices}");
            return defaultConfig;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"? Error in LoadConfiguration:");
            Console.WriteLine($"   Message: {ex.Message}");
            Console.WriteLine($"   Type: {ex.GetType().Name}");
            Console.WriteLine($"   Stack: {ex.StackTrace}");
            Console.WriteLine(string.Format(Localization.Messages.ConsoleConfigLoadError, ex.Message));
            Console.WriteLine(Localization.Messages.ConsoleUsingDefaultConfig);
            var defaultConfig = PricingConfig.GetDefault();
            Console.WriteLine($"?? Attempting to save default config...");
            try
            {
                SaveConfiguration(defaultConfig);
                Console.WriteLine($"? Default config saved successfully");
            }
            catch (Exception saveEx)
            {
                Console.WriteLine($"? Failed to save default config: {saveEx.Message}");
            }
            return defaultConfig;
        }
    }

    /// <summary>
    /// Saves pricing configuration to JSON file
    /// </summary>
    public void SaveConfiguration(PricingConfig config)
    {
        try
        {
            Console.WriteLine($"?? Saving configuration to: {_configFilePath}");
            
            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNamingPolicy = null // Use exact property names
            };

            var json = JsonSerializer.Serialize(config, options);
            Console.WriteLine($"?? Serialized config ({json.Length} chars):");
            Console.WriteLine(json);
            
            File.WriteAllText(_configFilePath, json);
            
            // Verify file was written
            if (File.Exists(_configFilePath))
            {
                var fileInfo = new FileInfo(_configFilePath);
                Console.WriteLine($"? Config file saved successfully");
                Console.WriteLine($"   File size: {fileInfo.Length} bytes");
                Console.WriteLine($"   Last modified: {fileInfo.LastWriteTime}");
            }
            else
            {
                Console.WriteLine($"? Config file was not created!");
            }
            
            Console.WriteLine(string.Format(Localization.Messages.ConsoleConfigSaved, ConfigFileName));
        }
        catch (Exception ex)
        {
            Console.WriteLine($"? Error in SaveConfiguration:");
            Console.WriteLine($"   Message: {ex.Message}");
            Console.WriteLine($"   Type: {ex.GetType().Name}");
            Console.WriteLine($"   Stack: {ex.StackTrace}");
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
