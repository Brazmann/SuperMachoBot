using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.SlashCommands;
using SuperMachoBot.Commands;
using Newtonsoft.Json;
using DSharpPlus.Entities;
using Microsoft.Extensions.Logging;
using System.Drawing;
using Emzi0767.Utilities;

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
                    var message = await e.Channel.GetMessageAsync(e.Message.Id);
                    if(message.Reactions[0].Count > 4 && !CheckPinID(message.Id))
                    {
                        string thumbnailURL = "";
                        string desc = "";
                        if (message.Embeds.Count > 0)
                        {
                            //thumbnailURL = bruh.Embeds[0].Image.Url.ToString();
                        }
                        if (message.Attachments.Count > 0)
                        {
                            thumbnailURL = message.Attachments[0].Url;
                        }

                        if(message.Content != "")
                        {
                            desc = $@"""{message.Content}""";
                        }

                        var bruhgimus = new DiscordEmbedBuilder
                        {
                            Title = $"GEM ALERT!",
                            Description = desc + "\n" + "",
                            ImageUrl = thumbnailURL,
                            Thumbnail = new DiscordEmbedBuilder.EmbedThumbnail { Url = "https://media.discordapp.net/attachments/977270567881298024/1076252389637627904/850_-_SoyBooru.gif" },
                            Footer = new DiscordEmbedBuilder.EmbedFooter { IconUrl = message.Author.GetAvatarUrl(DSharpPlus.ImageFormat.Png, 256), Text = $"{message.Author.Username}#{message.Author.Discriminator}" },
                            Color = DiscordColor.PhthaloBlue
                        }.AddField("Gem:", $"[link]({message.JumpLink})").Build();
                        await discord.SendMessageAsync(discord.GetChannelAsync(1075588362230042694).Result, bruhgimus);
                        File.AppendAllText(pinnedPath, message.Id.ToString() + "\n");
                    }
                }
                if (e.Emoji.Id == 1075778708629110885) //Coal
                {
                    var message = await e.Channel.GetMessageAsync(e.Message.Id);
                    foreach (var reaction in message.Reactions)
                    {
                        if(reaction.Emoji.Id == 1075778708629110885)
                        {
                            if (reaction.Count > 4 && !CheckPinID(message.Id))
                            {
                                string thumbnailURL = "";
                                string desc = "";
                                if(message.Embeds.Count > 0)
                                {
                                    thumbnailURL = message.Embeds[0].Image.Url.ToString();
                                }
                                if (message.Attachments.Count > 0)
                                {
                                    thumbnailURL = message.Attachments[0].Url;
                                }

                                if (message.Content != "")
                                {
                                    desc = $@"""{message.Content}""";
                                }

                                var embed = new DiscordEmbedBuilder
                                {
                                    Title = $"COAL!!!! STINKY PISSCOAL ALERT!!!!",
                                    Description = desc + "\n" + "",
                                    ImageUrl = thumbnailURL,
                                    Thumbnail = new DiscordEmbedBuilder.EmbedThumbnail { Url = "https://cdn.discordapp.com/attachments/977270567881298024/1076252390157733958/862_-_SoyBooru.gif" },
                                    Footer = new DiscordEmbedBuilder.EmbedFooter { IconUrl = message.Author.GetAvatarUrl(DSharpPlus.ImageFormat.Png, 256), Text = $"{message.Author.Username}#{message.Author.Discriminator}" },
                                    Color = DiscordColor.Black
                                }.AddField("Coal:", $"[link]({message.JumpLink})").Build();
                                await discord.SendMessageAsync(discord.GetChannelAsync(1075588362230042694).Result, embed);
                                File.AppendAllText(pinnedPath, message.Id.ToString() + "\n");
                            }   
                        }
                    }
                }
                bool debug = true;
                if (e.Emoji.Id == 959642740277252136) //Delete
                {
                    var bruh = await e.Channel.GetMessageAsync(e.Message.Id);
                    if(e.User.Id == 304033317513199617 && bruh.Author.Id == 305520963238494219) //Only let me delete messages from the bot, so it's not a le epic backdoor.
                    {
                        await bruh.DeleteAsync();
                    }
                }
                if (e.Emoji.Id == 820033186008399903) //Debug
                {
                    var message = await e.Channel.GetMessageAsync(e.Message.Id);
                    if (e.User.Id == 304033317513199617)
                    {
                        foreach (var reaction in message.Reactions)
                        {
                            if (reaction.Emoji.Id == 820033186008399903)
                            {
                                if (reaction.Count > 0)
                                {
                                    string thumbnailURL = "";
                                    string desc = "";
                                    if (message.Embeds.Count > 0)
                                    {
                                        thumbnailURL = message.Embeds[0].Thumbnail.Url.ToString();
                                        var video = message.Embeds[0].Video;
                                        if(video != null)
                                        {
                                            thumbnailURL = video.Url.ToString();
                                        }
                                    }
                                    if (message.Attachments.Count > 0)
                                    {
                                        thumbnailURL = message.Attachments[0].Url;
                                    }

                                    if (message.Content != "")
                                    {
                                        desc = $@"""{message.Content}""";
                                    }

                                    var embed = new DiscordEmbedBuilder
                                    {
                                        Title = $"....Debug alert?",
                                        Description = desc + "\n" + "",
                                        ImageUrl = thumbnailURL,
                                        Thumbnail = new DiscordEmbedBuilder.EmbedThumbnail { Url = "https://cdn.discordapp.com/attachments/977270567881298024/1078813136649474128/pinson.gif" },
                                        Footer = new DiscordEmbedBuilder.EmbedFooter { IconUrl = message.Author.GetAvatarUrl(DSharpPlus.ImageFormat.Png, 256), Text = $"{message.Author.Username}#{message.Author.Discriminator}" },
                                        Color = DiscordColor.Black
                                    }.AddField("Debug:", $"[link]({message.JumpLink})").Build();
                                    await discord.SendMessageAsync(discord.GetChannelAsync(1075588362230042694).Result, embed);
                                    //File.AppendAllText(pinnedPath, bruh.Id.ToString() + "\n");
                                }
                            }
                        }
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