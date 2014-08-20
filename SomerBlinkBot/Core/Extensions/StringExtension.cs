using System;
using System.Globalization;

namespace Core.Extensions
{
    /// <summary>
    /// String Extension class
    /// </summary>
    public static class StringExtension
    {
        /// <summary>
        /// Converts to isk.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static long ConvertToIsk(this string value)
        {
            try
            {
                var iskAsText = value.Split(' ')[0];

                var isk = long.Parse(iskAsText, NumberStyles.AllowThousands, CultureInfo.InvariantCulture);

                return isk;
            }
            catch (Exception e)
            {
                throw new Exception("ConvertToIsk problem");
            }

        }

        /// <summary>
        /// Converts to token.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static long ConvertToToken(this string value)
        {
            try
            {
                var tokenAsText = value.Split(' ')[0];
                var tokens = long.Parse(tokenAsText, NumberStyles.AllowThousands, CultureInfo.InvariantCulture);

                return tokens;
            }
            catch (Exception e)
            {
                throw new Exception("ConvertToToken problem");
            }

        }

        /// <summary>
        /// Determines whether [is null or blank] [the specified s].
        /// </summary>
        /// <param name="s">The s.</param>
        /// <returns></returns>
        public static bool IsNullOrBlank(this string s)
        {
            return s == null || s.Trim().Length == 0;
        }
    }
}
