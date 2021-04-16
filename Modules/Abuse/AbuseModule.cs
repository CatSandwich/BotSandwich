using System;
using System.Collections.Generic;
using System.Text;
using BotSandwich.Data;
using BotSandwich.Modules.Abuse.Commands;
using Discord.WebSocket;

namespace BotSandwich.Modules.Abuse
{
    class AbuseModule : Module
    {
        public override void Load(DiscordSocketClient client)
        {
            InitCommandHandler("a!")
                .WithCommand(new Admin())
                .WithHelp()
                .Register(client);
        }
    }
}
