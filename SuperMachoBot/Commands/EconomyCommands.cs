using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using Newtonsoft.Json;

namespace SuperMachoBot.Commands
{
    public class EconomyCommands : ApplicationCommandModule
    {
        public static string jsonPath = "";
        Random rnd = new Random();

        #region Economy Commands

        /*[SlashCommand("Shutdown", "Kills the SuperMachoBot with a password.")]
        public async Task EconTestCommand(InteractionContext ctx, [Option("Password", "Enter it.")] string pass)
        {
            string shutdownPass = "TRUTH HAD GONE, TRUTH HAD GONE, AND TRUTH HAD GONE. AH, NOW TRUTH IS ASLEEP IN THE DARKNESS OF THE SINISTER HAND.";
            if (pass == shutdownPass)
            {
                await ctx.CreateResponseAsync("Shutting down. Thanks.");
                System.Environment.Exit(0);
            }
            else
            {
                await ctx.CreateResponseAsync("Wrong password! Try again! :P");
            }
        }*/

        public enum BetflipChoice
        {
            [ChoiceName("heads")]
            heads,
            [ChoiceName("tails")]
            tails,
        }

        public void CreateEconomyEntry(ulong userid, UserData data, ulong guildid)
        {
            // Add a new entry to the dictionary
            string jsonFilePath = @$"{jsonPath}/{guildid}/Economy.json";

            ulong newUserId = userid;

            Dictionary<ulong, UserData> userDataDict;

            string json = File.ReadAllText(jsonFilePath);
            userDataDict = JsonConvert.DeserializeObject<Dictionary<ulong, UserData>>(json);

            userDataDict.Add(newUserId, data);

            // Serialize the updated data and write it back to the file
            string newJson = JsonConvert.SerializeObject(userDataDict, Formatting.Indented);
            File.WriteAllText(jsonFilePath, newJson);
        }

        public void CreateEconomyFile(ulong initialUserID, UserData initialUserData, ulong guildid)
        {
            if (!Directory.Exists(@$"{jsonPath}/{guildid}/"))
            {
                Directory.CreateDirectory(@$"{jsonPath}/{guildid}/");
            }
            string jsonFilePath = @$"{jsonPath}/{guildid}/Economy.json";
            var dataDict = new Dictionary<ulong, UserData>();
            dataDict.Add(initialUserID, initialUserData);
            string newJson = JsonConvert.SerializeObject(dataDict, Formatting.Indented);
            File.WriteAllText(jsonFilePath, newJson);
        }

        public UserData GetEconomyEntry(ulong userid, ulong guildid)
        {
            string jsonFilePath = @$"{jsonPath}/{guildid}/Economy.json";
            // Read the JSON file and deserialize it into a dictionary
            if (!Directory.Exists(jsonFilePath))
            {
                Directory.CreateDirectory(@$"{jsonPath}/{guildid}/");
            }
            if (!File.Exists(jsonFilePath))
            {
                File.Create(jsonFilePath).Close();
            }
            Dictionary<ulong, UserData> userDataDict;

            string json = File.ReadAllText(jsonFilePath);
            userDataDict = JsonConvert.DeserializeObject<Dictionary<ulong, UserData>>(json);

            if (userDataDict == null)
            {
                CreateEconomyFile(userid, new UserData { money = 0, lastDaily = 0 }, guildid);
                return null;
            }
            else if (userDataDict.ContainsKey(userid))
            {
                UserData userData = userDataDict[userid];
                var money = userData.money;
                var lastDaily = userData.lastDaily;
                return userData;
            }
            else
            {
                var data = new UserData
                {
                    money = 0,
                    lastDaily = 0
                };
                CreateEconomyEntry(userid, data, guildid);
                return null;
            }
        }

