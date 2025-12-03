using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using PulseGroup.Models;
using PulseGroup.Services;
using PulseGroup.Configuration;

namespace PulseGroup.Handlers;

/// <summary>
/// Handler for admin panel operations
/// </summary>
public class AdminHandler
{
    private readonly Dictionary<long, AdminSession> _adminSessions;
    private readonly PricingConfig _pricingConfig;
    private readonly CalculationService _calculationService;
    private readonly StatisticsService _statisticsService;
    private readonly Dictionary<long, CarCalculation> _userSessions;
    private readonly ConfigurationService _configurationService;

    public AdminHandler(
        Dictionary<long, AdminSession> adminSessions,
        PricingConfig pricingConfig,
        CalculationService calculationService,
        StatisticsService statisticsService,
        Dictionary<long, CarCalculation> userSessions,
        ConfigurationService configurationService)
    {
        _adminSessions = adminSessions;
        _pricingConfig = pricingConfig;
        _calculationService = calculationService;
        _statisticsService = statisticsService;
        _userSessions = userSessions;
        _configurationService = configurationService;
    }

    /// <summary>
    /// Handles admin authentication
    /// </summary>
    public async Task HandleAuthenticationAsync(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken)
    {
        var chatId = message.Chat.Id;
        var password = message.Text!.Trim();

        Console.WriteLine($"?? Admin auth attempt from @{message.From?.Username} (ChatID: {chatId})");
        Console.WriteLine($"?? Password received: '{password}' (length: {password.Length})");
        Console.WriteLine($"?? Expected password: '{BotConfig.AdminPassword}' (length: {BotConfig.AdminPassword.Length})");
        Console.WriteLine($"?? Passwords match: {password == BotConfig.AdminPassword}");

        // Delete password message for security
        try
        {
            await botClient.DeleteMessage(chatId, message.MessageId, cancellationToken);
            Console.WriteLine(Localization.Messages.ConsolePasswordDeleted);
        }
        catch (Exception ex)
        {
            Console.WriteLine(string.Format(Localization.Messages.ConsolePasswordDeleteFailed, ex.Message));
        }

        if (password == BotConfig.AdminPassword)
        {
            _adminSessions[chatId].IsAuthenticated = true;
            Console.WriteLine(string.Format(Localization.Messages.ConsoleAdminAuthSuccess, message.From?.Username, chatId));

            await MessageHelper.SendMessageSafeAsync(botClient, chatId, Localization.Messages.AdminAccessGrantedMessage, cancellationToken);
            await ShowAdminMenuAsync(botClient, chatId, cancellationToken);
        }
        else
        {
            Console.WriteLine(string.Format(Localization.Messages.ConsoleAdminAuthFailed, message.From?.Username, chatId));

            var failMessage = $"{Localization.Messages.AdminWrongPassword}\n\n{Localization.Messages.AdminAccessDenied}";
            await MessageHelper.SendMessageSafeAsync(botClient, chatId, failMessage, cancellationToken);

            _adminSessions.Remove(chatId);
        }
    }

    /// <summary>
    /// Handles admin input for setting changes
    /// </summary>
    public async Task HandleAdminInputAsync(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken)
    {
        var chatId = message.Chat.Id;
        var session = _adminSessions[chatId];

        if (session.AwaitingInput && session.CurrentSetting != null)
        {
            if (decimal.TryParse(message.Text, out decimal newValue) && newValue >= 0)
            {
                UpdateSetting(session.CurrentSetting, newValue);
                
                // Save configuration to file
                _configurationService.SaveConfiguration(_pricingConfig);

                Console.WriteLine(string.Format(Localization.Messages.ConsoleSettingUpdated, session.CurrentSetting, newValue));

                var successMessage = $"{Localization.Messages.AdminSettingUpdated}\n\n" +
                                    $"{string.Format(Localization.Messages.AdminNewValueSaved, newValue)}";

                await MessageHelper.SendMessageSafeAsync(botClient, chatId, successMessage, cancellationToken, ParseMode.Markdown);

                session.AwaitingInput = false;
                session.CurrentSetting = null;

                await ShowAdminMenuAsync(botClient, chatId, cancellationToken);
            }
            else
            {
                var errorMessage = $"{Localization.Messages.AdminInvalidValue}\n\n{Localization.Messages.AdminEnterNumber}";
                await MessageHelper.SendMessageSafeAsync(botClient, chatId, errorMessage, cancellationToken);
            }
        }
        else
        {
            var infoMessage = $"{Localization.Messages.AdminAlreadyInPanel}\n\n{Localization.Messages.AdminUseButtonsOrCommand}";
            await MessageHelper.SendMessageSafeAsync(botClient, chatId, infoMessage, cancellationToken);
            await ShowAdminMenuAsync(botClient, chatId, cancellationToken);
        }
    }

    /// <summary>
    /// Shows main admin menu
    /// </summary>
    public async Task ShowAdminMenuAsync(ITelegramBotClient botClient, long chatId, CancellationToken cancellationToken)
    {
        var keyboard = new InlineKeyboardMarkup(new[]
        {
            new[]
            {
                InlineKeyboardButton.WithCallbackData(Localization.Buttons.ButtonShowPricing, "admin_show_pricing"),
            },
            new[]
            {
                InlineKeyboardButton.WithCallbackData("?? ?????????? ???????", "admin_edit_import_preparation"),
                InlineKeyboardButton.WithCallbackData("?? ????????", "admin_edit_land_sea_delivery")
            },
            new[]
            {
                InlineKeyboardButton.WithCallbackData("?? ??????", "admin_edit_broker"),
                InlineKeyboardButton.WithCallbackData("?? ????????? ?? ?????", "admin_edit_transport_from_port")
            },
            new[]
            {
                InlineKeyboardButton.WithCallbackData("?? ??????? %", "admin_edit_customs_percent"),
                InlineKeyboardButton.WithCallbackData("?? ??????", "admin_edit_import_services")
            },
            new[]
            {
                InlineKeyboardButton.WithCallbackData(Localization.Buttons.ButtonResetAll, "admin_reset_all"),
                InlineKeyboardButton.WithCallbackData(Localization.Buttons.ButtonShowStats, "admin_show_stats")
            },
            new[]
            {
                InlineKeyboardButton.WithCallbackData(Localization.Buttons.ButtonLogout, "admin_logout")
            },
            new[]
            {
                InlineKeyboardButton.WithCallbackData(Localization.Buttons.ButtonMainMenu, "main_menu")
            }
        });

        await MessageHelper.SendMessageSafeAsync(botClient, chatId, Localization.Messages.AdminMenuTitle, cancellationToken, replyMarkup: keyboard);
    }

    private void UpdateSetting(string settingName, decimal newValue)
    {
        switch (settingName)
        {
            case "import_preparation":
                _pricingConfig.ImportPreparation = newValue;
                break;
            case "land_sea_delivery":
                _pricingConfig.LandSeaDelivery = newValue;
                break;
            case "broker":
                _pricingConfig.Broker = newValue;
                break;
            case "transport_from_port":
                _pricingConfig.TransportFromPort = newValue;
                break;
            case "import_services":
                _pricingConfig.ImportServices = newValue;
                break;
            case "customs_percent":
                _pricingConfig.CustomsPercent = newValue / 100m;
                break;
        }

        // Save the updated configuration
        _configurationService.SaveConfiguration(_pricingConfig);
    }
}
