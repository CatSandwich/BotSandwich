using System;
using System.Collections.Generic;
using System.Text;
using BotSandwich.Data;
using BotSandwich.Modules.Abuse.Commands;
using Discord.WebSocket;

namespace BotSandwich.Modules.Abuse
{
    sealed class AbuseModule : Module
    {
        protected override string CommandPrefix => "a!";

        public AbuseModule(DiscordSocketClient client) : base(client)
        {
            
        }
    }
}
