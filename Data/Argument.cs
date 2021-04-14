using System;
using System.Threading.Tasks;

namespace BotSandwich.Data
{
    public class Argument
    {
        public delegate Task<bool> InvokeSig(string value);

        public string[] Name;
        public InvokeSig Invoke = null;
        public string Description = "No description.";
        public bool Required = false;
        public bool Supplied = false;
        
        /*public static InvokeSig ParseValue(ref int i)
        {
            return value => i = int.Parse(value);
        }        
        public static InvokeSig ParseValue(ref uint i)
        {
            return value => i = uint.Parse(value);
        }        
        public static InvokeSig ParseValue(ref ulong i)
        {
            return value => i = ulong.Parse(value);
        }*/
    }
}
