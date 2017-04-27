using System;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Dopost.SandwichService;
using Discord.Commands;
using SandwichBot.SandwichBase;
using OrderStatusEnums;
using ChefStatusEnums;
using SandwichBot.ChefBase;
using RequireSandwichArtistPrecon;
using RequireBlacklistPrecon;
using inUSRPrecon;
using NotBlacklistedPreCon;

namespace SandwichDeliveryBot3.Modules.Public
{
    public class SandwichModule : ModuleBase
    {
        SandwichService SS;
        public SandwichModule(SandwichService s)
        {
            SS = s;
        }

        [Command("getordercount")]
        public async Task GetOrderCount()
        {
            await ReplyAsync($"We have served `{SS.totalOrders}`");
        }

        [Command("getallorders")]
        [Alias("gao")]
        [NotBlacklisted]
        [inUSR]
        [RequireSandwichArtist]
        public async Task GetAllOrders()
        {
         
            var s = string.Join("` \r\n `", SS.activeOrders.Keys);
            await ReplyAsync($"`{s}`");
        }

        [Command("orderinfo")]
        [Alias("oi")]
        [NotBlacklisted]
        [inUSR]
        [RequireSandwichArtist]
        public async Task OrderInfo(int id)
        {
            

            if (SS.activeOrders.FirstOrDefault(s => s.Value.Id == id).Value != null)
            {
                Sandwich order = SS.activeOrders.FirstOrDefault(s => s.Value.Id == id).Value;
                Color c = new Color(102, 102, 153);
                await ReplyAsync("Here is your requested information!", embed: new EmbedBuilder()
                .AddField(builder =>
                {
                    builder.Name = "Order";
                    builder.Value = order.Desc;
                    builder.IsInline = true;
                })
                .AddField(builder =>
                {
                    builder.Name = "Artist";
                    if (order.OrderChef != null)
                    {
                        builder.Value = order.OrderChef.ChefName;
                    }
                    else
                    {
                        builder.Value = "None";
                    }
                    builder.IsInline = true;
                })
                .AddField(builder =>
                {
                    builder.Name = "Order Id";
                    builder.Value = order.Id;
                    builder.IsInline = true;
                })
                .AddField(builder =>
                {
                    builder.Name = "Order Server";
                    builder.Value = order.GuildName;
                    builder.IsInline = true;
                })
                .AddField(builder =>
                {
                    builder.Name = "Order Date";
                    builder.Value = order.date;
                    builder.IsInline = true;
                })
                .AddField(builder =>
                {
                    builder.Name = "Customer";
                    builder.Value = order.UserName + "#" + order.Discriminator;
                    builder.IsInline = true;
                })
                 .AddField(builder =>
                 {
                     builder.Name = "Order Status";
                     builder.Value = order.Status;
                     builder.IsInline = true;
                 })
                .WithUrl("https://discord.gg/XgeZfE2")
                .WithColor(c)
                .WithThumbnailUrl(order.AvatarUrl)
                .WithTitle("Order information")
                .WithTimestamp(DateTime.Now));



                SS.Save();
            }
        }

