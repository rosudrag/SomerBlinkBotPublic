#region

using System;

#endregion

namespace Core.Common.TimeHandler
{
    public static class WaitTimeGenerator
    {
        private static readonly Random Randomiser = new Random();

        /// <summary>
        /// Gets the user settings.
        /// </summary>
        /// <value>
        /// The user settings.
        /// </value>
        private static UserSettings UserSettings
        {
            get { return UserSettingsHelper.GlobalUserSettings; }
        }

        /// <summary>
        /// Randomises the time.
        /// </summary>
        /// <param name="minWaitMs">The minimum wait ms.</param>
        /// <param name="maxWaitMs">The maximum wait ms.</param>
        /// <returns></returns>
        public static int RandomiseTime(int? minWaitMs = null, int? maxWaitMs = null)
        {
            var min = minWaitMs ?? UserSettings.MinWaitTime;
            var max = maxWaitMs ?? UserSettings.MaxWaitTime;

            return Randomiser.Next(min, max);
        }
    }
}