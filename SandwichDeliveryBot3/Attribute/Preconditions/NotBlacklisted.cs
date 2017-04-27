using System;
using System.Threading.Tasks;
using Discord.Commands;
using Discord.WebSocket;
using Dopost.SandwichService;

namespace NotBlacklistedPreCon
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    class NotBlacklisted : PreconditionAttribute
    {
        public override Task<PreconditionResult> CheckPermissions(ICommandContext context, CommandInfo command, IDependencyMap map)
        {
            var user = context.User as SocketGuildUser;
            if (user == null)
                return Task.FromResult(PreconditionResult.FromError("The command was not used in a guild."));

            SandwichService SandwichService = map.Get<SandwichService>();

            if(SandwichService.blacklisted.Contains(context.User.Id))
                return Task.FromResult(PreconditionResult.FromError("Your account is blacklisted from using this bot. Please note this is **not** a server blacklist."));

            if (SandwichService.blacklisted.Contains(context.Guild.Id))
                return Task.FromResult(PreconditionResult.FromError("This server is blacklisted from using this bot."));

            return Task.FromResult(PreconditionResult.FromSuccess());

        }
    }
}
