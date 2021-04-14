using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord.WebSocket;

namespace BotSandwich.Modules.TwoOfUs
{
    class AudioHandler
    {
        private readonly Dictionary<ulong, ServerHandler> _serverHandlers;
        public AudioHandler(DiscordSocketClient client)
        {
            _serverHandlers = new Dictionary<ulong, ServerHandler>();
            client.UserVoiceStateUpdated += _onUserVoiceStateUpdated;
        }

        private async Task _onUserVoiceStateUpdated(SocketUser user, SocketVoiceState before, SocketVoiceState after)
        {
            if (user.IsBot) return;

            var guild = before.VoiceChannel?.Guild.Id ?? after.VoiceChannel.Guild.Id;

            if (_serverHandlers.TryGetValue(guild, out var handler))
            {
                //if current voice channel is fine, continue
                if (handler.CheckCurrentVoiceChannel()) return;
                //else remove current handler
                await _removeHandler(guild);
            }

            var differentChannels = before.VoiceChannel?.Id != after.VoiceChannel?.Id;

            //try to join previous channel, then try current channel
            if (!_checkVoiceChannel(guild, before.VoiceChannel) && differentChannels)
                _checkVoiceChannel(guild, after.VoiceChannel);
        }

        private bool _checkVoiceChannel(ulong guild, SocketVoiceChannel channel)
        {
            //if user leaves, after voice channel will be null; ignore
            if (channel is null) return false;
            
            //check if right number of users in channel
            var users = channel.Users.Count(u => !u.IsBot);
            if (users != 2) return false;
            
            //create new handler if so
            var handler = new ServerHandler(channel, async exception => await _removeHandler(guild));
            _serverHandlers.Add(guild, handler);
            return true;
        }

        private async Task _removeHandler(ulong guild)
        {
            if(_serverHandlers.TryGetValue(guild, out var handler)) await handler.Stop();
            _serverHandlers.Remove(guild);
        }
    }
}
