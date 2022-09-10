using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using Newtonsoft.Json.Linq;

namespace SuperMachoBot.Commands
{
    public class GeneralCommands : BaseCommandModule
    {
        public bool playMode = false;

        public static Random random = new Random();
        [Description("Sends a wholesome personalized greeting.")]
        [Command("greet")]
        public async Task GreetCommand(CommandContext ctx, [RemainingText] string name)
        {
            await ctx.RespondAsync($"Greetings, {name}! test");
        }
        [Description("Generate number between a specified minimum and maximum.")]
        [Command("random")]
        public async Task RandomCommand(CommandContext ctx, int min, int max)
        {
            var random = new Random();
            await ctx.RespondAsync($"Your number is: {random.Next(min, max)}");
        }
        [Description("Ping specified user after a specified amount of time with a specified message.... specifically.")]
        [Command("reminder")]
        public async Task WaitCommand(CommandContext ctx, [Description("How long to wait")] float waittime, [Description("Unit of time")] string unit, [Description("User to ping")] DiscordUser user, [Description("Reminder message")][RemainingText] string remindmessage)
        {
            float delay = 10; //have to assign a dummy number at first because compiler is stupid
            bool unitValid = true;
            switch (unit)
            {
                case "seconds":
                case "second":
                    delay = 1000 * waittime;
                    break;
                case "minutes":
                case "minute":
                    delay = 60000 * waittime;
                    break;
                case "hours":
                case "hour":
                    delay = 3600000 * waittime;
                    break;
                default:
                    await ctx.RespondAsync("THATS NOT A VALID UNIT OF TIME!! https://tenor.com/view/soyjak-gif-23540164");
                    unitValid = false;
                    break;

            }
            if (unitValid == true)
            {
                int sleepDelay = (int)delay;
                await ctx.RespondAsync($"Will ping in {waittime} {unit}");
                Thread.Sleep(sleepDelay);
                await ctx.RespondAsync($"{remindmessage}{user.Mention}");
            }
        }
        [Description("Rolls dice with modifier one at a time")]
        [Command("sroll")]
        public async Task DebugCommand(CommandContext ctx, string dice, float modifier = 0)
        {
            var splitdice = dice.Split('d');

            if (Int32.TryParse(splitdice[0], out int amount))
            {

            }

            for (int i = 0; i < amount; i++)
            {
                Random rnd = new Random();
                if (Int32.TryParse(splitdice[1], out int sides))
                {
                    float diceroll = rnd.Next(1, sides + 1);
                    await ctx.RespondAsync($"rolled {diceroll} + {modifier} = {diceroll + modifier}");
                }
                else
                {
                    await ctx.RespondAsync("Invalid dice!");
                }
            }
        }
        Random rnd = new Random();
        [Description("Rolls dice the specified amount of times, then adds the modifier onto the total")]
        [Command("mroll")]
        public async Task MultipleRollCommand(CommandContext ctx, string dice, float modifier = 0)
        {
            //seperate the letter 'd' from dice
            var splitdice = dice.Split('d');
            //check if the first part is a number
            if (Int32.TryParse(splitdice[0], out int amount))
            {
                //check if the second part is a number
                if (Int32.TryParse(splitdice[1], out int sides))
                {
                    //roll the dice
                    float total = 0;
                    for (int i = 0; i < amount; i++)
                    {
                        float diceroll = rnd.Next(1, sides + 1);
                        total += diceroll;
                    }
                    //add the modifier
                    total += modifier;
                    //send the result
                    await ctx.RespondAsync($"{total}");
                }
                else
                {
                    await ctx.RespondAsync("Invalid dice!");
                }
            }
            else
            {
                await ctx.RespondAsync("Invalid dice!");
            }
        }
        [Description("Rolls character stats for d&d 5e using the 4d6k3 calculation")]
        [Command("statroller")]
        public async Task StatRollerCommand(CommandContext ctx)
        {
            int[] stats = { StatRoller(), StatRoller(), StatRoller(), StatRoller(), StatRoller(), StatRoller() };
            await ctx.RespondAsync($"{stats[0]}\n{stats[1]}\n{stats[2]}\n{stats[3]}\n{stats[4]}\n{stats[5]}");
        }

        public int StatRoller()
        {
            Console.WriteLine("Stat is being rolled!");
            int[] rolls = { rnd.Next(1, 6), rnd.Next(1, 6), rnd.Next(1, 6), rnd.Next(1, 6) };
            int result = rolls[0] + rolls[1] + rolls[2] + rolls[3] - rolls.Min();
            return result;
        }

        [Command("betflip")]

        public async Task BetflipCommand(CommandContext ctx, string choice)
        {
            int flip = rnd.Next(1, 3); // 1 = heads, 2 = tails

            if (choice.ToLower() == "heads")
            {
                switch (flip)
                {
                    case 1:
                        await ctx.RespondAsync(embed: new DiscordEmbedBuilder { Title = "Heads! You win!", ImageUrl = "https://cdn.discordapp.com/attachments/897548763021864970/972648591107698698/domcoinheads.png" }.Build());
                        break;
                    case 2:
                        await ctx.RespondAsync(embed: new DiscordEmbedBuilder { Title = "Tails! You lose!", ImageUrl = "https://cdn.discordapp.com/attachments/897548763021864970/972648580185718834/domcointails.png" }.Build());
                        break;
                }
            }
            if (choice.ToLower() == "tails")
            {
                switch (flip)
                {
                    case 1:
                        await ctx.RespondAsync("Heads! You lose!");
                        await ctx.RespondAsync(embed: new DiscordEmbedBuilder { Title = "Heads! You lose!", ImageUrl = "https://cdn.discordapp.com/attachments/897548763021864970/972648591107698698/domcoinheads.png" }.Build());
                        break;
                    case 2:
                        await ctx.RespondAsync(embed: new DiscordEmbedBuilder { Title = "Tails! You win!", ImageUrl = "https://cdn.discordapp.com/attachments/897548763021864970/972648580185718834/domcointails.png" }.Build());
                        break;
                }
            }
        }
        [Hidden]
        [Command("setactivity")]
        private async Task SetActivityCommand(CommandContext ctx)
        {
            ulong channel = ctx.Channel.Id;
            ulong messageid = ctx.Message.Id;
            ulong ownerid = Program.configItems[0].OwnerID;
            if (ctx.User.Id == ownerid) //Set this to use the user ID of owner in config file.
            {
                DiscordActivity activity = new DiscordActivity();
                DiscordClient discord = ctx.Client;
                string input = Console.ReadLine();
                activity.Name = input;
                await discord.UpdateStatusAsync(activity);
                return;
            }
            else
            {
                Console.WriteLine($"Secret setactivity command run by {ctx.User} in https://discord.com/channels/{ctx.Channel.GuildId}/{channel}/{messageid}!");
            }
        }
    }
}