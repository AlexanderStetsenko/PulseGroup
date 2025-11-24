using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using PulseGroup.Models;
using PulseGroup.Configuration;

namespace PulseGroup.Handlers;

/// <summary>
/// Handler for bot commands
/// </summary>
public class CommandHandler
{
    private readonly Dictionary<long, CarCalculation> _userSessions;
    private readonly Dictionary<long, AdminSession> _adminSessions;
    private readonly CalculationHandler _calculationHandler;
    private readonly AdminHandler _adminHandler;

    public CommandHandler(
        Dictionary<long, CarCalculation> userSessions,
        Dictionary<long, AdminSession> adminSessions,
        CalculationHandler calculationHandler,
        AdminHandler adminHandler)
    {
        _userSessions = userSessions;
        _adminSessions = adminSessions;
        _calculationHandler = calculationHandler;
        _adminHandler = adminHandler;
    }

    /// <summary>
    /// Handles bot commands
    /// </summary>
    public async Task HandleCommandAsync(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken)
    {
        var command = message.Text!.Split(' ')[0].ToLower();
        var chatId = message.Chat.Id;

        Console.WriteLine(string.Format(Localization.Messages.ConsoleCommand, command));

        switch (command)
        {
            case "/start":
                await HandleStartCommand(botClient, chatId, cancellationToken);
                break;

            case "/calculate":
                _adminSessions.Remove(chatId);
                await _calculationHandler.StartCalculationAsync(botClient, chatId, cancellationToken);
                break;

            case "/example":
                await _calculationHandler.ShowExampleAsync(botClient, chatId, cancellationToken);
                break;

            case "/admin":
                await HandleAdminCommand(botClient, chatId, cancellationToken);
                break;

            case "/admin_settings":
                await HandleAdminSettingsCommand(botClient, chatId, cancellationToken);
                break;

            case "/help":
                await HandleHelpCommand(botClient, chatId, cancellationToken);
                break;

            case "/about":
                await HandleAboutCommand(botClient, chatId, cancellationToken);
                break;

            case "/info":
                await HandleInfoCommand(botClient, message, cancellationToken);
                break;

            default:
                await MessageHelper.SendMessageSafeAsync(botClient, chatId,
                    string.Format(Localization.Messages.UnknownCommand, command),
                    cancellationToken);
                break;
        }
    }

    private async Task HandleStartCommand(ITelegramBotClient botClient, long chatId, CancellationToken cancellationToken)
    {
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

    private async Task HandleAdminCommand(ITelegramBotClient botClient, long chatId, CancellationToken cancellationToken)
    {
        _userSessions.Remove(chatId);

        Console.WriteLine(string.Format(Localization.Messages.ConsoleStartingAdminAuth, chatId));
        _adminSessions[chatId] = new AdminSession { IsAuthenticated = false };

        var message = $"{Localization.Messages.AdminLoginTitle}\n\n" +
                     $"{Localization.Messages.AdminEnterPassword}\n" +
                     $"{Localization.Messages.AdminPasswordWillBeDeleted}";

        await MessageHelper.SendMessageSafeAsync(botClient, chatId, message, cancellationToken, ParseMode.Markdown);
    }

    private async Task HandleAdminSettingsCommand(ITelegramBotClient botClient, long chatId, CancellationToken cancellationToken)
    {
        if (_adminSessions.ContainsKey(chatId) && _adminSessions[chatId].IsAuthenticated)
        {
            await _adminHandler.ShowAdminMenuAsync(botClient, chatId, cancellationToken);
        }
        else
        {
            await MessageHelper.SendMessageSafeAsync(botClient, chatId,
                Localization.Messages.AccessDenied,
                cancellationToken);
        }
    }

    private async Task HandleHelpCommand(ITelegramBotClient botClient, long chatId, CancellationToken cancellationToken)
    {
        var message = $"{Localization.Messages.HelpTitle}\n\n" +
                     $"{Localization.Messages.HelpCarCalculatorTitle}\n" +
                     $"{Localization.Messages.HelpDescription}\n\n" +
                     $"{Localization.Messages.HelpItemCarPrice}\n" +
                     $"{Localization.Messages.HelpItemDocsChina}\n" +
                     $"{Localization.Messages.HelpItemDelivery}\n" +
                     $"{Localization.Messages.HelpItemPort}\n" +
                     $"{Localization.Messages.HelpItemCustoms}\n" +
                     $"{Localization.Messages.HelpItemEvacuator}\n" +
                     $"{Localization.Messages.HelpItemEuroRegistration}\n" +
                     $"{Localization.Messages.HelpItemServices}\n\n" +
                     $"{Localization.Messages.HelpUseCalculate}";

        await MessageHelper.SendMessageSafeAsync(botClient, chatId, message, cancellationToken, ParseMode.Markdown);
    }

    private async Task HandleAboutCommand(ITelegramBotClient botClient, long chatId, CancellationToken cancellationToken)
    {
        var message = $"{Localization.Messages.AboutTitle}\n\n" +
                     $"{string.Format(Localization.Messages.AboutVersion, BotConfig.BotVersion)}\n" +
                     $"{Localization.Messages.AboutSubtitle}\n\n" +
                     $"{Localization.Messages.AboutDevelopedWith}\n" +
                     $"{Localization.Messages.AboutTelegramBot}\n" +
                     $"{Localization.Messages.AboutDotNet}\n\n" +
                     $"{Localization.Messages.AboutTurnkey}";

        await MessageHelper.SendMessageSafeAsync(botClient, chatId, message, cancellationToken, ParseMode.Markdown);
    }

    private async Task HandleInfoCommand(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken)
    {
        var chatId = message.Chat.Id;
        var info = $"{Localization.Messages.InfoTitle}\n\n" +
                  $"{string.Format(Localization.Messages.InfoChatId, chatId)}\n" +
                  $"{string.Format(Localization.Messages.InfoChatType, message.Chat.Type)}\n" +
                  $"{string.Format(Localization.Messages.InfoUsername, message.From?.Username ?? "N/A")}\n" +
                  $"{string.Format(Localization.Messages.InfoUserId, message.From?.Id)}\n" +
                  $"{string.Format(Localization.Messages.InfoName, message.From?.FirstName)}";

        await MessageHelper.SendMessageSafeAsync(botClient, chatId, info, cancellationToken, ParseMode.Markdown);
    }
}