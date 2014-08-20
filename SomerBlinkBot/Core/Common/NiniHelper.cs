using System;
using Core.Extensions;
using Core.Logging;
using Nini.Config;

namespace Core.Common
{
    public static class NiniHelper
    {
        /// <summary>
        /// Initialises the somer blink configuration file at path.
        /// </summary>
        /// <param name="path">The path.</param>
        public static void InitialiseSomerBlinkConfigFileAtPath(string path)
        {
            var source = new IniConfigSource();
            source.AddConfig("Credentials");
            source.Configs["Credentials"].Set("Username", "");
            source.Configs["Credentials"].Set("Password", "");

            source.AddConfig("Settings");
            source.Configs["Settings"].Set("MinWaitTime", "2000");
            source.Configs["Settings"].Set("MaxWaitTime", "60000");
            source.Configs["Settings"].Set("MinBlinkBidIsk", "2500000");
            source.Configs["Settings"].Set("MaxBlinkBidIsk", "10000000");
            source.Configs["Settings"].Set("DownTime", "4");
            source.Configs["Settings"].Set("RunTime", "4");

            source.AddConfig("Extra");
            source.Configs["Settings"].Set("DebugMode", "0");
            source.Configs["Settings"].Set("proxyIp", "x.x.x.x");
            source.Configs["Settings"].Set("proxyPort", "8080");
            source.Configs["Settings"].Set("proxyUser", "");
            source.Configs["Settings"].Set("proxyPass", "");
            source.Configs["Settings"].Set("useProxy", "false");

            source.Save(path);

            SaveToDesktop(path);
            Logger.LogMessage();
            Logger.LogMessage("--------------------");
            Logger.LogMessage("ConfigFile shortcut created on desktop");
        }

        /// <summary>
        /// Saves to desktop.
        /// </summary>
        /// <param name="path">The path.</param>
        private static void SaveToDesktop(string path)
        {
            ShortcutHelper.UrlShortcutToDesktop("ConfigMe.txt", path);
        }

        /// <summary>
        /// Validates the somer blink configuration file at path.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        public static bool ValidateSomerBlinkConfigFileAtPath(string path)
        {
            if (path.IsNullOrBlank())
            {
                return false;
            }

            try
            {
                var source = new IniConfigSource(path);

                var username = source.Configs["Credentials"].Get("Username");
                var password = source.Configs["Credentials"].Get("Password");

                if (username.IsNullOrBlank())
                {
                    throw new NullReferenceException("Username");
                }

                if (password.IsNullOrBlank())
                {
                    throw new NullReferenceException("Password");
                }

                source.Configs["Settings"].Get("MinWaitTime");
                source.Configs["Settings"].Get("MaxWaitTime");
                source.Configs["Settings"].Get("MinBlinkBidIsk");
                source.Configs["Settings"].Get("MaxBlinkBidIsk");
                source.Configs["Settings"].Get("DownTime");
                source.Configs["Settings"].Get("RunTime");

                source.Configs["Settings"].Get("DebugMode");
                source.Configs["Settings"].Get("proxyIp");
                source.Configs["Settings"].Get("proxyPort");
                source.Configs["Settings"].Get("proxyUser");
                source.Configs["Settings"].Get("proxyPass");
                source.Configs["Settings"].Get("useProxy");
            }
            catch (Exception e)
            {
                //Add logging here
                return false;
            }
            
            return true;
        }

        /// <summary>
        /// Reads the user settings from path.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">path</exception>
        /// <exception cref="System.NullReferenceException">
        /// Username
        /// or
        /// Password
        /// </exception>
        public static UserSettings ReadUserSettingsFromPath(string path)
        {
            if (path.IsNullOrBlank())
            {
                throw new ArgumentNullException("path");
            }

            var settings = new UserSettings();

            var source = new IniConfigSource(path);

            settings.Username = source.Configs["Credentials"].Get("Username");
            settings.Password = source.Configs["Credentials"].Get("Password");

            if (settings.Username.IsNullOrBlank())
            {
                throw new NullReferenceException("Username not specified");
            }

            if (settings.Password.IsNullOrBlank())
            {
                throw new NullReferenceException("Password not specified");
            }

            settings.MinWaitTime = int.Parse(source.Configs["Settings"].Get("MinWaitTime"));
            settings.MaxWaitTime = int.Parse(source.Configs["Settings"].Get("MaxWaitTime"));
            settings.MinBlinkBidIsk = long.Parse(source.Configs["Settings"].Get("MinBlinkBidIsk"));
            settings.MaxBlinkBidIsk = long.Parse(source.Configs["Settings"].Get("MaxBlinkBidIsk"));
            settings.DownTime = int.Parse(source.Configs["Settings"].Get("DownTime"));
            settings.RunTime = int.Parse(source.Configs["Settings"].Get("RunTime"));

            settings.DebugMode = int.Parse(source.Configs["Settings"].Get("DebugMode"));
            settings.proxyIp = source.Configs["Settings"].Get("proxyIp");
            settings.proxyPort = source.Configs["Settings"].Get("proxyPort");
            settings.proxyUser = source.Configs["Settings"].Get("proxyUser");
            settings.proxyPass = source.Configs["Settings"].Get("proxyPass");
            settings.useProxy = bool.Parse(source.Configs["Settings"].Get("useProxy"));
            settings.proxyType = int.Parse(source.Configs["Settings"].Get("proxyType", "0"));

            source.Save(path);

            return settings;
        }

        /// <summary>
        /// Augments the configuration file.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">path</exception>
        public static bool AugmentConfigFile(string path)
        {
            if (path.IsNullOrBlank())
            {
                throw new ArgumentNullException("path");
            }

            var changed = false;

            var source = new IniConfigSource(path);

            var configs = source.Configs["Settings"];

            var proxyType = configs.Get("proxyType");

            if (proxyType.IsNullOrBlank())
            {
                configs.Set("proxyType", 0);
                changed = true;
            }

            source.Save(path);

            return changed;
        }
    }
}
