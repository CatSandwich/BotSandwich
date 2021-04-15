using System;

namespace BotSandwich.Data.Commands
{
    class ArgumentDescriptionAttribute : Attribute
    {
        public string Description;
        public ArgumentDescriptionAttribute(string description)
        {
            Description = description;
        }
    }
}
