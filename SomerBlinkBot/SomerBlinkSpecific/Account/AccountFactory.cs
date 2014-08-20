using System.Configuration;
using Core.Common;

namespace SomerBlinkSpecific.Account
{
    public static class AccountFactory
    {
        /// <summary>
        /// Gets the local account.
        /// </summary>
        /// <returns></returns>
        public static Account GetLocalAccount()
        {
            var username = UserSettingsHelper.GlobalUserSettings.Username;
            var password = UserSettingsHelper.GlobalUserSettings.Password;

            return new Account {Username = username, Password = password};
        }
    }
}
