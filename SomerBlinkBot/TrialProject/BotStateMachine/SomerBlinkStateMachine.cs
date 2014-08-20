#region

using System;
using System.Reflection;
using System.Threading;
using Core;
using Core.Common;
using Core.Common.TimeHandler;
using Core.Logging;
using Core.Time;
using SomerBlinkPromoBot.BotLogic;
using SomerBlinkPromoBot.Debugging;
using SomerBlinkPromoBot.PromoCheckingRestClient;
using SomerBlinkSpecific.Blink;

#endregion

namespace SomerBlinkPromoBot.BotStateMachine
{
    public static class SomerBlinkStateMachine
    {
        /// <summary>
        ///     To run
        /// </summary>
        private static bool _toRun = true;

        /// <summary>
        ///     The _start of run
        /// </summary>
        private static DateTime _startOfRun;

        /// <summary>
        /// Gets or sets down time provider.
        /// </summary>
        /// <value>
        /// Down time provider.
        /// </value>
        private static DownTimeProvider DownTimeProvider { get; set; }


        /// <summary>
        ///     Gets or sets the user settings.
        /// </summary>
        /// <value>
        ///     The user settings.
        /// </value>
        private static UserSettings UserSettings { get; set; }

        /// <summary>
        /// Gets or sets the last cumulative remote run.
        /// </summary>
        /// <value>
        /// The last cumulative remote run.
        /// </value>
        private static TimeSpan LastCumulativeRemoteRun { get; set; }


        /// <summary>
        ///     Gets or sets a value indicating whether [are promo running].
        /// </summary>
        /// <value>
        ///     <c>true</c> if [are promo running]; otherwise, <c>false</c>.
        /// </value>
        public static Blink CurrentPromo { get; set; }

        /// <summary>
        ///     Gets or sets the local bot.
        /// </summary>
        /// <value>
        ///     The local bot.
        /// </value>
        private static LocalBot LocalBot { get; set; }

        /// <summary>
        ///     Gets or sets the state.
        /// </summary>
        /// <value>
        ///     The state.
        /// </value>
        private static BotState State { get; set; }

        /// <summary>
        ///     Runs this instance.
        /// </summary>
        public static void Run()
        {
            InitialSetup();

            while (_toRun)
            {
                TimeSpan timeSpent;

                switch (State)
                {
                    case BotState.FullRun:
                        timeSpent = TimedActionRunner.RunActionWithRandomWait(() => ShieldedRunMethod(FullRun));
                        DownTimeProvider.RegisterFullRun(timeSpent);
                        break;

                    case BotState.RemoteRun:
                        timeSpent = TimedActionRunner.RunActionWithRandomWait(() => ShieldedRunMethod(RemoteRun));
                        DownTimeProvider.RegisterRemoteRun(timeSpent);
                        break;

                    case BotState.Cancelling:
                        //ShieldedRunMethod(CancelProgram);
                        //_toRun = false;
                        break;
                }

                var shouldDoDownTime = DownTimeProvider.ShouldDoDowntime(LocalBot.PromosRunning);

                if (shouldDoDownTime)
                {
                    DownTimeProvider.DoDowntime();
                }
            }
        }

        /// <summary>
        ///     Runs the promo check.
        /// </summary>
        public static void RunPromoCheck()
        {
            while (_toRun)
            {
                try
                {
                    CurrentPromo = PromoRun();

                    DoWait();
                }
                catch (Exception e)
                {
                    Logger.LogMessage("\n Shielding exception from method: {0} \n", "PromoRun");

                    if (SBDebugger.DebugMode)
                    {
                        Logger.LogMessage("Exception: \n{0}", e);
                    }

                    CurrentPromo = null;
                }
            }
        }

        /// <summary>
        ///     Does the wait.
        /// </summary>
        private static int DoWait()
        {
            var sleepTime = WaitTimeGenerator.RandomiseTime(UserSettings.MinWaitTime, UserSettings.MaxWaitTime);

            Logger.LogMessage("Bot waiting for {0} ms", sleepTime);

            Thread.Sleep(sleepTime);

            return sleepTime;
        }