        public Dictionary<ulong, UserData> GetEconomyEntries(ulong guildid)
        {
            string jsonFilePath = @$"{jsonPath}/{guildid}/Economy.json";
            // Read the JSON file and deserialize it into a dictionary
            if (!File.Exists(jsonFilePath))
            {
                throw new EconDatabaseNotFoundException("Could not find guild economy database file!");
            }
            Dictionary<ulong, UserData> userDataDict;

            string json = File.ReadAllText(jsonFilePath);
            userDataDict = JsonConvert.DeserializeObject<Dictionary<ulong, UserData>>(json);

            if (userDataDict == null)
            {
                throw new EconDatabaseNotFoundException("Invalid economy database data!");
            }
            return userDataDict;
        }

        public void EditEconomyEntry(ulong userid, UserData data, ulong guildid)
        {
            string jsonFilePath = @$"{jsonPath}/{guildid}/Economy.json";
            string json = File.ReadAllText(jsonFilePath);
            var userDataDict = JsonConvert.DeserializeObject<Dictionary<ulong, UserData>>(json);

            if(data.money < 0) //Check to prevent balances from entering the negatives
            {
                data.money = 0;
            }

            if (userDataDict.ContainsKey(userid))
            {
                UserData userData = userDataDict[userid];
                userData.money = data.money; // Update the money field
                userData.lastDaily = data.lastDaily; // Update the timestamp field
            }

            // Serialize the updated data and write it back to the file
            string newJson = JsonConvert.SerializeObject(userDataDict, Formatting.Indented);
            File.WriteAllText(jsonFilePath, newJson);
        }



        [SlashCommand("Balance", "Check users balance")]
        public async Task BalanceCommand(InteractionContext ctx, [Option("User", "User to check balance of")] DiscordUser du = null)
        {
            // Access the data using the userid key
            if(du == null)
            {
                du = ctx.User;
            }
            ulong userid = du.Id;
            UserData userData = GetEconomyEntry(userid, ctx.Guild.Id);
            if (userData != null)
            {
                var money = userData.money;
                var lastDaily = userData.lastDaily;
                await ctx.CreateResponseAsync($"{du.Username}: ${money}");
            }
            else //TODO: Fix bug which causes the response after new entry creation to not be sent, requiring the user to query again to see their balance.
            {
                await ctx.CreateResponseAsync($"No entry found! Creating new one. Please run this command again.");
                Thread.Sleep(1000);
                var newData = GetEconomyEntry(userid, ctx.Guild.Id);
                var money = newData.money;
                var lastDaily = newData.lastDaily;
                await ctx.CreateResponseAsync($"{du.Username}#{du.Discriminator}:${money} Last claimed daily:(Unix){lastDaily}");
            }
        }

        [SlashCommand("Baltop", "Shows leaderboard for top users by current balance")]
        public async Task BaltopCommand(InteractionContext ctx)
        {
            var dataDic = GetEconomyEntries(ctx.Guild.Id);
            if (dataDic != null) //Note: It should never return null, but I'm checking for futureproofing
            {
                var sortedDict = dataDic.OrderByDescending(pair => pair.Value.money).Take(20).ToDictionary(pair => pair.Key, pair => pair.Value);
                Console.WriteLine(sortedDict.FirstOrDefault().Key);
                string output = "";
                int count = 0;
                foreach (var pair in sortedDict) //TODO: Modify user data to contain their username, so it doesn't have to make an expensive GetUserAsync call every time.
                {
                    var username = Program.discord.GetUserAsync(pair.Key);
                    if(username == null)
                    {
                        await ctx.CreateResponseAsync("Encountered an non-existent user! What the fuck!?");
                    }
                    count++;
                    output += $"{count}.{username.Result.Username}: ${pair.Value.money}\n";
                }
                await ctx.CreateResponseAsync(output);
            }
            await ctx.CreateResponseAsync("Failure!");
        }

