using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using Newtonsoft.Json;
using SuperMachoBot.Commands;
using System.Data.SqlTypes;
using System.Linq.Expressions;

namespace SuperMachoBot
{
    class Program
    {
        public static string rootPath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
        public static bool moneyCooldown = true;
        public static List<Config> configItems = new List<Config>();
        public static DiscordClient discord;
        public static string databasePath = "";
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

            databasePath = configItems[0].EconomyDatabasePath;
            discord.MessageReactionAdded += async (s, e) =>
            {
                try //I don't think this is good practice. Fuck it.
                {
                    List<GuildConfig> config = JsonConvert.DeserializeObject<List<GuildConfig>>(File.ReadAllText(@$"{Program.databasePath}/{e.Guild.Id}/Config.json"));
                    var bruh = config[0].coalEmoteId;
                    if (e.Emoji.Id == config[0].gemEmoteId) //Gem
                    {
                        var message = await e.Channel.GetMessageAsync(e.Message.Id);
                        foreach (var reaction in message.Reactions)
                        {
                            if (reaction.Emoji.Id == config[0].gemEmoteId)
                            {
                                if (reaction.Count > config[0].gemAmount - 1 && !CheckPinID(message.Id, message.Channel.GuildId) && message.ChannelId != config[0].gemboardChannelId && !message.Channel.IsNSFW)
                                {
                                    string thumbnailURL = GetRelevantEmbedURL(message);
                                    string desc = "";
                                    File.AppendAllText($"{databasePath}/{message.Channel.GuildId}/Pinned.txt", message.Id.ToString() + $",{message.Author.Id}\n");
                                    if (message.Content != "")
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
                                    await discord.SendMessageAsync(discord.GetChannelAsync(config[0].gemboardChannelId).Result, bruhgimus);
                                }
                            }
                        }
                    }
                    if (e.Emoji.Id == config[0].gemEmoteId) //GEMERALD!!
                    {
                        var message = await e.Channel.GetMessageAsync(e.Message.Id);
                        foreach (var reaction in message.Reactions)
                        {
                            if (reaction.Emoji.Id == config[0].gemEmoteId)
                            {
                                if (reaction.Count > config[0].turboAmount - 1 && !CheckUltraPinID(message.Id, message.Channel.GuildId) && message.ChannelId != config[0].gemboardChannelId && !message.Channel.IsNSFW)
                                {
                                    string thumbnailURL = GetRelevantEmbedURL(message);
                                    string desc = "";
                                    File.AppendAllText($"{databasePath}/{message.Channel.GuildId}/UltraPinned.txt", message.Id.ToString() + $",{message.Author.Id}\n");
                                    if (message.Content != "")
                                    {
                                        desc = $@"""{message.Content}""";
                                    }

                                    var bruhgimus = new DiscordEmbedBuilder
                                    {
                                        Title = $"GEMERALD ALERT! GEMERALD ALERT! {config[0].turboAmount}+ GEMS!",
                                        Description = desc + "\n" + "",
                                        ImageUrl = thumbnailURL,
                                        Thumbnail = new DiscordEmbedBuilder.EmbedThumbnail { Url = "https://cdn.discordapp.com/attachments/977270567881298024/1093186592782422057/Gemerald.png" },
                                        Footer = new DiscordEmbedBuilder.EmbedFooter { IconUrl = message.Author.GetAvatarUrl(DSharpPlus.ImageFormat.Png, 256), Text = $"{message.Author.Username}#{message.Author.Discriminator}" },
                                        Color = new DiscordColor("#66ff33")
                                    }.AddField("Gemerald:", $"[link]({message.JumpLink})").Build();
                                    await discord.SendMessageAsync(discord.GetChannelAsync(config[0].gemboardChannelId).Result, bruhgimus);
                                }
                            }
                        }
                    }
                    if (e.Emoji.Id == config[0].coalEmoteId) //Coal
                    {
                        var message = await e.Channel.GetMessageAsync(e.Message.Id);
                        foreach (var reaction in message.Reactions)
                        {
                            if (reaction.Emoji.Id == config[0].coalEmoteId)
                            {
                                if (reaction.Count > config[0].gemAmount - 1 && !CheckPinID(message.Id, message.Channel.GuildId) && message.ChannelId != config[0].gemboardChannelId && !message.Channel.IsNSFW)
                                {
                                    string thumbnailURL = GetRelevantEmbedURL(message);
                                    string desc = "";

                                    File.AppendAllText($"{databasePath}/{message.Channel.GuildId}/Pinned.txt", message.Id.ToString() + $",{message.Author.Id}\n");

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
                                    await discord.SendMessageAsync(discord.GetChannelAsync(config[0].gemboardChannelId).Result, embed);
                                }
                            }
                        }
                    }
                    if (e.Emoji.Id == config[0].coalEmoteId) //BRIMSTONE!!
                    {
                        var message = await e.Channel.GetMessageAsync(e.Message.Id);
                        foreach (var reaction in message.Reactions)
                        {
                            if (reaction.Emoji.Id == config[0].coalEmoteId)
                            {
                                if (reaction.Count > config[0].turboAmount - 1 && !CheckUltraPinID(message.Id, message.Channel.GuildId) && message.ChannelId != config[0].gemboardChannelId && !message.Channel.IsNSFW)
                                {
                                    string thumbnailURL = GetRelevantEmbedURL(message);
                                    string desc = "";

                                    File.AppendAllText($"{databasePath}/{message.Channel.GuildId}/UltraPinned.txt", message.Id.ToString() + $",{message.Author.Id}\n");

                                    if (message.Content != "")
                                    {
                                        desc = $@"""{message.Content}""";
                                    }

                                    var embed = new DiscordEmbedBuilder
                                    {
                                        Title = $"BRIMSTONE!!!! HELLISH TORTURECOAL ALERT!!!! {config[0].turboAmount}+ COALS!!!!",
                                        Description = desc + "\n" + "",
                                        ImageUrl = thumbnailURL,
                                        Thumbnail = new DiscordEmbedBuilder.EmbedThumbnail { Url = "https://cdn.discordapp.com/attachments/977270567881298024/1095576471235465266/Brimstone.png" },
                                        Footer = new DiscordEmbedBuilder.EmbedFooter { IconUrl = message.Author.GetAvatarUrl(DSharpPlus.ImageFormat.Png, 256), Text = $"{message.Author.Username}#{message.Author.Discriminator}" },
                                        Color = DiscordColor.DarkRed
                                    }.AddField("Brimstone:", $"[link]({message.JumpLink})").Build();
                                    await discord.SendMessageAsync(discord.GetChannelAsync(config[0].gemboardChannelId).Result, embed);
                                }
                            }
                        }
                    }
                    bool debug = true;
                    if (e.Emoji.Id == 959642740277252136) //Delete
                    {
                        var bruha = await e.Channel.GetMessageAsync(e.Message.Id);
                        if (e.User.Id == 304033317513199617 && bruha.Author.Id == 305520963238494219) //Only let me delete messages from the bot, so it's not a le epic backdoor.
                        {
                            await bruha.DeleteAsync();
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
                                            if (video != null)
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
                                        await discord.SendMessageAsync(discord.GetChannelAsync(config[0].gemboardChannelId).Result, embed);
                                        //File.AppendAllText(pinnedPath, bruh.Id.ToString() + "\n");
                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {

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

            await discord.ConnectAsync();
            await Task.Delay(-1);
        }
        static void cooldown()
        {
            moneyCooldown = true;
            Thread.Sleep(10000);
            moneyCooldown = false;
        }

        static bool CheckPinID(ulong messageid, ulong? guildid)
        {
            if (!Directory.Exists($"{databasePath}/{guildid}/"))
            {
                Directory.CreateDirectory(@$"{databasePath}/{guildid}/");
            }
            if (!File.Exists($"{databasePath}/{guildid}/Pinned.txt"))
            {
                File.Create(@$"{databasePath}/{guildid}/Pinned.txt").Close();
            }
            foreach (var line in File.ReadAllLines($"{databasePath}/{guildid}/Pinned.txt"))
            {
                if (line.Split(',')[0] == messageid.ToString())
                {
                    return true;
                }
            }
            return false;
        }

        static bool CheckUltraPinID(ulong messageid, ulong? guildid)
        {
            if (!Directory.Exists($"{databasePath}/{guildid}/"))
            {
                Directory.CreateDirectory(@$"{databasePath}/{guildid}/");
            }
            if (!File.Exists($"{databasePath}/{guildid}/UltraPinned.txt"))
            {
                File.Create(@$"{databasePath}/{guildid}/UltraPinned.txt").Close();
            }
            foreach (var line in File.ReadAllLines($"{databasePath}/{guildid}/UltraPinned.txt"))
            {
                if (line.Split(',')[0] == messageid.ToString())
                {
                    return true;
                }
            }
            return false;
        }

        static string GetRelevantEmbedURL(DiscordMessage message)
        {
            string thumbnailURL = "";
            if (message.Embeds.Count > 0)
            {
                //thumbnailURL = message.Embeds[0].Image.Url.ToString();
                var video = message.Embeds[0].Video;
                var image = message.Embeds[0].Image;
                //thumbnailURL = message.Embeds[0].Video != null ? message.Embeds[0].Video.Url.ToString() : message.Embeds[0].Image.Url.ToString();
                if (video != null)
                {
                    var urlString = video.Url.ToString(); //Hate Tenor Hate Tenor Hate Tenor Hate Tenor Hate Tenor Hate Tenor RAGE!!!!
                    bool isTenorUrl = urlString.StartsWith("https://media.tenor.com");
                    if (isTenorUrl)
                    {
                        /*RANT: I HATE TENOR
                         * IF YOU WANT TO ACTUALLY GET THE MP4 FILE, YOU HAVE TO CHANGE THE WEIRD FILETYPE IDENTIFIER LETTERS IN THE URL FROM 'AAAPo' to 'AAAAd'.
                         * WHY WOULD YOU DO THIS TO ME YOU MOTHERFUCKERS? THIS SHIT LITERALLY BREAKS EMBED ON DISCORD MOBILE TOO!
                         * Oh also Discord is a bit stupid and I have to change the end of the url from '.mp4' to '.gif' for it to autoplay correctly. Lol!
                        */
                        thumbnailURL = urlString.Replace("AAAPo", "AAAAd");
                        thumbnailURL = thumbnailURL.Substring(0, thumbnailURL.Length - 3) + "gif";
                    }
                    else
                    {
                        thumbnailURL = video.Url.ToString();
                    }
                }
                else
                {
                    thumbnailURL = image.Url.ToString();
                }
            }
            else if (message.Attachments.Count > 0)
            {
                thumbnailURL = message.Attachments[0].Url;
            }
            return thumbnailURL;
        }
    }
    public class Config
    {
        public string Token;
        public ulong OwnerID;
        public string EconomyDatabasePath;
    }

    public class GuildConfig
    {
        public ulong gemboardChannelId;
        public long gemAmount;
        public long turboAmount;
        public ulong gemEmoteId;
        public ulong coalEmoteId;
    }
}