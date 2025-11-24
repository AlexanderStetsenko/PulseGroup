using Telegram.Bot;
using PulseGroup.Configuration;

namespace PulseGroup.Services;

/// <summary>
/// Service for initializing and testing bot connection
/// </summary>
public static class BotInitializer
{
    /// <summary>
    /// Creates a new bot client
    /// </summary>
    public static TelegramBotClient CreateBotClient()
    {
        return new TelegramBotClient(BotConfig.BotToken);
    }

    /// <summary>
    /// Tests bot connection and displays bot info
    /// </summary>
    public static async Task<bool> TestConnectionAsync(ITelegramBotClient botClient, CancellationToken cancellationToken)
    {
        try
        {
            var me = await botClient.GetMe(cancellationToken);

            Console.WriteLine($"Bot connected successfully!");
            Console.WriteLine($"Bot ID: {me.Id}");
            Console.WriteLine($"Bot Name: {me.FirstName}");
            Console.WriteLine($"Bot Username: @{me.Username}");
            Console.WriteLine();

            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to connect to Telegram:");
            Console.WriteLine($"Error: {ex.Message}");
            Console.WriteLine();
            Console.WriteLine("Please check:");
            Console.WriteLine("1. Your bot token is correct");
            Console.WriteLine("2. You have internet connection");
            Console.WriteLine("3. Telegram is not blocked (try VPN if needed)");
            Console.WriteLine();

            return false;
        }
    }
}
