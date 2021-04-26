using BotSandwich.Data;
using BotSandwich.Modules.Utils.Commands;
using Discord.WebSocket;

namespace BotSandwich.Modules.TwoOfUs
{
    sealed class TwoOfUsModule : Module
    {
        private AudioHandler _audioHandler;
        protected override string CommandPrefix => "2!";

        public TwoOfUsModule(DiscordSocketClient client) : base(client)
        {
            _audioHandler = new AudioHandler(client);
        }
    }
}
