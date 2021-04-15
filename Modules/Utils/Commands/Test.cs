using System;
using System.Threading.Tasks;
using BotSandwich.Data;
using Discord.WebSocket;

namespace BotSandwich.Modules.Utils.Commands
{
    class Test : Command
    {
        public override string Name => "test";

        [Argument(true, "1")]
        public string A1;

        [Argument(true, "2")]
        public string A2;

        public override async Task Run(SocketMessage sm, string remainder)
        {
            Console.WriteLine($"A1: {A1}");
            Console.WriteLine($"A2: {A2}");
        }
    }
}