        [Command("order")]
        [Alias("o")]
        [Summary("Ogre!")]
        [RequireBotPermission(GuildPermission.CreateInstantInvite)]
        public async Task Order([Remainder]string order)
        {
            if (order != null)
            {
                var i = 0;

                if (SS.hasAnOrder.ContainsKey(Context.User.Id)) { await ReplyAsync($"You already haven an order placed! :angry: "); return; }

                try
                {
                    IGuild usr = await Context.Client.GetGuildAsync(SS.usrID);
                    ITextChannel usrc = await usr.GetTextChannelAsync(SS.usrcID);
                    ITextChannel usrclog = await usr.GetTextChannelAsync(287990510428225537);

                    var r = new Random();
                    i = r.Next(1, 999);

                    while (SS.activeOrders.ContainsKey(i))
                    {
                        i = r.Next(1, 999);
                        await usrclog.SendMessageAsync($"<@131182268021604352> Rerolled order, matching ids. {i}");
                    }

                    var s = Context.Guild;
                    IGuildUser customer = await Context.Guild.GetUserAsync(Context.User.Id);


                    var o = new Sandwich(order, i, DateTime.Now, OrderStatus.Waiting, customer.GetAvatarUrl(), customer.Discriminator, customer.Username, customer.Guild.IconUrl, customer.Guild.Name, customer.Guild.DefaultChannelId, Context.Channel.Id, customer.Id, customer.Guild.Id);

                    SS.activeOrders.Add(i, o);
                    var builder = new EmbedBuilder();
                    builder.ThumbnailUrl = Context.User.GetAvatarUrl();
                    builder.Title = $"New order from {Context.Guild.Name}(`{Context.Guild.Id}`)";
                    var desc = $"Ordered by: **{Context.User.Username}**#**{Context.User.Discriminator}**(`{Context.User.Id}`)\n" +
                       $"Channel: `{Context.Channel.Name}`\n" +
                       $"Id: `{i}`\n" +
                       $"```{order}```";
                    builder.Description = desc;
                    builder.Color = new Color(84, 176, 242);
                    builder.WithFooter(x =>
                    {
                        x.Text = "Is this order abusive? Please tell Lemon or Fires immediately!";
                    });
                    builder.Timestamp = DateTime.Now;


                    SS.hasAnOrder.Add(Context.User.Id, i);
                    SS.totalOrders += 1;
                    await usrc.SendMessageAsync("", embed: builder);


                }
                catch (Exception e)
                {
                    await ReplyAsync("This error should not happen! Contact Fires#1043 immediately!");
                    await ReplyAsync($"```{e}```");
                    return;
                }

                await ReplyAsync($"Your order has been delivered! Thank you for ordering! Please wait while someone accepts your order. :slight_smile: - ID `{i}`");
                SS.Save();
            }
            else { await ReplyAsync("You need to specify what you want idiot! :confused:"); }
        }

        [Command("acceptorder")]
        [Alias("ao")]
        [NotBlacklisted]
        [inUSR]
        [RequireSandwichArtist]
        public async Task AcceptOrder(int id)
        {
            if (id > 0)
            {

                Chef c = SS.chefList.FirstOrDefault(a => a.Value.ChefId == Context.User.Id).Value;
                if (SS.activeOrders.FirstOrDefault(s => s.Value.Id == id).Value != null)
                {
                    Sandwich order = SS.activeOrders.FirstOrDefault(a => a.Value.Id == id).Value;
                    if (SS.toBeDelivered.Contains(order.Id)) { await ReplyAsync("This order is already ready to be delivered! :angry: "); return; }
                    try //TODO: FIX 403 ERROR
                    {
                        await ReplyAsync($"{Context.User.Mention} Sandwich order is now ready for delivery! Please assemble the sandwich, once you are complete. `;deliver {id}` to continue! Type `;orderinfo {id}`(short: `;oi {id}`) if you need more info. :wave: ");
                        IGuild s = await Context.Client.GetGuildAsync(order.GuildId);
                        ITextChannel ch = await s.GetTextChannelAsync(order.ChannelId);
                        IGuildUser u = await s.GetUserAsync(order.UserId);
                        try
                        {
                            IDMChannel dm = await u.CreateDMChannelAsync();
                            var builder = new EmbedBuilder();

                            builder.ThumbnailUrl = Context.User.GetAvatarUrl();
                            builder.Title = $"Your order has been accepted by {Context.User.Username}#{Context.User.Discriminator}!";
                            var desc = $"```{order.Desc}```\n" +
                                       $"Id: `{order.Id}`\n" +
                                       $"**Watch this chat for an updates on when it is on it's way! It is ready for delivery!";
                            builder.Description = desc;
                            builder.Color = new Color(36, 78, 145);
                            builder.Url = "https://discord.gg/XgeZfE2";
                            builder.WithFooter(x =>
                            {
                                x.IconUrl = u.GetAvatarUrl();
                                x.Text = $"Ordered at: {order.date}.";
                            });
                            builder.Timestamp = DateTime.UtcNow;
                            order.Status = OrderStatus.ReadyToDeliver; //like a dirty jew
                            SS.toBeDelivered.Add(order.Id);
                            c.ordersAccepted += 1;
                            await dm.SendMessageAsync("", embed: builder);
                            SS.Save();
                        }
                        catch (NullReferenceException)
                        {
                            await ReplyAsync("Null ref. Did they kick our bot or delete the channel? Try to add the user and ask.");
                            //delete it too???
                        }
                        catch (Exception e)
                        {
                            await ReplyAsync("Error :ghost:");
                            await ReplyAsync($"```{e}```");
                            await ch.SendMessageAsync(u.Mention + " I cannot send dms to you! Please give me the ability to by going to the servers settings in the top left > privacy settings and enabled direct messages from server users. Thank you. If you believe this error was a mistake, please join our server using `;server` and contact Fires#1043.");
                            return;
                        }
                    }
                    catch (Exception d)
                    {
                        await ReplyAsync("SEND ERROR TO Fires#4553 IMMEDIATELY");
                        await ReplyAsync($"```{d}```");
                    }
                    SS.Save(); return;
                }
                else
                {
                    await ReplyAsync("Sorry bud this order doesn't exist!"); return;
                }
            }

        }
            
        

