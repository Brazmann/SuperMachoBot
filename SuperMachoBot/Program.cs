using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.SlashCommands;
using SuperMachoBot.Commands;
using Newtonsoft.Json;

namespace SuperMachoBot
{
    class Program
    {
        public static string rootPath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
        public static bool moneyCooldown = true;
        public static List<Config> configItems = new List<Config>();
        static void Main(string[] args)
        {
            MainAsync().GetAwaiter().GetResult();
        }

        internal static async Task MainAsync()
        {
            using (StreamReader r = new StreamReader(@$"{rootPath}\config\config.json"))
            {
                string json = r.ReadToEnd();
                configItems = JsonConvert.DeserializeObject<List<Config>>(json);
            }
            var discord = new DiscordClient(new DiscordConfiguration()
            {
                Token = configItems[0].Token,
                TokenType = TokenType.Bot
            });

            var slash = discord.UseSlashCommands();

            discord.MessageCreated += async (s, e) =>
            {
                if (e.Message.Content.Contains("money") && e.Message.Content.Contains("tenor") == false && moneyCooldown == false)
                {
                    await e.Message.RespondAsync("https://tenor.com/view/money-breaking-bad-sleep-on-money-lay-on-money-money-pile-gif-5382667");
                    cooldown();
                }
            };

            var commands = discord.UseCommandsNext(new CommandsNextConfiguration()
            {
                StringPrefixes = new[] { "tf" }
            });

            commands.RegisterCommands<GeneralCommands>();
            slash.RegisterCommands<SlashCommands>();

            await discord.ConnectAsync();
            await Task.Delay(-1);
        }
        static void cooldown()
        {
            moneyCooldown = true;
            Thread.Sleep(10000);
            moneyCooldown = false;
        }
    }
    public class Config
    {
        public string Token;
        public ulong OwnerID;
    }
}