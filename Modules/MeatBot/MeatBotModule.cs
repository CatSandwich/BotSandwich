using System;
using System.Collections.Generic;
using System.Text;
using BotSandwich.Data;
using Discord.WebSocket;

namespace BotSandwich.Modules.MeatBot
{
    class MeatBotModule : Module
    {
        private AudioClips _audioClips;
        public override void Load(DiscordSocketClient client)
        {
            _audioClips = new AudioClips(client);
        }
    }
}
