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
        public static Bot Instance;
        public static ulong Id => Instance.Client.CurrentUser.Id;
        
        public readonly DiscordSocketClient Client;

        public Bot()
        {
            if(Instance != null) Console.WriteLine("Multiple Bot instances, careful.");
            Instance = this;
            Client = new DiscordSocketClient();
        }

        public void LoadModule(Module module) => module.Load(Client);
        
        public async Task Run(string token)
        {
            Client.Log += Log;
            await Client.LoginAsync(TokenType.Bot, token);
            await Client.StartAsync();
            await Task.Delay(-1);
        }

        private Task Log(LogMessage msg)
        {
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }
    }
}