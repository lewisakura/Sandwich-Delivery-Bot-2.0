using System;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Dopost.SandwichService;
using Discord.Commands;
using SandwichBot.ChefBase;
using ChefStatusEnums;
using RequireBlacklistPrecon;
using inUSRPrecon;
using NotBlacklistedPreCon;

namespace ChefModule
{
    [Group("artist")]
    public class ChefModule : ModuleBase
    {

        SandwichService SS;
        public ChefModule(SandwichService s)
        {
            SS = s;
        }

        [Command("add")]
        [Alias("a")]
        [NotBlacklisted]
        [inUSR]
        [RequireBlacklist]
        public async Task AddChef(IGuildUser chef)
        {

            if (SS.chefList.FirstOrDefault(a => a.Value.ChefId == Context.User.Id).Value != null)
            {
                Chef s = SS.chefList.FirstOrDefault(a => a.Value.ChefId == Context.User.Id).Value;
                if (s.canBlacklist)
                {
                    string n = string.Format(chef.Username + "#" + chef.Discriminator);
                    if (!SS.chefList.ContainsKey(n))
                    {
                        Chef c = new Chef(n, chef.Id, chef.Discriminator, 0, 0, DateTime.Now.ToString("MMMM dd, yyyy"), ChefStatus.Trainee);
                        SS.chefList.Add(n, c);
                        await ReplyAsync($"<@{chef.Id}> had been added as a Trainee Sandwich Artist!");
                        SS.Save();
                    }
                    else
                    {
                        await ReplyAsync("An entry for this user already exists!");
                    }
                }
            }
        }

        [Command("del")]
        [Alias("d")]
        [NotBlacklisted]
        [inUSR]
        [RequireBlacklist]
        public async Task DeleteChef(IGuildUser chef)
        {
            if (SS.chefList.FirstOrDefault(a => a.Value.ChefId == Context.User.Id).Value != null)
            {
                Chef s = SS.chefList.FirstOrDefault(a => a.Value.ChefId == Context.User.Id).Value;
                if (s.canBlacklist)
                {
                    string n = string.Format(chef.Username + "#" + chef.Discriminator);
                    if (!SS.chefList.ContainsKey(n))
                    {
                        await ReplyAsync("An entry for this user doesn't exist!");
                    }
                    else
                    {
                        SS.chefList.Remove(n);
                        await ReplyAsync($"{n} has been removed!");
                        SS.Save();
                    }
                }
            }
        }

        [Command("listdebug")]
        [NotBlacklisted]
        [inUSR]
        [RequireBlacklist]
        [Alias("l")]
        public async Task ListChefs()
        {
            foreach (var obj in SS.chefList)
            {
                
                var c = obj.Value;
                var col = new Color(36, 78, 145);
                DateTimeOffset parseddate;
                if (DateTimeOffset.TryParse(c.HiredDate, out parseddate))
                    Console.WriteLine("Good to go!");
                else
                    parseddate = DateTime.Now;


                await ReplyAsync("Here is your requested information!", embed: new EmbedBuilder()
                 .AddField(builder =>
                 {
                     builder.Name = "Name";
                     builder.Value = c.ChefName;
                     builder.IsInline = true;
                 })
                 .AddField(builder =>
                 {
                     builder.Name = "Orders Accepted";
                     builder.Value = c.ordersAccepted;
                     builder.IsInline = true;
                 })
                 .AddField(builder =>
                 {
                     builder.Name = "Orders Delivered";
                     builder.Value = c.ordersDelivered;
                     builder.IsInline = true;
                 })
                 .AddField(builder =>
                 {
                     builder.Name = "Rank";
                     builder.Value = c.status;
                     builder.IsInline = true;
                 })
                 .AddField(builder =>
                 {
                     builder.Name = "CanBlacklist?";
                     builder.Value = c.canBlacklist;
                     builder.IsInline = true;
                 })
                 .WithUrl("https://discord.gg/XgeZfE2")
                 .WithColor(col)
                 .WithThumbnailUrl(Context.User.GetAvatarUrl())
                 .WithTitle("Chef info.")
                 .WithTimestamp(parseddate));
            }
        }

