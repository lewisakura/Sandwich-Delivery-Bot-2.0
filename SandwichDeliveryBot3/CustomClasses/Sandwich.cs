using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using OrderStatusEnums;
using SandwichBot.ChefBase;
using Discord.Net;
using Discord;
using Discord.Commands;

namespace SandwichBot.SandwichBase
{

    [JsonObject]
    public class Sandwich
    {
        public int Id { get; set; }
        public string Desc { get; set; } //your oder
        public DateTime date { get; set; }
        public OrderStatus Status { get; set; }
        public Chef OrderChef { get; set; }
        public string AvatarUrl { get; set; }
        public string Discriminator { get; set; }
        public string UserName { get; set; }
        public string GuildIcon { get; set; }
        public string GuildName { get; set; }
        public ulong GuildDefaultChannelId { get; set; }
        public ulong ChannelId { get; set; }
        public ulong UserId { get; set; }
        public ulong GuildId { get; set; }

        [JsonConstructor]
        public Sandwich( int id, string desc, DateTime ordertime, OrderStatus status, Chef chef, string aurl, string disc, string usernam, string gicon, string gname, ulong gdcid, ulong cid, ulong uid, ulong gid)
        {
            Id = id;
            Desc = desc;
            date = ordertime;
            Status = status;
            OrderChef = chef;
            AvatarUrl = aurl;
            Discriminator = disc;
            UserName = usernam;
            GuildIcon = gicon;
            GuildName = gname;
            GuildDefaultChannelId = gdcid;
            ChannelId = cid;
            UserId = uid;
            GuildId = gid;
        }

        /// <summary>
        /// Create a sandwich
        /// </summary>
        /// <param name="d">THE order</param>
        /// <param name="inf">Information Class</param>
        /// <param name="id">Random ID</param>
        /// <param name="dat">current date</param>
        /// <param name="s">Order STatus (default something something)</param>
        public Sandwich(string d, int id, DateTime dat, OrderStatus s, string aurl, string disc, string usernam, string gicon, string gname, ulong gdcid, ulong cid, ulong uid, ulong gid)
        {
            this.Desc = d; //setting the variables at the top to what we give the sandwich originally
            this.Id = id;
            this.date = dat;
            this.Status = s;
            this.AvatarUrl = aurl;
            this.Discriminator = disc;
            this.UserName = usernam;
            this.GuildIcon = gicon;
            this.GuildName = gname;
            this.GuildDefaultChannelId = gdcid;
            this.ChannelId = cid;
            this.UserId = uid;
            this.GuildId = gid;
        }
    }
}
