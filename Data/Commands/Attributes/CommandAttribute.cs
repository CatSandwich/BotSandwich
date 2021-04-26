using System;
using System.Collections.Generic;
using System.Text;

namespace BotSandwich.Data.Commands
{
    class CommandAttribute : Attribute
    {
        public Type Module;
        public CommandAttribute(Type module)
        {
            Module = module;
        }
    }
}
