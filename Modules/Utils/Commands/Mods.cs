using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BotSandwich.Data;
using BotSandwich.Data.Commands;
using Discord.WebSocket;

namespace BotSandwich.Modules.Utils.Commands
{
    [Command(typeof(UtilsModule))]
    class Mods : Command
    {
        public override string Name => "mods";
        public override async Task Run(Module module, SocketMessage sm, string msg)
        {
            msg = "";

            foreach (var file in System.IO.Directory.GetFiles(@"C:\Users\Josh\AppData\Roaming\.minecraft\mods").Where(f => f.EndsWith(".jar")))
            {
                var f = file.Substring(file.LastIndexOf(@"\", StringComparison.Ordinal) + 1);
                msg += f + "\n";
                if (msg.Length > 2000)
                {
                    msg = msg.Substring(0, msg.Length - (f.Length + 1));
                    await sm.Channel.SendMessageAsync(msg);
                    msg = "";
                }
            }

            await sm.Channel.SendMessageAsync(msg);
        }
    }
}
