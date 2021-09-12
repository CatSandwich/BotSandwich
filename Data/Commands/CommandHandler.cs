/*using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BotSandwich.Commands;
using Discord;
using Discord.WebSocket;

namespace BotSandwich.Data.Commands
{
    public class CommandHandler
    {
        private readonly IEnumerable<Command> _commands;
        private readonly Module _module;
        private readonly string _prefix;

        private readonly EmbedBuilder _globalHelpEmbed;
        private readonly List<Embed> _instanceHelpEmbed;

        public CommandHandler(DiscordSocketClient client, string prefix, bool helpCommand, Module module)
        {
            _globalHelpEmbed = new EmbedBuilder { Title = "Commands" };
            _instanceHelpEmbed = new List<Embed>();
            
            _commands =
                from type in typeof(CommandHandler).Assembly.GetTypes()
                from att in Attribute.GetCustomAttributes(type)
                where (att as CommandAttribute)?.Module == module.GetType()
                select Activator.CreateInstance(type) as Command;
            
            if(helpCommand) _commands = _commands.Append(new Help(_globalHelpEmbed, _instanceHelpEmbed));
            
            _module = module;
            _prefix = prefix;

            client.MessageReceived += _messageReceived;
        }

        private async Task _messageReceived(SocketMessage sm)
        {
            if (!sm.Content.StartsWith(_prefix)) return;
            var content = sm.Content.Substring(_prefix.Length);
            
            var command = _commands.FirstOrDefault(c => content.StartsWith(c.Name));
            if (command is null) return;

            try
            {
                await command.ParseArguments(sm);
                await command.Run(_module, sm, content);
            }
            catch (ParseException e)
            {
                var eb = new EmbedBuilder {Color = new Color(255, 0, 0), Title = "Error!", Description = _getCheekyErrorMessage()};
                eb.AddField($"Arg: -{e.ArgName}", $"Error: {e.Error}");
                await sm.Channel.SendMessageAsync(embed: eb.Build());
            }
        }

        private string _getCheekyErrorMessage()
        {
            string[] list =
            {
                "Somebody messed up...",
                "Might wanna consult the help command...",
                "Have you tried turning it off and on again?",
                "Just google it",
                "RTFM error",
                $"{_prefix}help"
            };

            var r = new Random();
            return list[r.Next(list.Length)];
        }
    }
}
*/