        [Command("deliver")]
        [Alias("d")]
        [NotBlacklisted]
        [inUSR]
        [RequireSandwichArtist]
        public async Task Deliver(int id)
        {
            if (id > 0)
            {
                if (SS.toBeDelivered.Contains(id))
                {

                    if (SS.activeOrders.FirstOrDefault(s => s.Value.Id == id).Value != null)
                    {
                        Chef c = SS.chefList.FirstOrDefault(a => a.Value.ChefId == Context.User.Id).Value;
                        Sandwich order = SS.activeOrders.FirstOrDefault(s => s.Value.Id == id).Value;
                        if (order.Status == OrderStatus.ReadyToDeliver)
                        {
                            try
                            {
                                Console.WriteLine("passed finish");
                                await ReplyAsync("DMing you an invite! Go deliver it! Remember to be nice and ask for `;feedback`");
                                IGuild s = await Context.Client.GetGuildAsync(order.GuildId);
                                ITextChannel ch = await s.GetTextChannelAsync(order.ChannelId);
                                IGuildUser u = await s.GetUserAsync(order.UserId);
                                IDMChannel dm = await u.CreateDMChannelAsync();
                                await dm.SendMessageAsync($"Your sandwich is being delivered soon! Watch out!");
                                IInvite inv = await ch.CreateInviteAsync(0, 1, false, true);
                                IDMChannel artistdm = await Context.User.CreateDMChannelAsync();

                                var builder = new EmbedBuilder();
                                builder.ThumbnailUrl = order.AvatarUrl;
                                builder.Title = $"Your order is being delivered by {Context.User.Username}#{Context.User.Discriminator}!";
                                var desc = $"```{order.Desc}```\n" +
                                           $"**Incoming sandwich! Watch {order.GuildName}!**";
                                builder.Description = desc;
                                builder.Color = new Color(163, 198, 255);
                                builder.WithFooter(x =>
                                {
                                    x.IconUrl = u.GetAvatarUrl();
                                    x.Text = $"Ordered at: {order.date}.";
                                });
                                builder.Timestamp = DateTime.UtcNow;
                                await artistdm.SendMessageAsync(inv.ToString());
                                SS.cache.Add(order);
                                order.Status = OrderStatus.Delivered;
                                SS.toBeDelivered.Remove(order.Id);
                                SS.activeOrders.Remove(order.Id);
                                SS.hasAnOrder.Remove(order.UserId);
                                c.ordersDelivered += 1;
                                //await e.Channel.SendMessage("The Order has been completed and removed from the system. You cannot go back now!");
                                SS.Save();
                            }
                            catch (Exception ex)
                            {
                                await ReplyAsync(":ghost:");
                                await ReplyAsync($"```{ex}```"); return;
                            }
                        }
                        else
                        {
                            await ReplyAsync($"This order is not ready to be delivered just yet! It is currently Order Status {order.Status}"); return;
                        }
                    }
                    else
                    {
                        await ReplyAsync("Invalid order probably (tell Fires its a problem with the checky thingy, the thing thing)"); return;
                    }
                }
                else
                {
                    await ReplyAsync("You are not high enough rank to deliver orders!"); return;
                }
            }
            else
            {
                await ReplyAsync("This order is not ready to be delivered yet! (this error can also occur if you are not using the right id)"); return;
            }

        }
        

