using System.Configuration;
using System.Runtime.InteropServices;
using Core.Common;

namespace SomerBlinkPromoBot.Debugging
{
    public static class SBDebugger
    {
        /// <summary>
        /// Gets a value indicating whether [debug mode].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [debug mode]; otherwise, <c>false</c>.
        /// </value>
        public static bool DebugMode
        {
            get
            {
                var toDebug = UserSettingsHelper.GlobalUserSettings.DebugMode;

                return toDebug == 1;
            }
        }
    }
}
