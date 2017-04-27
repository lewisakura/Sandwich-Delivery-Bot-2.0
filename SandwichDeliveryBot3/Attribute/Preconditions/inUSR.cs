using System;
using System.Threading.Tasks;
using Discord.Commands;
using Discord.WebSocket;
using Dopost.SandwichService;

namespace inUSRPrecon
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    class inUSR : PreconditionAttribute
    {
        public override Task<PreconditionResult> CheckPermissions(ICommandContext context, CommandInfo command, IDependencyMap map)
        {
            var user = context.User as SocketGuildUser;
            if (user == null)
                return Task.FromResult(PreconditionResult.FromError("The command was not used in a guild."));

            SandwichService SandwichService = map.Get<SandwichService>();

            if (context.Guild.Id == SandwichService.usrID)
                return Task.FromResult(PreconditionResult.FromSuccess());
            else
                return Task.FromResult(PreconditionResult.FromError("This command cannot be ran outside of our `;server`."));

        }
    }
}