        [Command("denyorder")]
        [Alias("do")]
        [NotBlacklisted]
        [inUSR]
        [RequireSandwichArtist]
        public async Task DenyOrder(int id, [Remainder] string reason)
        {

            if (SS.activeOrders.FirstOrDefault(s => s.Value.Id == id).Value != null)
            {
                Sandwich order = SS.activeOrders.FirstOrDefault(a => a.Value.Id == id).Value;
                order.Status = OrderStatus.Delivered;
                SS.activeOrders.Remove(id);
                await ReplyAsync($"Deleted order {order.Id}!");
                IGuild s = await Context.Client.GetGuildAsync(order.GuildId);
                ITextChannel ch = await s.GetTextChannelAsync(order.ChannelId);
                IGuildUser u = await s.GetUserAsync(order.UserId);
                IDMChannel dm = await u.CreateDMChannelAsync();
                SS.hasAnOrder.Remove(order.UserId);
                SS.totalOrders -= 1;
                await dm.SendMessageAsync($"Your sandwich order has been denied! ", embed: new EmbedBuilder()
                    .WithThumbnailUrl(Context.User.GetAvatarUrl())
                    .WithUrl("https://discord.gg/XgeZfE2")
                    .AddField(builder =>
                    {
                        builder.Name = "Order:";
                        builder.Value = order.Desc;
                        builder.IsInline = true;
                    })
                    .AddField(builder =>
                    {
                        builder.Name = "Denied By:";
                        builder.Value = string.Join("#", Context.User.Username, Context.User.Discriminator);
                        builder.IsInline = true;
                    })
                    .AddField(builder =>
                    {
                        builder.Name = "Denied because:";
                        builder.Value = reason;
                        builder.IsInline = true;
                    })
                    .AddField(builder =>
                    {
                        builder.Name = "Ordered at:";
                        builder.Value = order.date;
                        builder.IsInline = true;
                    })
                    .AddField(builder =>
                    {
                        builder.Name = "Server:";
                        builder.Value = order.GuildName;
                        builder.IsInline = true;
                    })
                    .AddField(builder =>
                    {
                        builder.Name = "Order Status:";
                        builder.Value = order.Status;
                        builder.IsInline = true;
                    })
                    .AddField(builder =>
                    {
                        builder.Name = "Order Id:";
                        builder.Value = order.Id;
                        builder.IsInline = true;
                    })
                    .WithCurrentTimestamp()
                    .WithTitle("Denied order:"));
            }
            else
            {
                await ReplyAsync("This order does not exist!"); return;
            }
        }
        

        [Command("delorder")]
        [Alias("delo")]
        [NotBlacklisted]
        public async Task DelOrder()
        {
            try
            {
                Sandwich order = SS.activeOrders.First(s => s.Value.UserId == Context.User.Id).Value;
                IUserMessage msg = await ReplyAsync("Deleting order...");
                SS.activeOrders.Remove(order.Id);
                IGuild usr = await Context.Client.GetGuildAsync(SS.usrID);
                ITextChannel usrc = await usr.GetTextChannelAsync(SS.usrcID);
                await usrc.SendMessageAsync($"Order `{order.Id}`,`{order.Desc}` has been **REMOVED**.");
                SS.Save();
                await msg.ModifyAsync(x =>
                {
                    x.Content = "Successfully deleted order!";
                });
            }
            catch (Exception e)
            {
                await ReplyAsync("Failed to delete. Are you sure you have one?");
            }
        }

