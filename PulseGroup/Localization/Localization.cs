namespace PulseGroup.Handlers;

/// <summary>
/// Localization service for bot messages
/// </summary>
public static class Localization
{
    public static class Commands
    {
        public const string Start = "/start";
        public const string Calculate = "/calculate";
        public const string Example = "/example";
        public const string Help = "/help";
        public const string About = "/about";
        public const string Admin = "/admin";
        public const string AdminSettings = "/admin_settings";
        public const string Info = "/info";
    }

    public static class Messages
    {
        // Welcome messages
        public const string WelcomeTitle = "👋 Добро пожаловать в PulseGroup Bot!";
        public const string WelcomeSubtitle = "🚗 Калькулятор стоимости автомобиля из Китая";
        public const string AvailableCommands = "Доступные команды:";
        
        // Command descriptions
        public const string CommandCalculate = "/calculate - 🧮 Рассчитать стоимость авто";
        public const string CommandExample = "/example - 📋 Показать пример расчета";
        public const string CommandHelp = "/help - ℹ️ Получить справку";
        public const string CommandAbout = "/about - 🤖 О боте";
        public const string CommandAdmin = "/admin - 🔐 Админ-панель";

        // Errors
        public const string UnknownCommand = "❌ Неизвестная команда: {0}\nИспользуйте /help для списка команд.";
        public const string AccessDenied = "❌ Доступ запрещен!\n\nИспользуйте /admin для входа.";
        public const string ErrorDataIncomplete = "❌ Ошибка: не все данные заполнены. Начните заново с /calculate";
        public const string InvalidPrice = "❌ Пожалуйста, введите корректную цену (только число).\nНапример: 93285";
        public const string UnknownMessage = "❓ Я не понимаю. Используйте /start чтобы увидеть доступные команды.";
        public const string UnknownUpdateType = "❓ Неизвестный тип обновления: {0}";

        // Admin panel
        public const string AdminLoginTitle = "🔐 *Вход в админ-панель*";
        public const string AdminEnterPassword = "Введите пароль:";
        public const string AdminPasswordWillBeDeleted = "(сообщение будет удалено автоматически)";
        public const string AdminAccessGranted = "✅ Доступ разрешен!";
        public const string AdminPanelActivated = "🔧 Админ-панель активирована.";
        public const string AdminUseSettings = "Используйте /admin_settings для управления настройками.";
        public const string AdminWrongPassword = "❌ Неверный пароль!";
        public const string AdminAccessDenied = "Доступ запрещен. Попробуйте снова с /admin";
        public const string AdminPanelTitle = "🔧 Админ-панель";
        public const string AdminSelectSetting = "Выберите настройку для изменения:";
        public const string AdminSettingUpdated = "✅ *Настройка обновлена!*";
        public const string AdminNewValueSaved = "Новое значение сохранено: ${0:N2}";
        public const string AdminInvalidValue = "❌ Неверное значение!";
        public const string AdminEnterNumber = "Пожалуйста, введите число (например: 1500 или 0.31 для процента)";
        public const string AdminAlreadyInPanel = "ℹ️ Вы уже в админ-панели!";
        public const string AdminUseButtonsOrCommand = "Используйте кнопки меню ниже или команду /admin_settings";
        public const string AdminAccessGrantedMessage = "✅ Доступ разрешен!\n\n🔧 Админ-панель активирована.\nИспользуйте /admin_settings для управления настройками.";
        public const string AdminMenuTitle = "🔧 Админ-панель\n\nВыберите настройку для изменения:";
        public const string AdminAccessDeniedCallback = "❌ Доступ запрещен! Используйте /admin для входа.";
        public const string AdminQuickSettingsComingSoon = "⚙️ Быстрые настройки - скоро!";
        public const string AdminEnterNewValue = "Введите новое значение для *{0}*:\n\n(Текущее значение: ${1:N2})";
        public const string AdminResetAllConfirm = "✅ Все настройки сброшены на значения по умолчанию!";
        public const string AdminLogoutMessage = "🚪 Вы вышли из админ-панели!";
        public const string AdminResetStatsConfirm = "✅ Статистика сброшена!";

