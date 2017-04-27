using System;
using System.Collections.Generic;
using System.Linq;
using SandwichBot.SandwichBase;
using Newtonsoft.Json;
using System.IO;
using Discord;
using SandwichBot.ChefBase;
using System.Threading.Tasks;

namespace Dopost.SandwichService
{
    public class SandwichService
    {
        public Dictionary<int, Sandwich> activeOrders = new Dictionary<int, Sandwich>();
        public Dictionary<ulong, int> hasAnOrder = new Dictionary<ulong, int>();
        public Dictionary<string, Chef> chefList = new Dictionary<string, Chef>();
        public List<ulong> blacklisted = new List<ulong>();
        public List<ulong> givenFeedback = new List<ulong>();
        public List<int> toBeDelivered = new List<int>();
        public List<Sandwich> cache = new List<Sandwich>();
        public int totalOrders = 0;
        public string version = "2.1";
        public string date = "April 27th 2017, 6:15pm CST";
        public string updatename = "I am under the impression I have fixed people losing the ability to order, with the bot claiming they have an order already made. \n Switched a bunch of things to use Preconditional attributes instead of their previous system. Minor bug fixes";
        public string motd;
        public ulong usrID = 264222431172886529;    //264222431172886529  297910882976006154
        public ulong usrcID = 285529162511286282;   //285529162511286282 298552977504075777
        public ulong usrlogcID = 287990510428225537; //287990510428225537 306909741622362112


        public void Save()
        {
            try
            {
                using (var sw = new StreamWriter(@"data/orders.json", false))
                {
                    JsonSerializer.Create().Serialize(sw, activeOrders);
                    Console.WriteLine("serialized order");
                }
                using (var sw = new StreamWriter(@"data/ordercount.json", false))
                {
                    JsonSerializer.Create().Serialize(sw, totalOrders);
                    Console.WriteLine("serialized order count");
                }
                using (var sw = new StreamWriter(@"data/cache.json", false))
                {
                    JsonSerializer.Create().Serialize(sw, cache);
                    Console.WriteLine("serialized cache");
                }
                using (var sw = new StreamWriter(@"data/blacklisted.json", false))
                {
                    JsonSerializer.Create().Serialize(sw, blacklisted);
                    Console.WriteLine("serialized blacklist");
                }
                using (var sw = new StreamWriter(@"data/givenfeedback.json", false))
                {
                    JsonSerializer.Create().Serialize(sw, givenFeedback);
                    Console.WriteLine("serialized feedback list");
                }
                using (var sw = new StreamWriter(@"data/chef.json", false))
                {
                    JsonSerializer.Create().Serialize(sw, chefList);
                    Console.WriteLine("serialized chef");
                }
            }
            catch (Exception n)
            {
                Console.WriteLine("Failed to save!");
                Console.WriteLine(n);
            }
        }

        public void Load()
        {
            try
            {
                using (var sr = new StreamReader(@"data/orders.json"))
                {
                    var myLovelyReader = new JsonTextReader(sr);
                    activeOrders = JsonSerializer.Create().Deserialize<Dictionary<int, Sandwich>>(myLovelyReader);
                    Console.WriteLine("Deserialized Orders.");
                    Console.WriteLine(activeOrders.Count());
                }
                using (var sr = new StreamReader(@"data/blacklisted.json"))
                {
                    var myLovelyReader = new JsonTextReader(sr);
                    blacklisted = JsonSerializer.Create().Deserialize<List<ulong>>(myLovelyReader);
                    Console.WriteLine("Deserialized Blacklist.");
                    Console.WriteLine(blacklisted.Count());
                }
                using (var sr = new StreamReader(@"data/givenfeedback.json"))
                {
                    var myLovelyReader = new JsonTextReader(sr);
                    givenFeedback = JsonSerializer.Create().Deserialize<List<ulong>>(myLovelyReader);
                    Console.WriteLine("Deserialized GivenFeedback.");
                    Console.WriteLine(blacklisted.Count());
                }
                using (var sr = new StreamReader(@"data/ordercount.json"))
                {
                    var myLovelyReader = new JsonTextReader(sr);
                    totalOrders = JsonSerializer.Create().Deserialize<int>(myLovelyReader);
                    Console.WriteLine("Deserialized number of total orders.");
                    Console.WriteLine(totalOrders);
                }
                using (var sr = new StreamReader(@"data/cache.json"))
                {
                    var myLovelyReader = new JsonTextReader(sr);
                    cache = JsonSerializer.Create().Deserialize<List<Sandwich>>(myLovelyReader);
                    Console.WriteLine("Deserialized order cache.");
                    Console.WriteLine(cache.Count());
                }
                using (var sr = new StreamReader(@"data/chef.json"))
                {
                    var myLovelyReader = new JsonTextReader(sr);
                    chefList = JsonSerializer.Create().Deserialize<Dictionary<string,Chef>>(myLovelyReader);
                    Console.WriteLine("Deserialized chef.");
                    Console.WriteLine(chefList.Count());
                }
                using (var sr = new StreamReader(@"data/motd.json"))
                {
                    var myLovelyReader = new JsonTextReader(sr);
                    motd = JsonSerializer.Create().Deserialize<string>(myLovelyReader);
                    Console.WriteLine("Deserialized motd.");
                    Console.WriteLine(motd);
                }
            }
            catch
            {
                Console.WriteLine("ERROR TERROR MY FRIENDO | BOY LOTS OF SHIT IS GOING WRONG NOW. THE FILE IS BROKE FAM");
            }
        }
    }
   
}
