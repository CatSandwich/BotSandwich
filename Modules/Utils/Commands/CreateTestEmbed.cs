using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using BotSandwich.Data;
using BotSandwich.Data.Commands;
using BotSandwich.Modules.Utils.Embeds;
using Discord.WebSocket;

namespace BotSandwich.Modules.Utils.Commands
{
    class CreateTestEmbed : Command
    {
        public override string Name => "createembed";
        public override async Task Run(Module module, SocketMessage sm, string msg)
        {
            await module.InputHandler.Create(new TestEmbed(), sm.Channel);
        }
    }
}
