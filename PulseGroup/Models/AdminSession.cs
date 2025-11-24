namespace PulseGroup.Models;

/// <summary>
/// Represents an admin session with authentication state
/// </summary>
public class AdminSession
{
    public bool IsAuthenticated { get; set; }
    public bool AwaitingInput { get; set; }
    public string? CurrentSetting { get; set; }
}
