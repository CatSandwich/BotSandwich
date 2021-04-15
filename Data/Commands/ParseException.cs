using System;

namespace BotSandwich.Data.Commands
{
    class ParseException : Exception
    {
        public string ArgName;
        public string Error;

        public ParseException(string argName, string error)
        {
            ArgName = argName;
            Error = error;
        }
    }
}
