using System;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Discord.Commands;
using Dopost.SandwichService;
using System.Linq;
using OrderStatusEnums;

namespace DiscordExampleBot
{
    public class Program
    {
        // Convert our sync main to an async main.
        public static void Main(string[] args) =>
            new Program().Start().GetAwaiter().GetResult();

        private DiscordSocketClient client;
        private CommandHandler handler;
        private SandwichService ss;
        private ulong usrID = 264222431172886529;    //264222431172886529 264222431172886529
        private ulong usrlogcID = 287990510428225537; //287990510428225537 306909741622362112


        public async Task Start()
        {
            // Define the DiscordSocketClient
            client = new DiscordSocketClient();

            var token = "Mshitlol"

            client.Log += async (message) =>
            {
                await Log(message);
            };

            client.JoinedGuild += async (m) =>
            {
                IGuild usr =  client.GetGuild(usrID);
                ITextChannel usrc = await usr.GetTextChannelAsync(usrlogcID);
                await usrc.SendMessageAsync($":slight_smile: I have joined `{m.Name}`(`{m.Id}`), I am now in {client.Guilds.Count} servers!");
                await m.DefaultChannel.SendMessageAsync("Hi I am **Sandwich Delivery Bot**.Thank you for choosing our bot! To order a Sandwich, type `;order extra large blt`. For the list of commands, we have `;help`. If you wish to contact us, please use `; server`. \r\n **Please understand that when you order, you will recieve what you ordered from a *****real*****person. They have feelings.**");
                if (!m.CurrentUser.GuildPermissions.CreateInstantInvite)
                {
                    await m.DefaultChannel.SendMessageAsync(":warning: The bot cannot create instant invites! Please give the bot permission or else it will not work! :warning: ");
                }
            };

            client.LeftGuild += async (m) =>
            {
                IGuild usr = client.GetGuild(usrID);
                ITextChannel usrc = await usr.GetTextChannelAsync(usrlogcID);
                await usrc.SendMessageAsync($":slight_frown: I have left `{m.Name}`(`{m.Id}`), I am now in {client.Guilds.Count} servers...");

            };

            client.MessageReceived += async (m) =>
            {
                if (m.Author.IsBot) return;
                if (m.Channel.Id == client.CurrentUser.Id) return;
                if (m.MentionedUsers.Any(u => u.Id == client.CurrentUser.Id))
                {
                    if (m.Content.Contains("help"))
                    {
                        await m.Channel.SendMessageAsync("Type `;help`. `;server` if you have any problems.");
                    }
                }
                var rnd = new Random();
                var r = rnd.Next(1, 25);
                if (r == 2)
                {
                    var totalChannels = 0;
                    var totalUsers = 0;
                    foreach (var obj in client.Guilds)
                    {
                        totalChannels = totalChannels + obj.Channels.Count;
                        totalUsers = totalUsers + obj.Users.Count;
                    }
                    await client.SetGameAsync($"In {totalChannels} channels with a total of {totalUsers} users! | Type ;motd for help!");
                }
                if (r == 6)
                {
                    await client.SetGameAsync($"In {client.Guilds.Count()} servers! | Type ;motd for help!");
                }

            };

            

            ss = new SandwichService();
            ss.Load();
            foreach (var obj in ss.activeOrders)
            {
                if (obj.Value.Status == OrderStatus.ReadyToDeliver)
                {
                    ss.toBeDelivered.Add(obj.Value.Id);
                    Console.WriteLine($"Added order {obj.Value.Id} to toBeDelivered.");
                }
            }
            // Login and connect to Discord.
            var map = new DependencyMap();
            map.Add(client);
            handler = new CommandHandler();
            map.Add(ss);
            await handler.Install(map);
            await client.LoginAsync(TokenType.Bot, token);
            await client.StartAsync();

            // Block this program until it is closed.
            await Task.Delay(-1);
        }

        private Task Log(LogMessage msg)
        {
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }

    }

}