        [SlashCommand("Transfer", "Transfer your money to another user")]
        public async Task EconTransferCommand(InteractionContext ctx, [Option("Amount", "Amount to transfer")] long amount, [Option("User", "User to transfer money to")] DiscordUser du)
        {
            try
            {
                var guildid = ctx.Guild.Id;
                var origGiver = GetEconomyEntry(ctx.User.Id, ctx.Guild.Id);
                var origTarget = GetEconomyEntry(du.Id, ctx.Guild.Id);
                if (origGiver.money < amount)
                {
                    await ctx.CreateResponseAsync($"{ctx.User.Username}, YOU CANNOT AFFORD!!");
                }
                else if (amount == 0)
                {
                    await ctx.CreateResponseAsync($"{ctx.User.Username} transferred.... $0 to {du.Username}. What a waste of time.");
                }
                else if (amount < 0)
                {
                    await ctx.CreateResponseAsync($"Sorry, robbery has not been implemented yet {ctx.User.Username}!");
                }
                else
                {
                    EditEconomyEntry(ctx.User.Id, new UserData { money = origGiver.money - amount, lastDaily = origGiver.lastDaily }, guildid);
                    EditEconomyEntry(du.Id, new UserData { money = origTarget.money + amount, lastDaily = origTarget.lastDaily }, guildid);
                    await ctx.CreateResponseAsync($"{ctx.User.Username} transferred {amount}$ to {du.Username}!");
                }
            }
            catch (Exception e)
            {
                await ctx.CreateResponseAsync($"Error encountered! {e.Message}");
            }
        }

        [SlashCommand("Betflip", "Bet your money on a coin flip!")]
        public async Task BetFlipCommand(InteractionContext ctx, [Option("Amount", "Amount to bet")] long amount, [Option("Choice", "Make your choice....")] BetflipChoice choice = BetflipChoice.heads)
        {
            var entry = GetEconomyEntry(ctx.User.Id, ctx.Guild.Id);
            if (entry == null)
            {
                entry = GetEconomyEntry(ctx.User.Id, ctx.Guild.Id); //Get it again. Chud.
            }
            if(amount < 0)
            {
                await ctx.CreateResponseAsync($"Invalid amount!");
                return;
            }
            if(amount > entry.money)
            {
                await ctx.CreateResponseAsync($"Invalid amount!");
                return;
            }
            var headEmbed = new DiscordEmbedBuilder
            {
                Title = $"Heads!",
                Description = "",
                ImageUrl = "https://cdn.discordapp.com/attachments/977270567881298024/1079241412811423744/domcoinheads.png",
                Footer = new DiscordEmbedBuilder.EmbedFooter { IconUrl = ctx.User.GetAvatarUrl(DSharpPlus.ImageFormat.Png, 256), Text = $"final balance" },
                Color = DiscordColor.Black
            };

            var tailEmbed = new DiscordEmbedBuilder
            {
                Title = $"Tails!",
                Description = "",
                ImageUrl = "https://cdn.discordapp.com/attachments/977270567881298024/1079241395161800854/domcointails.png",
                Footer = new DiscordEmbedBuilder.EmbedFooter { IconUrl = ctx.User.GetAvatarUrl(DSharpPlus.ImageFormat.Png, 256), Text = $"final balance" },
                Color = DiscordColor.Black
            };

            int result = rnd.Next(0, 2); //Could massively reduce the amount of lines below, but I want custom messages dependent on all the outcomes, so COPE.
            if (result == 0) //Heads
            {
                if(choice == BetflipChoice.heads)
                {
                    EditEconomyEntry(ctx.User.Id, new UserData { money = entry.money + amount, lastDaily = entry.lastDaily}, ctx.Guild.Id);
                    headEmbed.Description = $"Yipee! You win ${amount}!";
                    headEmbed.Footer.Text = $"Resulting balance: ${entry.money + amount}";
                    headEmbed.Build();
                    await ctx.CreateResponseAsync(headEmbed);
                }
                else
                {
                    EditEconomyEntry(ctx.User.Id, new UserData { money = entry.money - amount, lastDaily = entry.lastDaily }, ctx.Guild.Id);
                    headEmbed.Description = $"Drat! You lose ${amount}!";
                    headEmbed.Footer.Text = $"Remaining balance: ${entry.money - amount}";
                    headEmbed.Build();
                    await ctx.CreateResponseAsync(headEmbed);
                }
            }
            else if (result == 1) //Tails
            {
                if (choice == BetflipChoice.tails)
                {
                    EditEconomyEntry(ctx.User.Id, new UserData { money = entry.money + amount, lastDaily = entry.lastDaily }, ctx.Guild.Id);
                    tailEmbed.Description = $"Yipee! You win ${amount}";
                    tailEmbed.Footer.Text = $"Resulting balance: ${entry.money + amount}";
                    tailEmbed.Build();
                    await ctx.CreateResponseAsync(tailEmbed);
                }
                else
                {
                    EditEconomyEntry(ctx.User.Id, new UserData { money = entry.money - amount, lastDaily = entry.lastDaily }, ctx.Guild.Id);
                    tailEmbed.Description = $"Drat! You lose ${amount}!";
                    tailEmbed.Footer.Text = $"Remaining balance: ${entry.money - amount}";
                    tailEmbed.Build();
                    await ctx.CreateResponseAsync(tailEmbed);
                }
            }
        }

