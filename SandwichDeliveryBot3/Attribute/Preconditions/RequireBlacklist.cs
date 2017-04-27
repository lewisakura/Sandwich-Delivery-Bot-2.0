using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using System.Linq;
using Discord.WebSocket;
using Dopost.SandwichService;
using SandwichBot.ChefBase;

namespace RequireBlacklistPrecon
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    class RequireBlacklist : PreconditionAttribute
    {

        public override Task<PreconditionResult> CheckPermissions(ICommandContext context, CommandInfo command, IDependencyMap map)
        {

            SandwichService SandwichService = map.Get<SandwichService>();


            var user = context.User as SocketGuildUser;

            if (user == null)
                return Task.FromResult(PreconditionResult.FromError("The command was not used in a guild."));

            string[] roleNames = { "god sandwich artist", "admin", "senate" };

            var matchingRoles = context.Guild.Roles.Where(role => roleNames.Any(name => name == role.Name.ToLower()));

            if (matchingRoles == null)
                return Task.FromResult(PreconditionResult.FromError("There are no matching roles on the server."));

            if (user.Roles.Any(role => matchingRoles.Contains(role)))
            {
                if (SandwichService.chefList.Any(c => user.Id == c.Value.ChefId))
                {
                    Chef c = SandwichService.chefList.FirstOrDefault(ch => user.Id == ch.Value.ChefId).Value;
                    if (c.canBlacklist)
                        return Task.FromResult(PreconditionResult.FromSuccess());
                    else
                        return Task.FromResult(PreconditionResult.FromError("You do not have the ability to blacklist, which is required for this command to run."));


                }
                else
                {
                    return Task.FromResult(PreconditionResult.FromError("You are not registered as a Sandwich Artist!"));
                }
            }

            return Task.FromResult(PreconditionResult.FromError("User does not have sandwich roles."));


        }
    }
}
