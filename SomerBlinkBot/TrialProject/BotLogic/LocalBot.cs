#region

using System;
using System.Configuration;
using Core.Common;
using Core.Extensions;
using Core.Logging;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using SomerBlinkSpecific.Account;
using SomerBlinkSpecific.Blink;
using SomerBlinkSpecific.CogdevDotNet;

#endregion

namespace SomerBlinkPromoBot.BotLogic
{
    public class LocalBot
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="LocalBot" /> class.
        /// </summary>
        public LocalBot()
        {
            InitialiseDriverAndLogic();
            MustCancel = false;
        }

        /// <summary>
        ///     Gets a value indicating whether [promos running].
        /// </summary>
        /// <value>
        ///     <c>true</c> if [promos running]; otherwise, <c>false</c>.
        /// </value>
        public bool PromosRunning
        {
            get { return LocalCogdev.IsPromoActiveFromCacheList(); }
        }

        /// <summary>
        ///     Gets or sets the local driver.
        /// </summary>
        /// <value>
        ///     The local driver.
        /// </value>
        private IWebDriver LocalDriver { get; set; }

        /// <summary>
        ///     Gets or sets the local account.
        /// </summary>
        /// <value>
        ///     The local account.
        /// </value>
        public Account LocalAccount { get; set; }

        /// <summary>
        ///     Gets or sets the local cogdev.
        /// </summary>
        /// <value>
        ///     The local cogdev.
        /// </value>
        private CogdevDotNet LocalCogdev { get; set; }

        /// <summary>
        ///     Gets or sets the firefox profile.
        /// </summary>
        /// <value>
        ///     The firefox profile.
        /// </value>
        private FirefoxProfile FirefoxProfile { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether [must cancel].
        /// </summary>
        /// <value>
        ///     <c>true</c> if [must cancel]; otherwise, <c>false</c>.
        /// </value>
        private bool MustCancel { get; set; }

        /// <summary>
        ///     Gets a value indicating whether [valid browser].
        /// </summary>
        /// <value>
        ///     <c>true</c> if [valid browser]; otherwise, <c>false</c>.
        /// </value>
        public bool ValidBrowser
        {
            get
            {
                try
                {
                    return LocalDriver.WindowHandles.Count > 0;
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// Initialises the driver and logic.
        /// </summary>
        private void InitialiseDriverAndLogic()
        {
            FirefoxProfile = GetProxyProfile();
            LocalDriver = new FirefoxDriver(FirefoxProfile);

            LocalDriver.Navigate().GoToUrl("http://google.com");

            LocalAccount = AccountFactory.GetLocalAccount();
            LocalCogdev = new CogdevDotNet(LocalDriver, LocalAccount, "LocalBot");
        }

        /// <summary>
        ///     Gets the proxy profile.
        /// </summary>
        /// <returns></returns>
        private FirefoxProfile GetProxyProfile()
        {
            var settings = UserSettingsHelper.GlobalUserSettings;

            var useProxy = settings.useProxy;

            if (!useProxy)
            {
                return new FirefoxProfile();
            }

            var proxy = new Proxy();

            var proxyType = settings.proxyType;
            var port = settings.proxyPort;
            var ip = settings.proxyIp;

            if (ip.IsNullOrBlank() || port.IsNullOrBlank())
            {
                return new FirefoxProfile();
            }

            ip += ":" + port;

            var user = settings.proxyUser;
            var pass = settings.proxyPass;

            proxy.Kind = ProxyKind.Manual;

            //HTTP PROXY
            if (proxyType == 0)
            {
                proxy.HttpProxy = ip;
            }
            else
            {
                proxy.SocksProxy = ip;
            }

            proxy.SslProxy = ip;
            proxy.FtpProxy = ip;
            proxy.SocksUserName = user;
            proxy.SocksPassword = pass;
            var fp = new FirefoxProfile();
            fp.SetProxyPreferences(proxy);
            fp.SetPreference("network.websocket.enabled", "false");

            return fp;
        }


        /// <summary>
        ///     Fulls the run.
        /// </summary>
        /// <param name="minIsk">The minimum isk.</param>
        /// <param name="maxIsk">The maximum isk.</param>
        public void FullRun(long minIsk, long maxIsk)
        {
            if (LocalDriver != null)
            {
                try
                {
                    LocalCogdev.ReadMainMetaDataIntoAccount();
                    ReportAccountStatistics();
                    LocalCogdev.ReadBlinks();
                    LocalCogdev.BidOnBlinks(minIsk, maxIsk);
                }
                catch (WebDriverException wde)
                {
                    Logger.LogMessage(wde.Message);
                    //RestartLocalDriver();
                }
            }
            else
            {
                throw new NullReferenceException("LocalDriver");
            }
        }

        /// <summary>
        ///     Restarts the local driver.
        /// </summary>
        private void RestartLocalDriver()
        {
            LocalDriver.Close();

            InitialiseDriverAndLogic();
        }

        /// <summary>
        /// Reports the account statistics.
        /// </summary>
        private void ReportAccountStatistics()
        {
            Logger.LogMessage("\n Isk {0}  --- Tokens {1}", LocalAccount.Isk, LocalAccount.Tokens);

            if (LocalAccount.Tokens <= 0)
            {
                Logger.LogMessage("\n\n Out of tokens, deposit isk to somer blink.");
                Logger.LogMessage("\n Press enter when tokens are greater than 0 on account.");
                Console.ReadLine();
            }
        }

        /// <summary>
        ///     Logins this instance.
        /// </summary>
        public void Login()
        {
            LocalCogdev.Login();
        }


        /// <summary>
        ///     Determines whether [is promo active].
        /// </summary>
        /// <returns></returns>
        public Blink GetPromos()
        {
            LocalCogdev.ReadMainMetaDataIntoAccount();

            LocalCogdev.ReadBlinks();

            var promo = LocalCogdev.GetPromos();

            //var runningPromos = LocalCogdev.IsRunningPromosActiveQuery();

            return promo;
        }

        /// <summary>
        ///     Cleans up.
        /// </summary>
        public void CleanUp()
        {
            try
            {
                LocalDriver.Quit();
                LocalDriver.Close();
                LocalDriver.Dispose();
            }
            catch (Exception e)
            {
                Logger.LogMessage("Local browser unable to close");
            }
        }
    }
}