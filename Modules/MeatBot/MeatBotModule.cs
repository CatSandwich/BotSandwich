using System;
using System.Collections.Generic;
using System.Text;
using BotSandwich.Data;
using Discord.WebSocket;

namespace BotSandwich.Modules.MeatBot
{
    sealed class MeatBotModule : Module
    {
        protected override string CommandPrefix => "m!";
        private AudioClips _audioClips;

        public MeatBotModule(DiscordSocketClient client) : base(client)
        {
            _audioClips = new AudioClips(client);
        }
    }
}
