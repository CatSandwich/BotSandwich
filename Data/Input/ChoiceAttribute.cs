using System;
using System.Collections.Generic;
using System.Text;

namespace BotSandwich.Data.Input
{
    public class ChoiceAttribute : Attribute
    {
        public string Name;
        public ChoiceAttribute(string name)
        {
            Name = name;
        }
    }
}
