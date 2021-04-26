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
        protected virtual bool UseHelpCommand => true;
        protected abstract string CommandPrefix { get; }
        
        public CommandHandler CommandHandler;
        public InputHandler InputHandler;

        protected Module(DiscordSocketClient client)
        {
            CommandHandler = new CommandHandler(client, CommandPrefix, UseHelpCommand, this);
            InputHandler = new InputHandler(client);
        }
    }
}