        // Help
        public const string HelpTitle = "ℹ️ *Справка*";
        public const string HelpCarCalculatorTitle = "🚗 *Калькулятор автомобилей*";
        public const string HelpDescription = "Этот бот поможет вам рассчитать полную стоимость автомобиля из Китая с учетом всех расходов:";
        public const string HelpItemCarPrice = "• Стоимость авто";
        public const string HelpItemDocsChina = "• Доки в Китае";
        public const string HelpItemDelivery = "• Доставка (корабль/поезд)";
        public const string HelpItemPort = "• Порт";
        public const string HelpItemCustoms = "• Таможня";
        public const string HelpItemEvacuator = "• Эвакуатор";
        public const string HelpItemEuroRegistration = "• Учет Европа";
        public const string HelpItemServices = "• Услуги за привоз";
        public const string HelpUseCalculate = "Используйте /calculate для начала расчета!";

        // About
        public const string AboutTitle = "🤖 *О PulseGroup Bot*";
        public const string AboutVersion = "Версия: {0}";
        public const string AboutSubtitle = "🚗 Калькулятор автомобилей из Китая";
        public const string AboutDevelopedWith = "Разработано с использованием:";
        public const string AboutTelegramBot = "• Telegram.Bot library";
        public const string AboutDotNet = "• .NET 10";
        public const string AboutTurnkey = "🇨🇳 Помогаем рассчитать стоимость авто под ключ!";

        // Info
        public const string InfoTitle = "📊 *Информация о чате*";
        public const string InfoChatId = "Chat ID: `{0}`";
        public const string InfoChatType = "Тип чата: {0}";
        public const string InfoUsername = "Ваш Username: @{0}";
        public const string InfoUserId = "Ваш ID: `{0}`";
        public const string InfoName = "Имя: {0}";

        // Calculator
        public const string CalcTitle = "🧮 *Калькулятор стоимости автомобиля*";
        public const string CalcStep1 = "Шаг 1/3: Выберите страну, откуда везем автомобиль:";
        public const string CalcStep2 = "Шаг 2/3: Введите стоимость автомобиля в долларах США ($)";
        public const string CalcStep3 = "🚚 Выберите тип доставки:";
        public const string CalcPriceExample = "Например: 93285";
        public const string CalcCountrySelected = "✅ Выбрана страна: *{0}*";
        public const string CalcPriceSaved = "✅ Цена сохранена!";
        public const string CalcDeliverySelected = "✅ Выбрана доставка: *{0}*";
        public const string CalcCalculating = "⏳ Рассчитываю итоговую стоимость...";
        public const string CalcWarningTitle = "⚠️ *Обратите внимание:*";
        public const string CalcWarningUSAEurope = "Калькуляторы для США и Европы находятся в разработке.\nДоступен полный расчет только для Китая.";
        public const string CalcWarningCountryInDevelopment = "⚠️ *Внимание:* Калькулятор для этой страны еще в разработке.\nБудет показан примерный расчет.";

        // Countries - Names
        public const string CountryChina = "Китай";
        public const string CountryUSA = "США";
        public const string CountryEurope = "Европа";
        public const string CountryUnknown = "Неизвестно";

        // Countries - Flags
        public const string CountryChinaFlag = "🇨🇳";
        public const string CountryUSAFlag = "🇺🇸";
        public const string CountryEuropeFlag = "🇪🇺";

        // Countries - Notes
        public const string CountryChinaNotes = "🇨🇳 Включает полный пакет документов для растаможки из Китая";
        public const string CountryUSANotes = "⚠️ Калькулятор для США еще в разработке. Показан примерный расчет на основе Китая.";
        public const string CountryEuropeNotes = "⚠️ Калькулятор для Европы еще в разработке. Показан примерный расчет.";

        // Delivery
        public const string DeliveryShip = "🚢 Корабль (Море)";
        public const string DeliveryTrain = "🚂 Поезд (ЖД)";
        public const string DeliveryRoad = "🚛 Автовоз (дорога)";

