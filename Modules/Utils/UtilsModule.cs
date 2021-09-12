using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BotSandwich.Data;
using BotSandwich.Data.Commands;
using BotSandwich.Data.Commands.Attributes;
using BotSandwich.Modules.Utils.Embeds;
using Discord;
using Discord.WebSocket;

namespace BotSandwich.Modules.Utils
{
    sealed class UtilsModule : Module
    {
        protected override string CommandPrefix => "u!";

        [Command("createembed")]
        public async Task CreateEmbed([Message] SocketMessage sm) => await InputHandler.Create(new TestEmbed(), sm.Channel);
        
        [Command("louder")]
        public async Task Louder([Message] SocketMessage sm, [Argument(true, "s", "start")] IMessage start, [Argument(true, "n", "number", "num")] uint num)
        {
            var channel = sm.Channel;

            // Get n messages after message m1. Comes in newest first hence the reverse.
            IEnumerable<IMessage> messages = await channel.GetMessagesAsync(start, Direction.After, (int)num - 1, CacheMode.AllowDownload, RequestOptions.Default).FirstAsync();
            messages = messages.Reverse().Prepend(start);

            await channel.SendMessageAsync($"**{_repeat(messages).Replace("**", "")}**");
        }

        // Returns a string that repeats the content of a group of messages.
        private static string _repeat(IEnumerable<IMessage> messages)
        {
            var msg = "";
            IUser author = null;

            foreach (var m in messages)
            {
                if (m.Author != author)
                {
                    author = m.Author;
                    msg += $"\n{author.Username} - {m.Timestamp:g}\n";
                }

                msg += m.Content.ToUpper() + '\n';
            }

            return msg;
        }
    }
}
