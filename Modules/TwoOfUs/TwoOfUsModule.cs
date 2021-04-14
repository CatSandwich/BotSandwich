using BotSandwich.Data;
using BotSandwich.Modules.Utils.Commands;
using Discord.WebSocket;

namespace BotSandwich.Modules.TwoOfUs
{
    class TwoOfUsModule : Module
    {
        private AudioHandler _audioHandler;
        public override void Load(DiscordSocketClient client)
        {
            _audioHandler = new AudioHandler(client);
        }
    }
}
