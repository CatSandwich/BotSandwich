using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using BotSandwich.Data.Input;
using Discord;
using Discord.WebSocket;

namespace BotSandwich.Modules.Utils.Embeds
{
    class TestEmbed : InputEmbed
    {
        public override Embed Embed => new EmbedBuilder{Title = "Test"}.Build();
        
        [Choice("✅")]
        private async Task _checkmark(IUserMessage message, ISocketMessageChannel channel)
        {
            await channel.SendMessageAsync("Accepted");
            Remove?.Invoke(message.Id);
        }

        [Choice("❎")]
        private async Task _cross(IUserMessage message, ISocketMessageChannel channel)
        {
            await channel.SendMessageAsync("Rejected");
            await Delete();
        }
    }
}
