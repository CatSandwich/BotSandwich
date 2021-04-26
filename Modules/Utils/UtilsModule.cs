using BotSandwich.Data;
using BotSandwich.Modules.Utils.Commands;
using Discord.WebSocket;

namespace BotSandwich.Modules.Utils
{
    sealed class UtilsModule : Module
    {
        protected override string CommandPrefix => "u!";
        public UtilsModule(DiscordSocketClient client) : base(client)
        {
            
        }

    }
}
