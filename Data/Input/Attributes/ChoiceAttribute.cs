using System;

namespace BotSandwich.Data.Input.Attributes
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
