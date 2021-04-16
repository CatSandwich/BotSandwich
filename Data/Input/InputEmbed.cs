using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using BotSandwich.Data.Commands;
using Discord;
using Discord.WebSocket;

namespace BotSandwich.Data.Input
{
    public abstract class InputEmbed
    {
        public Tuple<ChoiceAttribute, MethodInfo>[] ChoiceMethods;
        public abstract Embed Embed { get; }

        public IUserMessage Message;
        public Func<bool> Remove;
        
        public InputEmbed()
        {
            ChoiceMethods = (
                from field in GetType().GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                from a in Attribute.GetCustomAttributes(field)
                let fromArgument = a as ChoiceAttribute
                where fromArgument != null
                select new Tuple<ChoiceAttribute, MethodInfo>(fromArgument, field)
            ).ToArray();
        }

        public async Task OnReact(IUserMessage message, ISocketMessageChannel channel, SocketReaction reaction)
        {
            foreach (var (choice, method) in ChoiceMethods)
            {
                if (reaction.Emote.Name != choice.Name) continue;
                var task = method.Invoke(this, new object[]{message, channel}) as Task;
                await task;
            }
        }

        public async Task Delete()
        {
            await Message.DeleteAsync();
        }
    }
}
