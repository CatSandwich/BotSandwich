using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Discord;
using Discord.Audio;
using Discord.WebSocket;

namespace BotSandwich.Modules.MeatBot
{
    class AudioClips
    {
        private readonly DiscordSocketClient _client;
        private IAudioClient _audioClient;

        public readonly List<Tuple<string, string>> Paths = new List<Tuple<string, string>>();

        public AudioClips(DiscordSocketClient client)
        {
            _client = client;

            var files = Directory.EnumerateFiles(_getAudioPath());

            foreach (var f in files)
            {
                var last = f.LastIndexOf('/');
                var file = f.Substring(last + 1);
                var name = file[..^4];
                _add(name, file);
            }

            _client.MessageReceived += _onMessage;
        }

        private async Task _onMessage(SocketMessage sm)
        {
            if (sm.MentionedUsers.All(u => u.Id != _client.CurrentUser.Id)) return;

            string content = sm.Content.ToLower();
            foreach (var (name, path) in Paths)
            {
                if (!content.Contains(name)) continue;

                var voiceChannel = (sm.Author as IVoiceState)?.VoiceChannel;

                if (voiceChannel is null)
                {
                    await sm.Channel.SendMessageAsync("Not being in a voice channel isn't very meat of you.");
                    return;
                }

                var botVoiceChannel = (sm.Author as IGuildUser)?.Guild.GetUserAsync(_client.CurrentUser.Id).Result?.VoiceChannel;

                if (botVoiceChannel == null || botVoiceChannel.Id != voiceChannel.Id)
                {
                    Console.WriteLine("Connecting to voice channel.");
                    var connect = Task.Run(async () =>
                    {
                        _audioClient = voiceChannel.ConnectAsync().Result;
                        await _sendAudio(path);
                    });
                }
                else
                {
                    Console.WriteLine("Already in voice channel.");
                    var sendAudio = Task.Run(async () => await _sendAudio(path));
                }
                return;
            }
        }

        private void _add(string name, string path) => Paths.Add(new Tuple<string, string>(name, path));

        private Process _createStream(string path)
        {
            return Process.Start(new ProcessStartInfo
            {
                FileName = "ffmpeg",
                Arguments = $"-hide_banner -loglevel panic -i \"{path}\" -ac 2 -f s16le -ar 48000 pipe:1",
                UseShellExecute = false,
                RedirectStandardOutput = true,
            });
        }

        private async Task _sendAudio(string path)
        {
            // Create FFmpeg using the previous example
            using var ffmpeg = _createStream(_getAudioPath() + path);
            await using var output = ffmpeg.StandardOutput.BaseStream;
            await using var discord = _audioClient.CreatePCMStream(AudioApplication.Mixed);
            try { await output.CopyToAsync(discord); }
            finally { await discord.FlushAsync(); }
        }

        // Returns this file's path + Audio directory.
        private static string _getAudioPath([CallerFilePath] string filePath = "") 
            => filePath.Substring(0, filePath.LastIndexOf("\\", StringComparison.Ordinal)) + "/Audio/";
    }
}