        // Results
        public const string ResultTitle = "📊 *Расчет стоимости автомобиля*";
        public const string ResultCountry = "{0} Страна: *{1}*";
        public const string ResultCarPrice = "🚗 Стоимость авто: *${0:N0}*";
        public const string ResultDelivery = "🚚 Доставка: *{0}*";
        public const string ResultTotal = "💰 *ИТОГО: ${0:N0}*";
        public const string ResultDetailsTitle = "*Детализация расходов:*";
        public const string ResultItemCar = "• авто ${0:N0}";
        public const string ResultItemDocsChina = "• доки в Китае ${0:N0}";
        public const string ResultItemDelivery = "• доставка ${0:N0}";
        public const string ResultItemPort = "• порт ${0:N0}";
        public const string ResultItemCustoms = "• таможня ${0:N0}";
        public const string ResultItemEvacuator = "• эвакуатор ${0:N0}";
        public const string ResultItemEuroReg = "• учёт Европы ${0:N0}";
        public const string ResultItemServices = "• услуги за привоз ${0:N0}";
        public const string ResultTurnkey = "✅ Полностью готовая машина с документами и номерами.";
        public const string ResultNewCalculation = "📝 Для нового расчета используйте /calculate";

        // Example
        public const string ExampleTitle = "📋 *Пример расчета стоимости автомобиля*";
        public const string ExampleCarModel = "🚗 *ZEEKR 9X 2025 Ultra 70kWh (10 км)*";
        public const string ExampleCarDescription = "🇨🇳 Авто из Китая, под ключ на польском учёте";
        public const string ExampleTotal = "💰 *Итого: ${0:N0}*";
        public const string ExampleIncluded = "*Включено:*";
        public const string ExampleUseCalculate = "💡 Используйте /calculate для своего расчета!";

        // Pricing
        public const string PricingTitle = "💰 *Текущие тарифы*";
        public const string PricingDocsChina = "📝 Доки в Китае: ${0:N2}";
        public const string PricingPort = "⚓ Порт: ${0:N2}";
        public const string PricingEvacuator = "🚛 Эвакуатор: ${0:N2}";
        public const string PricingEuroReg = "📋 Учет Европа: ${0:N2}";
        public const string PricingServices = "🔧 Услуги за привоз: ${0:N2}";
        public const string PricingDeliveryShip = "🚢 Доставка корабль: ${0:N2}";
        public const string PricingDeliveryTrain = "🚂 Доставка поезд: ${0:N2}";
        public const string PricingCustoms = "📊 Таможня: {0:N2}%";

        // Console messages
        public const string ConsoleCommand = "🎮 Command: {0}";
        public const string ConsoleAdminAuth = "🔑 Admin auth attempt from @{0} (ChatID: {1})";
        public const string ConsolePasswordDeleted = "🗑️ Password message deleted for security";
        public const string ConsolePasswordDeleteFailed = "⚠️ Could not delete password message: {0}";
        public const string ConsoleAdminAuthSuccess = "✅ Admin authenticated successfully: @{0} (ChatID: {1})";
        public const string ConsoleAdminAuthFailed = "❌ Failed admin auth attempt: @{0} (ChatID: {1}) - Wrong password";
        public const string ConsoleSettingUpdated = "💰 Setting updated: {0} = {1}";
        public const string ConsoleStartingAdminAuth = "🔐 Starting admin auth for ChatID: {0}";
        public const string ConsoleMessageSendError = "❌ Failed to send message: {0}";
        public const string ConsoleStatsUpdate = "📊 Stats: Total calculations: {0}, Total amount: ${1:N0}";
        public const string ConsoleConfigLoaded = "✅ Configuration loaded from {0}";
        public const string ConsoleConfigNotFound = "⚠️ Configuration file not found. Creating default configuration...";
        public const string ConsoleConfigSaved = "💾 Configuration saved to {0}";
        public const string ConsoleConfigLoadError = "❌ Error loading configuration: {0}";
        public const string ConsoleConfigSaveError = "❌ Error saving configuration: {0}";
        public const string ConsoleUsingDefaultConfig = "Using default configuration...";
        public const string ConsoleStatsLoaded = "✅ Statistics loaded from {0}";
        public const string ConsoleStatsLoadedInfo = "📊 Total calculations: {0}, Total amount: ${1:N0}";
        public const string ConsoleStatsNotFound = "⚠️ Statistics file not found. Creating new statistics...";
        public const string ConsoleStatsSaved = "💾 Statistics saved to {0}";
        public const string ConsoleStatsLoadError = "❌ Error loading statistics: {0}";
        public const string ConsoleStatsSaveError = "❌ Error saving statistics: {0}";
        public const string ConsoleUsingDefaultStats = "Using default statistics...";

