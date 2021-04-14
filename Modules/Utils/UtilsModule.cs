using BotSandwich.Data;
using BotSandwich.Modules.Utils.Commands;
using Discord.WebSocket;

namespace BotSandwich.Modules.Utils
{
    class UtilsModule : Module
    {
        public override void Load(DiscordSocketClient client)
        {
            InitCommandHandler("u!")
                .WithCommand(new Louder())
                .WithHelp()
                .Register(client);
        }
    }
}
