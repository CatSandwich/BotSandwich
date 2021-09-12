using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using BotSandwich.Data;
using BotSandwich.Data.Commands;
using BotSandwich.Data.Commands.Attributes;
using Discord;
using Discord.WebSocket;

namespace BotSandwich.Modules
{
    class CampfireModule : Module
    {
        protected override string CommandPrefix => "c!";

        [Command("hug")]
        public async Task Hug([Message] SocketMessage message)
        {
            string[] urls = 
            {
                @"https://media1.tenor.com/images/5ccc34d0e6f1dccba5b1c13f8539db77/tenor.gif",
                @"https://media1.tenor.com/images/2d4138c7c24d21b9d17f66a54ee7ea03/tenor.gif",
                @"https://media1.tenor.com/images/0ba7e0d8cd4ea0e6043119030f2b051f/tenor.gif",
                @"https://media1.tenor.com/images/6db54c4d6dad5f1f2863d878cfb2d8df/tenor.gif",
                @"https://media1.tenor.com/images/53c1172d85491e363ce58b20ba83cdab/tenor.gif",
                @"https://thumbs.gfycat.com/RashJointCow-max-1mb.gif"
            };

            var mentions = message.Content.GetMentions();
            
            var eb = new EmbedBuilder
            {
                Title = "Uh oh! Someone needs a hug!",
                Description = $"{message.Author.Mention} hugs {(mentions.Length == 0 ? "you" : string.Join(", ", mentions))}.",
                ImageUrl = urls.Random()
            };
            await message.Channel.SendMessageAsync(embed: eb.Build());
        }
    }

    public static class Helpers
    {
        public static T Random<T>(this T[] arr) => arr[new Random().Next(arr.Length)];

        public static string[] GetMentions(this string message)
        {
            var r = new Regex(@"<@!\d+>");
            return r.Matches(message).Select(m => m.Value).ToArray();
        }
    }
}
