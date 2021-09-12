using System;

namespace BotSandwich.Data.Commands.Attributes
{
    public class CommandAttribute : Attribute
    {
        public string Name;
        public CommandAttribute(string name = "")
        {
            Name = name;
        }
    }
}
