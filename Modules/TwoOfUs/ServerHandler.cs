using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Discord.Audio;
using Discord.WebSocket;

namespace BotSandwich.Modules.TwoOfUs
{
    class ServerHandler
    {
        private readonly SocketVoiceChannel _voiceChannel;
        private readonly Func<Exception, Task> _disconnectCallback;
        private readonly CancellationTokenSource _cts;

        private IAudioClient _audioClient;
        private Task _task;
        
        public ServerHandler(SocketVoiceChannel voiceChannel, Func<Exception, Task> disconnectCallback)
        {
            _cts = new CancellationTokenSource();
            _voiceChannel = voiceChannel;
            _disconnectCallback = disconnectCallback;
            _task = Task.Run(_stream);
        }

        public async Task Stop()
        {
            _cts.Cancel();
            await _task;
            await _voiceChannel.DisconnectAsync();
        }

        public bool CheckCurrentVoiceChannel() => _voiceChannel.Users.Count(u => !u.IsBot) == 2;

        private async void _stream()
        {
            _audioClient = await _voiceChannel.ConnectAsync();
            _audioClient.Disconnected += _disconnectCallback;
            _task = Task.Run(_sendMusic, _cts.Token);
        }
        
        private async void _sendMusic()
        {
            using var ffmpeg = _createStream();
            await using var output = ffmpeg.StandardOutput.BaseStream;
            await using var discord = _audioClient.CreatePCMStream(AudioApplication.Mixed);
            
            try { await output.CopyToAsync(discord, _cts.Token); }
            catch(Exception e) { Console.WriteLine($"Exception in CopyToAsync: {e.Message}"); }
            finally { await discord.FlushAsync(); }
        }

        private static Process _createStream()
        {
            return Process.Start(new ProcessStartInfo
            {
                FileName = "ffmpeg",
                Arguments = "-hide_banner -loglevel panic -i \"./twoofus.mp3\" -ac 2 -f s16le -ar 48000 pipe:1",
                UseShellExecute = false,
                RedirectStandardOutput = true
            });
        }
    }
}
