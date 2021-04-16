using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;

namespace BotSandwich.Data.Input
{
    public class InputHandler
    {
        public Dictionary<ulong, InputEmbed> Embeds;

        public InputHandler()
        {
            Embeds = new Dictionary<ulong, InputEmbed>();
        }
        
        public void Register(DiscordSocketClient client)
        {
            client.ReactionAdded += _reactionAdded;
            client.MessageDeleted += _messageDeleted;
        }

        public async Task Create(InputEmbed embed, IMessageChannel channel)
        {
            var message = await channel.SendMessageAsync(embed: embed.Embed);
            embed.Message = message;
            embed.Remove = () => Embeds.Remove(embed.Message.Id);
            
            foreach (var (att, _) in embed.ChoiceMethods)
            {
                await message.AddReactionAsync(new Emoji(att.Name));
            }
            Embeds.Add(message.Id, embed);
        }
        
        private async Task _reactionAdded(Cacheable<IUserMessage, ulong> cacheable, ISocketMessageChannel channel, SocketReaction reaction)
        {
            if (reaction.UserId == Bot.Id) return;
            
            var embed = Embeds.FirstOrDefault(e => e.Key == cacheable.Id).Value;
            if (embed is null) return;

            await embed.OnReact(await cacheable.GetOrDownloadAsync(), channel, reaction);
        }

        private async Task _messageDeleted(Cacheable<IMessage, ulong> cacheable, ISocketMessageChannel channel)
        {
            Embeds.Remove(cacheable.Id);
        }
    }
}
