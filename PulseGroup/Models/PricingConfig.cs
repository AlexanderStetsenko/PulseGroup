using System.Text.Json.Serialization;

namespace PulseGroup.Models;

/// <summary>
/// Configuration for pricing/tariffs that can be modified by admin
/// </summary>
public class PricingConfig
{
    [JsonPropertyName("ImportPreparation")]
    public decimal ImportPreparation { get; set; }
    
    [JsonPropertyName("LandSeaDelivery")]
    public decimal LandSeaDelivery { get; set; }
    
    [JsonPropertyName("Broker")]
    public decimal Broker { get; set; }
    
    [JsonPropertyName("TransportFromPort")]
    public decimal TransportFromPort { get; set; }
    
    [JsonPropertyName("CustomsPercent")]
    public decimal CustomsPercent { get; set; }
    
    [JsonPropertyName("ImportServices")]
    public decimal ImportServices { get; set; }

    /// <summary>
    /// Returns default pricing configuration
    /// </summary>
    public static PricingConfig GetDefault()
    {
        return new PricingConfig
        {
            ImportPreparation = 2000m,
            LandSeaDelivery = 3000m,
            Broker = 400m,
            TransportFromPort = 700m,
            CustomsPercent = 0.31m,
            ImportServices = 1600m
        };
    }

    /// <summary>
    /// Resets all values to defaults
    /// </summary>
    public void ResetToDefaults()
    {
        var defaults = GetDefault();
        ImportPreparation = defaults.ImportPreparation;
        LandSeaDelivery = defaults.LandSeaDelivery;
        Broker = defaults.Broker;
        TransportFromPort = defaults.TransportFromPort;
        CustomsPercent = defaults.CustomsPercent;
        ImportServices = defaults.ImportServices;
    }

    /// <summary>
    /// Applies a multiplier to all price values (except customs percent)
    /// </summary>
    public void ApplyMultiplier(decimal multiplier)
    {
        ImportPreparation = Math.Round(ImportPreparation * multiplier, 2);
        LandSeaDelivery = Math.Round(LandSeaDelivery * multiplier, 2);
        Broker = Math.Round(Broker * multiplier, 2);
        TransportFromPort = Math.Round(TransportFromPort * multiplier, 2);
        ImportServices = Math.Round(ImportServices * multiplier, 2);
        // Don't adjust CustomsPercent - it's a percentage
    }
}
