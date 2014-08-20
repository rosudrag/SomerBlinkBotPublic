using System;
using System.Net;
using System.Web.Script.Serialization;


namespace SomerBlinkPromoBot.PromoCheckingRestClient
{
    public static class PromoCheckingClient
    {
        private const string PromoCheckerLink = @"http://promos.rosudrag.co.uk:1337/promorunning?format=json";

        private static readonly JavaScriptSerializer Serializer = new JavaScriptSerializer();

        /// <summary>
        /// Determines whether [is running promos].
        /// </summary>
        /// <returns></returns>
        public static bool IsRunningPromos()
        {
            using (var client = new WebClient())
            {
                string result = client.DownloadString(PromoCheckerLink);

                var promoResult = Serializer.Deserialize<PromoInfo>(result);

                return promoResult.Result;
            }
        }
    }
}
