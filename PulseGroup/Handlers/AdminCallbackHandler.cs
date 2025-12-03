using Telegram.Bot;
using PulseGroup.Models;
using PulseGroup.Services;

namespace PulseGroup.Handlers;

/// <summary>
/// Handler for admin panel callback queries
/// </summary>
public class AdminCallbackHandler
{
    private readonly Dictionary<long, AdminSession> _adminSessions;
    private readonly PricingConfig _pricingConfig;
    private readonly CalculationService _calculationService;
    private readonly StatisticsService _statisticsService;
    private readonly Dictionary<long, CarCalculation> _userSessions;
    private readonly AdminHandler _adminHandler;
    private readonly ConfigurationService _configurationService;

    public AdminCallbackHandler(
        Dictionary<long, AdminSession> adminSessions,
        PricingConfig pricingConfig,
        CalculationService calculationService,
        StatisticsService statisticsService,
        Dictionary<long, CarCalculation> userSessions,
        AdminHandler adminHandler,
        ConfigurationService configurationService)
    {
        _adminSessions = adminSessions;
        _pricingConfig = pricingConfig;
        _calculationService = calculationService;
        _statisticsService = statisticsService;
        _userSessions = userSessions;
        _adminHandler = adminHandler;
        _configurationService = configurationService;
    }

    /// <summary>
    /// Handles admin callback queries
    /// </summary>
    public async Task HandleAdminCallbackAsync(ITelegramBotClient botClient, long chatId, string data, CancellationToken cancellationToken)
    {
        if (!_adminSessions.ContainsKey(chatId) || !_adminSessions[chatId].IsAuthenticated)
        {
            await MessageHelper.SendMessageSafeAsync(botClient, chatId,
                Localization.Messages.AdminAccessDeniedCallback,
                cancellationToken);
            return;
        }

        switch (data)
        {
            case "admin_show_pricing":
                await ShowPricingAsync(botClient, chatId, cancellationToken);
                break;

            case "admin_quick_settings":
                await ShowQuickSettingsAsync(botClient, chatId, cancellationToken);
                break;

            case "admin_edit_import_preparation":
                await PromptForSettingAsync(botClient, chatId, "import_preparation", Localization.AdminSettings.AdminSettingImportPreparation, cancellationToken);
                break;

            case "admin_edit_land_sea_delivery":
                await PromptForSettingAsync(botClient, chatId, "land_sea_delivery", Localization.AdminSettings.AdminSettingLandSeaDelivery, cancellationToken);
                break;

            case "admin_edit_broker":
                await PromptForSettingAsync(botClient, chatId, "broker", Localization.AdminSettings.AdminSettingBroker, cancellationToken);
                break;

            case "admin_edit_transport_from_port":
                await PromptForSettingAsync(botClient, chatId, "transport_from_port", Localization.AdminSettings.AdminSettingTransportFromPort, cancellationToken);
                break;

            case "admin_edit_import_services":
                await PromptForSettingAsync(botClient, chatId, "import_services", Localization.AdminSettings.AdminSettingImportServices, cancellationToken);
                break;

            case "admin_edit_customs_percent":
                await PromptForSettingAsync(botClient, chatId, "customs_percent", Localization.AdminSettings.AdminSettingCustoms, cancellationToken);
                break;

            case "admin_reset_all":
                await ResetAllSettingsAsync(botClient, chatId, cancellationToken);
                break;

            case "admin_show_stats":
                await ShowStatisticsAsync(botClient, chatId, cancellationToken);
                break;

            case "admin_logout":
                await LogoutAsync(botClient, chatId, cancellationToken);
                break;

            case "admin_reset_stats":
                await ResetStatisticsAsync(botClient, chatId, cancellationToken);
                break;

            case "admin_back_to_menu":
                await _adminHandler.ShowAdminMenuAsync(botClient, chatId, cancellationToken);
                break;
        }
    }

