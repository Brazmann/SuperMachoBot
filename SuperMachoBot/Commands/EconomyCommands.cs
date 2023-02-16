using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using Newtonsoft.Json;

namespace SuperMachoBot.Commands
{
    public class EconomyCommands : ApplicationCommandModule
    {
        public static string jsonPath = "";


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
            string jsonFilePath = @$"{jsonPath}{guildid}.json";

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
            string jsonFilePath = @$"{jsonPath}{guildid}.json";
            var dataDict = new Dictionary<ulong, UserData>();
            dataDict.Add(initialUserID, initialUserData);
            string newJson = JsonConvert.SerializeObject(dataDict, Formatting.Indented);
            File.WriteAllText(jsonFilePath, newJson);
        }

        public UserData GetEconomyEntry(ulong userid, ulong guildid)
        {
            string jsonFilePath = @$"{jsonPath}{guildid}.json";
            // Read the JSON file and deserialize it into a dictionary
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

        public void EditEconomyEntry(ulong userid, UserData data, ulong guildid)
        {
            string jsonFilePath = @$"{jsonPath}{guildid}.json";
            string json = File.ReadAllText(jsonFilePath);
            var userDataDict = JsonConvert.DeserializeObject<Dictionary<ulong, UserData>>(json);

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
                await ctx.CreateResponseAsync($"{du.Username}: {money}$");
            }
            else //TODO: Fix bug which causes the response after new entry creation to not be sent, requiring the user to query again to see their balance.
            {
                await ctx.CreateResponseAsync($"No entry found! Creating new one. Please run this command again.");
                Thread.Sleep(1000);
                var newData = GetEconomyEntry(userid, ctx.Guild.Id);
                var money = newData.money;
                var lastDaily = newData.lastDaily;
                await ctx.CreateResponseAsync($"{du.Username}#{du.Discriminator}:{money}$ Last claimed daily:(Unix){lastDaily}");
            }
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
                    await ctx.CreateResponseAsync($"{ctx.User.Username} transferred.... 0$ to {du.Username}. What a waste of time.");
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
            Random rnd = new Random();
            var entry = GetEconomyEntry(ctx.User.Id, ctx.Guild.Id);
            if (entry == null)
            {
                entry = GetEconomyEntry(ctx.User.Id, ctx.Guild.Id); //Get it again. Chud.
            }
            if(amount < 0)
            {
                await ctx.CreateResponseAsync($"Invalid amount!");
            }
            if(amount > entry.money)
            {
                await ctx.CreateResponseAsync($"Invalid amount!");
            }

            int result = rnd.Next(0, 2); //Could massively reduce the amount of lines below, but I want custom messages dependent on all the outcomes, so COPE.
            if (result == 0) //Heads
            {
                if(choice == BetflipChoice.heads)
                {
                    EditEconomyEntry(ctx.User.Id, new UserData { money = entry.money + amount, lastDaily = entry.lastDaily}, ctx.Guild.Id);
                    await ctx.CreateResponseAsync($"Heads! You win ${amount}!");
                }
                else
                {
                    EditEconomyEntry(ctx.User.Id, new UserData { money = entry.money - amount, lastDaily = entry.lastDaily }, ctx.Guild.Id);
                    await ctx.CreateResponseAsync($"Drat, heads! You lose ${amount}!");
                }
            }
            else if (result == 1) //Tails
            {
                if (choice == BetflipChoice.tails)
                {
                    EditEconomyEntry(ctx.User.Id, new UserData { money = entry.money + amount, lastDaily = entry.lastDaily }, ctx.Guild.Id);
                    await ctx.CreateResponseAsync($"Tails! You win ${amount}!");
                }
                else
                {
                    EditEconomyEntry(ctx.User.Id, new UserData { money = entry.money - amount, lastDaily = entry.lastDaily }, ctx.Guild.Id);
                    await ctx.CreateResponseAsync($"Drat, tails! You lose ${amount}!");
                }
            }
        }

        [SlashCommand("Wheel", "Spin the wheel of CobFortune™")]
        public async Task WheelCommand(InteractionContext ctx, [Option("Amount", "Amount to bet")] long amount)
        {
            Random rnd = new Random();
            var entry = GetEconomyEntry(ctx.User.Id, ctx.Guild.Id);
            if(entry == null)
            {
                entry = GetEconomyEntry(ctx.User.Id, ctx.Guild.Id); //Get it again. Chud.
            }
            var roll = rnd.Next(0, 7);
            double multiplier = 1;
            if (amount <= 0)
            {
                await ctx.CreateResponseAsync($"Invalid amount! Try again!");
            } else if(entry.money < amount)
            {
                await ctx.CreateResponseAsync($"YOU CANNOT AFFORD! TRY AGAIN!");
            }
            switch (roll)
            {
                case 0:
                    multiplier = -1.4;
                    break;
                case 1:
                    multiplier = -0.8;
                    break;
                case 2:
                    multiplier = -0.4;
                    break;
                case 3:
                    multiplier = 1;
                    break;
                case 4:
                    multiplier = 1.4;
                    break;
                case 5:
                    multiplier = 1.8;
                    break;
                case 6:
                    multiplier = 2.4;
                    break;
            }
            var money = amount * multiplier - amount;
            EditEconomyEntry(ctx.User.Id, new UserData { money = entry.money + (long)money, lastDaily = entry.lastDaily }, ctx.Guild.Id);
            if(money < 0)
            {
                await ctx.CreateResponseAsync($"{ctx.User.Username} lost {money}$!");
            }
            await ctx.CreateResponseAsync($"{ctx.User.Username} gained {money}!");
        }

        [SlashCommand("Daily", "Claim your daily 100$!")]
        public async Task DailyCommand(InteractionContext ctx, [Option("Amount", "Amount to bet")] long amount)
        {

            var entry = GetEconomyEntry(ctx.User.Id, ctx.Guild.Id);
            ulong time = (ulong)(DateTimeOffset.UtcNow.ToUnixTimeSeconds());
            if (time - entry.lastDaily > 86400)
            {
                EditEconomyEntry(ctx.User.Id, new UserData { money = entry.money + 100, lastDaily = time }, ctx.Guild.Id);
                await ctx.CreateResponseAsync($"Daily claimed!");
            }
            await ctx.CreateResponseAsync($"Can't claim daily yet! Come back tomorrow, coalposter!");
        }

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