        [Command("canblacklist")]
        [NotBlacklisted]
        [Alias("cb")]
        [inUSR]
        [RequireBlacklist]
        public async Task CanBlacklist(IGuildUser user)
        {
            ulong n = user.Id;
            if (SS.chefList.FirstOrDefault(a => a.Value.ChefId == Context.User.Id).Value != null)
            {
                if (SS.chefList.FirstOrDefault(s => s.Value.ChefId == n).Value != null)
                {
                    Chef c = SS.chefList.FirstOrDefault(a => a.Value.ChefId == n).Value;
                    Chef s = SS.chefList.FirstOrDefault(a => a.Value.ChefId == Context.User.Id).Value;
                    if (s.canBlacklist)
                    {
                        c.canBlacklist = true;
                        SS.Save();
                    }
                }
                else
                {
                    await ReplyAsync("No can do, not a real person.");
                }
            }
        }

        [Command("promote")]
        [NotBlacklisted]
        [Alias("p")]
        [inUSR]
        [RequireBlacklist]
        public async Task PromoteArtist(IGuildUser chef)
        {
            ulong n = chef.Id;
            if (SS.chefList.FirstOrDefault(a => a.Value.ChefId == Context.User.Id).Value != null)
            {
                if (SS.chefList.FirstOrDefault(s => s.Value.ChefId == n).Value != null)
                {
                    Chef c = SS.chefList.FirstOrDefault(a => a.Value.ChefId == n).Value;
                    Chef s = SS.chefList.FirstOrDefault(a => a.Value.ChefId == Context.User.Id).Value;
                    if (s.canBlacklist)
                    {
                        switch (c.status)
                        {
                            case ChefStatus.Trainee:
                                c.status = ChefStatus.Artist;
                                await ReplyAsync($"Promoted {chef.Username}#{chef.Discriminator} from Trainee to Sandwich Artist");
                                break;
                            case ChefStatus.Artist:
                                c.status = ChefStatus.MasterArtist;
                                await ReplyAsync($"Promoted {chef.Username}#{chef.Discriminator} from Artist to Master Sandwich Artist");
                                break;
                            case ChefStatus.MasterArtist:
                                c.status = ChefStatus.GodArtist;
                                await ReplyAsync($"Promoted {chef.Username}#{chef.Discriminator} from Master Sandwich Artist to **GOD** Sandwich Artist");
                                break;
                            case ChefStatus.GodArtist:
                                await ReplyAsync("You cannot promote a user past God Sandwich Artist!");
                                break;
                        }
                    }
                    SS.Save();
                }
            }
        }

        [Command("count")]
        [NotBlacklisted]
        [Alias("c")]
        public async Task ChefCount()
        {
            await ReplyAsync($"There are currently {SS.chefList.Count} Sandwich Artists in the database.");
        }

        [Command("stats")]
        [Alias("d")]
        [NotBlacklisted]
        [inUSR]
        [RequireBlacklist]
        public async Task GetDeliveries(IGuildUser chef)
        {
            ulong n = chef.Id;
            if (SS.chefList.FirstOrDefault(s => s.Value.ChefId == n).Value != null)
            {
                Chef c = SS.chefList.FirstOrDefault(a => a.Value.ChefId == n).Value;
                await ReplyAsync($"{chef.Mention} has accepted `{c.ordersAccepted}` orders and delivered `{c.ordersDelivered}`. They have been working here since `{c.HiredDate}` and have the `{c.status}` rank. Their blacklist ability is set to {c.canBlacklist}.");

            }
            else
            {
                await ReplyAsync("Failed linq pass.");
            }
        }
        
        [Command("list")]
        [NotBlacklisted]
        public async Task listImproved()
        {
            var s = string.Join("` \r\n `", SS.chefList.Keys);
            await ReplyAsync(s);
        }

        [Command("testattribute")]
        [NotBlacklisted]
        [inUSR]
        [RequireBlacklist]
        public async Task testattrivute()
        {
            await ReplyAsync("it works");
        }

    }
}
