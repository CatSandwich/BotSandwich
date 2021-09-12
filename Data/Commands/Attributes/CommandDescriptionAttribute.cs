using System;

namespace BotSandwich.Data.Commands.Attributes
{
    public class CommandDescriptionAttribute : Attribute
    {
        public string Description;
        public CommandDescriptionAttribute(string description)
        {
            Description = description;
        }
    }
}
