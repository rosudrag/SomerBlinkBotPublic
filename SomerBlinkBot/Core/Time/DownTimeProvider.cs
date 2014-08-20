#region

using System;
using System.Threading;
using Core.Common;
using Core.Logging;

#endregion

namespace Core.Time
{
    public class DownTimeProvider : IDownTimeProvider
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="DownTimeProvider" /> class.
        /// </summary>
        /// <param name="timeStartedAt">The time started at.</param>
        /// <exception cref="System.ArgumentNullException">timeStartedAt</exception>
        public DownTimeProvider(DateTime timeStartedAt)
        {
            if (timeStartedAt == null)
            {
                throw new ArgumentNullException("timeStartedAt");
            }

            BotOnlineDateTime = timeStartedAt;

            TimeSpentOnFullRun = new TimeSpan();
            LastStateRan = BotState.None;
        }

        /// <summary>
        /// Gets a value indicating whether [_must do downtime].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [_must do downtime]; otherwise, <c>false</c>.
        /// </value>
        private bool PlainDowntimeCheck
        {
            get { return TimeOnline > TimeSpan.FromHours(UserSettings.RunTime); }
        }

        /// <summary>
        ///     Gets or sets the bot started at.
        /// </summary>
        /// <value>
        ///     The bot started at.
        /// </value>
        private DateTime BotOnlineDateTime { get; set; }

        /// <summary>
        ///     Gets the time online.
        /// </summary>
        /// <value>
        ///     The time online.
        /// </value>
        private TimeSpan TimeOnline
        {
            get
            {
                var result = DateTime.Now - BotOnlineDateTime;
                return result;
            }
        }

        /// <summary>
        ///     Gets or sets the last state ran.
        /// </summary>
        /// <value>
        ///     The last state ran.
        /// </value>
        private BotState LastStateRan { get; set; }

        /// <summary>
        ///     Gets or sets the time spent on full run.
        /// </summary>
        /// <value>
        ///     The time spent on full run.
        /// </value>
        private TimeSpan TimeSpentOnFullRun { get; set; }

        /// <summary>
        ///     Gets the user settings.
        /// </summary>
        /// <value>
        ///     The user settings.
        /// </value>
        private UserSettings UserSettings
        {
            get { return UserSettingsHelper.GlobalUserSettings; }
        }

        /// <summary>
        ///     Gets or sets the time spent on remote run.
        /// </summary>
        /// <value>
        ///     The time spent on remote run.
        /// </value>
        private TimeSpan TimeSpentOnRemoteRun { get; set; }

        /// <summary>
        ///     Registers the full run.
        /// </summary>
        /// <param name="timeSpent">The time spent.</param>
        public void RegisterFullRun(TimeSpan timeSpent)
        {
            if (LastStateRan == BotState.None)
            {
                LastStateRan = BotState.FullRun;
            }

            TimeSpentOnFullRun += timeSpent;

            TimeSpentOnRemoteRun = TimeSpan.Zero;

            LastStateRan = BotState.FullRun;
        }

        /// <summary>
        ///     Registers the remote run.
        /// </summary>
        /// <param name="timeSpent">The time spent.</param>
        public void RegisterRemoteRun(TimeSpan timeSpent)
        {
            if (LastStateRan == BotState.None)
            {
                LastStateRan = BotState.RemoteRun;
            }

            //Restart timer 
            if (LastStateRan != BotState.RemoteRun)
            {
                TimeSpentOnRemoteRun = TimeSpan.Zero;
            }

            TimeSpentOnRemoteRun += timeSpent;

            LastStateRan = BotState.RemoteRun;
        }

        public bool ShouldDoDowntime(bool bypass)
        {
            if (bypass)
            {
                return false;
            }

            return PlainDowntimeCheck;
        }

        /// <summary>
        ///     Does the downtime.
        /// </summary>
        /// <returns></returns>
        public TimeSpan DoDowntime()
        {
            var timeToSleep = TimeSpan.FromHours(UserSettings.DownTime);

            timeToSleep -= TimeSpentOnRemoteRun;

            if (timeToSleep.TotalSeconds < 0)
            {
                BotOnlineDateTime = DateTime.Now;
                return TimeSpan.Zero;
            }

            Logger.LogMessage("Going to sleep for {0} mins", timeToSleep);

            Thread.Sleep(timeToSleep);

            BotOnlineDateTime = DateTime.Now;

            Logger.LogMessage();
            Logger.LogMessage("================= =================");
            Logger.LogMessage("Finishing downtime of: {0} mins", timeToSleep.TotalMinutes);

            return timeToSleep;
        }
    }
}