using System;
using System.Linq;

namespace HelpMyStreet.Utils.Utils
{
    public static class PostcodeFormatter
    {
        /// <summary>
        /// Returns a new postcode string in a consistent format
        /// </summary>
        /// <param name="postcode">Postcode to format</param>
        /// <returns>New formatted postcode string</returns>
        /// <exception cref="System.ArgumentNullException">Thrown when postcode is null</exception>
        public static string FormatPostcode(string postcode)
        {
            if (postcode == null)
            {
                throw new ArgumentNullException(nameof(postcode));
            }

            // guarantee a new string is always returned (except if the string is empty as Microsoft's string operations return the same static instance)
            string cleanedPostcode = postcode.ToUpper();

            // this method is not responsible for validation, but equally we don't want it to error
            if (!IsEmptyOrWhiteSpace(cleanedPostcode))
            {
                cleanedPostcode = cleanedPostcode.Replace(" ", "");

                // don't try to format a postcode that is obviously invalid
                if (cleanedPostcode.Length >= 5 && cleanedPostcode.Length <= 7)
                {
                    cleanedPostcode = cleanedPostcode.Insert(cleanedPostcode.Length - 3, " ");
                }
            }

            return cleanedPostcode;
        }

        private static bool IsEmptyOrWhiteSpace(string value) => value.All(char.IsWhiteSpace);
    }
}
