using Newtonsoft.Json;
using ChefStatusEnums;

namespace SandwichBot.ChefBase
{
    [JsonObject]
    public class Chef
    {
        public ulong ChefId { get; set; } //Id of user who ordered
        public string ChefName { get; set; } //your oder
        public string ChefDistin { get; set; } //self explanatory
        public int ordersAccepted { get; set; } //self explanatory
        public int ordersDelivered { get; set; } //self explanatory
        public ChefStatus status { get; set; } = ChefStatus.Trainee;
        public bool canBlacklist { get; set; } = false;
        public string HiredDate { get; set; }


        [JsonConstructor]
        public Chef(ulong chefid, string cname, string chefd, int ordera, int orderd, string hd, bool cb)
        {
            ChefId = chefid;
            ChefName = cname;
            ChefDistin = chefd;
            ordersAccepted = ordera;
            ordersDelivered = orderd;
            HiredDate = hd;
            canBlacklist = cb;
        }

        public Chef(string d, ulong od, string sid, int cid, int sname, string oa, ChefStatus s)
        {
            this.ChefName = d;
            this.ChefId = od;
            this.ChefDistin = sid;
            this.ordersAccepted = cid;
            this.ordersDelivered = sname;
            this.HiredDate = oa;
            this.status = s;
        }

        
    }
}