        /// <summary>
        ///     Initials the setup.
        /// </summary>
        private static void InitialSetup()
        {
            Logger.LogMessage("Bot firing up initial setup");
            Logger.LogMessage();
            Logger.LogMessage();


            ////////////////////////////
            DownTimeProvider = new DownTimeProvider(DateTime.Now);

            ////////////////////////////

            CurrentPromo = null;
            State = BotState.FullRun;

            UserSettings = UserSettingsHelper.ReadUserSettings();

            LocalBot = new LocalBot();
            LocalBot.Login();
        }


        /// <summary>
        ///     Fulls the run.
        /// </summary>
        private static void FullRun()
        {
            Logger.LogMessage("Initiating a full run \n");

            LocalBot.FullRun(UserSettings.MinBlinkBidIsk, UserSettings.MaxBlinkBidIsk);

            var promoRunning = LocalBot.PromosRunning;

            if (promoRunning)
            {
                Logger.LogMessage("State: full run -> full run [promos!]");
                State = BotState.FullRun;
            }
            else if (LocalBot.LocalAccount.Isk <= UserSettings.MinBlinkBidIsk + 5000000)
            {
                Logger.LogMessage("State: FullRun -> RemoteRun");
                State = BotState.RemoteRun;
            }
        }

        /// <summary>
        ///     Promoes the run.
        /// </summary>
        /// <returns></returns>
        private static Blink PromoRun()
        {
            Logger.LogMessage("Initiation Promo check run");

            try
            {
                if (LocalBot == null || !LocalBot.ValidBrowser)
                {
                    LocalBot = new LocalBot();
                    LocalBot.Login();
                }

                var currentPromo = LocalBot.GetPromos();

                return currentPromo;
            }
            catch (Exception e)
            {
                Logger.LogMessage("Remote exception: \n {0}", e.Message);

                if (LocalBot != null)
                {
                    LocalBot.CleanUp();
                }
            }

            return null;
        }

        /// <summary>
        ///     Remotes the run.
        /// </summary>
        private static void RemoteRun()
        {
            Logger.LogMessage("Initiating remote run");

            try
            {
                var promoRunning = PromoCheckingClient.IsRunningPromos();

                if (promoRunning)
                {
                    Logger.LogMessage("Promos running: true");
                    Logger.LogMessage("State: remoteRun -> fullRun");
                    State = BotState.FullRun;
                }
                else
                {
                    Logger.LogMessage("Promos running: false");
                }
            }
            catch (Exception e)
            {
                Logger.LogMessage("Remote exception: \n {0}", e.Message);

                var cascadeExc = e;
                var i = 1;
                while (cascadeExc.InnerException != null)
                {
                    Logger.LogMessage("{0}.Error: {1}", i++, cascadeExc.InnerException.Message);
                    Logger.LogMessage();
                    cascadeExc = cascadeExc.InnerException;
                }
                Logger.LogMessage("Stack trace: {0}", e.StackTrace);
                State = BotState.RemoteRun;
                Logger.LogMessage("State: remoteRun failure -> remoteRun");
            }
        }

        /// <summary>
        ///     Shieldeds the run method.
        /// </summary>
        /// <param name="action">The action.</param>
        private static void ShieldedRunMethod(Action action)
        {
            try
            {
                action.Invoke();
            }
            catch (Exception e)
            {
                Logger.LogMessage("\n Doing a: {0}", action.GetMethodInfo().Name);

                if (SBDebugger.DebugMode)
                {
                    Logger.LogMessage("Exception: {0} \n {1}", e, e.StackTrace);
                }
            }
        }

        /// <summary>
        ///     Cancels the program.
        /// </summary>
        private static void CancelProgram()
        {
        }
    }
}