    private async Task ShowPricingAsync(ITelegramBotClient botClient, long chatId, CancellationToken cancellationToken)
    {
        var pricingText = _calculationService.GetPricingText();
        
        var keyboard = new Telegram.Bot.Types.ReplyMarkups.InlineKeyboardMarkup(new[]
        {
            new[]
            {
                Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData(Localization.Buttons.ButtonBackToMenu, "admin_back_to_menu"),
                Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData(Localization.Buttons.ButtonMainMenu, "main_menu")
            }
        });

        await MessageHelper.SendMessageSafeAsync(botClient, chatId, pricingText, cancellationToken, Telegram.Bot.Types.Enums.ParseMode.Markdown, keyboard);
    }

    private async Task ShowQuickSettingsAsync(ITelegramBotClient botClient, long chatId, CancellationToken cancellationToken)
    {
        await MessageHelper.SendMessageSafeAsync(botClient, chatId,
            Localization.Messages.AdminQuickSettingsComingSoon,
            cancellationToken);
    }

    private async Task PromptForSettingAsync(ITelegramBotClient botClient, long chatId, string settingName, string settingDisplay, CancellationToken cancellationToken)
    {
        _adminSessions[chatId].AwaitingInput = true;
        _adminSessions[chatId].CurrentSetting = settingName;

        var message = string.Format(Localization.Messages.AdminEnterNewValue, settingDisplay, GetCurrentValue(settingName));
        await MessageHelper.SendMessageSafeAsync(botClient, chatId, message, cancellationToken, Telegram.Bot.Types.Enums.ParseMode.Markdown);
    }

    private async Task ResetAllSettingsAsync(ITelegramBotClient botClient, long chatId, CancellationToken cancellationToken)
    {
        _pricingConfig.ResetToDefaults();
        
        // Save configuration to file
        _configurationService.SaveConfiguration(_pricingConfig);

        await MessageHelper.SendMessageSafeAsync(botClient, chatId,
            Localization.Messages.AdminResetAllConfirm,
            cancellationToken);

        await _adminHandler.ShowAdminMenuAsync(botClient, chatId, cancellationToken);
    }

    private async Task ShowStatisticsAsync(ITelegramBotClient botClient, long chatId, CancellationToken cancellationToken)
    {
        var stats = _statisticsService.GetStatisticsText();
        
        // Add reset button
        var keyboard = new Telegram.Bot.Types.ReplyMarkups.InlineKeyboardMarkup(new[]
        {
            new[]
            {
                Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData(Localization.Buttons.ButtonResetStats, "admin_reset_stats"),
                Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData(Localization.Buttons.ButtonBackToMenu, "admin_back_to_menu")
            }
        });
        
        await MessageHelper.SendMessageSafeAsync(botClient, chatId, stats, cancellationToken, Telegram.Bot.Types.Enums.ParseMode.Markdown, keyboard);
    }

    private async Task LogoutAsync(ITelegramBotClient botClient, long chatId, CancellationToken cancellationToken)
    {
        _adminSessions.Remove(chatId);

        await MessageHelper.SendMessageSafeAsync(botClient, chatId,
            Localization.Messages.AdminLogoutMessage,
            cancellationToken,
            includeMainMenuButton: true);
    }

    private async Task ResetStatisticsAsync(ITelegramBotClient botClient, long chatId, CancellationToken cancellationToken)
    {
        _statisticsService.Reset();

        await MessageHelper.SendMessageSafeAsync(botClient, chatId,
            Localization.Messages.AdminResetStatsConfirm,
            cancellationToken);

        await _adminHandler.ShowAdminMenuAsync(botClient, chatId, cancellationToken);
    }

    private decimal GetCurrentValue(string settingName)
    {
        return settingName switch
        {
            "import_preparation" => _pricingConfig.ImportPreparation,
            "land_sea_delivery" => _pricingConfig.LandSeaDelivery,
            "broker" => _pricingConfig.Broker,
            "transport_from_port" => _pricingConfig.TransportFromPort,
            "import_services" => _pricingConfig.ImportServices,
            "customs_percent" => _pricingConfig.CustomsPercent * 100,
            _ => 0
        };
    }
}