        // Statistics
        public const string StatsTitle = "📊 *Статистика бота*";
        public const string StatsTotalCalculations = "Всего расчетов: {0}";
        public const string StatsTotalAmount = "Общая сумма: ${0:N0}";
        public const string StatsAverageAmount = "Средняя сумма: ${0:N0}";
        public const string StatsMinAmount = "Мин. сумма: ${0:N0}";
        public const string StatsMaxAmount = "Макс. сумма: ${0:N0}";
        public const string StatsUptime = "⏱ Время работы: {0}д {1}ч {2}м";
        public const string StatsStarted = "🚀 Запущен: {0:yyyy-MM-dd HH:mm:ss}";
    }

    public static class Buttons
    {
        // Country buttons
        public const string ButtonChina = "🇨🇳 Китай";
        public const string ButtonUSA = "⚠️ 🇺🇸 США (в разработке)";
        public const string ButtonEurope = "⚠️ 🇪🇺 Европа (в разработке)";

        // Delivery buttons
        public const string ButtonDeliveryShip = "🚢 Корабль (Море)";
        public const string ButtonDeliveryTrain = "🚂 Поезд (ЖД)";

        // Admin buttons
        public const string ButtonShowPricing = "📋 Показать тарифы";
        public const string ButtonQuickSettings = "⚙️ Быстрые настройки";
        public const string ButtonEditDocsChina = "📝 Доки в Китае";
        public const string ButtonEditPort = "⚓ Порт";
        public const string ButtonEditEvacuator = "🚛 Эвакуатор";
        public const string ButtonEditEuroReg = "📋 Учет Европа";
        public const string ButtonEditServices = "🔧 Услуги";
        public const string ButtonEditCustoms = "📊 Таможня %";
        public const string ButtonEditDeliveryShip = "🚢 Доставка корабль";
        public const string ButtonEditDeliveryTrain = "🚂 Доставка поезд";
        public const string ButtonResetAll = "🔄 Сбросить все";
        public const string ButtonShowStats = "📈 Статистика";
        public const string ButtonLogout = "🚪 Выйти";

        // Statistics buttons
        public const string ButtonResetStats = "🔄 Сбросить статистику";
        public const string ButtonBackToMenu = "◀️ Назад";
    }

    public static class Errors
    {
        public const string TelegramApiError = "Telegram API Error: [{0}] {1}";
        public const string HttpError = "HTTP Error: {0}";
        public const string RequestTimeout = "Request timeout - проверьте подключение к интернету или Telegram заблокирован";
        public const string GenericError = "Error: {0}";
        public const string ConnectionHint = "⚠️ Совет: попробуйте VPN если Telegram заблокирован в вашей стране";
    }

    public static class AdminSettings
    {
        // Admin panel - Setting Names
        public const string AdminSettingDocsChina = "Доки в Китае";
        public const string AdminSettingPort = "Порт";
        public const string AdminSettingEvacuator = "Эвакуатор";
        public const string AdminSettingEuroReg = "Учёт Европа";
        public const string AdminSettingServices = "Услуги за привоз";
        public const string AdminSettingCustoms = "Таможня %";
        public const string AdminSettingDeliveryShip = "Доставка корабль";
        public const string AdminSettingDeliveryTrain = "Доставка поезд";
    }
}
