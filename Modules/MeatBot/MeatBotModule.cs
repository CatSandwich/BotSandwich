using System;
using System.Collections.Generic;
using System.Text;
using BotSandwich.Data;
using Discord.WebSocket;

namespace BotSandwich.Modules.MeatBot
{
    public class MeatBotModule : Module
    {
        protected override string CommandPrefix => "m!";

        public override void Init(DiscordSocketClient client)
        {
            base.Init(client);
            new AudioClips(client);
        }
    }
}
