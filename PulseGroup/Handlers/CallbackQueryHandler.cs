using Telegram.Bot;
using Telegram.Bot.Types;
using PulseGroup.Models;

namespace PulseGroup.Handlers;

/// <summary>
/// Handler for callback query (button presses)
/// </summary>
public class CallbackQueryHandler
{
    private readonly Dictionary<long, CarCalculation> _userSessions;
    private readonly Dictionary<long, AdminSession> _adminSessions;
    private readonly CalculationHandler _calculationHandler;
    private readonly AdminCallbackHandler _adminCallbackHandler;

    public CallbackQueryHandler(
        Dictionary<long, CarCalculation> userSessions,
        Dictionary<long, AdminSession> adminSessions,
        CalculationHandler calculationHandler,
        AdminCallbackHandler adminCallbackHandler)
    {
        _userSessions = userSessions;
        _adminSessions = adminSessions;
        _calculationHandler = calculationHandler;
        _adminCallbackHandler = adminCallbackHandler;
    }

    /// <summary>
    /// Handles callback query from inline buttons
    /// </summary>
    public async Task HandleCallbackQueryAsync(ITelegramBotClient botClient, CallbackQuery callbackQuery, CancellationToken cancellationToken)
    {
        var chatId = callbackQuery.Message!.Chat.Id;
        var data = callbackQuery.Data!;

        Console.WriteLine($"?? Callback: {data} from ChatID: {chatId}");

        // Answer callback query to remove loading state
        await botClient.AnswerCallbackQuery(callbackQuery.Id, cancellationToken: cancellationToken);

        // Route to appropriate handler
        if (data == "main_menu")
        {
            await HandleMainMenuCallback(botClient, chatId, cancellationToken);
        }
        else if (data == "new_calculation")
        {
            _adminSessions.Remove(chatId);
            await _calculationHandler.StartCalculationAsync(botClient, chatId, cancellationToken);
        }
        else if (data.StartsWith("country_"))
        {
            var country = data.Replace("country_", "");
            await _calculationHandler.HandleCountrySelectionAsync(botClient, chatId, country, cancellationToken);
        }
        else if (data.StartsWith("delivery_"))
        {
            var deliveryType = data.Replace("delivery_", "");
            await _calculationHandler.HandleDeliverySelectionAsync(botClient, chatId, deliveryType, cancellationToken);
        }
        else if (data.StartsWith("admin_"))
        {
            await _adminCallbackHandler.HandleAdminCallbackAsync(botClient, chatId, data, cancellationToken);
        }
    }

    private async Task HandleMainMenuCallback(ITelegramBotClient botClient, long chatId, CancellationToken cancellationToken)
    {
        // Clear any active sessions
        _userSessions.Remove(chatId);
        _adminSessions.Remove(chatId);

        var message = $"{Localization.Messages.WelcomeTitle}\n\n" +
                     $"{Localization.Messages.WelcomeSubtitle}\n\n" +
                     $"{Localization.Messages.AvailableCommands}\n" +
                     $"{Localization.Messages.CommandCalculate}\n" +
                     $"{Localization.Messages.CommandExample}\n" +
                     $"{Localization.Messages.CommandHelp}\n" +
                     $"{Localization.Messages.CommandAbout}\n" +
                     $"{Localization.Messages.CommandAdmin}";

        await MessageHelper.SendMessageSafeAsync(botClient, chatId, message, cancellationToken);
    }
}
