using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace PulseGroup.Handlers;

/// <summary>
/// Helper class for sending messages with retry logic
/// </summary>
public static class MessageHelper
{
    /// <summary>
    /// Sends a message with automatic retry on failure
    /// </summary>
    public static async Task SendMessageSafeAsync(
        ITelegramBotClient botClient,
        long chatId,
        string text,
        CancellationToken cancellationToken,
        ParseMode parseMode = default,
        InlineKeyboardMarkup? replyMarkup = null)
    {
        int retryCount = 0;
        int maxRetries = 3;
        bool parseModeFailed = false;

        while (retryCount < maxRetries)
        {
            try
            {
                var currentParseMode = parseModeFailed ? ParseMode.None : parseMode;

                await botClient.SendMessage(
                    chatId: chatId,
                    text: text,
                    parseMode: currentParseMode == default ? ParseMode.None : currentParseMode,
                    replyMarkup: replyMarkup,
                    cancellationToken: cancellationToken);
                return; // Success
            }
            catch (ApiRequestException ex) when (ex.Message.Contains("can't parse entities"))
            {
                Console.WriteLine($"?? Markdown parse error: {ex.Message}");

                if (!parseModeFailed)
                {
                    Console.WriteLine("?? Retrying without ParseMode...");
                    parseModeFailed = true;
                    retryCount++; // Count this as a retry
                    continue; // Try again without ParseMode
                }

                Console.WriteLine($"? Failed to send message even without ParseMode");
                throw;
            }
            catch (Exception ex)
            {
                retryCount++;
                Console.WriteLine($"? Failed to send message: {ex.Message}");
                Console.WriteLine($"Retry attempt {retryCount}/{maxRetries}");

                if (retryCount < maxRetries)
                {
                    await Task.Delay(2000 * retryCount, cancellationToken); // Exponential backoff
                }
                else
                {
                    Console.WriteLine($"Failed to send message after {maxRetries} attempts");
                    throw;
                }
            }
        }
    }
}
