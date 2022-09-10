using DSharpPlus.Entities;
using DSharpPlus;
using System.Net;

namespace SuperMachoBot.Tools
{
    class General
    {
        /// <returns>
        /// The status code of the specified url.
        /// </returns>
        public static int GetPage(String url)
        {
            try {
            HttpWebRequest myHttpWebRequest = (HttpWebRequest)WebRequest.Create(url);
            HttpWebResponse myHttpWebResponse = (HttpWebResponse)myHttpWebRequest.GetResponse();

            return (int)myHttpWebResponse.StatusCode;
            }
            catch (WebException e)
            {
                return 0;
            }
        }



        /// <summary>
        /// Checks the HTTP status code of the specified user's avatar url to determine if it's a gif or image.
        /// </summary>
        /// <returns>
        /// The appropriate avatar url for the user.
        /// </returns>
        public static string AvatarParser(DiscordUser du)
        {
            var avatarUrl = du.GetAvatarUrl(ImageFormat.Gif);
            if (GetPage(avatarUrl) == 200)
            {
                return du.GetAvatarUrl(ImageFormat.Gif);
            }
            else
            {
                return du.GetAvatarUrl(ImageFormat.Png);
            }
        }
    }
    class Economy
    {

    }
}