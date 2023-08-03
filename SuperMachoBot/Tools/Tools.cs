using DSharpPlus.Entities;
using DSharpPlus;
using System.Net;
using Newtonsoft.Json;

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

        public static bool CreateConfig (GuildConfig guildConfig, ulong guildId)
        {
            try {
            var config = new List<GuildConfig>();
            config.Add(guildConfig);
            string newJson = JsonConvert.SerializeObject(config, Formatting.Indented);
            File.WriteAllText(@$"{Program.databasePath}/{guildId}/Config.json", newJson);
                return true;
            } catch (Exception e)
            {
                Console.WriteLine($"Exception occured in config creation: {e.Message}");
                return false;
            }
        }

        /// <summary>
        /// Checks the amount of messages pinned from a specific user in a guild.
        /// </summary>
        /// <returns>
        /// First: Amount of standard pins. Second: Amount of ultra pins.
        /// </returns>
        public static (int, int) GetUserPinCount(ulong userid, ulong guildid)
        {   //There fucking HAS to be a better way to do this.
            var pinPath = $"{Program.databasePath}/{guildid}/Pinned.txt";
            var pinCsv = File.ReadAllLines(pinPath);
            var ultraPath = $"{Program.databasePath}/{guildid}/UltraPinned.txt";
            var ultraCsv = File.ReadAllLines(ultraPath);
            int pins = 0;
            int ultraPins = 0;
            foreach(var line in pinCsv)
            {
                var entry = line.Split(',');
                ulong.TryParse(entry[1], out var pinUserid);
                if (pinUserid != null && pinUserid == userid)
                {
                    pins++;
                }
            }
            foreach (var line in ultraCsv)
            {
                var entry = line.Split(',');
                ulong.TryParse(entry[1], out var pinUserid);
                if (pinUserid != null && pinUserid == userid)
                {
                    ultraPins++;
                }
            }
            return (pins, ultraPins);
        }
    class Economy
    {

    }

    class FunDatabase
    {

    }
}
}