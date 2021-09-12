using System;
using System.Collections.Generic;
using System.Text;

namespace BotSandwich.Data.AutoResponse.Attributes
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    public class AutoResponse : Attribute
    {
        public string Keyword;

        public AutoResponse(string keyword)
        {
            Keyword = keyword;
        }
    }
}
