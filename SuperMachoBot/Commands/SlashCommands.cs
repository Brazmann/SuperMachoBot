using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using Newtonsoft.Json;

namespace SuperMachoBot.Commands
{
    public class SlashCommands : ApplicationCommandModule
    {
        public static string rootPath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
        [SlashCommand("Avatar", "Gets high resolution avatar of specified user.")]
        public async Task AvatarCommand(InteractionContext ctx, [Option("user", "Discord user to grab avatar from")] DiscordUser du)
        {
            var color = new DiscordColor(2, 200, 2);
            var embed = new DiscordEmbedBuilder
            {
                Title = du.Username,
                Color = color,
                ImageUrl = Tools.General.AvatarParser(du)
            };
            await ctx.CreateResponseAsync(embed);
        }

        Random rnd = new Random();
        [SlashCommand("StatRoller", "Rolls character stats for d&d 5e using the 4d6k3 calculation")]
        public async Task StatRollerCommand(InteractionContext ctx)
        {
            int[] stats = { StatRoller(), StatRoller(), StatRoller(), StatRoller(), StatRoller(), StatRoller() };
            await ctx.CreateResponseAsync($"{stats[0]}\n{stats[1]}\n{stats[2]}\n{stats[3]}\n{stats[4]}\n{stats[5]}");
        }
        public int StatRoller()
        {
            int[] rolls = { rnd.Next(1, 7), rnd.Next(1, 7), rnd.Next(1, 7), rnd.Next(1, 7) };
            int result = rolls[0] + rolls[1] + rolls[2] + rolls[3] - rolls.Min();
            return result;
        }

        [SlashCommand("EmbedTest", "Tests discord embed feature lol")]
        public async Task DebugEmbedCommand(InteractionContext ctx)
        {
            var color = new DiscordColor(200, 2, 2);
            var embed = new DiscordEmbedBuilder
            {
                Title = "bruh",
                Description = "bruh",
                Color = color
            };
            await ctx.CreateResponseAsync(embed);
        }
        [SlashCommand("Banner", "Gets the banner of the current server.")]
        public async Task GuildBannerCommand(InteractionContext ctx)
        {
            var bannerUrl = ctx.Guild.BannerUrl;
            if (bannerUrl == null)
            {
                await ctx.CreateResponseAsync("Error! Current server does not have a banner!");
            }
            else
            {
                var embed = new DiscordEmbedBuilder
                {
                    ImageUrl = bannerUrl
                };
                await ctx.CreateResponseAsync(embed);
            }
        }

        [SlashCommand("UserInfo", "Gets info from user")]
        public async Task UserInfoCommand(InteractionContext ctx, [Option("user", "Discord user to grab info from")] DiscordUser du)
        {
            var hashcode = du.GetHashCode();
            string[] Info =
            {
                $"**Account Creation Date:** {du.CreationTimestamp}",
                $"**Account ID:** {du.Id}",
                $"**User Language:** (Currently not functional)",
                $"**Is Bot?** {du.IsBot}",
                $"**Is Discord Admin?:** {du.IsSystem}"
            };
            var embed = new DiscordEmbedBuilder
            {
                Title = $"{du.Username}#{du.Discriminator}",
                Description = $"{Info[0]} \n {Info[1]} \n {Info[2]} \n {Info[3]} \n {Info[4]}",
                ImageUrl = Tools.General.AvatarParser(du)
            };
            await ctx.CreateResponseAsync(embed);
        }

        [SlashCommand("OwnerSetup", "Allows the owner to setup/change their guilds SuperMachoBot settings.")]
        public async Task OwnerSetupCommand(InteractionContext ctx, [Option("gemboard", "Channel to designate as gemboard.")] DiscordChannel dc,
            [Option("gemamount", "Amount of gems required for a message to be added to gemboard.")] long gemAmount,
            [Option("turboamount", "Amount of gem/coal required for a message to be added to gemboard as a gemerald/brimstone.")] long turboAmount,
            [Option("gemEmoteId", "ID of emote to use as gem.")] string gemEmoteId,
            [Option("coalEmoteId", "ID of emote to use as coal.")] string coalEmoteId)
        {
            if (ctx.Member.IsOwner)
            {
                if (!Directory.Exists(@$"{Program.databasePath}/{ctx.Guild.Id}/"))
                {
                    Directory.CreateDirectory(@$"{Program.databasePath}/{ctx.Guild.Id}/");
                }
                if (!File.Exists(@$"{Program.databasePath}/{ctx.Guild.Id}/Config.json"))
                {
                    File.Create(@$"{Program.databasePath}/{ctx.Guild.Id}/Config.json").Close();
                }
                if(!ulong.TryParse(gemEmoteId, out var gemEmoteParsed))
                {
                    await ctx.CreateResponseAsync("Invalid gem id! Try again!");
                } else if (!ulong.TryParse(coalEmoteId, out var coalEmoteParsed))
                {
                    await ctx.CreateResponseAsync("Invalid coal id! Try again!");
                } else {
                    var config = new List<GuildConfig>();
                    var configuration = new GuildConfig()
                    {
                        gemboardChannelId = dc.Id,
                        gemAmount = gemAmount,
                        turboAmount = turboAmount,
                        gemEmoteId = gemEmoteParsed,
                        coalEmoteId = coalEmoteParsed
                    };
                    if(Tools.General.CreateConfig(configuration, ctx.Guild.Id))
                    {
                        await ctx.CreateResponseAsync($"Configuration applied successfully!");
                    } else
                    {
                        await ctx.CreateResponseAsync($"Configuration NOT applied successfully! Yell at bot developer! NOW!!");
                    }
                }
            }
            else
            {
                await ctx.CreateResponseAsync("You aren't owner. Stop trying to do owner stuff when you aren't an owner, peasant.");
            }
        }

        [SlashCommand("EmbeddingTest", "Sends a placeholder embed for gemboard.")]
        public async Task UserInfoCommand(InteractionContext ctx)
        {
            var bruhgimus = new DiscordEmbedBuilder
            {
                Title = $"GEM ALERT!",
                Description = $@"""https://twitter.com/cametek/status/1626024042254962688?t=qO5w7KG_5pAO2fBc0D3zOg&s=19""" + "\n" + "",
                Thumbnail = new DiscordEmbedBuilder.EmbedThumbnail { Url = "https://images-ext-2.discordapp.net/external/eF0rSZ4LMUqftzoQmSqKq9P4-nGoyU7W7G74KSnLSls/https/pbs.twimg.com/ext_tw_video_thumb/1626022911822934016/pu/img/7yXC_-9lc9dWtC07.jpg" },
                Footer = new DiscordEmbedBuilder.EmbedFooter { IconUrl = ctx.User.GetAvatarUrl(DSharpPlus.ImageFormat.Png, 256), Text = "TestUser#0000" },
                Color = DiscordColor.Red
            }.AddField("Gem:", "[link](https://discord.com/channels/977270567881298021/977270567881298024/1075763823740461056)").Build();
            await ctx.CreateResponseAsync(bruhgimus);
        }
    }

    public class PlayerData
    {
        public int CoordinateY;
        public int CoordinateX;
    }
    public class Entry
    {
        [JsonProperty("userid")]
        public string UserId { get; set; }

        [JsonProperty("money")]
        public string Money { get; set; }

        [JsonProperty("timesincelastdailyunixtimestamp")]
        public string TimeSinceLastDailyUnixTimestamp { get; set; }
    }
}