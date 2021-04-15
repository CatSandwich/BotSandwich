using System;
using System.Collections.Generic;
using System.Text;

namespace BotSandwich.Data
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
