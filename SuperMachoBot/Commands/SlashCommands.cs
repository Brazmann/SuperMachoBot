using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using Newtonsoft.Json;
using static System.Net.WebRequestMethods;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SuperMachoBot.Commands
{
    public class SlashCommands : ApplicationCommandModule
    {
        public static string rootPath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
        #region General Commands
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

        [SlashCommand("EmbeddingTest", "Sends a placeholder embed for gemboard.")]
        public async Task UserInfoCommand(InteractionContext ctx)
        {
            var bruhgimus = new DiscordEmbedBuilder { Title = $"GEM ALERT!",
                Description = $@"""https://twitter.com/cametek/status/1626024042254962688?t=qO5w7KG_5pAO2fBc0D3zOg&s=19""" + "\n" + "", 
                Thumbnail = new DiscordEmbedBuilder.EmbedThumbnail { Url = "https://images-ext-2.discordapp.net/external/eF0rSZ4LMUqftzoQmSqKq9P4-nGoyU7W7G74KSnLSls/https/pbs.twimg.com/ext_tw_video_thumb/1626022911822934016/pu/img/7yXC_-9lc9dWtC07.jpg"},
                Footer = new DiscordEmbedBuilder.EmbedFooter { IconUrl = ctx.User.GetAvatarUrl(DSharpPlus.ImageFormat.Png, 256), Text = "TestUser#0000" },
                Color = DiscordColor.Red }.AddField("Gem:", "[link](https://discord.com/channels/977270567881298021/977270567881298024/1075763823740461056)").Build();
            await ctx.CreateResponseAsync(bruhgimus);
        }

        /*[SlashCommand("Testing", "Tests.")]
        public async Task TestingCommand(InteractionContext ctx)
        {
            try
            {
            int[] row1 = new int[6] { 0, 0, 0, 0, 0, 0 }; //row = Y axis, entries within array = X axis. 0 = empty, 1 = player.
            StringBuilder sb = new StringBuilder("", row1.Length);
            var path = @$"{rootPath}\FunDatabase\MapTest\{ctx.User.Id}.json";

            if (File.Exists(path) == false)
            {
                List<PlayerData> playerData = new List<PlayerData>();

                playerData.Add(new PlayerData()
                {
                    CoordinateY = 1,
                    CoordinateX = 4
                });
                string json = JsonConvert.SerializeObject(playerData.ToArray());

                File.AppendAllText(path, json);
            }
            string playerDataJson = File.ReadAllText(path);

            var playerDataParsed = JsonConvert.DeserializeObject<List<PlayerData>>(playerDataJson);


            for (int i = 0; i < row1.Length; i++)
            {
                if (i == playerDataParsed[0].CoordinateX)
                {
                    sb.Append(":person_in_motorized_wheelchair:");
                }
                else
                {
                    switch (row1[i])
                    {
                        case 0:
                            sb.Append(":eight_pointed_black_star:");
                            break;
                    }
                }
            }

            var builder = new DiscordMessageBuilder().WithContent(sb.ToString()).AddComponents(new DiscordComponent[]
            {
                    new DiscordButtonComponent(ButtonStyle.Primary, "1_left", "Left"),
                        new DiscordButtonComponent(ButtonStyle.Secondary, "2_up", "Up"),
                        new DiscordButtonComponent(ButtonStyle.Success, "3_down", "Down"),
                        new DiscordButtonComponent(ButtonStyle.Danger, "4_right", "Right")
            }).SendAsync(ctx.Channel);

            ctx.Client.ComponentInteractionCreated += async (s, e) =>
            {
                Console.WriteLine("Ben?");
                int playerY = playerDataParsed[0].CoordinateY;
                int playerX = playerDataParsed[0].CoordinateX;
                switch (e.Interaction.Data.CustomId)
                {
                    case "1_left":
                        if(playerX > 0)
                        {
                            playerX = playerX - 1;
                        }
                        break;
                    case "2_up":
                        break;
                    case "3_down":
                        break;
                    case "4_right":
                        if(playerX < 5)
                        {
                            playerX++;
                        }
                        break;
                }
                Console.WriteLine($"playerX: {playerX} playerY: {playerY}");
                await e.Interaction.CreateResponseAsync(InteractionResponseType.UpdateMessage, new DiscordInteractionResponseBuilder().WithContent(":thumbsup:"));

                List<PlayerData> playerDataNew = new List<PlayerData>();

                playerDataNew.Add(new PlayerData()
                {
                    CoordinateY = playerY,
                    CoordinateX = playerX
                });
                string json = JsonConvert.SerializeObject(playerDataNew.ToArray());

                File.WriteAllText(path, json);
                TestingCommand(ctx);

            };
            } catch (Exception ex)
            {
               await ctx.CreateResponseAsync(ex.Message);
            }
        }*/

        #endregion
        #region Economy Commands
        /*[SlashCommand("Balance", "Checks your balance")]
        public async Task BalanceCommand(InteractionContext ctx, [Option("User", "User to check balance of")] DiscordUser du)
        {
            var entry = EconDatabaseChecker(du.Id, ctx.Guild.Id);
            var entryParsed = entry[0].Split('|');
            if (entry[0] == "noentry")
            {
                await ctx.CreateResponseAsync("No entry found! Generating one, please try again.");
            }
            await ctx.CreateResponseAsync($"{du.Username}: ${entryParsed[1]}");
        }


        [ContextMenu(ApplicationCommandType.UserContextMenu, "Check balance")]
        public async Task BalanceMenuCommand(ContextMenuContext ctx)
        {
            var entry = EconDatabaseChecker(ctx.TargetUser.Id, ctx.Guild.Id);
            var entryParsed = entry[0].Split('|');
            if (entry[0] == "noentry")
            {
                await ctx.CreateResponseAsync("No entry found! Generating one, please try again.");
            }
            await ctx.CreateResponseAsync($"{ctx.TargetUser.Username}: ${entryParsed[1]}");
        }

        [SlashCommand("Daily", "Adds $100 to your balance")]
        public async Task DailyCommand(InteractionContext ctx)
        {
            var path = $@"{rootPath}\EconomyDatabase\{ctx.Guild.Id}.csv";
            var amount = 100;
            var entry = EconDatabaseChecker(ctx.User.Id, ctx.Guild.Id);
            var entryParsed = entry[0].Split('|');
            var entryNumber = Int32.Parse(entry[1]);
            Int32 unixTimestamp = (Int32)(DateTime.Now.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
            if (entryParsed[2] == "none")
            {
                string[] lines = File.ReadAllLines(path);
                lines[entryNumber - 1] = $"{entryParsed[0]}|{entryParsed[1]}|{unixTimestamp}|";
                WriteAllLinesBetter(path, lines);
                AddSubtractUserMoney(ctx.User.Id, ctx.Guild.Id, amount);
                await ctx.CreateResponseAsync("First daily! Come back in 24 hours!");
            }
            else
            {
                Int32 secondsSinceLastDaily = unixTimestamp - Convert.ToInt32(entryParsed[2]);
                if (secondsSinceLastDaily > 86400) //Check if a day has passed
                {
                    string[] lines = File.ReadAllLines(path);
                    lines[entryNumber - 1] = $"{entryParsed[0]}|{entryParsed[1]}|{unixTimestamp}|";
                    WriteAllLinesBetter(path, lines);
                    AddSubtractUserMoney(ctx.User.Id, ctx.Guild.Id, amount);
                    await ctx.CreateResponseAsync("Daily claimed! Come back in 24 hours!");
                }
                else if (secondsSinceLastDaily < 86400)
                {
                    var secondsUntilClaim = 86400 - secondsSinceLastDaily;
                    await ctx.CreateResponseAsync($"Daily already claimed! Come back in {secondsUntilClaim / 3600} hours!");
                }
            }
        }

        [SlashCommand("Transfer", "Transfer your money to another user.")]
        public async Task TransferCommand(InteractionContext ctx, [Option("Amount", "Amount to transfer")] long amount, [Option("User", "User to transfer money to")] DiscordUser du)
        {
            if (amount < 0)
            {
                await ctx.CreateResponseAsync("Negative amount detected! Sorry, robbery has not been implemented yet!");
            }
            else
            {
                AddSubtractUserMoney(ctx.User.Id, ctx.Guild.Id, -amount);
                AddSubtractUserMoney(du.Id, ctx.Guild.Id, amount);
                await ctx.CreateResponseAsync($"${amount} transferred from {ctx.User.Mention} to {du.Mention}");
            }
        }

        [SlashCommand("Twash", "The.")]
        public async Task TwashCommand(InteractionContext ctx, [Option("Year", "Age")]long age)
        {
            string message = $"{age} year old Twash be like: anyone under {age + 4} is an infant to me now.";
            await ctx.CreateResponseAsync(message);
        }

        [SlashCommand("Betflip", "Heads or Tails coinflip!")]
        public async Task BetflipCommand(InteractionContext ctx, [Option("Choice", "Heads or Tails? H or T? Choose your path wisely.")] string choice, [Option("Amount", "Real: (typing 'All' currently doesn't work, do it manually.)")] long betAmount)
        {
            var uid = ctx.User.Id;
            var gid = ctx.Guild.Id;
            var entry = EconDatabaseChecker(ctx.User.Id, ctx.Guild.Id);
            var entryParsed = entry[0].Split('|');
            var playerMoney = Convert.ToInt64(entryParsed[1]);
            var moneyEarned = 0;
            if (betAmount > playerMoney)
            {
                await ctx.CreateResponseAsync("You do not have enough money!");
            }
            else
            {
                int flip = rnd.Next(1, 3); // 1 = heads, 2 = tails
                string decision = "";
                string headsURL = "https://cdn.discordapp.com/attachments/978411926222684220/1006493578186469376/domcoinheads.png";
                string tailsURL = "https://cdn.discordapp.com/attachments/978411926222684220/1006493587342622730/domcointails.png";
                switch (choice.ToLower())
                {
                    case "h":
                    case "head":
                    case "heads":
                        decision = "heads";
                        break;
                    case "t":
                    case "tail":
                    case "tails":
                        decision = "tails";
                        break;
                }
                if (decision.ToLower() == "heads")
                {
                    switch (flip)
                    {
                        case 1:
                            await ctx.CreateResponseAsync(embed: new DiscordEmbedBuilder { Title = $"Heads! You win ${betAmount}!", ImageUrl = headsURL }.Build());
                            AddSubtractUserMoney(uid, gid, betAmount);
                            break;
                        case 2:
                            await ctx.CreateResponseAsync(embed: new DiscordEmbedBuilder { Title = $"Tails! You lose ${betAmount}!", ImageUrl = tailsURL }.Build());
                            AddSubtractUserMoney(uid, gid, -betAmount);
                            break;
                    }
                }
                if (decision.ToLower() == "tails")
                {
                    switch (flip)
                    {
                        case 1:
                            await ctx.CreateResponseAsync("Heads! You lose!");
                            await ctx.CreateResponseAsync(embed: new DiscordEmbedBuilder { Title = $"Heads! You lose ${betAmount}!", ImageUrl = headsURL }.Build());
                            AddSubtractUserMoney(uid, gid, -betAmount);
                            break;
                        case 2:
                            await ctx.CreateResponseAsync(embed: new DiscordEmbedBuilder { Title = $"Tails! You win ${betAmount}!", ImageUrl = tailsURL }.Build());
                            AddSubtractUserMoney(uid, gid, betAmount);
                            break;
                    }
                }
            }
        }


        [SlashCommand("Wheel", "Roll the wheel of Macho Fortune!")]
        public async Task WheelCommand(InteractionContext ctx, [Option("Amount", "Real: (typing 'All' currently doesn't work, do it manually.)")] long betAmount)
        {
            if (betAmount < 0)
            {
                await ctx.CreateResponseAsync("Negative numbers are not allowed!");
            }
            else
            {
                var entry = EconDatabaseChecker(ctx.User.Id, ctx.Guild.Id);
                var entryParsed = entry[0].Split('|');
                double playerMoney = Convert.ToDouble(entryParsed[1]);
                double moneyEarned = 0;
                if (betAmount > playerMoney)
                {
                    await ctx.CreateResponseAsync("You do not have enough money!");
                }
                else
                {
                    var roll = rnd.Next(1, 8);
                    double multiplier = 1;
                    bool shit = false;
                    switch (roll)
                    {
                        case 1:
                            multiplier = 2.4;
                            shit = true;
                            break;
                        case 2:
                            multiplier = 1.8;
                            shit = true;
                            break;
                        case 3:
                            multiplier = 1.4;
                            shit = true;
                            break;
                        case 4:
                            multiplier = 0;
                            break;
                        case 5:
                            multiplier = 1.4;
                            break;
                        case 6:
                            multiplier = 1.8;
                            break;
                        case 7:
                            multiplier = 2.4;
                            break;
                    }
                    if (shit == true)
                    {
                        moneyEarned = betAmount * multiplier;
                        AddSubtractUserMoney(ctx.User.Id, ctx.Guild.Id, -Convert.ToInt64(moneyEarned));
                        await ctx.CreateResponseAsync($"Money multiplied by -{multiplier}x, lost ${moneyEarned}! Sad!");
                    }
                    else
                    {
                        moneyEarned = betAmount * multiplier;
                        AddSubtractUserMoney(ctx.User.Id, ctx.Guild.Id, Convert.ToInt64(moneyEarned));
                        await ctx.CreateResponseAsync($"Money multiplied by {multiplier}x, ${moneyEarned}!");
                    }
                }
            }
        }
        #endregion
        #region Economy Tools (I really need to stuff this into the library)
        public void AddSubtractUserMoney(ulong userID, ulong guildID, long amount)
        {
            var entry = EconDatabaseChecker(userID, guildID);
            var path = $@"{rootPath}\EconomyDatabase\{guildID}.csv";
            var entryNumber = Int32.Parse(entry[1]);
            var entryParsed = entry[0].Split('|');
            var currentAmount = entryParsed[1];
            long finalAmount = Convert.ToInt64(currentAmount) + amount;
            string[] lines = File.ReadAllLines(path);
            lines[entryNumber - 1] = $"{userID}|{finalAmount.ToString()}|{entryParsed[2]}|";
            WriteAllLinesBetter(path, lines);
        }
        //Thank you microsoft for requiring a rewrite of your entire method just to not have it add an extra new line at the end of a file. :tf:
        public static void WriteAllLinesBetter(string path, params string[] lines)
        {
            if (path == null)
                throw new ArgumentNullException("path");
            if (lines == null)
                throw new ArgumentNullException("lines");

            using (var stream = File.OpenWrite(path))
            using (StreamWriter writer = new StreamWriter(stream))
            {
                if (lines.Length > 0)
                {
                    for (int i = 0; i < lines.Length - 1; i++)
                    {
                        writer.WriteLine(lines[i]);
                    }
                    writer.Write(lines[lines.Length - 1]);
                }
            }
        }




        public void MultiplyUserMoney(ulong userID, ulong guildID, float multiplier)
        {
            var entry = EconDatabaseChecker(userID, guildID);
            var path = $@"{rootPath}\EconomyDatabase\{guildID}.csv";
            var entryNumber = Int32.Parse(entry[1]);
            var entryParsed = entry[0].Split('|');
            var currentAmount = entryParsed[1];
            float finalAmount = float.Parse(currentAmount) * multiplier;
            string[] lines = File.ReadAllLines(path);
            lines[entryNumber - 1] = $"{userID}|{finalAmount.ToString()}";
            WriteAllLinesBetter(path, lines);
        }





        /// <summary>
        /// Finds the economy database entry for the specified UserID, or creates a new entry for the server/user if an entry for one is missing.
        /// </summary>
        /// <returns>
        /// The contents, and line number of the entry if found, in a string array.
        /// </returns>
        public static string[] EconDatabaseChecker(ulong userID, ulong guildID)
        {
            int lineCount = 0;
            string[] entry = { "noentry", "bingus" };
            var path = $@"{rootPath}\EconomyDatabase\{guildID}.csv";
            if (File.Exists(path) == false)
            {
                string entryToCreate = $"{userID}|100|none";
                File.AppendAllText(path, entryToCreate);
            }

            foreach (var line in File.ReadAllLines(path))
            {
                var entryparsed = line.Split('|');
                lineCount++;
                if (entryparsed[0] == userID.ToString())
                {
                    entry[0] = line; //Contents of entry line
                    entry[1] = $"{lineCount}"; //Number of line in .csv file
                    break;
                }
            }
            if (entry[0] == "noentry") //If after the file has been searched, no entry has been found, create a new one and stuff it in the 'entry' variable
            {
                string entryToCreate = $"{userID}|100|none";
                File.AppendAllText(path, Environment.NewLine + entryToCreate);
                return entry;
            }
            return entry;
        }*/
        #endregion
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