namespace PulseGroup.Models;

/// <summary>
/// Configuration for pricing/tariffs that can be modified by admin
/// </summary>
public class PricingConfig
{
    public decimal Docs { get; set; }
    public decimal PortFee { get; set; }
    public decimal Evacuator { get; set; }
    public decimal EuroRegistration { get; set; }
    public decimal ServicesFee { get; set; }
    public decimal DeliveryShip { get; set; }
    public decimal DeliveryTrain { get; set; }
    public decimal CustomsPercent { get; set; }

    /// <summary>
    /// Returns default pricing configuration
    /// </summary>
    public static PricingConfig GetDefault()
    {
        return new PricingConfig
        {
            Docs = 1500m,
            PortFee = 700m,
            Evacuator = 3050m,
            EuroRegistration = 1500m,
            ServicesFee = 1600m,
            DeliveryShip = 1500m,
            DeliveryTrain = 3500m,
            CustomsPercent = 0.31m
        };
    }

    /// <summary>
    /// Resets all values to defaults
    /// </summary>
    public void ResetToDefaults()
    {
        var defaults = GetDefault();
        Docs = defaults.Docs;
        PortFee = defaults.PortFee;
        Evacuator = defaults.Evacuator;
        EuroRegistration = defaults.EuroRegistration;
        ServicesFee = defaults.ServicesFee;
        DeliveryShip = defaults.DeliveryShip;
        DeliveryTrain = defaults.DeliveryTrain;
        CustomsPercent = defaults.CustomsPercent;
    }

    /// <summary>
    /// Applies a multiplier to all price values (except customs percent)
    /// </summary>
    public void ApplyMultiplier(decimal multiplier)
    {
        Docs = Math.Round(Docs * multiplier, 2);
        PortFee = Math.Round(PortFee * multiplier, 2);
        Evacuator = Math.Round(Evacuator * multiplier, 2);
        EuroRegistration = Math.Round(EuroRegistration * multiplier, 2);
        ServicesFee = Math.Round(ServicesFee * multiplier, 2);
        DeliveryShip = Math.Round(DeliveryShip * multiplier, 2);
        DeliveryTrain = Math.Round(DeliveryTrain * multiplier, 2);
        // Don't adjust CustomsPercent - it's a percentage
    }
}
