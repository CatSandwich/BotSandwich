using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BotSandwich.Data;
using Discord;
using Discord.WebSocket;

namespace BotSandwich.Commands
{
    class Help : Command
    {
        public override string Name => "help";
        public override string Description => "List all commands. Use 'help -c command' for info on a specific command.";
        public override string Example => "-c help";
        public Help(EmbedBuilder all, List<EmbedBuilder> single)
        {
            _all = all;
            _single = single;

            AddArgument(new ArgumentBuilder()
                .WithNames("command", "c")
                .WithDescription("Get information on a specific command instead.")
                .WithCallback(async value =>
                {
                    _command = value;
                    return true;
                })
                .Build()
            );
        }

        private string _command;
        private readonly EmbedBuilder _all;
        private readonly List<EmbedBuilder> _single;

        public override async Task Run(SocketMessage sm, string content)
        {
            if (HasArgument("command"))
            {
                var EBList = _single.Where(eb => eb.Title.Contains(_command)).ToList();
                if (EBList.Count == 0) await sm.Channel.SendMessageAsync($"Command '{_command}' not found.");
                await sm.Channel.SendMessageAsync(embed: EBList.First().Build());
            }
            else
                await sm.Channel.SendMessageAsync(embed: _all.Build());
        }
    }
}