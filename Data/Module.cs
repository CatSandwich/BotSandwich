using System;
using System.Collections.Generic;
using System.Text;
using BotSandwich.Data.Commands;
using BotSandwich.Data.Input;
using Discord.WebSocket;

namespace BotSandwich.Data
{
    public abstract class Module
    {
        public CommandHandler CommandHandler;
        public InputHandler InputHandler;

        protected CommandHandler InitCommandHandler(string prefix)
        {
            CommandHandler = new CommandHandler(this, prefix);
            return CommandHandler;
        }

        protected InputHandler InitInputHandler()
        {
            InputHandler = new InputHandler();
            return InputHandler;
        }
        
        public abstract void Load(DiscordSocketClient client);
    }
}
