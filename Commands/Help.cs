/*using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BotSandwich.Data;
using BotSandwich.Data.Commands;
using Discord;
using Discord.WebSocket;

namespace BotSandwich.Commands
{
    class Help : Command
    {
        public override string Name => "help";
        public override string Description => "List all commands. Use 'help -c command' for info on a specific command.";
        public override string[] Examples => new []{"-c help"};
        public Help(EmbedBuilder all, List<Embed> single)
        {
            _all = all;
            _single = single;
        }

        [Argument(false, "c", "command")]
        private string _command;
        
        private readonly EmbedBuilder _all;
        private readonly List<Embed> _single;

        public override async Task Run(Module m, SocketMessage sm, string content)
        {
            if (HasArgument("command"))
            {
                var ebList = _single.Where(eb => eb.Title.Contains(_command));
                if (!ebList.Any()) await sm.Channel.SendMessageAsync($"_command '{_command}' not found.");
                await sm.Channel.SendMessageAsync(embed: ebList.First());
                return;
            }
            
            await sm.Channel.SendMessageAsync(embed: _all.Build());
        }
    }
}*/