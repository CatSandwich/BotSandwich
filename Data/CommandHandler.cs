using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BotSandwich.Commands;
using Discord;
using Discord.WebSocket;

namespace BotSandwich.Data
{
    public class CommandHandler
    {
        private readonly List<Command> _commands;
        private readonly string _prefix;

        private readonly EmbedBuilder _globalHelpEmbed;
        private readonly List<Embed> _instanceHelpEmbed;

        public CommandHandler(string prefix)
        {
            _commands = new List<Command>();
            _prefix = prefix;

            _globalHelpEmbed = new EmbedBuilder {Title = "Commands"};
            _instanceHelpEmbed = new List<Embed>();
        }

        // Registers the callback on the provided client
        public void Register(DiscordSocketClient client)
        {
            client.MessageReceived += _messageReceived;
        }

        #region Modifiers
        public CommandHandler WithHelp()
        {
            WithCommand(new Help(_globalHelpEmbed, _instanceHelpEmbed));
            return this;
        }
        
        public CommandHandler WithCommand(Command command)
        {
            if (_commands.Count(c => c.Name == command.Name) != 0)
            {
                Console.WriteLine($"Error: Duplicate register of command '{command.Name}'. Skipping second instance.");
                return this;
            }
            
            _commands.Add(command);
            _instanceHelpEmbed.Add(command.BuildEmbed(_prefix));
            _globalHelpEmbed.AddField(_prefix + command.Name, command.Description);

            return this;
        }
        #endregion

        private async Task _messageReceived(SocketMessage sm)
        {
            if (!sm.Content.StartsWith(_prefix)) return;
            var content = sm.Content.Substring(_prefix.Length);
            
            var command = _commands.FirstOrDefault(c => content.StartsWith(c.Name));
            if (command is null) return;

            try
            {
                await command.ParseArguments(sm);
                await command.Run(sm, content);
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