        [Command("caniblacklist")]
        [Alias("cib")]
        [NotBlacklisted]
        public async Task CanIBlacklist()
        {
            if (SS.chefList.FirstOrDefault(s => s.Value.ChefId == Context.User.Id).Value != null)
            {
                Chef s = SS.chefList.FirstOrDefault(a => a.Value.ChefId == Context.User.Id).Value;
                if (s.canBlacklist)
                {
                    await ReplyAsync("**Yes.**");
                }
                else
                {
                    await ReplyAsync("**No.**");
                }
            }
            else
            {
                await ReplyAsync("You are not a Sandwich Artist!");
            }
        }

        [Command("feedback")]
        [Alias("f")]
        [NotBlacklisted]
        public async Task Feedback([Remainder]string f)
        {
            if (SS.givenFeedback.Contains(Context.User.Id)) { await ReplyAsync("You've already sent feedback :)"); return; }

            if (f != null)
            {
                try
                {
                    IGuild usr = await Context.Client.GetGuildAsync(SS.usrID);
                    ITextChannel usrc = await usr.GetTextChannelAsync(306941357795311617);

                    var builder = new EmbedBuilder();
                    builder.ThumbnailUrl = Context.User.GetAvatarUrl();
                    builder.Title = $"New feedback from {Context.User.Username}#{Context.User.Discriminator}(`{Context.User.Id}`)";
                    var desc = $"{f}";
                    builder.Description = desc;
                    builder.Color = new Color(242, 255, 5);
                    builder.WithFooter(x =>
                    {
                        x.Text = "Is this feedback abusive? Please tell Lemon, Fires or Beymoezy immediately!";
                    });
                    builder.Timestamp = DateTime.Now;

                    await usrc.SendMessageAsync("", embed: builder);
                    await ReplyAsync("Thank you!");
                    SS.givenFeedback.Add(Context.User.Id);
                    SS.Save();
                }
                catch (Exception e)
                {
                    await ReplyAsync($"Error! {e}");
                }
            }
            else
            {
                await ReplyAsync("Please enter something!");
            }
        }

        [Command("server")]
        [Alias("serv", "s")]
        [NotBlacklisted]
        public async Task servercom()
        {
            if (SS.blacklisted.Contains(Context.User.Id) || SS.blacklisted.Contains(Context.Guild.Id)) { await Context.Channel.SendMessageAsync("You have been blacklisted from this bot. :cry: "); return; }
            await ReplyAsync("Come join our server! Feel free to shitpost, spam and do whatever! https://discord.gg/XgeZfE2");
        }

        [Command("motd")]
        [NotBlacklisted]
        public async Task MOTD()
        {
            await ReplyAsync(SS.motd);
        }

        [Command("blacklist")]
        [Alias("b")]
        [inUSR]
        [RequireBlacklist]
        public async Task Blacklist(ulong id)
        {
            if (SS.chefList.FirstOrDefault(s => s.Value.ChefId == Context.User.Id).Value != null)
            {
                Chef s = SS.chefList.FirstOrDefault(a => a.Value.ChefId == Context.User.Id).Value;
                if (s.canBlacklist)
                {
                    SS.blacklisted.Add(id);
                    await ReplyAsync("Successfully blacklisted! :thumbsup: ");
                    IGuild usr = await Context.Client.GetGuildAsync(SS.usrID);
                    ITextChannel usrc = await usr.GetTextChannelAsync(SS.usrlogcID);
                    await usrc.SendMessageAsync($"{Context.User.Mention} blacklisted id {id}.");
                    SS.Save();
                }
                else
                {
                    await ReplyAsync("You cannot do this!");
                }
            }
            else
            {
                await ReplyAsync("No can do!");
            }
        }

