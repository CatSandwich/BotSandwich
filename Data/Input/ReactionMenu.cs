using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using BotSandwich.Data.Commands.Attributes;
using BotSandwich.Data.Input.Attributes;
using Discord;
using Discord.WebSocket;

namespace BotSandwich.Data.Input
{
    public class ReactionMenu
    {
        public ulong Id;

        public Dictionary<string, Func<Cacheable<IUserMessage, ulong>, Cacheable<IMessageChannel, ulong>, SocketReaction, Task>> Choices = new Dictionary<string, Func<Cacheable<IUserMessage, ulong>, Cacheable<IMessageChannel, ulong>, SocketReaction, Task>>();
        
        public async Task OnReact(Cacheable<IUserMessage, ulong> message, Cacheable<IMessageChannel, ulong> channel, SocketReaction reaction)
        {
            try
            {
                var choice = Choices.First(pair => pair.Key == reaction.Emote.Name);
                await choice.Value(message, channel, reaction);
            }
            catch (InvalidOperationException)
            {
                await (await message.GetOrDownloadAsync()).RemoveAllReactionsForEmoteAsync(reaction.Emote);
            }
        }

        public ReactionMenu()
        {
            var choices = GetType()
                         .GetMethods(BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                         .Where(method => method.IsDefined(typeof(ChoiceAttribute)));

            foreach (var choice in choices)
            {
                var att = choice.GetCustomAttribute<ChoiceAttribute>();
                var name = att.Name;
                Func<Cacheable<IUserMessage, ulong>, Cacheable<IMessageChannel, ulong>, SocketReaction, Task> handler = async (message, channel, reaction) =>
                    {
                        var args = new List<object>();

                        foreach (var param in choice.GetParameters())
                        {
                            var messageAttribute = param.GetCustomAttribute<MessageAttribute>();
                            if (!(messageAttribute is null))
                            {
                                args.Add(await message.GetOrDownloadAsync());
                                continue;
                            }

                            var reactionAttribute = param.GetCustomAttribute<ReactionAttribute>();
                            if (!(reactionAttribute is null))
                            {
                                args.Add(reaction);
                                continue;
                            }

                            throw new ArgumentException($"Invalid parameter {param.Name} on choice {choice.Name}.");
                        }

                        
                        choice.Invoke(this, args.ToArray());
                    };
                
                Choices.Add(name, handler);
            }
        }
    }
}
