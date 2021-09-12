using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using BotSandwich.Data.Commands;
using BotSandwich.Data.Commands.Attributes;
using BotSandwich.Data.Input;
using Discord;
using Discord.WebSocket;

namespace BotSandwich.Data
{
    public abstract class Module 
    {
        protected virtual bool UseHelpCommand => true;
        protected abstract string CommandPrefix { get; }
        
        protected InputHandler InputHandler;
        protected DiscordSocketClient Client;

        private readonly Dictionary<ulong, ReactionMenu>               _reactionMenus = new Dictionary<ulong, ReactionMenu>();
        private readonly Dictionary<string, Func<SocketMessage, Task>> _commands      = new Dictionary<string, Func<SocketMessage, Task>>();
        private readonly Dictionary<string, string>                    _autoResponses = new Dictionary<string, string>();

        public async Task CreateReactionMenu(ReactionMenu menu, ulong channel, Embed embed)
        {
            var msg = await (Client.GetChannel(channel) as ITextChannel).SendMessageAsync(embed: embed);
            await _createReactionMenu(menu, msg);
        }
        public async Task CreateReactionMenu(ReactionMenu menu, ulong channel, string content)
        {
            var msg = await (Client.GetChannel(channel) as ITextChannel).SendMessageAsync(content);
            await _createReactionMenu(menu, msg);
        }

        private async Task _createReactionMenu(ReactionMenu menu, IUserMessage msg)
        {
            await msg.AddReactionsAsync(menu.Choices.Keys.Select(k => new Emoji(k)).ToArray());
            _reactionMenus.Add(msg.Id, menu);
            Client.ReactionAdded += menu.OnReact;
        }
        
        private static bool _tryGetArgValue(string str, string name, out string result)
        {
            var quoteMatch = Regex.Match(str, $"-{name} \"[^\"]*");
            var noQuoteMatch = Regex.Match(str, $"-{name} [^ ]*");

            // If no success, try next
            if (!quoteMatch.Success && !noQuoteMatch.Success)
            {
                result = null;
                return false;
            }

            var match = quoteMatch.Success ? quoteMatch.Value : noQuoteMatch.Value;
            result = match.Replace("\"", "")[(name.Length + 2)..];
            return true;
        }

        protected Module()
        {
            var type = GetType();
            var commands = type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance).Where(f => f.IsDefined(typeof(CommandAttribute), false));

            foreach (var command in commands)
            {
                var commandName = command.GetCustomAttribute<CommandAttribute>()?.Name ?? throw new ArgumentException("Missing name.");

                async Task handler(SocketMessage message)
                {
                    // Build argument array according to parameter types and attributes
                    var args = new List<object>();
                    foreach (var param in command.GetParameters())
                    {
                        #region Argument
                        var arg = param.GetCustomAttribute<ArgumentAttribute>();
                        if (!(arg is null))
                        {
                            try
                            {
                                string val = null;
                                if (arg.Names.FirstOrDefault(attributeName => _tryGetArgValue(message.Content, attributeName, out val)) is null && arg.Required)
                                    throw new ParseException(arg.Names[0], "Argument is required.");

                                args.Add(await ArgumentAttribute.TryParse(val, param.ParameterType, new ParseContext(message)));
                            }
                            catch (ParseException exc)
                            {
                                string[] list = 
                                {
                                    "Somebody messed up...",
                                    "Might wanna consult the help command...",
                                    "Have you tried turning it off and on again?",
                                    "Just google it",
                                    "RTFM error",
                                    $"{CommandPrefix}help"
                                };
                                
                                var eb = new EmbedBuilder { Color = new Color(255, 0, 0), Title = "Error!", Description = list[new Random().Next(list.Length)] };
                                eb.AddField($"Arg: -{exc.ArgName}", $"Error: {exc.Error}");
                                await message.Channel.SendMessageAsync(embed: eb.Build());
                                return;
                            }

                            continue;
                        }
                        #endregion

                        #region Message
                        var msg = param.GetCustomAttribute<MessageAttribute>();
                        if (!(msg is null))
                        {
                            args.Add(message);
                            continue;
                        }
                        #endregion

                        throw new ArgumentException($"Invalid parameter {param.Name}");
                    }

                    // Invoke with built argument list
                    command.Invoke(this, args.ToArray());
                }

                _commands.Add(commandName, handler);
            }

            var autoResponses =
                type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance)
                    .Where(f => f.IsDefined(typeof(AutoResponse.Attributes.AutoResponse)))
                    .Where(f => f.FieldType == typeof(string));

            foreach (var response in autoResponses)
            {
                foreach (var att in response.GetCustomAttributes<AutoResponse.Attributes.AutoResponse>())
                {
                    _autoResponses.Add(att.Keyword, response.GetValue(this) as string);
                }
            }
        }
        
        public virtual void Init(DiscordSocketClient client)
        {
            Client = client;
            InputHandler = new InputHandler(client);

            client.MessageReceived += async message =>
            {
                if (message.Author.IsBot) return;
                foreach (var command in _commands)
                {
                    if (message.Content.StartsWith($"{CommandPrefix}{command.Key}"))
                    {
                        await command.Value(message);
                    }
                }

                foreach (var response in _autoResponses)
                {
                    if (message.Content.ToLower().Contains(response.Key.ToLower()))
                    {
                        await message.Channel.SendMessageAsync(response.Value);
                    }
                }
            };

            /*client.MessageDeleted += async (message, channel) =>
            {
                if (_reactionMenus.Remove((await message.GetOrDownloadAsync()).Id, out var menu))
                {
                    Client.ReactionAdded -= menu.OnReact;
                }
            };*/
        }
    }
}
