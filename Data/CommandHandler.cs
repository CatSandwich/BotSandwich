using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        private readonly List<EmbedBuilder> _instanceHelpEmbed;

        public CommandHandler(string prefix)
        {
            _commands = new List<Command>();
            _prefix = prefix;

            _globalHelpEmbed = new EmbedBuilder {Title = "Commands"};
            _instanceHelpEmbed = new List<EmbedBuilder>();
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

            // Add command's help embed
            var temp = new EmbedBuilder
            {
                Title = ("Command: " + _prefix + command.Name), 
                Description = command.Description
            };

            foreach (var a in command.Arguments)
            {
                var aliases = a.Name.Aggregate("", (current, n) => current + $"{n}, ");
                temp.AddField($"Argument: {aliases.Substring(0, aliases.Length -2)} {(a.Required ? "(Required)" : "(Optional)")} ", a.Description);
            }
            temp.AddField("Example:", $"{_prefix}{command.Name} {command.Example}");

            _instanceHelpEmbed.Add(temp);

            // Add command to global help embed
            _globalHelpEmbed.AddField(_prefix + command.Name, command.Description);

            return this;
        }
        #endregion

        private async Task _messageReceived(SocketMessage sm)
        {
            if (!sm.Content.StartsWith(_prefix)) return;
            var content = sm.Content.Substring(_prefix.Length);

            Command command;
            try { command = _commands.First(c => content.StartsWith(c.Name)); }
            catch (InvalidOperationException) { return; }

            // Run argument callbacks
            var success = await command.RunArguments(sm, content.Substring(command.Name.Length));

            // If all arguments succeeded, run command
            if (success)
            {
                await command.Run(sm, content);
                return;
            }

            // Else tell caller which args are missing
            var missing = command.Arguments.Where(c => c.Required && !c.Supplied);
            
            var msg = "Missing the following args:\n";
            foreach (var a in missing) { msg += $"\t-{a.Name[0]}\n"; }
            
            await sm.Channel.SendMessageAsync(msg);
        }
    }
}
