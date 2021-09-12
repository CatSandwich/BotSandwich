using BotSandwich.Data;
using Discord.WebSocket;

namespace BotSandwich.Modules.TwoOfUs
{
    sealed class TwoOfUsModule : Module
    {
        private AudioHandler _audioHandler;
        protected override string CommandPrefix => "2!";

        public override void Init(DiscordSocketClient client)
        {
            base.Init(client);
            _audioHandler = new AudioHandler(client);
        }
    }
}
