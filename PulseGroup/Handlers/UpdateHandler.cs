using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using PulseGroup.Models;

namespace PulseGroup.Handlers;

/// <summary>
/// Main update handler that routes updates to appropriate handlers
/// </summary>
public class UpdateHandler
{
    private readonly Dictionary<long, CarCalculation> _userSessions;
    private readonly Dictionary<long, AdminSession> _adminSessions;
    private readonly CommandHandler _commandHandler;
    private readonly CalculationHandler _calculationHandler;
    private readonly AdminHandler _adminHandler;
    private readonly CallbackQueryHandler _callbackQueryHandler;

    public UpdateHandler(
        Dictionary<long, CarCalculation> userSessions,
        Dictionary<long, AdminSession> adminSessions,
        CommandHandler commandHandler,
        CalculationHandler calculationHandler,
        AdminHandler adminHandler,
        CallbackQueryHandler callbackQueryHandler)
    {
        _userSessions = userSessions;
        _adminSessions = adminSessions;
        _commandHandler = commandHandler;
        _calculationHandler = calculationHandler;
        _adminHandler = adminHandler;
        _callbackQueryHandler = callbackQueryHandler;
    }

    /// <summary>
    /// Main update handler
    /// </summary>
    public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        try
        {
            Console.WriteLine($"????????????????????????????????????????");
            Console.WriteLine($"?? UPDATE RECEIVED: Type = {update.Type}");
            Console.WriteLine($"? Time: {DateTime.Now:HH:mm:ss}");

            if (update.Message != null)
            {
                Console.WriteLine($"?? From: @{update.Message.From?.Username ?? "Unknown"} (ID: {update.Message.From?.Id})");
                Console.WriteLine($"?? Message: {update.Message.Text ?? "[non-text]"}");
            }
            else if (update.CallbackQuery != null)
            {
                Console.WriteLine($"?? Callback: {update.CallbackQuery.Data}");
                Console.WriteLine($"?? From: @{update.CallbackQuery.From?.Username ?? "Unknown"}");
            }
            Console.WriteLine($"????????????????????????????????????????");
            Console.WriteLine();

            var handler = update.Type switch
            {
                UpdateType.Message => HandleMessageAsync(botClient, update.Message!, cancellationToken),
                UpdateType.CallbackQuery => _callbackQueryHandler.HandleCallbackQueryAsync(botClient, update.CallbackQuery!, cancellationToken),
                _ => HandleUnknownUpdateAsync(update)
            };

            await handler;

            Console.WriteLine("? Update processed successfully");
            Console.WriteLine();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"? ERROR in HandleUpdateAsync:");
            Console.WriteLine($"   Message: {ex.Message}");
            Console.WriteLine($"   Type: {ex.GetType().Name}");
            Console.WriteLine($"   Stack trace:");
            Console.WriteLine($"   {ex.StackTrace}");
            Console.WriteLine();
        }
    }

    /// <summary>
    /// Handles message updates
    /// </summary>
    private async Task HandleMessageAsync(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken)
    {
        if (message.Text == null)
            return;

        var chatId = message.Chat.Id;

        Console.WriteLine($"?? Message from @{message.From?.Username} (ID: {chatId}): {message.Text}");

        // Check if admin is in authentication process
        if (_adminSessions.ContainsKey(chatId) && !_adminSessions[chatId].IsAuthenticated)
        {
            Console.WriteLine($"?? Admin auth process detected for chatId: {chatId}");
            await _adminHandler.HandleAuthenticationAsync(botClient, message, cancellationToken);
            return;
        }

        // Check if admin is providing input for settings
        if (_adminSessions.ContainsKey(chatId) && _adminSessions[chatId].IsAuthenticated && _adminSessions[chatId].AwaitingInput)
        {
            Console.WriteLine($"?? Admin input detected for chatId: {chatId}");
            await _adminHandler.HandleAdminInputAsync(botClient, message, cancellationToken);
            return;
        }

        // Handle commands
        if (message.Text.StartsWith('/'))
        {
            Console.WriteLine($"?? Command detected: {message.Text}");
            await _commandHandler.HandleCommandAsync(botClient, message, cancellationToken);
            return;
        }

        // Handle calculation steps
        if (_userSessions.ContainsKey(chatId))
        {
            Console.WriteLine($"?? Calculation step for chatId: {chatId}");
            await _calculationHandler.HandleCalculationStepAsync(botClient, message, cancellationToken);
            return;
        }

        // Unknown message
        Console.WriteLine($"? Unknown message type from chatId: {chatId}");
        await MessageHelper.SendMessageSafeAsync(botClient, chatId,
            Localization.Messages.UnknownMessage,
            cancellationToken,
            includeMainMenuButton: true);
    }

    /// <summary>
    /// Handles unknown update types
    /// </summary>
    private Task HandleUnknownUpdateAsync(Update update)
    {
        Console.WriteLine(string.Format(Localization.Messages.UnknownUpdateType, update.Type));
        return Task.CompletedTask;
    }
}
