// This file is part of the DisCatSharp project.
//
// Copyright (c) 2021 AITSYS
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using System;
using System.Buffers;
using System.Buffers.Binary;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using DisCatSharp.Entities;
using DisCatSharp.EventArgs;
using DisCatSharp.Net;
using DisCatSharp.Net.Udp;
using DisCatSharp.Net.WebSocket;
using DisCatSharp.VoiceNext.Codec;
using DisCatSharp.VoiceNext.Entities;
using DisCatSharp.VoiceNext.EventArgs;
using DisCatSharp.Common.Utilities;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DisCatSharp.VoiceNext
{
    internal delegate Task VoiceDisconnectedEventHandler(DiscordGuild guild);

    /// <summary>
    /// VoiceNext connection to a voice channel.
    /// </summary>
    public sealed class VoiceNextConnection : IDisposable
    {
        /// <summary>
        /// Triggered whenever a user speaks in the connected voice channel.
        /// </summary>
        public event AsyncEventHandler<VoiceNextConnection, UserSpeakingEventArgs> UserSpeaking
        {
            add { this._userSpeaking.Register(value); }
            remove { this._userSpeaking.Unregister(value); }
        }
        private readonly AsyncEvent<VoiceNextConnection, UserSpeakingEventArgs> _userSpeaking;

        /// <summary>
        /// Triggered whenever a user joins voice in the connected guild.
        /// </summary>
        public event AsyncEventHandler<VoiceNextConnection, VoiceUserJoinEventArgs> UserJoined
        {
            add { this._userJoined.Register(value); }
            remove { this._userJoined.Unregister(value); }
        }
        private readonly AsyncEvent<VoiceNextConnection, VoiceUserJoinEventArgs> _userJoined;

        /// <summary>
        /// Triggered whenever a user leaves voice in the connected guild.
        /// </summary>
        public event AsyncEventHandler<VoiceNextConnection, VoiceUserLeaveEventArgs> UserLeft
        {
            add { this._userLeft.Register(value); }
            remove { this._userLeft.Unregister(value); }
        }
        private readonly AsyncEvent<VoiceNextConnection, VoiceUserLeaveEventArgs> _userLeft;

        /// <summary>
        /// Triggered whenever voice data is received from the connected voice channel.
        /// </summary>
        public event AsyncEventHandler<VoiceNextConnection, VoiceReceiveEventArgs> VoiceReceived
        {
            add { this._voiceReceived.Register(value); }
            remove { this._voiceReceived.Unregister(value); }
        }
        private readonly AsyncEvent<VoiceNextConnection, VoiceReceiveEventArgs> _voiceReceived;

        /// <summary>
        /// Triggered whenever voice WebSocket throws an exception.
        /// </summary>
        public event AsyncEventHandler<VoiceNextConnection, SocketErrorEventArgs> VoiceSocketErrored
        {
            add { this._voiceSocketError.Register(value); }
            remove { this._voiceSocketError.Unregister(value); }
        }
        private readonly AsyncEvent<VoiceNextConnection, SocketErrorEventArgs> _voiceSocketError;

        internal event VoiceDisconnectedEventHandler VoiceDisconnected;

        /// <summary>
        /// Gets the unix epoch.
        /// </summary>
        private static DateTimeOffset UnixEpoch { get; } = new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero);

        /// <summary>
        /// Gets the discord.
        /// </summary>
        private DiscordClient Discord { get; }
        /// <summary>
        /// Gets the guild.
        /// </summary>
        private DiscordGuild Guild { get; }
        /// <summary>
        /// Gets the transmitting s s r cs.
        /// </summary>
        private ConcurrentDictionary<uint, AudioSender> TransmittingSSRCs { get; }

        /// <summary>
        /// Gets the udp client.
        /// </summary>
        private BaseUdpClient UdpClient { get; }
        /// <summary>
        /// Gets or sets the voice ws.
        /// </summary>
        private IWebSocketClient VoiceWs { get; set; }
        /// <summary>
        /// Gets or sets the heartbeat task.
        /// </summary>
        private Task HeartbeatTask { get; set; }
        /// <summary>
        /// Gets or sets the heartbeat interval.
        /// </summary>
        private int HeartbeatInterval { get; set; }
        /// <summary>
        /// Gets or sets the last heartbeat.
        /// </summary>
        private DateTimeOffset LastHeartbeat { get; set; }

        /// <summary>
        /// Gets or sets the token source.
        /// </summary>
        private CancellationTokenSource TokenSource { get; set; }
        /// <summary>
        /// Gets the token.
        /// </summary>
        private CancellationToken Token
            => this.TokenSource.Token;

        /// <summary>
        /// Gets or sets the server data.
        /// </summary>
        internal VoiceServerUpdatePayload ServerData { get; set; }
        /// <summary>
        /// Gets or sets the state data.
        /// </summary>
        internal VoiceStateUpdatePayload StateData { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether resume.
        /// </summary>
        internal bool Resume { get; set; }

        /// <summary>
        /// Gets the configuration.
        /// </summary>
        private VoiceNextConfiguration Configuration { get; }
        /// <summary>
        /// Gets or sets the opus.
        /// </summary>
        private Opus Opus { get; set; }
        /// <summary>
        /// Gets or sets the sodium.
        /// </summary>
        private Sodium Sodium { get; set; }
        /// <summary>
        /// Gets or sets the rtp.
        /// </summary>
        private Rtp Rtp { get; set; }
        /// <summary>
        /// Gets or sets the selected encryption mode.
        /// </summary>
        private EncryptionMode SelectedEncryptionMode { get; set; }
        /// <summary>
        /// Gets or sets the nonce.
        /// </summary>
        private uint Nonce { get; set; } = 0;

        /// <summary>
        /// Gets or sets the sequence.
        /// </summary>
        private ushort Sequence { get; set; }
        /// <summary>
        /// Gets or sets the timestamp.
        /// </summary>
        private uint Timestamp { get; set; }
        /// <summary>
        /// Gets or sets the s s r c.
        /// </summary>
        private uint SSRC { get; set; }
        /// <summary>
        /// Gets or sets the key.
        /// </summary>
        private byte[] Key { get; set; }
        /// <summary>
        /// Gets or sets the discovered endpoint.
        /// </summary>
        private IpEndpoint DiscoveredEndpoint { get; set; }
        /// <summary>
        /// Gets or sets the web socket endpoint.
        /// </summary>
        internal ConnectionEndpoint WebSocketEndpoint { get; set; }
        /// <summary>
        /// Gets or sets the udp endpoint.
        /// </summary>
        internal ConnectionEndpoint UdpEndpoint { get; set; }

        /// <summary>
        /// Gets or sets the ready wait.
        /// </summary>
        private TaskCompletionSource<bool> ReadyWait { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether is initialized.
        /// </summary>
        private bool IsInitialized { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether is disposed.
        /// </summary>
        private bool IsDisposed { get; set; }

        /// <summary>
        /// Gets or sets the playing wait.
        /// </summary>
        private TaskCompletionSource<bool> PlayingWait { get; set; }

        /// <summary>
        /// Gets the pause event.
        /// </summary>
        private AsyncManualResetEvent PauseEvent { get; }
        /// <summary>
        /// Gets or sets the transmit stream.
        /// </summary>
        private VoiceTransmitSink TransmitStream { get; set; }
        /// <summary>
        /// Gets the transmit channel.
        /// </summary>
        private Channel<RawVoicePacket> TransmitChannel { get; }
        /// <summary>
        /// Gets the keepalive timestamps.
        /// </summary>
        private ConcurrentDictionary<ulong, long> KeepaliveTimestamps { get; }
        private ulong _lastKeepalive = 0;

        /// <summary>
        /// Gets or sets the sender task.
        /// </summary>
        private Task SenderTask { get; set; }
        /// <summary>
        /// Gets or sets the sender token source.
        /// </summary>
        private CancellationTokenSource SenderTokenSource { get; set; }
        /// <summary>
        /// Gets the sender token.
        /// </summary>
        private CancellationToken SenderToken
            => this.SenderTokenSource.Token;

        /// <summary>
        /// Gets or sets the receiver task.
        /// </summary>
        private Task ReceiverTask { get; set; }
        /// <summary>
        /// Gets or sets the receiver token source.
        /// </summary>
        private CancellationTokenSource ReceiverTokenSource { get; set; }
        /// <summary>
        /// Gets the receiver token.
        /// </summary>
        private CancellationToken ReceiverToken
            => this.ReceiverTokenSource.Token;

        /// <summary>
        /// Gets or sets the keepalive task.
        /// </summary>
        private Task KeepaliveTask { get; set; }
        /// <summary>
        /// Gets or sets the keepalive token source.
        /// </summary>
        private CancellationTokenSource KeepaliveTokenSource { get; set; }
        /// <summary>
        /// Gets the keepalive token.
        /// </summary>
        private CancellationToken KeepaliveToken
            => this.KeepaliveTokenSource.Token;

        private volatile bool _isSpeaking = false;

        /// <summary>
        /// Gets the audio format used by the Opus encoder.
        /// </summary>
        public AudioFormat AudioFormat => this.Configuration.AudioFormat;

        /// <summary>
        /// Gets whether this connection is still playing audio.
        /// </summary>
        public bool IsPlaying
            => this.PlayingWait != null && !this.PlayingWait.Task.IsCompleted;

        /// <summary>
        /// Gets the websocket round-trip time in ms.
        /// </summary>
        public int WebSocketPing
            => Volatile.Read(ref this._wsPing);
        private int _wsPing = 0;

        /// <summary>
        /// Gets the UDP round-trip time in ms.
        /// </summary>
        public int UdpPing
            => Volatile.Read(ref this._udpPing);
        private int _udpPing = 0;

        private int _queueCount;

        /// <summary>
        /// Gets the channel this voice client is connected to.
        /// </summary>
        public DiscordChannel TargetChannel { get; internal set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="VoiceNextConnection"/> class.
        /// </summary>
        /// <param name="client">The client.</param>
        /// <param name="guild">The guild.</param>
        /// <param name="channel">The channel.</param>
        /// <param name="config">The config.</param>
        /// <param name="server">The server.</param>
        /// <param name="state">The state.</param>
        internal VoiceNextConnection(DiscordClient client, DiscordGuild guild, DiscordChannel channel, VoiceNextConfiguration config, VoiceServerUpdatePayload server, VoiceStateUpdatePayload state)
        {
            this.Discord = client;
            this.Guild = guild;
            this.TargetChannel = channel;
            this.TransmittingSSRCs = new ConcurrentDictionary<uint, AudioSender>();

            this._userSpeaking = new AsyncEvent<VoiceNextConnection, UserSpeakingEventArgs>("VNEXT_USER_SPEAKING", TimeSpan.Zero, this.Discord.EventErrorHandler);
            this._userJoined = new AsyncEvent<VoiceNextConnection, VoiceUserJoinEventArgs>("VNEXT_USER_JOINED", TimeSpan.Zero, this.Discord.EventErrorHandler);
            this._userLeft = new AsyncEvent<VoiceNextConnection, VoiceUserLeaveEventArgs>("VNEXT_USER_LEFT", TimeSpan.Zero, this.Discord.EventErrorHandler);
            this._voiceReceived = new AsyncEvent<VoiceNextConnection, VoiceReceiveEventArgs>("VNEXT_VOICE_RECEIVED", TimeSpan.Zero, this.Discord.EventErrorHandler);
            this._voiceSocketError = new AsyncEvent<VoiceNextConnection, SocketErrorEventArgs>("VNEXT_WS_ERROR", TimeSpan.Zero, this.Discord.EventErrorHandler);
            this.TokenSource = new CancellationTokenSource();

            this.Configuration = config;
            this.Opus = new Opus(this.AudioFormat);
            //this.Sodium = new Sodium();
            this.Rtp = new Rtp();

            this.ServerData = server;
            this.StateData = state;

            var eps = this.ServerData.Endpoint;
            var epi = eps.LastIndexOf(':');
            var eph = string.Empty;
            var epp = 443;
            if (epi != -1)
            {
                eph = eps.Substring(0, epi);
                epp = int.Parse(eps.Substring(epi + 1));
            }
            else
            {
                eph = eps;
            }
            this.WebSocketEndpoint = new ConnectionEndpoint { Hostname = eph, Port = epp };

            this.ReadyWait = new TaskCompletionSource<bool>();
            this.IsInitialized = false;
            this.IsDisposed = false;

            this.PlayingWait = null;
            this.TransmitChannel = Channel.CreateBounded<RawVoicePacket>(new BoundedChannelOptions(this.Configuration.PacketQueueSize));
            this.KeepaliveTimestamps = new ConcurrentDictionary<ulong, long>();
            this.PauseEvent = new AsyncManualResetEvent(true);

            this.UdpClient = this.Discord.Configuration.UdpClientFactory();
            this.VoiceWs = this.Discord.Configuration.WebSocketClientFactory(this.Discord.Configuration.Proxy);
            this.VoiceWs.Disconnected += this.VoiceWS_SocketClosed;
            this.VoiceWs.MessageReceived += this.VoiceWS_SocketMessage;
            this.VoiceWs.Connected += this.VoiceWS_SocketOpened;
            this.VoiceWs.ExceptionThrown += this.VoiceWs_SocketException;
        }

        ~VoiceNextConnection()
        {
            this.Dispose();
        }

        /// <summary>
        /// Connects to the specified voice channel.
        /// </summary>
        /// <returns>A task representing the connection operation.</returns>
        internal Task ConnectAsync()
        {
            var gwuri = new UriBuilder
            {
                Scheme = "wss",
                Host = this.WebSocketEndpoint.Hostname,
                Query = "encoding=json&v=4"
            };

            return this.VoiceWs.ConnectAsync(gwuri.Uri);
        }

        /// <summary>
        /// Reconnects .
        /// </summary>
        /// <returns>A Task.</returns>
        internal Task ReconnectAsync()
            => this.VoiceWs.DisconnectAsync();

        /// <summary>
        /// Starts .
        /// </summary>
        /// <returns>A Task.</returns>
        internal async Task StartAsync()
        {
            // Let's announce our intentions to the server
            var vdp = new VoiceDispatch();

            if (!this.Resume)
            {
                vdp.OpCode = 0;
                vdp.Payload = new VoiceIdentifyPayload
                {
                    ServerId = this.ServerData.GuildId,
                    UserId = this.StateData.UserId.Value,
                    SessionId = this.StateData.SessionId,
                    Token = this.ServerData.Token
                };
                this.Resume = true;
            }
            else
            {
                vdp.OpCode = 7;
                vdp.Payload = new VoiceIdentifyPayload
                {
                    ServerId = this.ServerData.GuildId,
                    SessionId = this.StateData.SessionId,
                    Token = this.ServerData.Token
                };
            }
            var vdj = JsonConvert.SerializeObject(vdp, Formatting.None);
            await this.WsSendAsync(vdj).ConfigureAwait(false);
        }

        /// <summary>
        /// Waits the for ready async.
        /// </summary>
        /// <returns>A Task.</returns>
        internal Task WaitForReadyAsync()
            => this.ReadyWait.Task;

        /// <summary>
        /// Enqueues the packet async.
        /// </summary>
        /// <param name="packet">The packet.</param>
        /// <param name="token">The token.</param>
        /// <returns>A Task.</returns>
        internal async Task EnqueuePacketAsync(RawVoicePacket packet, CancellationToken token = default)
        {
            await this.TransmitChannel.Writer.WriteAsync(packet, token).ConfigureAwait(false);
            this._queueCount++;
        }

        /// <summary>
        /// Prepares the packet.
        /// </summary>
        /// <param name="pcm">The pcm.</param>
        /// <param name="target">The target.</param>
        /// <param name="length">The length.</param>
        /// <returns>A bool.</returns>
        internal bool PreparePacket(ReadOnlySpan<byte> pcm, out byte[] target, out int length)
        {
            target = null;
            length = 0;

            if (this.IsDisposed)
                return false;

            var audioFormat = this.AudioFormat;

            var packetArray = ArrayPool<byte>.Shared.Rent(this.Rtp.CalculatePacketSize(audioFormat.SampleCountToSampleSize(audioFormat.CalculateMaximumFrameSize()), this.SelectedEncryptionMode));
            var packet = packetArray.AsSpan();

            this.Rtp.EncodeHeader(this.Sequence, this.Timestamp, this.SSRC, packet);
            var opus = packet.Slice(Rtp.HeaderSize, pcm.Length);
            this.Opus.Encode(pcm, ref opus);

            this.Sequence++;
            this.Timestamp += (uint)audioFormat.CalculateFrameSize(audioFormat.CalculateSampleDuration(pcm.Length));

            Span<byte> nonce = stackalloc byte[Sodium.NonceSize];
            switch (this.SelectedEncryptionMode)
            {
                case EncryptionMode.XSalsa20_Poly1305:
                    this.Sodium.GenerateNonce(packet.Slice(0, Rtp.HeaderSize), nonce);
                    break;

                case EncryptionMode.XSalsa20_Poly1305_Suffix:
                    this.Sodium.GenerateNonce(nonce);
                    break;

                case EncryptionMode.XSalsa20_Poly1305_Lite:
                    this.Sodium.GenerateNonce(this.Nonce++, nonce);
                    break;

                default:
                    ArrayPool<byte>.Shared.Return(packetArray);
                    throw new Exception("Unsupported encryption mode.");
            }

            Span<byte> encrypted = stackalloc byte[Sodium.CalculateTargetSize(opus)];
            this.Sodium.Encrypt(opus, encrypted, nonce);
            encrypted.CopyTo(packet.Slice(Rtp.HeaderSize));
            packet = packet.Slice(0, this.Rtp.CalculatePacketSize(encrypted.Length, this.SelectedEncryptionMode));
            this.Sodium.AppendNonce(nonce, packet, this.SelectedEncryptionMode);

            target = packetArray;
            length = packet.Length;
            return true;
        }

        /// <summary>
        /// Voices the sender task.
        /// </summary>
        /// <returns>A Task.</returns>
        private async Task VoiceSenderTask()
        {
            var token = this.SenderToken;
            var client = this.UdpClient;
            var reader = this.TransmitChannel.Reader;

            byte[] data = null;
            var length = 0;

            var synchronizerTicks = (double)Stopwatch.GetTimestamp();
            var synchronizerResolution = Stopwatch.Frequency * 0.005;
            var tickResolution = 10_000_000.0 / Stopwatch.Frequency;
            this.Discord.Logger.LogDebug(VoiceNextEvents.Misc, "Timer accuracy: {0}/{1} (high resolution? {2})", Stopwatch.Frequency, synchronizerResolution, Stopwatch.IsHighResolution);

            while (!token.IsCancellationRequested)
            {
                await this.PauseEvent.WaitAsync().ConfigureAwait(false);

                var hasPacket = reader.TryRead(out var rawPacket);
                if (hasPacket)
                {
                    this._queueCount--;

                    if (this.PlayingWait == null || this.PlayingWait.Task.IsCompleted)
                        this.PlayingWait = new TaskCompletionSource<bool>();
                }

                // Provided by Laura#0090 (214796473689178133); this is Python, but adaptable:
                //
                // delay = max(0, self.delay + ((start_time + self.delay * loops) + - time.time()))
                //
                // self.delay
                //   sample size
                // start_time
                //   time since streaming started
                // loops
                //   number of samples sent
                // time.time()
                //   DateTime.Now

                if (hasPacket)
                {
                    hasPacket = this.PreparePacket(rawPacket.Bytes.Span, out data, out length);
                    if (rawPacket.RentedBuffer != null)
                        ArrayPool<byte>.Shared.Return(rawPacket.RentedBuffer);
                }

                var durationModifier = hasPacket ? rawPacket.Duration / 5 : 4;
                var cts = Math.Max(Stopwatch.GetTimestamp() - synchronizerTicks, 0);
                if (cts < synchronizerResolution * durationModifier)
                    await Task.Delay(TimeSpan.FromTicks((long)(((synchronizerResolution * durationModifier) - cts) * tickResolution))).ConfigureAwait(false);

                synchronizerTicks += synchronizerResolution * durationModifier;

                if (!hasPacket)
                    continue;

                await this.SendSpeakingAsync(true).ConfigureAwait(false);
                await client.SendAsync(data, length).ConfigureAwait(false);
                ArrayPool<byte>.Shared.Return(data);

                if (!rawPacket.Silence && this._queueCount == 0)
                {
                    var nullpcm = new byte[this.AudioFormat.CalculateSampleSize(20)];
                    for (var i = 0; i < 3; i++)
                    {
                        var nullpacket = new byte[nullpcm.Length];
                        var nullpacketmem = nullpacket.AsMemory();
                        await this.EnqueuePacketAsync(new RawVoicePacket(nullpacketmem, 20, true)).ConfigureAwait(false);
                    }
                }
                else if (this._queueCount == 0)
                {
                    await this.SendSpeakingAsync(false).ConfigureAwait(false);
                    this.PlayingWait?.SetResult(true);
                }
            }
        }

        /// <summary>
        /// Processes the packet.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="opus">The opus.</param>
        /// <param name="pcm">The pcm.</param>
        /// <param name="pcmPackets">The pcm packets.</param>
        /// <param name="voiceSender">The voice sender.</param>
        /// <param name="outputFormat">The output format.</param>
        /// <returns>A bool.</returns>
        private bool ProcessPacket(ReadOnlySpan<byte> data, ref Memory<byte> opus, ref Memory<byte> pcm, IList<ReadOnlyMemory<byte>> pcmPackets, out AudioSender voiceSender, out AudioFormat outputFormat)
        {
            voiceSender = null;
            outputFormat = default;

            if (!this.Rtp.IsRtpHeader(data))
                return false;

            this.Rtp.DecodeHeader(data, out var sequence, out var timestamp, out var ssrc, out var hasExtension);

            if (!this.TransmittingSSRCs.TryGetValue(ssrc, out var vtx))
            {
                var decoder = this.Opus.CreateDecoder();

                vtx = new AudioSender(ssrc, decoder)
                {
                    // user isn't present as we haven't received a speaking event yet.
                    User = null
                };
            }

            voiceSender = vtx;
            if (sequence <= vtx.LastSequence) // out-of-order packet; discard
                return false;
            var gap = vtx.LastSequence != 0 ? sequence - 1 - vtx.LastSequence : 0;

            if (gap >= 5)
                this.Discord.Logger.LogWarning(VoiceNextEvents.VoiceReceiveFailure, "5 or more voice packets were dropped when receiving");

            Span<byte> nonce = stackalloc byte[Sodium.NonceSize];
            this.Sodium.GetNonce(data, nonce, this.SelectedEncryptionMode);
            this.Rtp.GetDataFromPacket(data, out var encryptedOpus, this.SelectedEncryptionMode);

            var opusSize = Sodium.CalculateSourceSize(encryptedOpus);
            opus = opus.Slice(0, opusSize);
            var opusSpan = opus.Span;
            try
            {
                this.Sodium.Decrypt(encryptedOpus, opusSpan, nonce);

                // Strip extensions, if any
                if (hasExtension)
                {
                    // RFC 5285, 4.2 One-Byte header
                    // http://www.rfcreader.com/#rfc5285_line186
                    if (opusSpan[0] == 0xBE && opusSpan[1] == 0xDE)
                    {
                        var headerLen = (opusSpan[2] << 8) | opusSpan[3];
                        var i = 4;
                        for (; i < headerLen + 4; i++)
                        {
                            var @byte = opusSpan[i];

                            // ID is currently unused since we skip it anyway
                            //var id = (byte)(@byte >> 4);
                            var length = (byte)(@byte & 0x0F) + 1;

                            i += length;
                        }

                        // Strip extension padding too
                        while (opusSpan[i] == 0)
                            i++;

                        opusSpan = opusSpan.Slice(i);
                    }

                    // TODO: consider implementing RFC 5285, 4.3. Two-Byte Header
                }

                if (opusSpan[0] == 0x90)
                {
                    // I'm not 100% sure what this header is/does, however removing the data causes no
                    // real issues, and has the added benefit of removing a lot of noise.
                    opusSpan = opusSpan.Slice(2);
                }

                if (gap == 1)
                {
                    var lastSampleCount = this.Opus.GetLastPacketSampleCount(vtx.Decoder);
                    var fecpcm = new byte[this.AudioFormat.SampleCountToSampleSize(lastSampleCount)];
                    var fecpcmMem = fecpcm.AsSpan();
                    this.Opus.Decode(vtx.Decoder, opusSpan, ref fecpcmMem, true, out _);
                    pcmPackets.Add(fecpcm.AsMemory(0, fecpcmMem.Length));
                }
                else if (gap > 1)
                {
                    var lastSampleCount = this.Opus.GetLastPacketSampleCount(vtx.Decoder);
                    for (var i = 0; i < gap; i++)
                    {
                        var fecpcm = new byte[this.AudioFormat.SampleCountToSampleSize(lastSampleCount)];
                        var fecpcmMem = fecpcm.AsSpan();
                        this.Opus.ProcessPacketLoss(vtx.Decoder, lastSampleCount, ref fecpcmMem);
                        pcmPackets.Add(fecpcm.AsMemory(0, fecpcmMem.Length));
                    }
                }

                var pcmSpan = pcm.Span;
                this.Opus.Decode(vtx.Decoder, opusSpan, ref pcmSpan, false, out outputFormat);
                pcm = pcm.Slice(0, pcmSpan.Length);
            }
            finally
            {
                vtx.LastSequence = sequence;
            }

            return true;
        }

        /// <summary>
        /// Processes the voice packet.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns>A Task.</returns>
        private async Task ProcessVoicePacket(byte[] data)
        {
            if (data.Length < 13) // minimum packet length
                return;

            try
            {
                var pcm = new byte[this.AudioFormat.CalculateMaximumFrameSize()];
                var pcmMem = pcm.AsMemory();
                var opus = new byte[pcm.Length];
                var opusMem = opus.AsMemory();
                var pcmFillers = new List<ReadOnlyMemory<byte>>();
                if (!this.ProcessPacket(data, ref opusMem, ref pcmMem, pcmFillers, out var vtx, out var audioFormat))
                    return;

                foreach (var pcmFiller in pcmFillers)
                    await this._voiceReceived.InvokeAsync(this, new VoiceReceiveEventArgs
                    {
                        SSRC = vtx.SSRC,
                        User = vtx.User,
                        PcmData = pcmFiller,
                        OpusData = new byte[0].AsMemory(),
                        AudioFormat = audioFormat,
                        AudioDuration = audioFormat.CalculateSampleDuration(pcmFiller.Length)
                    }).ConfigureAwait(false);

                await this._voiceReceived.InvokeAsync(this, new VoiceReceiveEventArgs
                {
                    SSRC = vtx.SSRC,
                    User = vtx.User,
                    PcmData = pcmMem,
                    OpusData = opusMem,
                    AudioFormat = audioFormat,
                    AudioDuration = audioFormat.CalculateSampleDuration(pcmMem.Length)
                }).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                this.Discord.Logger.LogError(VoiceNextEvents.VoiceReceiveFailure, ex, "Exception occurred when decoding incoming audio data");
            }
        }

        /// <summary>
        /// Processes the keepalive.
        /// </summary>
        /// <param name="data">The data.</param>
        private void ProcessKeepalive(byte[] data)
        {
            try
            {
                var keepalive = BinaryPrimitives.ReadUInt64LittleEndian(data);

                if (!this.KeepaliveTimestamps.TryRemove(keepalive, out var timestamp))
                    return;

                var tdelta = (int)((Stopwatch.GetTimestamp() - timestamp) / (double)Stopwatch.Frequency * 1000);
                this.Discord.Logger.LogDebug(VoiceNextEvents.VoiceKeepalive, "Received UDP keepalive {0} (ping {1}ms)", keepalive, tdelta);
                Volatile.Write(ref this._udpPing, tdelta);
            }
            catch (Exception ex)
            {
                this.Discord.Logger.LogError(VoiceNextEvents.VoiceKeepalive, ex, "Exception occurred when handling keepalive");
            }
        }

        /// <summary>
        /// Udps the receiver task.
        /// </summary>
        /// <returns>A Task.</returns>
        private async Task UdpReceiverTask()
        {
            var token = this.ReceiverToken;
            var client = this.UdpClient;

            while (!token.IsCancellationRequested)
            {
                var data = await client.ReceiveAsync().ConfigureAwait(false);
                if (data.Length == 8)
                    this.ProcessKeepalive(data);
                else if (this.Configuration.EnableIncoming)
                    await this.ProcessVoicePacket(data).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Sends a speaking status to the connected voice channel.
        /// </summary>
        /// <param name="speaking">Whether the current user is speaking or not.</param>
        /// <returns>A task representing the sending operation.</returns>
        public async Task SendSpeakingAsync(bool speaking = true)
        {
            if (!this.IsInitialized)
                throw new InvalidOperationException("The connection is not initialized");

            if (this._isSpeaking != speaking)
            {
                this._isSpeaking = speaking;
                var pld = new VoiceDispatch
                {
                    OpCode = 5,
                    Payload = new VoiceSpeakingPayload
                    {
                        Speaking = speaking,
                        Delay = 0
                    }
                };

                var plj = JsonConvert.SerializeObject(pld, Formatting.None);
                await this.WsSendAsync(plj).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Gets a transmit stream for this connection, optionally specifying a packet size to use with the stream. If a stream is already configured, it will return the existing one.
        /// </summary>
        /// <param name="sampleDuration">Duration, in ms, to use for audio packets.</param>
        /// <returns>Transmit stream.</returns>
        public VoiceTransmitSink GetTransmitSink(int sampleDuration = 20)
        {
            if (!AudioFormat.AllowedSampleDurations.Contains(sampleDuration))
                throw new ArgumentOutOfRangeException(nameof(sampleDuration), "Invalid PCM sample duration specified.");

            if (this.TransmitStream == null)
                this.TransmitStream = new VoiceTransmitSink(this, sampleDuration);

            return this.TransmitStream;
        }

        /// <summary>
        /// Asynchronously waits for playback to be finished. Playback is finished when speaking = false is signalled.
        /// </summary>
        /// <returns>A task representing the waiting operation.</returns>
        public async Task WaitForPlaybackFinishAsync()
        {
            if (this.PlayingWait != null)
                await this.PlayingWait.Task.ConfigureAwait(false);
        }

        /// <summary>
        /// Pauses playback.
        /// </summary>
        public void Pause()
            => this.PauseEvent.Reset();

        /// <summary>
        /// Asynchronously resumes playback.
        /// </summary>
        /// <returns></returns>
        public async Task ResumeAsync()
            => await this.PauseEvent.SetAsync().ConfigureAwait(false);

        /// <summary>
        /// Disconnects and disposes this voice connection.
        /// </summary>
        public void Disconnect()
            => this.Dispose();

        /// <summary>
        /// Disconnects and disposes this voice connection.
        /// </summary>
        public void Dispose()
        {
            if (this.IsDisposed)
                return;

            this.IsDisposed = true;
            this.IsInitialized = false;
            this.TokenSource.Cancel();
            this.SenderTokenSource.Cancel();
            this.ReceiverTokenSource?.Cancel();
            this.KeepaliveTokenSource.Cancel();
            this.TokenSource.Dispose();
            this.SenderTokenSource.Dispose();
            this.ReceiverTokenSource?.Dispose();
            this.KeepaliveTokenSource.Dispose();

            try
            {
                this.VoiceWs.DisconnectAsync().ConfigureAwait(false).GetAwaiter().GetResult();
                this.UdpClient.Close();
            }
            catch { }

            this.Opus?.Dispose();
            this.Opus = null;
            this.Sodium?.Dispose();
            this.Sodium = null;
            this.Rtp?.Dispose();
            this.Rtp = null;

            this.VoiceDisconnected?.Invoke(this.Guild);
        }

        /// <summary>
        /// Heartbeats .
        /// </summary>
        /// <returns>A Task.</returns>
        private async Task HeartbeatAsync()
        {
            await Task.Yield();

            var token = this.Token;
            while (true)
            {
                try
                {
                    token.ThrowIfCancellationRequested();

                    var dt = DateTime.Now;
                    this.Discord.Logger.LogTrace(VoiceNextEvents.VoiceHeartbeat, "Sent heartbeat");

                    var hbd = new VoiceDispatch
                    {
                        OpCode = 3,
                        Payload = UnixTimestamp(dt)
                    };
                    var hbj = JsonConvert.SerializeObject(hbd);
                    await this.WsSendAsync(hbj).ConfigureAwait(false);

                    this.LastHeartbeat = dt;
                    await Task.Delay(this.HeartbeatInterval).ConfigureAwait(false);
                }
                catch (OperationCanceledException)
                {
                    return;
                }
            }
        }

        /// <summary>
        /// Keepalives .
        /// </summary>
        /// <returns>A Task.</returns>
        private async Task KeepaliveAsync()
        {
            await Task.Yield();

            var token = this.KeepaliveToken;
            var client = this.UdpClient;

            while (!token.IsCancellationRequested)
            {
                var timestamp = Stopwatch.GetTimestamp();
                var keepalive = Volatile.Read(ref this._lastKeepalive);
                Volatile.Write(ref this._lastKeepalive, keepalive + 1);
                this.KeepaliveTimestamps.TryAdd(keepalive, timestamp);

                var packet = new byte[8];
                BinaryPrimitives.WriteUInt64LittleEndian(packet, keepalive);

                await client.SendAsync(packet, packet.Length).ConfigureAwait(false);

                await Task.Delay(5000, token).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Stage1S .
        /// </summary>
        /// <param name="voiceReady">The voice ready.</param>
        /// <returns>A Task.</returns>
        private async Task Stage1(VoiceReadyPayload voiceReady)
        {
            // IP Discovery
            this.UdpClient.Setup(this.UdpEndpoint);

            var pck = new byte[70];
            PreparePacket(pck);
            await this.UdpClient.SendAsync(pck, pck.Length).ConfigureAwait(false);

            var ipd = await this.UdpClient.ReceiveAsync().ConfigureAwait(false);
            ReadPacket(ipd, out var ip, out var port);
            this.DiscoveredEndpoint = new IpEndpoint
            {
                Address = ip,
                Port = port
            };
            this.Discord.Logger.LogTrace(VoiceNextEvents.VoiceHandshake, "Endpoint dicovery finished - discovered endpoint is {0}:{1}", ip, port);

            void PreparePacket(byte[] packet)
            {
                var ssrc = this.SSRC;
                var packetSpan = packet.AsSpan();
                MemoryMarshal.Write(packetSpan, ref ssrc);
                Helpers.ZeroFill(packetSpan);
            }

            void ReadPacket(byte[] packet, out System.Net.IPAddress decodedIp, out ushort decodedPort)
            {
                var packetSpan = packet.AsSpan();

                var ipString = Utilities.UTF8.GetString(packet, 4, 64 /* 70 - 6 */).TrimEnd('\0');
                decodedIp = System.Net.IPAddress.Parse(ipString);

                decodedPort = BinaryPrimitives.ReadUInt16LittleEndian(packetSpan.Slice(68 /* 70 - 2 */));
            }

            // Select voice encryption mode
            var selectedEncryptionMode = Sodium.SelectMode(voiceReady.Modes);
            this.SelectedEncryptionMode = selectedEncryptionMode.Value;

            // Ready
            this.Discord.Logger.LogTrace(VoiceNextEvents.VoiceHandshake, "Selected encryption mode is {0}", selectedEncryptionMode.Key);
            var vsp = new VoiceDispatch
            {
                OpCode = 1,
                Payload = new VoiceSelectProtocolPayload
                {
                    Protocol = "udp",
                    Data = new VoiceSelectProtocolPayloadData
                    {
                        Address = this.DiscoveredEndpoint.Address.ToString(),
                        Port = (ushort)this.DiscoveredEndpoint.Port,
                        Mode = selectedEncryptionMode.Key
                    }
                }
            };
            var vsj = JsonConvert.SerializeObject(vsp, Formatting.None);
            await this.WsSendAsync(vsj).ConfigureAwait(false);

            this.SenderTokenSource = new CancellationTokenSource();
            this.SenderTask = Task.Run(this.VoiceSenderTask, this.SenderToken);

            this.ReceiverTokenSource = new CancellationTokenSource();
            this.ReceiverTask = Task.Run(this.UdpReceiverTask, this.ReceiverToken);
        }

        /// <summary>
        /// Stage2S .
        /// </summary>
        /// <param name="voiceSessionDescription">The voice session description.</param>
        /// <returns>A Task.</returns>
        private async Task Stage2(VoiceSessionDescriptionPayload voiceSessionDescription)
        {
            this.SelectedEncryptionMode = Sodium.SupportedModes[voiceSessionDescription.Mode.ToLowerInvariant()];
            this.Discord.Logger.LogTrace(VoiceNextEvents.VoiceHandshake, "Discord updated encryption mode - new mode is {0}", this.SelectedEncryptionMode);

            // start keepalive
            this.KeepaliveTokenSource = new CancellationTokenSource();
            this.KeepaliveTask = this.KeepaliveAsync();

            // send 3 packets of silence to get things going
            var nullpcm = new byte[this.AudioFormat.CalculateSampleSize(20)];
            for (var i = 0; i < 3; i++)
            {
                var nullPcm = new byte[nullpcm.Length];
                var nullpacketmem = nullPcm.AsMemory();
                await this.EnqueuePacketAsync(new RawVoicePacket(nullpacketmem, 20, true)).ConfigureAwait(false);
            }

            this.IsInitialized = true;
            this.ReadyWait.SetResult(true);
        }

        /// <summary>
        /// Handles the dispatch.
        /// </summary>
        /// <param name="jo">The jo.</param>
        /// <returns>A Task.</returns>
        private async Task HandleDispatch(JObject jo)
        {
            var opc = (int)jo["op"];
            var opp = jo["d"] as JObject;

            switch (opc)
            {
                case 2: // READY
                    this.Discord.Logger.LogTrace(VoiceNextEvents.VoiceDispatch, "Received READY (OP2)");
                    var vrp = opp.ToObject<VoiceReadyPayload>();
                    this.SSRC = vrp.SSRC;
                    this.UdpEndpoint = new ConnectionEndpoint(vrp.Address, vrp.Port);
                    // this is not the valid interval
                    // oh, discord
                    //this.HeartbeatInterval = vrp.HeartbeatInterval;
                    this.HeartbeatTask = Task.Run(this.HeartbeatAsync);
                    await this.Stage1(vrp).ConfigureAwait(false);
                    break;

                case 4: // SESSION_DESCRIPTION
                    this.Discord.Logger.LogTrace(VoiceNextEvents.VoiceDispatch, "Received SESSION_DESCRIPTION (OP4)");
                    var vsd = opp.ToObject<VoiceSessionDescriptionPayload>();
                    this.Key = vsd.SecretKey;
                    this.Sodium = new Sodium(this.Key.AsMemory());
                    await this.Stage2(vsd).ConfigureAwait(false);
                    break;

                case 5: // SPEAKING
                    // Don't spam OP5
                    // No longer spam, Discord supposedly doesn't send many of these
                    this.Discord.Logger.LogTrace(VoiceNextEvents.VoiceDispatch, "Received SPEAKING (OP5)");
                    var spd = opp.ToObject<VoiceSpeakingPayload>();
                    var foundUserInCache = this.Discord.TryGetCachedUserInternal(spd.UserId.Value, out var resolvedUser);
                    var spk = new UserSpeakingEventArgs
                    {
                        Speaking = spd.Speaking,
                        SSRC = spd.SSRC.Value,
                        User = resolvedUser,
                    };

                    if (foundUserInCache && this.TransmittingSSRCs.TryGetValue(spk.SSRC, out var txssrc5) && txssrc5.Id == 0)
                    {
                        txssrc5.User = spk.User;
                    }
                    else
                    {
                        var opus = this.Opus.CreateDecoder();
                        var vtx = new AudioSender(spk.SSRC, opus)
                        {
                            User = await this.Discord.GetUserAsync(spd.UserId.Value).ConfigureAwait(false)
                        };

                        if (!this.TransmittingSSRCs.TryAdd(spk.SSRC, vtx))
                            this.Opus.DestroyDecoder(opus);
                    }

                    await this._userSpeaking.InvokeAsync(this, spk).ConfigureAwait(false);
                    break;

                case 6: // HEARTBEAT ACK
                    var dt = DateTime.Now;
                    var ping = (int)(dt - this.LastHeartbeat).TotalMilliseconds;
                    Volatile.Write(ref this._wsPing, ping);
                    this.Discord.Logger.LogTrace(VoiceNextEvents.VoiceDispatch, "Received HEARTBEAT_ACK (OP6, {0}ms)", ping);
                    this.LastHeartbeat = dt;
                    break;

                case 8: // HELLO
                    // this sends a heartbeat interval that we need to use for heartbeating
                    this.Discord.Logger.LogTrace(VoiceNextEvents.VoiceDispatch, "Received HELLO (OP8)");
                    this.HeartbeatInterval = opp["heartbeat_interval"].ToObject<int>();
                    break;

                case 9: // RESUMED
                    this.Discord.Logger.LogTrace(VoiceNextEvents.VoiceDispatch, "Received RESUMED (OP9)");
                    this.HeartbeatTask = Task.Run(this.HeartbeatAsync);
                    break;

                case 12: // CLIENT_CONNECTED
                    this.Discord.Logger.LogTrace(VoiceNextEvents.VoiceDispatch, "Received CLIENT_CONNECTED (OP12)");
                    var ujpd = opp.ToObject<VoiceUserJoinPayload>();
                    var usrj = await this.Discord.GetUserAsync(ujpd.UserId).ConfigureAwait(false);
                    {
                        var opus = this.Opus.CreateDecoder();
                        var vtx = new AudioSender(ujpd.SSRC, opus)
                        {
                            User = usrj
                        };

                        if (!this.TransmittingSSRCs.TryAdd(vtx.SSRC, vtx))
                            this.Opus.DestroyDecoder(opus);
                    }

                    await this._userJoined.InvokeAsync(this, new VoiceUserJoinEventArgs { User = usrj, SSRC = ujpd.SSRC }).ConfigureAwait(false);
                    break;

                case 13: // CLIENT_DISCONNECTED
                    this.Discord.Logger.LogTrace(VoiceNextEvents.VoiceDispatch, "Received CLIENT_DISCONNECTED (OP13)");
                    var ulpd = opp.ToObject<VoiceUserLeavePayload>();
                    var txssrc = this.TransmittingSSRCs.FirstOrDefault(x => x.Value.Id == ulpd.UserId);
                    if (this.TransmittingSSRCs.ContainsKey(txssrc.Key))
                    {
                        this.TransmittingSSRCs.TryRemove(txssrc.Key, out var txssrc13);
                        this.Opus.DestroyDecoder(txssrc13.Decoder);
                    }

                    var usrl = await this.Discord.GetUserAsync(ulpd.UserId).ConfigureAwait(false);
                    await this._userLeft.InvokeAsync(this, new VoiceUserLeaveEventArgs
                    {
                        User = usrl,
                        SSRC = txssrc.Key
                    }).ConfigureAwait(false);
                    break;

                default:
                    this.Discord.Logger.LogTrace(VoiceNextEvents.VoiceDispatch, "Received unknown voice opcode (OP{0})", opc);
                    break;
            }
        }

        /// <summary>
        /// Voices the w s_ socket closed.
        /// </summary>
        /// <param name="client">The client.</param>
        /// <param name="e">The e.</param>
        /// <returns>A Task.</returns>
        private async Task VoiceWS_SocketClosed(IWebSocketClient client, SocketCloseEventArgs e)
        {
            this.Discord.Logger.LogDebug(VoiceNextEvents.VoiceConnectionClose, "Voice WebSocket closed ({0}, '{1}')", e.CloseCode, e.CloseMessage);

            // generally this should not be disposed on all disconnects, only on requested ones
            // or something
            // otherwise problems happen
            //this.Dispose();

            if (e.CloseCode == 4006 || e.CloseCode == 4009)
                this.Resume = false;

            if (!this.IsDisposed)
            {
                this.TokenSource.Cancel();
                this.TokenSource = new CancellationTokenSource();
                this.VoiceWs = this.Discord.Configuration.WebSocketClientFactory(this.Discord.Configuration.Proxy);
                this.VoiceWs.Disconnected += this.VoiceWS_SocketClosed;
                this.VoiceWs.MessageReceived += this.VoiceWS_SocketMessage;
                this.VoiceWs.Connected += this.VoiceWS_SocketOpened;

                if (this.Resume) // emzi you dipshit
                    await this.ConnectAsync().ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Voices the w s_ socket message.
        /// </summary>
        /// <param name="client">The client.</param>
        /// <param name="e">The e.</param>
        /// <returns>A Task.</returns>
        private Task VoiceWS_SocketMessage(IWebSocketClient client, SocketMessageEventArgs e)
        {
            if (e is not SocketTextMessageEventArgs et)
            {
                this.Discord.Logger.LogCritical(VoiceNextEvents.VoiceGatewayError, "Discord Voice Gateway sent binary data - unable to process");
                return Task.CompletedTask;
            }

            this.Discord.Logger.LogTrace(VoiceNextEvents.VoiceWsRx, et.Message);
            return this.HandleDispatch(JObject.Parse(et.Message));
        }

        /// <summary>
        /// Voices the w s_ socket opened.
        /// </summary>
        /// <param name="client">The client.</param>
        /// <param name="e">The e.</param>
        /// <returns>A Task.</returns>
        private Task VoiceWS_SocketOpened(IWebSocketClient client, SocketEventArgs e)
            => this.StartAsync();

        /// <summary>
        /// Voices the ws_ socket exception.
        /// </summary>
        /// <param name="client">The client.</param>
        /// <param name="e">The e.</param>
        /// <returns>A Task.</returns>
        private Task VoiceWs_SocketException(IWebSocketClient client, SocketErrorEventArgs e)
            => this._voiceSocketError.InvokeAsync(this, new SocketErrorEventArgs { Exception = e.Exception });

        /// <summary>
        /// Ws the send async.
        /// </summary>
        /// <param name="payload">The payload.</param>
        /// <returns>A Task.</returns>
        private async Task WsSendAsync(string payload)
        {
            this.Discord.Logger.LogTrace(VoiceNextEvents.VoiceWsTx, payload);
            await this.VoiceWs.SendMessageAsync(payload).ConfigureAwait(false);
        }

        /// <summary>
        /// Gets the unix timestamp.
        /// </summary>
        /// <param name="dt">The datetine.</param>
        private static uint UnixTimestamp(DateTime dt)
        {
            var ts = dt - UnixEpoch;
            var sd = ts.TotalSeconds;
            var si = (uint)sd;
            return si;
        }
    }
}
