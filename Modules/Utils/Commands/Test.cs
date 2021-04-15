using System;
using System.Threading.Tasks;
using BotSandwich.Data;
using Discord.WebSocket;

namespace BotSandwich.Modules.Utils.Commands
{
    class Test : Command
    {
        public override string Name => "test";
        public override string Description => "A command used in testing.";
        public override string[] Examples => new[] {""};

        [Argument(true, "1")]
        [ArgumentDescription("The first argument")]
        public string A1;

        [Argument(true, "2")]
        [ArgumentDescription("The second argument")]
        public string A2;

        public override async Task Run(SocketMessage sm, string remainder)
        {
            Console.WriteLine($"A1: {A1}");
            Console.WriteLine($"A2: {A2}");
        }
    }
}
