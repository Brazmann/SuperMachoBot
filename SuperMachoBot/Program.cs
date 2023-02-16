using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.SlashCommands;
using SuperMachoBot.Commands;
using Newtonsoft.Json;
using DSharpPlus.Entities;
using Microsoft.Extensions.Logging;
using System.Drawing;

namespace SuperMachoBot
{
    class Program
    {
        public static string rootPath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
        public static bool moneyCooldown = true;
        public static List<Config> configItems = new List<Config>();
        public static DiscordClient discord;
        public static string pinnedPath = "";
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
            discord = new DiscordClient(new DiscordConfiguration()
            {
                Token = configItems[0].Token,
                TokenType = TokenType.Bot,
                Intents = DiscordIntents.Guilds | DiscordIntents.GuildMembers | DiscordIntents.GuildMessages | DiscordIntents.GuildMessageReactions | DiscordIntents.DirectMessages | DiscordIntents.MessageContents
            });

            var slash = discord.UseSlashCommands();

            discord.MessageCreated += async (s, e) =>
            {

            };

            pinnedPath = @$"{configItems[0].EconomyDatabasePath}Pinned.txt";
            discord.MessageReactionAdded += async (s, e) =>
            {
                if (e.Emoji.Id == 1075778692959183049) //Gem
                {
                    var bruh = await e.Channel.GetMessageAsync(e.Message.Id);
                    if(bruh.Reactions[0].Count > 4 && !CheckPinID(bruh.Id))
                    {
                        string thumbnailURL = "";
                        string desc = "";

                        if(bruh.Attachments.Count > 0)
                        {
                            thumbnailURL = bruh.Attachments[0].Url;
                        }

                        if(bruh.Content != "")
                        {
                            desc = $@"""{bruh.Content}""";
                        }

                        var bruhgimus = new DiscordEmbedBuilder
                        {
                            Title = $"GEM ALERT!",
                            Description = desc + "\n" + "",
                            ImageUrl = thumbnailURL,
                            Thumbnail = new DiscordEmbedBuilder.EmbedThumbnail { Url = "https://cdn.discordapp.com/attachments/977270567881298024/1075774698744455168/Gemson.png" },
                            Footer = new DiscordEmbedBuilder.EmbedFooter { IconUrl = bruh.Author.GetAvatarUrl(DSharpPlus.ImageFormat.Png, 256), Text = $"{bruh.Author.Username}#{bruh.Author.Discriminator}" },
                            Color = DiscordColor.PhthaloBlue
                        }.AddField("Gem:", $"[link]({bruh.JumpLink})").Build();
                        await discord.SendMessageAsync(discord.GetChannelAsync(1075588362230042694).Result, bruhgimus);
                        File.AppendAllText(pinnedPath, bruh.Id.ToString() + "\n");
                    }
                }
                if (e.Emoji.Id == 1075778708629110885) //Coal
                {
                    var bruh = await e.Channel.GetMessageAsync(e.Message.Id);
                    if (bruh.Reactions[0].Count > 4 && !CheckPinID(bruh.Id))
                    {
                        string thumbnailURL = "";
                        string desc = "";

                        if (bruh.Attachments.Count > 0)
                        {
                            thumbnailURL = bruh.Attachments[0].Url;
                        }

                        if (bruh.Content != "")
                        {
                            desc = $@"""{bruh.Content}""";
                        }

                        var bruhgimus = new DiscordEmbedBuilder
                        {
                            Title = $"COAL!!!! STINKY PISSCOAL ALERT!!!!",
                            Description = desc + "\n" + "",
                            ImageUrl = thumbnailURL,
                            Thumbnail = new DiscordEmbedBuilder.EmbedThumbnail { Url = "https://cdn.discordapp.com/attachments/977270567881298024/1075774690062249992/Coalson.png" },
                            Footer = new DiscordEmbedBuilder.EmbedFooter { IconUrl = bruh.Author.GetAvatarUrl(DSharpPlus.ImageFormat.Png, 256), Text = $"{bruh.Author.Username}#{bruh.Author.Discriminator}" },
                            Color = DiscordColor.Black
                        }.AddField("Coal:", $"[link]({bruh.JumpLink})").Build();
                        await discord.SendMessageAsync(discord.GetChannelAsync(1075588362230042694).Result, bruhgimus);
                        File.AppendAllText(pinnedPath, bruh.Id.ToString() + "\n");
                    }
                }
            };

            var commands = discord.UseCommandsNext(new CommandsNextConfiguration()
            {
                StringPrefixes = new[] { "tf" }
            });

            commands.RegisterCommands<GeneralCommands>();
            slash.RegisterCommands<SlashCommands>();
            slash.RegisterCommands<EconomyCommands>();
            EconomyCommands.jsonPath = configItems[0].EconomyDatabasePath;

            Console.WriteLine(EconomyCommands.jsonPath);
            Console.WriteLine(pinnedPath);

            await discord.ConnectAsync();
            await Task.Delay(-1);
        }
        static void cooldown()
        {
            moneyCooldown = true;
            Thread.Sleep(10000);
            moneyCooldown = false;
        }

        static bool CheckPinID(ulong messageid)
        {
            foreach (var line in File.ReadAllLines(pinnedPath))
            {
                if(line == messageid.ToString())
                {
                    return true;
                }
            }
            return false;
        }
    }
    public class Config
    {
        public string Token;
        public ulong OwnerID;
        public string EconomyDatabasePath;
    }
}