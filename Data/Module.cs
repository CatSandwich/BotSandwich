using System;
using System.Collections.Generic;
using System.Text;
using BotSandwich.Data.Commands;
using Discord.WebSocket;

namespace BotSandwich.Data
{
    public abstract class Module
    {
        private CommandHandler _commandHandler;

        protected CommandHandler InitCommandHandler(string prefix)
        {
            _commandHandler = new CommandHandler(prefix);
            return _commandHandler;
        }
        
        public abstract void Load(DiscordSocketClient client);
    }
}
