using System;
using System.ComponentModel;

namespace BotSandwich.Data
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
