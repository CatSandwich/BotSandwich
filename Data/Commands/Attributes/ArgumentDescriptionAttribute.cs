using System;

namespace BotSandwich.Data.Commands.Attributes
{
    public class ArgumentDescriptionAttribute : Attribute
    {
        public string Description;
        public ArgumentDescriptionAttribute(string description)
        {
            Description = description;
        }
    }
}
