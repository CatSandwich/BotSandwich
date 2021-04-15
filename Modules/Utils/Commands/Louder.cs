using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BotSandwich.Data;
using Discord;
using Discord.WebSocket;

namespace BotSandwich.Modules.Utils.Commands
{
    // Repeats a group of messages in bold and caps.
    class Louder : Command
    {
        public override string Name => "louder";
        public override string Description =>
            "Repeats n messages starting from s in bold and caps for the hearing impaired.";

        [Argument(true, "s", "start")]
        private ulong _s;
        [Argument(true, "n", "num", "number")]
        private uint? _n;

        public override async Task Run(SocketMessage sm, string remainder)
        {
            var channel = sm.Channel;

            if (_n is null)
            {
                await channel.SendMessageAsync("Number parse failed.");
                return;
            }

            // Get n messages after message m1. Comes in newest first hence the reverse.
            IEnumerable<IMessage> messages = await channel.GetMessagesAsync(_s, Direction.After, (int)_n - 1, CacheMode.AllowDownload, RequestOptions.Default).FirstAsync();
            messages = messages.Reverse().Prepend(await channel.GetMessageAsync(_s));

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
