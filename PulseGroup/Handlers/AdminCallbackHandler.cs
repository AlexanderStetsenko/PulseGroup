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

            case "admin_edit_docs_china":
                await PromptForSettingAsync(botClient, chatId, "docs_china", Localization.AdminSettings.AdminSettingDocsChina, cancellationToken);
                break;

            case "admin_edit_port_fee":
                await PromptForSettingAsync(botClient, chatId, "port_fee", Localization.AdminSettings.AdminSettingPort, cancellationToken);
                break;

            case "admin_edit_evacuator":
                await PromptForSettingAsync(botClient, chatId, "evacuator", Localization.AdminSettings.AdminSettingEvacuator, cancellationToken);
                break;

            case "admin_edit_euro_registration":
                await PromptForSettingAsync(botClient, chatId, "euro_registration", Localization.AdminSettings.AdminSettingEuroReg, cancellationToken);
                break;

            case "admin_edit_services_fee":
                await PromptForSettingAsync(botClient, chatId, "services_fee", Localization.AdminSettings.AdminSettingServices, cancellationToken);
                break;

            case "admin_edit_customs_percent":
                await PromptForSettingAsync(botClient, chatId, "customs_percent", Localization.AdminSettings.AdminSettingCustoms, cancellationToken);
                break;

            case "admin_edit_delivery_ship":
                await PromptForSettingAsync(botClient, chatId, "delivery_ship", Localization.AdminSettings.AdminSettingDeliveryShip, cancellationToken);
                break;

            case "admin_edit_delivery_train":
                await PromptForSettingAsync(botClient, chatId, "delivery_train", Localization.AdminSettings.AdminSettingDeliveryTrain, cancellationToken);
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
        await MessageHelper.SendMessageSafeAsync(botClient, chatId, pricingText, cancellationToken, Telegram.Bot.Types.Enums.ParseMode.Markdown);
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
            cancellationToken);
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
            "docs_china" => _pricingConfig.Docs,
            "port_fee" => _pricingConfig.PortFee,
            "evacuator" => _pricingConfig.Evacuator,
            "euro_registration" => _pricingConfig.EuroRegistration,
            "services_fee" => _pricingConfig.ServicesFee,
            "delivery_ship" => _pricingConfig.DeliveryShip,
            "delivery_train" => _pricingConfig.DeliveryTrain,
            "customs_percent" => _pricingConfig.CustomsPercent * 100,
            _ => 0
        };
    }
}
