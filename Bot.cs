using System;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using BotSandwich.Data;
using Discord;
using Discord.WebSocket;

namespace BotSandwich
{
    class Bot
    {
        private readonly DiscordSocketClient _client;

        public Bot()
        {
            _client = new DiscordSocketClient();
        }

        public void LoadModule(Module module) => module.Load(_client);
        
        public async Task Run(string token)
        {
            _client.Log += Log;
            await _client.LoginAsync(TokenType.Bot, token);
            await _client.StartAsync();
            await Task.Delay(-1);
        }

        private Task Log(LogMessage msg)
        {
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }
    }
}