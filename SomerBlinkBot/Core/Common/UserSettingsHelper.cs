using System;
using System.IO;
using Core.Logging;

namespace Core.Common
{
    /// <summary>
    /// Helper to return a GlobalUserSettings instance
    /// </summary>
    public static class UserSettingsHelper
    {
        /// <summary>
        /// The _user settings
        /// </summary>
        private static UserSettings _userSettings;

        /// <summary>
        /// Gets or sets the user settings.
        /// </summary>
        /// <value>
        /// The user settings.
        /// </value>
        public static UserSettings GlobalUserSettings
        {
            get
            {
                if (_userSettings == null)
                {
                    _userSettings = ReadUserSettings();
                }

                return _userSettings;
            }

        }

        /// <summary>
        /// The ini file name
        /// </summary>
        private const string IniFileName = "SBConfig.ini";

        /// <summary>
        /// Reads the user settings.
        /// </summary>
        public static UserSettings ReadUserSettings()
        {
            var path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

            path = Path.Combine(path, IniFileName);

            TryInitialiseIniFile(path);

            TryAugmentIniFile(path);

            var settings = NiniHelper.ReadUserSettingsFromPath(path);

            return settings;
        }

        private static void TryAugmentIniFile(string path)
        {
            if (!File.Exists(path))
            {
                return;
            }

            var didAugmentation = NiniHelper.AugmentConfigFile(path);

            if (didAugmentation)
            {
                Logger.LogMessage("Config file augmented; check for new settings available.");
            }
        }

        /// <summary>
        /// Tries the initialise ini file.
        /// </summary>
        /// <param name="path"></param>
        private static void TryInitialiseIniFile(string path)
        {
            if (!File.Exists(path))
            {
                Logger.LogMessage("No configuration file found");
                Logger.LogMessage("==== GENERATING CONFIG FILE ====");

                NiniHelper.InitialiseSomerBlinkConfigFileAtPath(path);

                Logger.LogMessage();
                Logger.LogMessage("--- CONFIG GENERATED AT PATH : ");
                Logger.LogMessage(path);
                Logger.LogMessage();
                Logger.LogMessage();
                Logger.LogMessage("Restart the program after editing the file. (Use notepad)");
                Console.ReadLine();
                Environment.Exit(0);
            }
            else
            {
                var validationResult = NiniHelper.ValidateSomerBlinkConfigFileAtPath(path);
                if (!validationResult)
                {
                    Logger.LogMessage("Invalid config file detected -> regenerating config file");
                    Logger.LogMessage();
                    Logger.LogMessage();
                    Logger.LogMessage();
                    NiniHelper.InitialiseSomerBlinkConfigFileAtPath(path);
                    Logger.LogMessage("--- CONFIG GENERATED AT PATH : ");
                    Logger.LogMessage(path);
                    Logger.LogMessage();
                    Logger.LogMessage();
                    Logger.LogMessage("Restart the program after editing the file. (Use notepad)");
                    Console.ReadLine();
                    Environment.Exit(0);
                }
            }
        }
    }
}
