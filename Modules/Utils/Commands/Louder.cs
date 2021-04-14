﻿using System.Collections.Generic;
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

        public Louder()
        {
            AddArgument(new ArgumentBuilder()
                            .WithNames("s", "start")
                            .WithDescription("Which message to start on.")
                            .WithCallback(_parseS)
                            .Required()
                            .Build()
            );

            AddArgument(new ArgumentBuilder()
                            .WithNames("n", "num", "number")
                            .WithDescription("How many messages to pull.")
                            .WithCallback(_parseN)
                            .Required()
                            .Build()
            );
        }

        private async Task<bool> _parseS(string value)
        {
            if (!ulong.TryParse(value, out var temp)) return false;
            _s = temp;
            return true;
        }

        private async Task<bool> _parseN(string value)
        {
            if (!uint.TryParse(value, out var temp)) return false;
            _n = temp;
            return true;
        }

        private ulong _s;
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