        [SlashCommand("Wheel", "Spin the wheel of CobFortune™")]
        public async Task WheelCommand(InteractionContext ctx, [Option("Amount", "Amount to bet")] long amount)
        {
            var entry = GetEconomyEntry(ctx.User.Id, ctx.Guild.Id);
            if(entry == null)
            {
                entry = GetEconomyEntry(ctx.User.Id, ctx.Guild.Id); //Get it again. Chud.
            }
            var roll = rnd.Next(0, 7);
            double multiplier = 1;
            double[] multiplierTable = new double[] { -1.4, -0.8, -0.4, 1, 1.4, 1.8, 2.4 };
            if (amount <= 0)
            {
                await ctx.CreateResponseAsync($"Invalid amount! Try again!");
                return;
            } else if(entry.money < amount)
            {
                await ctx.CreateResponseAsync($"YOU CANNOT AFFORD! TRY AGAIN!");
                return;
            }
            multiplier = multiplierTable[roll];
            var money = amount * multiplier - amount;
            EditEconomyEntry(ctx.User.Id, new UserData { money = entry.money + (long)money, lastDaily = entry.lastDaily }, ctx.Guild.Id);
            if(money < 0)
            {
                await ctx.CreateResponseAsync($"{ctx.User.Username} lost ${money} with multiplier {multiplier}!");
            }
            await ctx.CreateResponseAsync($"{ctx.User.Username} gained ${money} with multiplier {multiplier}!");
        }

        [SlashCommand("Daily", "Claim your daily $100!")]
        public async Task DailyCommand(InteractionContext ctx)
        {

            var entry = GetEconomyEntry(ctx.User.Id, ctx.Guild.Id);
            if(entry == null)
            {
                entry = GetEconomyEntry(ctx.User.Id, ctx.Guild.Id); //get it again, chud.
            }
            ulong time = (ulong)(DateTimeOffset.UtcNow.ToUnixTimeSeconds());
            ulong secondsSinceDaily = time - entry.lastDaily;
            if (secondsSinceDaily > 86400)
            {
                EditEconomyEntry(ctx.User.Id, new UserData { money = entry.money + 100, lastDaily = time }, ctx.Guild.Id);
                await ctx.CreateResponseAsync($"Daily claimed!");
            } else
            {
                float remainingSeconds = 86400 - secondsSinceDaily;
                float remainingHours = remainingSeconds / 3600;
                string displayInfo = String.Format("{0:0.0}", remainingHours);
                await ctx.CreateResponseAsync($"Can't claim daily yet! Come back in {displayInfo} hours! coalposter!");
            }
        }
    }

    public class UserData
    {
        public long money { get; set; }
        public ulong lastDaily { get; set; }
    }

    public class EconDatabaseNotFoundException : Exception
    {
        public EconDatabaseNotFoundException()
        {
        }

        public EconDatabaseNotFoundException(string message)
            : base(message)
        {
        }

        public EconDatabaseNotFoundException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}