        [Command("blacklistuser")]
        [Alias("bu")]
        [inUSR]
        [RequireBlacklist]
        public async Task BlacklistUser(IGuildUser user)
        {
            if (SS.chefList.FirstOrDefault(s => s.Value.ChefId == Context.User.Id).Value != null)
            {
                Chef s = SS.chefList.FirstOrDefault(a => a.Value.ChefId == Context.User.Id).Value;
                if (s.canBlacklist)
                {
                    SS.blacklisted.Add(user.Id);
                    await ReplyAsync("Successfully blacklisted! :thumbsup: ");
                    IGuild usr = await Context.Client.GetGuildAsync(SS.usrID);
                    ITextChannel usrc = await usr.GetTextChannelAsync(306909741622362112);
                    await usrc.SendMessageAsync($"{Context.User.Mention} blacklisted user {user.Mention}.");
                    SS.Save();
                }
                else
                {
                    await ReplyAsync("You cannot do this!");
                }
            }
            else
            {
                await ReplyAsync("No can do!");
            }
        }

        [Command("removefromblacklist")]
        [Alias("rfb")]
        [inUSR]
        [RequireBlacklist]
        public async Task removeFromBlacklist(ulong id)
        {
            if (SS.chefList.FirstOrDefault(s => s.Value.ChefId == Context.User.Id).Value != null)
            {
                Chef s = SS.chefList.FirstOrDefault(a => a.Value.ChefId == Context.User.Id).Value;
                if (s.canBlacklist)
                {
                    SS.blacklisted.Remove(id);
                    await ReplyAsync("Removed! :thumbsup: ");
                    SS.Save();
                }
                else
                { await ReplyAsync("You cannot do this!"); }
            }
            else
            {
                await ReplyAsync("You are not an Artist!");
            }
        }

        [Command("removeuserfromblacklist")]
        [Alias("rufb")]
        [inUSR]
        [RequireBlacklist]
        public async Task removeUserFromBlacklist(IGuildUser user)
        {
            if (SS.chefList.FirstOrDefault(s => s.Value.ChefId == Context.User.Id).Value != null)
            {
                Chef s = SS.chefList.FirstOrDefault(a => a.Value.ChefId == Context.User.Id).Value;
                if (s.canBlacklist)
                {
                    SS.blacklisted.Remove(user.Id);
                    await ReplyAsync("Removed! :thumbsup: ");
                    SS.Save();
                }
                else
                { await ReplyAsync("You cannot do this!"); }
            }
            else
            {
                await ReplyAsync("You are not an Artist!");
            }
        }

        [Command("totalorders")]
        [Alias("to")]
        [NotBlacklisted]
        public async Task TotalOrders()
        {
            await ReplyAsync($"We have proudly served {SS.totalOrders} sandwiches since April 26th 2017, We currently have {SS.cache.Count} saved.");
        }

        [Command("credits")]
        [Alias("cred")]
        [NotBlacklisted]
        public async Task credits()
        {
            await ReplyAsync($"Special thanks to ``` \r\n Lemon - Extremely helpful and efficient worker, helped establish a lot of the bot \r\n Melon - no, you suck \r\n JeuxJeux20 - Json help, bot wouldn't exist without you. \r\n Bloxri - Assorted C# knowledge \r\n Discord Pizza - Inspiration \r\n Discord Api DiscordNet Channel Members - Helped me get Discord.Net 1.0 set up and working, Love you flam: kissing_heart: ```");
        }

        [Command("help")]
        [Alias("h")]
        public async Task Help()
        {
            await ReplyAsync(@"**__COMMANDS__**
»order
    ; order medium blt with extra lettuce
    Orders something!
»feedback
    ; feedback I didnt get as much extra lettuce as I would have liked, but it was enough. Thanks!
     ; feedback I didn't get anything close to my order! WTF
    Sends feedback back to our server, It it highly reccomended to include the name of your delivery man / women, but please do NOT @ THEM IN THE FEEDBACK
»motd
    ; motd
     Send message that is sent when the bot first 
»total orders
    ; totalorders
     Returns the amount of orders we have done!
»credits
    ; credits
     Returns the credits
»help
    ; help
     THIS
»server
    ; server
     Gets our server!
 ");

        }


    }
}
