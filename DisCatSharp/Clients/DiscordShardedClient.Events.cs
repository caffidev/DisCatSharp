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
using System.Threading.Tasks;
using DisCatSharp.EventArgs;
using DisCatSharp.Common.Utilities;
using Microsoft.Extensions.Logging;

namespace DisCatSharp
{
    /// <summary>
    /// Represents a discord sharded client.
    /// </summary>
    public sealed partial class DiscordShardedClient
    {
        #region WebSocket

        /// <summary>
        /// Fired whenever a WebSocket error occurs within the client.
        /// </summary>
        public event AsyncEventHandler<DiscordClient, SocketErrorEventArgs> SocketErrored
        {
            add => this._socketErrored.Register(value);
            remove => this._socketErrored.Unregister(value);
        }
        private AsyncEvent<DiscordClient, SocketErrorEventArgs> _socketErrored;

        /// <summary>
        /// Fired whenever WebSocket connection is established.
        /// </summary>
        public event AsyncEventHandler<DiscordClient, SocketEventArgs> SocketOpened
        {
            add => this._socketOpened.Register(value);
            remove => this._socketOpened.Unregister(value);
        }
        private AsyncEvent<DiscordClient, SocketEventArgs> _socketOpened;

        /// <summary>
        /// Fired whenever WebSocket connection is terminated.
        /// </summary>
        public event AsyncEventHandler<DiscordClient, SocketCloseEventArgs> SocketClosed
        {
            add => this._socketClosed.Register(value);
            remove => this._socketClosed.Unregister(value);
        }
        private AsyncEvent<DiscordClient, SocketCloseEventArgs> _socketClosed;

        /// <summary>
        /// Fired when the client enters ready state.
        /// </summary>
        public event AsyncEventHandler<DiscordClient, ReadyEventArgs> Ready
        {
            add => this._ready.Register(value);
            remove => this._ready.Unregister(value);
        }
        private AsyncEvent<DiscordClient, ReadyEventArgs> _ready;

        /// <summary>
        /// Fired whenever a session is resumed.
        /// </summary>
        public event AsyncEventHandler<DiscordClient, ReadyEventArgs> Resumed
        {
            add => this._resumed.Register(value);
            remove => this._resumed.Unregister(value);
        }
        private AsyncEvent<DiscordClient, ReadyEventArgs> _resumed;

        /// <summary>
        /// Fired on received heartbeat ACK.
        /// </summary>
        public event AsyncEventHandler<DiscordClient, HeartbeatEventArgs> Heartbeated
        {
            add => this._heartbeated.Register(value);
            remove => this._heartbeated.Unregister(value);
        }
        private AsyncEvent<DiscordClient, HeartbeatEventArgs> _heartbeated;

        #endregion

        #region Channel

        /// <summary>
        /// Fired when a new channel is created.
        /// For this Event you need the <see cref="DiscordIntents.Guilds"/> intent specified in <seealso cref="DiscordConfiguration.Intents"/>
        /// </summary>
        public event AsyncEventHandler<DiscordClient, ChannelCreateEventArgs> ChannelCreated
        {
            add => this._channelCreated.Register(value);
            remove => this._channelCreated.Unregister(value);
        }
        private AsyncEvent<DiscordClient, ChannelCreateEventArgs> _channelCreated;

        /// <summary>
        /// Fired when a channel is updated.
        /// For this Event you need the <see cref="DiscordIntents.Guilds"/> intent specified in <seealso cref="DiscordConfiguration.Intents"/>
        /// </summary>
        public event AsyncEventHandler<DiscordClient, ChannelUpdateEventArgs> ChannelUpdated
        {
            add => this._channelUpdated.Register(value);
            remove => this._channelUpdated.Unregister(value);
        }
        private AsyncEvent<DiscordClient, ChannelUpdateEventArgs> _channelUpdated;

        /// <summary>
        /// Fired when a channel is deleted
        /// For this Event you need the <see cref="DiscordIntents.Guilds"/> intent specified in <seealso cref="DiscordConfiguration.Intents"/>
        /// </summary>
        public event AsyncEventHandler<DiscordClient, ChannelDeleteEventArgs> ChannelDeleted
        {
            add => this._channelDeleted.Register(value);
            remove => this._channelDeleted.Unregister(value);
        }
        private AsyncEvent<DiscordClient, ChannelDeleteEventArgs> _channelDeleted;

        /// <summary>
        /// Fired when a dm channel is deleted
        /// For this Event you need the <see cref="DiscordIntents.DirectMessages"/> intent specified in <seealso cref="DiscordConfiguration.Intents"/>
        /// </summary>
        public event AsyncEventHandler<DiscordClient, DmChannelDeleteEventArgs> DmChannelDeleted
        {
            add => this._dmChannelDeleted.Register(value);
            remove => this._dmChannelDeleted.Unregister(value);
        }
        private AsyncEvent<DiscordClient, DmChannelDeleteEventArgs> _dmChannelDeleted;

        /// <summary>
        /// Fired whenever a channel's pinned message list is updated.
        /// For this Event you need the <see cref="DiscordIntents.Guilds"/> intent specified in <seealso cref="DiscordConfiguration.Intents"/>
        /// </summary>
        public event AsyncEventHandler<DiscordClient, ChannelPinsUpdateEventArgs> ChannelPinsUpdated
        {
            add => this._channelPinsUpdated.Register(value);
            remove => this._channelPinsUpdated.Unregister(value);
        }
        private AsyncEvent<DiscordClient, ChannelPinsUpdateEventArgs> _channelPinsUpdated;

        #endregion

        #region Guild

        /// <summary>
        /// Fired when the user joins a new guild.
        /// For this Event you need the <see cref="DiscordIntents.Guilds"/> intent specified in <seealso cref="DiscordConfiguration.Intents"/>
        /// </summary>
        /// <remarks>[alias="GuildJoined"][alias="JoinedGuild"]</remarks>
        public event AsyncEventHandler<DiscordClient, GuildCreateEventArgs> GuildCreated
        {
            add => this._guildCreated.Register(value);
            remove => this._guildCreated.Unregister(value);
        }
        private AsyncEvent<DiscordClient, GuildCreateEventArgs> _guildCreated;

        /// <summary>
        /// Fired when a guild is becoming available.
        /// </summary>
        public event AsyncEventHandler<DiscordClient, GuildCreateEventArgs> GuildAvailable
        {
            add => this._guildAvailable.Register(value);
            remove => this._guildAvailable.Unregister(value);
        }
        private AsyncEvent<DiscordClient, GuildCreateEventArgs> _guildAvailable;

        /// <summary>
        /// Fired when a guild is updated.
        /// For this Event you need the <see cref="DiscordIntents.Guilds"/> intent specified in <seealso cref="DiscordConfiguration.Intents"/>
        /// </summary>
        public event AsyncEventHandler<DiscordClient, GuildUpdateEventArgs> GuildUpdated
        {
            add => this._guildUpdated.Register(value);
            remove => this._guildUpdated.Unregister(value);
        }
        private AsyncEvent<DiscordClient, GuildUpdateEventArgs> _guildUpdated;

        /// <summary>
        /// Fired when the user leaves or is removed from a guild.
        /// For this Event you need the <see cref="DiscordIntents.Guilds"/> intent specified in <seealso cref="DiscordConfiguration.Intents"/>
        /// </summary>
        public event AsyncEventHandler<DiscordClient, GuildDeleteEventArgs> GuildDeleted
        {
            add => this._guildDeleted.Register(value);
            remove => this._guildDeleted.Unregister(value);
        }
        private AsyncEvent<DiscordClient, GuildDeleteEventArgs> _guildDeleted;

        /// <summary>
        /// Fired when a guild becomes unavailable.
        /// </summary>
        public event AsyncEventHandler<DiscordClient, GuildDeleteEventArgs> GuildUnavailable
        {
            add => this._guildUnavailable.Register(value);
            remove => this._guildUnavailable.Unregister(value);
        }
        private AsyncEvent<DiscordClient, GuildDeleteEventArgs> _guildUnavailable;

        /// <summary>
        /// Fired when all guilds finish streaming from Discord.
        /// </summary>
        public event AsyncEventHandler<DiscordClient, GuildDownloadCompletedEventArgs> GuildDownloadCompleted
        {
            add => this._guildDownloadCompleted.Register(value);
            remove => this._guildDownloadCompleted.Unregister(value);
        }
        private AsyncEvent<DiscordClient, GuildDownloadCompletedEventArgs> _guildDownloadCompleted;

        /// <summary>
        /// Fired when a guilds emojis get updated
        /// For this Event you need the <see cref="DiscordIntents.GuildEmojisAndStickers"/> intent specified in <seealso cref="DiscordConfiguration.Intents"/>
        /// </summary>
        public event AsyncEventHandler<DiscordClient, GuildEmojisUpdateEventArgs> GuildEmojisUpdated
        {
            add => this._guildEmojisUpdated.Register(value);
            remove => this._guildEmojisUpdated.Unregister(value);
        }
        private AsyncEvent<DiscordClient, GuildEmojisUpdateEventArgs> _guildEmojisUpdated;

        /// <summary>
        /// Fired when a guilds stickers get updated
        /// For this Event you need the <see cref="DiscordIntents.GuildEmojisAndStickers"/> intent specified in <seealso cref="DiscordConfiguration.Intents"/>
        /// </summary>
        public event AsyncEventHandler<DiscordClient, GuildStickersUpdateEventArgs> GuildStickersUpdated
        {
            add => this._guildStickersUpdated.Register(value);
            remove => this._guildStickersUpdated.Unregister(value);
        }
        private AsyncEvent<DiscordClient, GuildStickersUpdateEventArgs> _guildStickersUpdated;

        /// <summary>
        /// Fired when a guild integration is updated.
        /// </summary>
        public event AsyncEventHandler<DiscordClient, GuildIntegrationsUpdateEventArgs> GuildIntegrationsUpdated
        {
            add => this._guildIntegrationsUpdated.Register(value);
            remove => this._guildIntegrationsUpdated.Unregister(value);
        }
        private AsyncEvent<DiscordClient, GuildIntegrationsUpdateEventArgs> _guildIntegrationsUpdated;

        #endregion

        #region Guild Ban

        /// <summary>
        /// Fired when a guild ban gets added
        /// For this Event you need the <see cref="DiscordIntents.GuildBans"/> intent specified in <seealso cref="DiscordConfiguration.Intents"/>
        /// </summary>
        public event AsyncEventHandler<DiscordClient, GuildBanAddEventArgs> GuildBanAdded
        {
            add => this._guildBanAdded.Register(value);
            remove => this._guildBanAdded.Unregister(value);
        }
        private AsyncEvent<DiscordClient, GuildBanAddEventArgs> _guildBanAdded;

        /// <summary>
        /// Fired when a guild ban gets removed
        /// For this Event you need the <see cref="DiscordIntents.GuildBans"/> intent specified in <seealso cref="DiscordConfiguration.Intents"/>
        /// </summary>
        public event AsyncEventHandler<DiscordClient, GuildBanRemoveEventArgs> GuildBanRemoved
        {
            add => this._guildBanRemoved.Register(value);
            remove => this._guildBanRemoved.Unregister(value);
        }
        private AsyncEvent<DiscordClient, GuildBanRemoveEventArgs> _guildBanRemoved;

        #endregion

        #region Guild Event

        /// <summary>
        /// Fired when a Sheduled Event is created.
        /// For this Event you need the <see cref="DiscordIntents.Guilds"/> intent specified in <seealso cref="DiscordConfiguration.Intents"/>
        /// </summary>
        public event AsyncEventHandler<DiscordClient, GuildSheduledEventCreateEventArgs> GuildSheduledEventCreated
        {
            add => this._guildSheduledEventCreated.Register(value);
            remove => this._guildSheduledEventCreated.Unregister(value);
        }
        private AsyncEvent<DiscordClient, GuildSheduledEventCreateEventArgs> _guildSheduledEventCreated;

        /// <summary>
        /// Fired when a Sheduled Event is updated.
        /// For this Event you need the <see cref="DiscordIntents.Guilds"/> intent specified in <seealso cref="DiscordConfiguration.Intents"/>
        /// </summary>
        public event AsyncEventHandler<DiscordClient, GuildSheduledEventUpdateEventArgs> GuildSheduledEventUpdated
        {
            add => this._guildSheduledEventUpdated.Register(value);
            remove => this._guildSheduledEventUpdated.Unregister(value);
        }
        private AsyncEvent<DiscordClient, GuildSheduledEventUpdateEventArgs> _guildSheduledEventUpdated;

        /// <summary>
        /// Fired when a Sheduled Event is deleted.
        /// For this Event you need the <see cref="DiscordIntents.Guilds"/> intent specified in <seealso cref="DiscordConfiguration.Intents"/>
        /// </summary>
        public event AsyncEventHandler<DiscordClient, GuildSheduledEventDeleteEventArgs> GuildSheduledEventDeleted
        {
            add => this._guildSheduledEventDeleted.Register(value);
            remove => this._guildSheduledEventDeleted.Unregister(value);
        }
        private AsyncEvent<DiscordClient, GuildSheduledEventDeleteEventArgs> _guildSheduledEventDeleted;

        #endregion

        #region Guild Integration

        /// <summary>
        /// Fired when a guild integration is created.
        /// For this Event you need the <see cref="DiscordIntents.GuildIntegrations"/> intent specified in <seealso cref="DiscordConfiguration.Intents"/>
        /// </summary>
        public event AsyncEventHandler<DiscordClient, GuildIntegrationCreateEventArgs> GuildIntegrationCreated
        {
            add => this._guildIntegrationCreated.Register(value);
            remove => this._guildIntegrationCreated.Unregister(value);
        }
        private AsyncEvent<DiscordClient, GuildIntegrationCreateEventArgs> _guildIntegrationCreated;

        /// <summary>
        /// Fired when a guild integration is updated.
        /// For this Event you need the <see cref="DiscordIntents.GuildIntegrations"/> intent specified in <seealso cref="DiscordConfiguration.Intents"/>
        /// </summary>
        public event AsyncEventHandler<DiscordClient, GuildIntegrationUpdateEventArgs> GuildIntegrationUpdated
        {
            add => this._guildIntegrationUpdated.Register(value);
            remove => this._guildIntegrationUpdated.Unregister(value);
        }
        private AsyncEvent<DiscordClient, GuildIntegrationUpdateEventArgs> _guildIntegrationUpdated;

        /// <summary>
        /// Fired when a guild integration is deleted.
        /// For this Event you need the <see cref="DiscordIntents.GuildIntegrations"/> intent specified in <seealso cref="DiscordConfiguration.Intents"/>
        /// </summary>
        public event AsyncEventHandler<DiscordClient, GuildIntegrationDeleteEventArgs> GuildIntegrationDeleted
        {
            add => this._guildIntegrationDeleted.Register(value);
            remove => this._guildIntegrationDeleted.Unregister(value);
        }
        private AsyncEvent<DiscordClient, GuildIntegrationDeleteEventArgs> _guildIntegrationDeleted;

        #endregion

        #region Guild Member

        /// <summary>
        /// Fired when a new user joins a guild.
        /// For this Event you need the <see cref="DiscordIntents.GuildMembers"/> intent specified in <seealso cref="DiscordConfiguration.Intents"/>
        /// </summary>
        public event AsyncEventHandler<DiscordClient, GuildMemberAddEventArgs> GuildMemberAdded
        {
            add => this._guildMemberAdded.Register(value);
            remove => this._guildMemberAdded.Unregister(value);
        }
        private AsyncEvent<DiscordClient, GuildMemberAddEventArgs> _guildMemberAdded;

        /// <summary>
        /// Fired when a user is removed from a guild (leave/kick/ban).
        /// For this Event you need the <see cref="DiscordIntents.GuildMembers"/> intent specified in <seealso cref="DiscordConfiguration.Intents"/>
        /// </summary>
        public event AsyncEventHandler<DiscordClient, GuildMemberRemoveEventArgs> GuildMemberRemoved
        {
            add => this._guildMemberRemoved.Register(value);
            remove => this._guildMemberRemoved.Unregister(value);
        }
        private AsyncEvent<DiscordClient, GuildMemberRemoveEventArgs> _guildMemberRemoved;

        /// <summary>
        /// Fired when a guild member is updated.
        /// For this Event you need the <see cref="DiscordIntents.GuildMembers"/> intent specified in <seealso cref="DiscordConfiguration.Intents"/>
        /// </summary>
        public event AsyncEventHandler<DiscordClient, GuildMemberUpdateEventArgs> GuildMemberUpdated
        {
            add => this._guildMemberUpdated.Register(value);
            remove => this._guildMemberUpdated.Unregister(value);
        }
        private AsyncEvent<DiscordClient, GuildMemberUpdateEventArgs> _guildMemberUpdated;

        /// <summary>
        /// Fired in response to Gateway Request Guild Members.
        /// </summary>
        public event AsyncEventHandler<DiscordClient, GuildMembersChunkEventArgs> GuildMembersChunked
        {
            add => this._guildMembersChunk.Register(value);
            remove => this._guildMembersChunk.Unregister(value);
        }
        private AsyncEvent<DiscordClient, GuildMembersChunkEventArgs> _guildMembersChunk;

        #endregion

        #region Guild Role

        /// <summary>
        /// Fired when a guild role is created.
        /// For this Event you need the <see cref="DiscordIntents.Guilds"/> intent specified in <seealso cref="DiscordConfiguration.Intents"/>
        /// </summary>
        public event AsyncEventHandler<DiscordClient, GuildRoleCreateEventArgs> GuildRoleCreated
        {
            add => this._guildRoleCreated.Register(value);
            remove => this._guildRoleCreated.Unregister(value);
        }
        private AsyncEvent<DiscordClient, GuildRoleCreateEventArgs> _guildRoleCreated;

        /// <summary>
        /// Fired when a guild role is updated.
        /// For this Event you need the <see cref="DiscordIntents.Guilds"/> intent specified in <seealso cref="DiscordConfiguration.Intents"/>
        /// </summary>
        public event AsyncEventHandler<DiscordClient, GuildRoleUpdateEventArgs> GuildRoleUpdated
        {
            add => this._guildRoleUpdated.Register(value);
            remove => this._guildRoleUpdated.Unregister(value);
        }
        private AsyncEvent<DiscordClient, GuildRoleUpdateEventArgs> _guildRoleUpdated;

        /// <summary>
        /// Fired when a guild role is updated.
        /// For this Event you need the <see cref="DiscordIntents.Guilds"/> intent specified in <seealso cref="DiscordConfiguration.Intents"/>
        /// </summary>
        public event AsyncEventHandler<DiscordClient, GuildRoleDeleteEventArgs> GuildRoleDeleted
        {
            add => this._guildRoleDeleted.Register(value);
            remove => this._guildRoleDeleted.Unregister(value);
        }
        private AsyncEvent<DiscordClient, GuildRoleDeleteEventArgs> _guildRoleDeleted;

        #endregion

        #region Invite

        /// <summary>
        /// Fired when an invite is created.
        /// For this Event you need the <see cref="DiscordIntents.GuildInvites"/> intent specified in <seealso cref="DiscordConfiguration.Intents"/>
        /// </summary>
        public event AsyncEventHandler<DiscordClient, InviteCreateEventArgs> InviteCreated
        {
            add => this._inviteCreated.Register(value);
            remove => this._inviteCreated.Unregister(value);
        }
        private AsyncEvent<DiscordClient, InviteCreateEventArgs> _inviteCreated;

        /// <summary>
        /// Fired when an invite is deleted.
        /// For this Event you need the <see cref="DiscordIntents.GuildInvites"/> intent specified in <seealso cref="DiscordConfiguration.Intents"/>
        /// </summary>
        public event AsyncEventHandler<DiscordClient, InviteDeleteEventArgs> InviteDeleted
        {
            add => this._inviteDeleted.Register(value);
            remove => this._inviteDeleted.Unregister(value);
        }
        private AsyncEvent<DiscordClient, InviteDeleteEventArgs> _inviteDeleted;

        #endregion

        #region Message

        /// <summary>
        /// Fired when a message is created.
        /// For this Event you need the <see cref="DiscordIntents.GuildMessages"/> intent specified in <seealso cref="DiscordConfiguration.Intents"/>
        /// </summary>
        public event AsyncEventHandler<DiscordClient, MessageCreateEventArgs> MessageCreated
        {
            add => this._messageCreated.Register(value);
            remove => this._messageCreated.Unregister(value);
        }
        private AsyncEvent<DiscordClient, MessageCreateEventArgs> _messageCreated;

        /// <summary>
        /// Fired when a message is updated.
        /// For this Event you need the <see cref="DiscordIntents.GuildMessages"/> intent specified in <seealso cref="DiscordConfiguration.Intents"/>
        /// </summary>
        public event AsyncEventHandler<DiscordClient, MessageUpdateEventArgs> MessageUpdated
        {
            add => this._messageUpdated.Register(value);
            remove => this._messageUpdated.Unregister(value);
        }
        private AsyncEvent<DiscordClient, MessageUpdateEventArgs> _messageUpdated;

        /// <summary>
        /// Fired when a message is deleted.
        /// For this Event you need the <see cref="DiscordIntents.GuildMessages"/> intent specified in <seealso cref="DiscordConfiguration.Intents"/>
        /// </summary>
        public event AsyncEventHandler<DiscordClient, MessageDeleteEventArgs> MessageDeleted
        {
            add => this._messageDeleted.Register(value);
            remove => this._messageDeleted.Unregister(value);
        }
        private AsyncEvent<DiscordClient, MessageDeleteEventArgs> _messageDeleted;

        /// <summary>
        /// Fired when multiple messages are deleted at once.
        /// For this Event you need the <see cref="DiscordIntents.GuildMessages"/> intent specified in <seealso cref="DiscordConfiguration.Intents"/>
        /// </summary>
        public event AsyncEventHandler<DiscordClient, MessageBulkDeleteEventArgs> MessagesBulkDeleted
        {
            add => this._messageBulkDeleted.Register(value);
            remove => this._messageBulkDeleted.Unregister(value);
        }
        private AsyncEvent<DiscordClient, MessageBulkDeleteEventArgs> _messageBulkDeleted;

        #endregion

        #region Message Reaction

        /// <summary>
        /// Fired when a reaction gets added to a message.
        /// For this Event you need the <see cref="DiscordIntents.GuildMessageReactions"/> intent specified in <seealso cref="DiscordConfiguration.Intents"/>
        /// </summary>
        public event AsyncEventHandler<DiscordClient, MessageReactionAddEventArgs> MessageReactionAdded
        {
            add => this._messageReactionAdded.Register(value);
            remove => this._messageReactionAdded.Unregister(value);
        }
        private AsyncEvent<DiscordClient, MessageReactionAddEventArgs> _messageReactionAdded;

        /// <summary>
        /// Fired when a reaction gets removed from a message.
        /// For this Event you need the <see cref="DiscordIntents.GuildMessageReactions"/> intent specified in <seealso cref="DiscordConfiguration.Intents"/>
        /// </summary>
        public event AsyncEventHandler<DiscordClient, MessageReactionRemoveEventArgs> MessageReactionRemoved
        {
            add => this._messageReactionRemoved.Register(value);
            remove => this._messageReactionRemoved.Unregister(value);
        }
        private AsyncEvent<DiscordClient, MessageReactionRemoveEventArgs> _messageReactionRemoved;

        /// <summary>
        /// Fired when all reactions get removed from a message.
        /// For this Event you need the <see cref="DiscordIntents.GuildMessageReactions"/> intent specified in <seealso cref="DiscordConfiguration.Intents"/>
        /// </summary>
        public event AsyncEventHandler<DiscordClient, MessageReactionsClearEventArgs> MessageReactionsCleared
        {
            add => this._messageReactionsCleared.Register(value);
            remove => this._messageReactionsCleared.Unregister(value);
        }
        private AsyncEvent<DiscordClient, MessageReactionsClearEventArgs> _messageReactionsCleared;

        /// <summary>
        /// Fired when all reactions of a specific reaction are removed from a message.
        /// For this Event you need the <see cref="DiscordIntents.GuildMessageReactions"/> intent specified in <seealso cref="DiscordConfiguration.Intents"/>
        /// </summary>
        public event AsyncEventHandler<DiscordClient, MessageReactionRemoveEmojiEventArgs> MessageReactionRemovedEmoji
        {
            add => this._messageReactionRemovedEmoji.Register(value);
            remove => this._messageReactionRemovedEmoji.Unregister(value);
        }
        private AsyncEvent<DiscordClient, MessageReactionRemoveEmojiEventArgs> _messageReactionRemovedEmoji;

        #endregion

        #region Stage Instance

        /// <summary>
        /// Fired when a Stage Instance is created.
        /// For this Event you need the <see cref="DiscordIntents.Guilds"/> intent specified in <seealso cref="DiscordConfiguration.Intents"/>
        /// </summary>
        public event AsyncEventHandler<DiscordClient, StageInstanceCreateEventArgs> StageInstanceCreated
        {
            add => this._stageInstanceCreated.Register(value);
            remove => this._stageInstanceCreated.Unregister(value);
        }
        private AsyncEvent<DiscordClient, StageInstanceCreateEventArgs> _stageInstanceCreated;

        /// <summary>
        /// Fired when a Stage Instance is updated.
        /// For this Event you need the <see cref="DiscordIntents.Guilds"/> intent specified in <seealso cref="DiscordConfiguration.Intents"/>
        /// </summary>
        public event AsyncEventHandler<DiscordClient, StageInstanceUpdateEventArgs> StageInstanceUpdated
        {
            add => this._stageInstanceUpdated.Register(value);
            remove => this._stageInstanceUpdated.Unregister(value);
        }
        private AsyncEvent<DiscordClient, StageInstanceUpdateEventArgs> _stageInstanceUpdated;

        /// <summary>
        /// Fired when a Stage Instance is deleted.
        /// For this Event you need the <see cref="DiscordIntents.Guilds"/> intent specified in <seealso cref="DiscordConfiguration.Intents"/>
        /// </summary>
        public event AsyncEventHandler<DiscordClient, StageInstanceDeleteEventArgs> StageInstanceDeleted
        {
            add => this._stageInstanceDeleted.Register(value);
            remove => this._stageInstanceDeleted.Unregister(value);
        }
        private AsyncEvent<DiscordClient, StageInstanceDeleteEventArgs> _stageInstanceDeleted;

        #endregion

        #region Thread

        /// <summary>
        /// Fired when a thread is created.
        /// For this Event you need the <see cref="DiscordIntents.Guilds"/> intent specified in <seealso cref="DiscordConfiguration.Intents"/>
        /// </summary>
        public event AsyncEventHandler<DiscordClient, ThreadCreateEventArgs> ThreadCreated
        {
            add => this._threadCreated.Register(value);
            remove => this._threadCreated.Unregister(value);
        }
        private AsyncEvent<DiscordClient, ThreadCreateEventArgs> _threadCreated;

        /// <summary>
        /// Fired when a thread is updated.
        /// For this Event you need the <see cref="DiscordIntents.Guilds"/> intent specified in <seealso cref="DiscordConfiguration.Intents"/>
        /// </summary>
        public event AsyncEventHandler<DiscordClient, ThreadUpdateEventArgs> ThreadUpdated
        {
            add => this._threadUpdated.Register(value);
            remove => this._threadUpdated.Unregister(value);
        }
        private AsyncEvent<DiscordClient, ThreadUpdateEventArgs> _threadUpdated;

        /// <summary>
        /// Fired when a thread is deleted.
        /// For this Event you need the <see cref="DiscordIntents.Guilds"/> intent specified in <seealso cref="DiscordConfiguration.Intents"/>
        /// </summary>
        public event AsyncEventHandler<DiscordClient, ThreadDeleteEventArgs> ThreadDeleted
        {
            add => this._threadDeleted.Register(value);
            remove => this._threadDeleted.Unregister(value);
        }
        private AsyncEvent<DiscordClient, ThreadDeleteEventArgs> _threadDeleted;

        /// <summary>
        /// Fired when a thread member is updated.
        /// For this Event you need the <see cref="DiscordIntents.Guilds"/> intent specified in <seealso cref="DiscordConfiguration.Intents"/>
        /// </summary>
        public event AsyncEventHandler<DiscordClient, ThreadListSyncEventArgs> ThreadListSynced
        {
            add => this._threadListSynced.Register(value);
            remove => this._threadListSynced.Unregister(value);
        }
        private AsyncEvent<DiscordClient, ThreadListSyncEventArgs> _threadListSynced;

        /// <summary>
        /// Fired when a thread member is updated.
        /// For this Event you need the <see cref="DiscordIntents.Guilds"/> intent specified in <seealso cref="DiscordConfiguration.Intents"/>
        /// </summary>
        public event AsyncEventHandler<DiscordClient, ThreadMemberUpdateEventArgs> ThreadMemberUpdated
        {
            add => this._threadMemberUpdated.Register(value);
            remove => this._threadMemberUpdated.Unregister(value);
        }
        private AsyncEvent<DiscordClient, ThreadMemberUpdateEventArgs> _threadMemberUpdated;

        /// <summary>
        /// Fired when the thread members are updated.
        /// For this Event you need the <see cref="DiscordIntents.GuildMembers"/> or <see cref="DiscordIntents.Guilds"/> intent specified in <seealso cref="DiscordConfiguration.Intents"/>
        /// </summary>
        public event AsyncEventHandler<DiscordClient, ThreadMembersUpdateEventArgs> ThreadMembersUpdated
        {
            add => this._threadMembersUpdated.Register(value);
            remove => this._threadMembersUpdated.Unregister(value);
        }
        private AsyncEvent<DiscordClient, ThreadMembersUpdateEventArgs> _threadMembersUpdated;

        #endregion

        #region User/Presence Update

        /// <summary>
        /// Fired when a presence has been updated.
        /// For this Event you need the <see cref="DiscordIntents.GuildPresences"/> intent specified in <seealso cref="DiscordConfiguration.Intents"/>
        /// </summary>
        public event AsyncEventHandler<DiscordClient, PresenceUpdateEventArgs> PresenceUpdated
        {
            add => this._presenceUpdated.Register(value);
            remove => this._presenceUpdated.Unregister(value);
        }
        private AsyncEvent<DiscordClient, PresenceUpdateEventArgs> _presenceUpdated;


        /// <summary>
        /// Fired when the current user updates their settings.
        /// For this Event you need the <see cref="DiscordIntents.GuildPresences"/> intent specified in <seealso cref="DiscordConfiguration.Intents"/>
        /// </summary>
        public event AsyncEventHandler<DiscordClient, UserSettingsUpdateEventArgs> UserSettingsUpdated
        {
            add => this._userSettingsUpdated.Register(value);
            remove => this._userSettingsUpdated.Unregister(value);
        }
        private AsyncEvent<DiscordClient, UserSettingsUpdateEventArgs> _userSettingsUpdated;

        /// <summary>
        /// Fired when properties about the current user change.
        /// For this Event you need the <see cref="DiscordIntents.GuildPresences"/> intent specified in <seealso cref="DiscordConfiguration.Intents"/>
        /// </summary>
        /// <remarks>
        /// NB: This event only applies for changes to the <b>current user</b>, the client that is connected to Discord.
        /// </remarks>
        public event AsyncEventHandler<DiscordClient, UserUpdateEventArgs> UserUpdated
        {
            add => this._userUpdated.Register(value);
            remove => this._userUpdated.Unregister(value);
        }
        private AsyncEvent<DiscordClient, UserUpdateEventArgs> _userUpdated;

        #endregion

        #region Voice

        /// <summary>
        /// Fired when someone joins/leaves/moves voice channels.
        /// For this Event you need the <see cref="DiscordIntents.GuildVoiceStates"/> intent specified in <seealso cref="DiscordConfiguration.Intents"/>
        /// </summary>
        public event AsyncEventHandler<DiscordClient, VoiceStateUpdateEventArgs> VoiceStateUpdated
        {
            add => this._voiceStateUpdated.Register(value);
            remove => this._voiceStateUpdated.Unregister(value);
        }
        private AsyncEvent<DiscordClient, VoiceStateUpdateEventArgs> _voiceStateUpdated;

        /// <summary>
        /// Fired when a guild's voice server is updated.
        /// For this Event you need the <see cref="DiscordIntents.GuildVoiceStates"/> intent specified in <seealso cref="DiscordConfiguration.Intents"/>
        /// </summary>
        public event AsyncEventHandler<DiscordClient, VoiceServerUpdateEventArgs> VoiceServerUpdated
        {
            add => this._voiceServerUpdated.Register(value);
            remove => this._voiceServerUpdated.Unregister(value);
        }
        private AsyncEvent<DiscordClient, VoiceServerUpdateEventArgs> _voiceServerUpdated;

        #endregion

        #region Application

        /// <summary>
        /// Fired when a new application command is registered.
        /// </summary>
        public event AsyncEventHandler<DiscordClient, ApplicationCommandEventArgs> ApplicationCommandCreated
        {
            add => this._applicationCommandCreated.Register(value);
            remove => this._applicationCommandCreated.Unregister(value);
        }
        private AsyncEvent<DiscordClient, ApplicationCommandEventArgs> _applicationCommandCreated;

        /// <summary>
        /// Fired when an application command is updated.
        /// </summary>
        public event AsyncEventHandler<DiscordClient, ApplicationCommandEventArgs> ApplicationCommandUpdated
        {
            add => this._applicationCommandUpdated.Register(value);
            remove => this._applicationCommandUpdated.Unregister(value);
        }
        private AsyncEvent<DiscordClient, ApplicationCommandEventArgs> _applicationCommandUpdated;

        /// <summary>
        /// Fired when an application command is deleted.
        /// </summary>
        public event AsyncEventHandler<DiscordClient, ApplicationCommandEventArgs> ApplicationCommandDeleted
        {
            add => this._applicationCommandDeleted.Register(value);
            remove => this._applicationCommandDeleted.Unregister(value);
        }
        private AsyncEvent<DiscordClient, ApplicationCommandEventArgs> _applicationCommandDeleted;

        /// <summary>
        /// Fired when a new application command is registered.
        /// </summary>
        public event AsyncEventHandler<DiscordClient, GuildApplicationCommandCountEventArgs> GuildApplicationCommandCountUpdated
        {
            add => this._guildApplicationCommandCountUpdated.Register(value);
            remove => this._guildApplicationCommandCountUpdated.Unregister(value);
        }
        private AsyncEvent<DiscordClient, GuildApplicationCommandCountEventArgs> _guildApplicationCommandCountUpdated;

        /// <summary>
        /// Fired when a user uses a context menu.
        /// </summary>
        public event AsyncEventHandler<DiscordClient, ContextMenuInteractionCreateEventArgs> ContextMenuInteractionCreated
        {
            add => this._contextMenuInteractionCreated.Register(value);
            remove => this._contextMenuInteractionCreated.Unregister(value);
        }

        private AsyncEvent<DiscordClient, ContextMenuInteractionCreateEventArgs> _contextMenuInteractionCreated;

        /// <summary>
        /// Fired when application command permissions gets updated.
        /// </summary>
        public event AsyncEventHandler<DiscordClient, ApplicationCommandPermissionsUpdateEventArgs> ApplicationCommandPermissionsUpdated
        {
            add => this._applicationCommandPermissionsUpdated.Register(value);
            remove => this._applicationCommandPermissionsUpdated.Unregister(value);
        }

        private AsyncEvent<DiscordClient, ApplicationCommandPermissionsUpdateEventArgs> _applicationCommandPermissionsUpdated;


        #endregion

        #region Misc

        /// <summary>
        /// Fired when an interaction is invoked.
        /// </summary>
        public event AsyncEventHandler<DiscordClient, InteractionCreateEventArgs> InteractionCreated
        {
            add => this._interactionCreated.Register(value);
            remove => this._interactionCreated.Unregister(value);
        }
        private AsyncEvent<DiscordClient, InteractionCreateEventArgs> _interactionCreated;

        /// <summary>
        /// Fired when a component is invoked.
        /// </summary>
        public event AsyncEventHandler<DiscordClient, ComponentInteractionCreateEventArgs> ComponentInteractionCreated
        {
            add => this._componentInteractionCreated.Register(value);
            remove => this._componentInteractionCreated.Unregister(value);
        }
        private AsyncEvent<DiscordClient, ComponentInteractionCreateEventArgs> _componentInteractionCreated;

        /// <summary>
        /// Fired when a user starts typing in a channel.
        /// </summary>
        public event AsyncEventHandler<DiscordClient, TypingStartEventArgs> TypingStarted
        {
            add => this._typingStarted.Register(value);
            remove => this._typingStarted.Unregister(value);
        }
        private AsyncEvent<DiscordClient, TypingStartEventArgs> _typingStarted;

        /// <summary>
        /// Fired when an unknown event gets received.
        /// </summary>
        public event AsyncEventHandler<DiscordClient, UnknownEventArgs> UnknownEvent
        {
            add => this._unknownEvent.Register(value);
            remove => this._unknownEvent.Unregister(value);
        }
        private AsyncEvent<DiscordClient, UnknownEventArgs> _unknownEvent;

        /// <summary>
        /// Fired whenever webhooks update.
        /// </summary>
        public event AsyncEventHandler<DiscordClient, WebhooksUpdateEventArgs> WebhooksUpdated
        {
            add => this._webhooksUpdated.Register(value);
            remove => this._webhooksUpdated.Unregister(value);
        }
        private AsyncEvent<DiscordClient, WebhooksUpdateEventArgs> _webhooksUpdated;

        /// <summary>
        /// Fired whenever an error occurs within an event handler.
        /// </summary>
        public event AsyncEventHandler<DiscordClient, ClientErrorEventArgs> ClientErrored
        {
            add => this._clientErrored.Register(value);
            remove => this._clientErrored.Unregister(value);
        }
        private AsyncEvent<DiscordClient, ClientErrorEventArgs> _clientErrored;

        #endregion

        #region Error Handling

        /// <summary>
        /// Events the error handler.
        /// </summary>
        /// <param name="asyncEvent">The async event.</param>
        /// <param name="ex">The ex.</param>
        /// <param name="handler">The handler.</param>
        /// <param name="sender">The sender.</param>
        /// <param name="eventArgs">The event args.</param>
        internal void EventErrorHandler<TArgs>(AsyncEvent<DiscordClient, TArgs> asyncEvent, Exception ex, AsyncEventHandler<DiscordClient, TArgs> handler, DiscordClient sender, TArgs eventArgs)
            where TArgs : AsyncEventArgs
        {
            if (ex is AsyncEventTimeoutException)
            {
                this.Logger.LogWarning(LoggerEvents.EventHandlerException, $"An event handler for {asyncEvent.Name} took too long to execute. Defined as \"{handler.Method.ToString().Replace(handler.Method.ReturnType.ToString(), "").TrimStart()}\" located in \"{handler.Method.DeclaringType}\".");
                return;
            }

            this.Logger.LogError(LoggerEvents.EventHandlerException, ex, "Event handler exception for event {0} thrown from {1} (defined in {2})", asyncEvent.Name, handler.Method, handler.Method.DeclaringType);
            this._clientErrored.InvokeAsync(sender, new ClientErrorEventArgs { EventName = asyncEvent.Name, Exception = ex }).ConfigureAwait(false).GetAwaiter().GetResult();
        }


        /// <summary>
        /// Fired on heartbeat attempt cancellation due to too many failed heartbeats.
        /// </summary>
        public event AsyncEventHandler<DiscordClient, ZombiedEventArgs> Zombied
        {
            add => this._zombied.Register(value);
            remove => this._zombied.Unregister(value);
        }
        private AsyncEvent<DiscordClient, ZombiedEventArgs> _zombied;

        /// <summary>
        /// Fired when a gateway
        /// </summary>
        public event AsyncEventHandler<DiscordClient, PayloadReceivedEventArgs> PayloadReceived
        {
            add => this._payloadReceived.Register(value);
            remove => this._payloadReceived.Unregister(value);
        }
        private AsyncEvent<DiscordClient, PayloadReceivedEventArgs> _payloadReceived;

        /// <summary>
        /// Goofs the.
        /// </summary>
        /// <param name="asyncEvent">The async event.</param>
        /// <param name="ex">The ex.</param>
        /// <param name="handler">The handler.</param>
        /// <param name="sender">The sender.</param>
        /// <param name="eventArgs">The event args.</param>
        private void Goof<TArgs>(AsyncEvent<DiscordClient, TArgs> asyncEvent, Exception ex, AsyncEventHandler<DiscordClient, TArgs> handler, DiscordClient sender, TArgs eventArgs)
            where TArgs : AsyncEventArgs => this.Logger.LogCritical(LoggerEvents.EventHandlerException, ex, "Exception event handler {0} (defined in {1}) threw an exception", handler.Method, handler.Method.DeclaringType);

        #endregion

        #region Event Dispatchers

        /// <summary>
        /// Client_S the zombied.
        /// </summary>
        /// <param name="client">The client.</param>
        /// <param name="e">The events.</param>
        /// <returns>A Task.</returns>
        private Task Client_Zombied(DiscordClient client, ZombiedEventArgs e)
            => this._zombied.InvokeAsync(client, e);

        /// <summary>
        /// Payload_S the received.
        /// </summary>
        /// <param name="client">The client.</param>
        /// <param name="e">The events.</param>
        /// <returns>A Task.</returns>
        private Task Client_PayloadReceived(DiscordClient client, PayloadReceivedEventArgs e)
            => this._payloadReceived.InvokeAsync(client, e);

        /// <summary>
        /// Client_S the client error.
        /// </summary>
        /// <param name="client">The client.</param>
        /// <param name="e">The events.</param>
        /// <returns>A Task.</returns>
        private Task Client_ClientError(DiscordClient client, ClientErrorEventArgs e)
            => this._clientErrored.InvokeAsync(client, e);

        /// <summary>
        /// Client_S the socket error.
        /// </summary>
        /// <param name="client">The client.</param>
        /// <param name="e">The events.</param>
        /// <returns>A Task.</returns>
        private Task Client_SocketError(DiscordClient client, SocketErrorEventArgs e)
            => this._socketErrored.InvokeAsync(client, e);

        /// <summary>
        /// Client_S the socket opened.
        /// </summary>
        /// <param name="client">The client.</param>
        /// <param name="e">The events.</param>
        /// <returns>A Task.</returns>
        private Task Client_SocketOpened(DiscordClient client, SocketEventArgs e)
            => this._socketOpened.InvokeAsync(client, e);

        /// <summary>
        /// Client_S the socket closed.
        /// </summary>
        /// <param name="client">The client.</param>
        /// <param name="e">The events.</param>
        /// <returns>A Task.</returns>
        private Task Client_SocketClosed(DiscordClient client, SocketCloseEventArgs e)
            => this._socketClosed.InvokeAsync(client, e);

        /// <summary>
        /// Client_S the ready.
        /// </summary>
        /// <param name="client">The client.</param>
        /// <param name="e">The events.</param>
        /// <returns>A Task.</returns>
        private Task Client_Ready(DiscordClient client, ReadyEventArgs e)
            => this._ready.InvokeAsync(client, e);

        /// <summary>
        /// Client_S the resumed.
        /// </summary>
        /// <param name="client">The client.</param>
        /// <param name="e">The events.</param>
        /// <returns>A Task.</returns>
        private Task Client_Resumed(DiscordClient client, ReadyEventArgs e)
            => this._resumed.InvokeAsync(client, e);

        /// <summary>
        /// Client_S the channel created.
        /// </summary>
        /// <param name="client">The client.</param>
        /// <param name="e">The events.</param>
        /// <returns>A Task.</returns>
        private Task Client_ChannelCreated(DiscordClient client, ChannelCreateEventArgs e)
            => this._channelCreated.InvokeAsync(client, e);

        /// <summary>
        /// Client_S the channel updated.
        /// </summary>
        /// <param name="client">The client.</param>
        /// <param name="e">The events.</param>
        /// <returns>A Task.</returns>
        private Task Client_ChannelUpdated(DiscordClient client, ChannelUpdateEventArgs e)
            => this._channelUpdated.InvokeAsync(client, e);

        /// <summary>
        /// Client_S the channel deleted.
        /// </summary>
        /// <param name="client">The client.</param>
        /// <param name="e">The events.</param>
        /// <returns>A Task.</returns>
        private Task Client_ChannelDeleted(DiscordClient client, ChannelDeleteEventArgs e)
            => this._channelDeleted.InvokeAsync(client, e);

        /// <summary>
        /// Client_S the d m channel deleted.
        /// </summary>
        /// <param name="client">The client.</param>
        /// <param name="e">The events.</param>
        /// <returns>A Task.</returns>
        private Task Client_DMChannelDeleted(DiscordClient client, DmChannelDeleteEventArgs e)
            => this._dmChannelDeleted.InvokeAsync(client, e);

        /// <summary>
        /// Client_S the channel pins updated.
        /// </summary>
        /// <param name="client">The client.</param>
        /// <param name="e">The events.</param>
        /// <returns>A Task.</returns>
        private Task Client_ChannelPinsUpdated(DiscordClient client, ChannelPinsUpdateEventArgs e)
            => this._channelPinsUpdated.InvokeAsync(client, e);

        /// <summary>
        /// Client_S the guild created.
        /// </summary>
        /// <param name="client">The client.</param>
        /// <param name="e">The events.</param>
        /// <returns>A Task.</returns>
        private Task Client_GuildCreated(DiscordClient client, GuildCreateEventArgs e)
            => this._guildCreated.InvokeAsync(client, e);

        /// <summary>
        /// Client_S the guild available.
        /// </summary>
        /// <param name="client">The client.</param>
        /// <param name="e">The events.</param>
        /// <returns>A Task.</returns>
        private Task Client_GuildAvailable(DiscordClient client, GuildCreateEventArgs e)
            => this._guildAvailable.InvokeAsync(client, e);

        /// <summary>
        /// Client_S the guild updated.
        /// </summary>
        /// <param name="client">The client.</param>
        /// <param name="e">The events.</param>
        /// <returns>A Task.</returns>
        private Task Client_GuildUpdated(DiscordClient client, GuildUpdateEventArgs e)
            => this._guildUpdated.InvokeAsync(client, e);

        /// <summary>
        /// Client_S the guild deleted.
        /// </summary>
        /// <param name="client">The client.</param>
        /// <param name="e">The events.</param>
        /// <returns>A Task.</returns>
        private Task Client_GuildDeleted(DiscordClient client, GuildDeleteEventArgs e)
            => this._guildDeleted.InvokeAsync(client, e);

        /// <summary>
        /// Client_S the guild unavailable.
        /// </summary>
        /// <param name="client">The client.</param>
        /// <param name="e">The events.</param>
        /// <returns>A Task.</returns>
        private Task Client_GuildUnavailable(DiscordClient client, GuildDeleteEventArgs e)
            => this._guildUnavailable.InvokeAsync(client, e);

        /// <summary>
        /// Client_S the guild download completed.
        /// </summary>
        /// <param name="client">The client.</param>
        /// <param name="e">The events.</param>
        /// <returns>A Task.</returns>
        private Task Client_GuildDownloadCompleted(DiscordClient client, GuildDownloadCompletedEventArgs e)
            => this._guildDownloadCompleted.InvokeAsync(client, e);

        /// <summary>
        /// Client_S the message created.
        /// </summary>
        /// <param name="client">The client.</param>
        /// <param name="e">The events.</param>
        /// <returns>A Task.</returns>
        private Task Client_MessageCreated(DiscordClient client, MessageCreateEventArgs e)
            => this._messageCreated.InvokeAsync(client, e);

        /// <summary>
        /// Client_S the invite created.
        /// </summary>
        /// <param name="client">The client.</param>
        /// <param name="e">The events.</param>
        /// <returns>A Task.</returns>
        private Task Client_InviteCreated(DiscordClient client, InviteCreateEventArgs e)
            => this._inviteCreated.InvokeAsync(client, e);

        /// <summary>
        /// Client_S the invite deleted.
        /// </summary>
        /// <param name="client">The client.</param>
        /// <param name="e">The events.</param>
        /// <returns>A Task.</returns>
        private Task Client_InviteDeleted(DiscordClient client, InviteDeleteEventArgs e)
            => this._inviteDeleted.InvokeAsync(client, e);

        /// <summary>
        /// Client_S the presence update.
        /// </summary>
        /// <param name="client">The client.</param>
        /// <param name="e">The events.</param>
        /// <returns>A Task.</returns>
        private Task Client_PresenceUpdate(DiscordClient client, PresenceUpdateEventArgs e)
            => this._presenceUpdated.InvokeAsync(client, e);

        /// <summary>
        /// Client_S the guild ban add.
        /// </summary>
        /// <param name="client">The client.</param>
        /// <param name="e">The events.</param>
        /// <returns>A Task.</returns>
        private Task Client_GuildBanAdd(DiscordClient client, GuildBanAddEventArgs e)
            => this._guildBanAdded.InvokeAsync(client, e);

        /// <summary>
        /// Client_S the guild ban remove.
        /// </summary>
        /// <param name="client">The client.</param>
        /// <param name="e">The events.</param>
        /// <returns>A Task.</returns>
        private Task Client_GuildBanRemove(DiscordClient client, GuildBanRemoveEventArgs e)
            => this._guildBanRemoved.InvokeAsync(client, e);

        /// <summary>
        /// Client_S the guild emojis update.
        /// </summary>
        /// <param name="client">The client.</param>
        /// <param name="e">The events.</param>
        /// <returns>A Task.</returns>
        private Task Client_GuildEmojisUpdate(DiscordClient client, GuildEmojisUpdateEventArgs e)
            => this._guildEmojisUpdated.InvokeAsync(client, e);

        /// <summary>
        /// Client_S the guild stickers update.
        /// </summary>
        /// <param name="client">The client.</param>
        /// <param name="e">The events.</param>
        /// <returns>A Task.</returns>
        private Task Client_GuildStickersUpdate(DiscordClient client, GuildStickersUpdateEventArgs e)
            => this._guildStickersUpdated.InvokeAsync(client, e);

        /// <summary>
        /// Client_S the guild integrations update.
        /// </summary>
        /// <param name="client">The client.</param>
        /// <param name="e">The events.</param>
        /// <returns>A Task.</returns>
        private Task Client_GuildIntegrationsUpdate(DiscordClient client, GuildIntegrationsUpdateEventArgs e)
            => this._guildIntegrationsUpdated.InvokeAsync(client, e);

        /// <summary>
        /// Client_S the guild member add.
        /// </summary>
        /// <param name="client">The client.</param>
        /// <param name="e">The events.</param>
        /// <returns>A Task.</returns>
        private Task Client_GuildMemberAdd(DiscordClient client, GuildMemberAddEventArgs e)
            => this._guildMemberAdded.InvokeAsync(client, e);

        /// <summary>
        /// Client_S the guild member remove.
        /// </summary>
        /// <param name="client">The client.</param>
        /// <param name="e">The events.</param>
        /// <returns>A Task.</returns>
        private Task Client_GuildMemberRemove(DiscordClient client, GuildMemberRemoveEventArgs e)
            => this._guildMemberRemoved.InvokeAsync(client, e);

        /// <summary>
        /// Client_S the guild member update.
        /// </summary>
        /// <param name="client">The client.</param>
        /// <param name="e">The events.</param>
        /// <returns>A Task.</returns>
        private Task Client_GuildMemberUpdate(DiscordClient client, GuildMemberUpdateEventArgs e)
            => this._guildMemberUpdated.InvokeAsync(client, e);

        /// <summary>
        /// Client_S the guild role create.
        /// </summary>
        /// <param name="client">The client.</param>
        /// <param name="e">The events.</param>
        /// <returns>A Task.</returns>
        private Task Client_GuildRoleCreate(DiscordClient client, GuildRoleCreateEventArgs e)
            => this._guildRoleCreated.InvokeAsync(client, e);

        /// <summary>
        /// Client_S the guild role update.
        /// </summary>
        /// <param name="client">The client.</param>
        /// <param name="e">The events.</param>
        /// <returns>A Task.</returns>
        private Task Client_GuildRoleUpdate(DiscordClient client, GuildRoleUpdateEventArgs e)
            => this._guildRoleUpdated.InvokeAsync(client, e);

        /// <summary>
        /// Client_S the guild role delete.
        /// </summary>
        /// <param name="client">The client.</param>
        /// <param name="e">The events.</param>
        /// <returns>A Task.</returns>
        private Task Client_GuildRoleDelete(DiscordClient client, GuildRoleDeleteEventArgs e)
            => this._guildRoleDeleted.InvokeAsync(client, e);

        /// <summary>
        /// Client_S the message update.
        /// </summary>
        /// <param name="client">The client.</param>
        /// <param name="e">The events.</param>
        /// <returns>A Task.</returns>
        private Task Client_MessageUpdate(DiscordClient client, MessageUpdateEventArgs e)
            => this._messageUpdated.InvokeAsync(client, e);

        /// <summary>
        /// Client_S the message delete.
        /// </summary>
        /// <param name="client">The client.</param>
        /// <param name="e">The events.</param>
        /// <returns>A Task.</returns>
        private Task Client_MessageDelete(DiscordClient client, MessageDeleteEventArgs e)
            => this._messageDeleted.InvokeAsync(client, e);

        /// <summary>
        /// Client_S the message bulk delete.
        /// </summary>
        /// <param name="client">The client.</param>
        /// <param name="e">The events.</param>
        /// <returns>A Task.</returns>
        private Task Client_MessageBulkDelete(DiscordClient client, MessageBulkDeleteEventArgs e)
            => this._messageBulkDeleted.InvokeAsync(client, e);

        /// <summary>
        /// Client_S the typing start.
        /// </summary>
        /// <param name="client">The client.</param>
        /// <param name="e">The events.</param>
        /// <returns>A Task.</returns>
        private Task Client_TypingStart(DiscordClient client, TypingStartEventArgs e)
            => this._typingStarted.InvokeAsync(client, e);

        /// <summary>
        /// Client_S the user settings update.
        /// </summary>
        /// <param name="client">The client.</param>
        /// <param name="e">The events.</param>
        /// <returns>A Task.</returns>
        private Task Client_UserSettingsUpdate(DiscordClient client, UserSettingsUpdateEventArgs e)
            => this._userSettingsUpdated.InvokeAsync(client, e);

        /// <summary>
        /// Client_S the user update.
        /// </summary>
        /// <param name="client">The client.</param>
        /// <param name="e">The events.</param>
        /// <returns>A Task.</returns>
        private Task Client_UserUpdate(DiscordClient client, UserUpdateEventArgs e)
            => this._userUpdated.InvokeAsync(client, e);

        /// <summary>
        /// Client_S the voice state update.
        /// </summary>
        /// <param name="client">The client.</param>
        /// <param name="e">The events.</param>
        /// <returns>A Task.</returns>
        private Task Client_VoiceStateUpdate(DiscordClient client, VoiceStateUpdateEventArgs e)
            => this._voiceStateUpdated.InvokeAsync(client, e);

        /// <summary>
        /// Client_S the voice server update.
        /// </summary>
        /// <param name="client">The client.</param>
        /// <param name="e">The events.</param>
        /// <returns>A Task.</returns>
        private Task Client_VoiceServerUpdate(DiscordClient client, VoiceServerUpdateEventArgs e)
            => this._voiceServerUpdated.InvokeAsync(client, e);

        /// <summary>
        /// Client_S the guild members chunk.
        /// </summary>
        /// <param name="client">The client.</param>
        /// <param name="e">The events.</param>
        /// <returns>A Task.</returns>
        private Task Client_GuildMembersChunk(DiscordClient client, GuildMembersChunkEventArgs e)
            => this._guildMembersChunk.InvokeAsync(client, e);

        /// <summary>
        /// Client_S the unknown event.
        /// </summary>
        /// <param name="client">The client.</param>
        /// <param name="e">The events.</param>
        /// <returns>A Task.</returns>
        private Task Client_UnknownEvent(DiscordClient client, UnknownEventArgs e)
            => this._unknownEvent.InvokeAsync(client, e);

        /// <summary>
        /// Client_S the message reaction add.
        /// </summary>
        /// <param name="client">The client.</param>
        /// <param name="e">The events.</param>
        /// <returns>A Task.</returns>
        private Task Client_MessageReactionAdd(DiscordClient client, MessageReactionAddEventArgs e)
            => this._messageReactionAdded.InvokeAsync(client, e);

        /// <summary>
        /// Client_S the message reaction remove.
        /// </summary>
        /// <param name="client">The client.</param>
        /// <param name="e">The events.</param>
        /// <returns>A Task.</returns>
        private Task Client_MessageReactionRemove(DiscordClient client, MessageReactionRemoveEventArgs e)
            => this._messageReactionRemoved.InvokeAsync(client, e);

        /// <summary>
        /// Client_S the message reaction remove all.
        /// </summary>
        /// <param name="client">The client.</param>
        /// <param name="e">The events.</param>
        /// <returns>A Task.</returns>
        private Task Client_MessageReactionRemoveAll(DiscordClient client, MessageReactionsClearEventArgs e)
            => this._messageReactionsCleared.InvokeAsync(client, e);

        /// <summary>
        /// Client_S the message reaction removed emoji.
        /// </summary>
        /// <param name="client">The client.</param>
        /// <param name="e">The events.</param>
        /// <returns>A Task.</returns>
        private Task Client_MessageReactionRemovedEmoji(DiscordClient client, MessageReactionRemoveEmojiEventArgs e)
            => this._messageReactionRemovedEmoji.InvokeAsync(client, e);

        /// <summary>
        /// Client_S the interaction create.
        /// </summary>
        /// <param name="client">The client.</param>
        /// <param name="e">The events.</param>
        /// <returns>A Task.</returns>
        private Task Client_InteractionCreate(DiscordClient client, InteractionCreateEventArgs e)
            => this._interactionCreated.InvokeAsync(client, e);

        /// <summary>
        /// Client_S the component interaction create.
        /// </summary>
        /// <param name="client">The client.</param>
        /// <param name="e">The events.</param>
        /// <returns>A Task.</returns>
        private Task Client_ComponentInteractionCreate(DiscordClient client, ComponentInteractionCreateEventArgs e)
            => this._componentInteractionCreated.InvokeAsync(client, e);

        /// <summary>
        /// Client_S the context menu interaction create.
        /// </summary>
        /// <param name="client">The client.</param>
        /// <param name="e">The events.</param>
        /// <returns>A Task.</returns>
        private Task Client_ContextMenuInteractionCreate(DiscordClient client, ContextMenuInteractionCreateEventArgs e)
            => this._contextMenuInteractionCreated.InvokeAsync(client, e);

        /// <summary>
        /// Client_S the webhooks update.
        /// </summary>
        /// <param name="client">The client.</param>
        /// <param name="e">The events.</param>
        /// <returns>A Task.</returns>
        private Task Client_WebhooksUpdate(DiscordClient client, WebhooksUpdateEventArgs e)
            => this._webhooksUpdated.InvokeAsync(client, e);

        /// <summary>
        /// Client_S the heart beated.
        /// </summary>
        /// <param name="client">The client.</param>
        /// <param name="e">The events.</param>
        /// <returns>A Task.</returns>
        private Task Client_HeartBeated(DiscordClient client, HeartbeatEventArgs e)
            => this._heartbeated.InvokeAsync(client, e);

        /// <summary>
        /// Client_S the application command created.
        /// </summary>
        /// <param name="client">The client.</param>
        /// <param name="e">The events.</param>
        /// <returns>A Task.</returns>
        private Task Client_ApplicationCommandCreated(DiscordClient client, ApplicationCommandEventArgs e)
            => this._applicationCommandCreated.InvokeAsync(client, e);

        /// <summary>
        /// Client_S the application command updated.
        /// </summary>
        /// <param name="client">The client.</param>
        /// <param name="e">The events.</param>
        private Task Client_ApplicationCommandUpdated(DiscordClient client, ApplicationCommandEventArgs e)
            => this._applicationCommandUpdated.InvokeAsync(client, e);

        /// <summary>
        /// Client_S the application command deleted.
        /// </summary>
        /// <param name="client">The client.</param>
        /// <param name="e">The events.</param>
        private Task Client_ApplicationCommandDeleted(DiscordClient client, ApplicationCommandEventArgs e)
            => this._applicationCommandDeleted.InvokeAsync(client, e);


        /// <summary>
        /// Client_S the guild application command count updated.
        /// </summary>
        /// <param name="client">The client.</param>
        /// <param name="e">The events.</param>
        private Task Client_GuildApplicationCommandCountUpdated(DiscordClient client, GuildApplicationCommandCountEventArgs e)
            => this._guildApplicationCommandCountUpdated.InvokeAsync(client, e);


        /// <summary>
        /// Client_S the application command permissions updated.
        /// </summary>
        /// <param name="client">The client.</param>
        /// <param name="e">The events.</param>
        private Task Client_ApplicationCommandPermissionsUpdated(DiscordClient client, ApplicationCommandPermissionsUpdateEventArgs e)
            => this._applicationCommandPermissionsUpdated.InvokeAsync(client, e);

        /// <summary>
        /// Client_S the guild integration created.
        /// </summary>
        /// <param name="client">The client.</param>
        /// <param name="e">The events.</param>
        /// <returns>A Task.</returns>
        private Task Client_GuildIntegrationCreated(DiscordClient client, GuildIntegrationCreateEventArgs e)
            => this._guildIntegrationCreated.InvokeAsync(client, e);

        /// <summary>
        /// Client_S the guild integration updated.
        /// </summary>
        /// <param name="client">The client.</param>
        /// <param name="e">The events.</param>
        /// <returns>A Task.</returns>
        private Task Client_GuildIntegrationUpdated(DiscordClient client, GuildIntegrationUpdateEventArgs e)
            => this._guildIntegrationUpdated.InvokeAsync(client, e);

        /// <summary>
        /// Client_S the guild integration deleted.
        /// </summary>
        /// <param name="client">The client.</param>
        /// <param name="e">The events.</param>
        /// <returns>A Task.</returns>
        private Task Client_GuildIntegrationDeleted(DiscordClient client, GuildIntegrationDeleteEventArgs e)
            => this._guildIntegrationDeleted.InvokeAsync(client, e);

        /// <summary>
        /// Client_S the stage instance created.
        /// </summary>
        /// <param name="client">The client.</param>
        /// <param name="e">The events.</param>
        /// <returns>A Task.</returns>
        private Task Client_StageInstanceCreated(DiscordClient client, StageInstanceCreateEventArgs e)
            => this._stageInstanceCreated.InvokeAsync(client, e);

        /// <summary>
        /// Client_S the stage instance updated.
        /// </summary>
        /// <param name="client">The client.</param>
        /// <param name="e">The events.</param>
        /// <returns>A Task.</returns>
        private Task Client_StageInstanceUpdated(DiscordClient client, StageInstanceUpdateEventArgs e)
            => this._stageInstanceUpdated.InvokeAsync(client, e);

        /// <summary>
        /// Client_S the stage instance deleted.
        /// </summary>
        /// <param name="client">The client.</param>
        /// <param name="e">The events.</param>
        /// <returns>A Task.</returns>
        private Task Client_StageInstanceDeleted(DiscordClient client, StageInstanceDeleteEventArgs e)
            => this._stageInstanceDeleted.InvokeAsync(client, e);

        /// <summary>
        /// Client_S the thread created.
        /// </summary>
        /// <param name="client">The client.</param>
        /// <param name="e">The events.</param>
        /// <returns>A Task.</returns>
        private Task Client_ThreadCreated(DiscordClient client, ThreadCreateEventArgs e)
            => this._threadCreated.InvokeAsync(client, e);

        /// <summary>
        /// Client_S the thread updated.
        /// </summary>
        /// <param name="client">The client.</param>
        /// <param name="e">The events.</param>
        /// <returns>A Task.</returns>
        private Task Client_ThreadUpdated(DiscordClient client, ThreadUpdateEventArgs e)
            => this._threadUpdated.InvokeAsync(client, e);

        /// <summary>
        /// Client_S the thread deleted.
        /// </summary>
        /// <param name="client">The client.</param>
        /// <param name="e">The events.</param>
        /// <returns>A Task.</returns>
        private Task Client_ThreadDeleted(DiscordClient client, ThreadDeleteEventArgs e)
            => this._threadDeleted.InvokeAsync(client, e);

        /// <summary>
        /// Client_S the thread list synced.
        /// </summary>
        /// <param name="client">The client.</param>
        /// <param name="e">The events.</param>
        /// <returns>A Task.</returns>
        private Task Client_ThreadListSynced(DiscordClient client, ThreadListSyncEventArgs e)
            => this._threadListSynced.InvokeAsync(client, e);

        /// <summary>
        /// Client_S the thread member updated.
        /// </summary>
        /// <param name="client">The client.</param>
        /// <param name="e">The events.</param>
        /// <returns>A Task.</returns>
        private Task Client_ThreadMemberUpdated(DiscordClient client, ThreadMemberUpdateEventArgs e)
            => this._threadMemberUpdated.InvokeAsync(client, e);

        /// <summary>
        /// Client_S the thread members updated.
        /// </summary>
        /// <param name="client">The client.</param>
        /// <param name="e">The events.</param>
        /// <returns>A Task.</returns>
        private Task Client_ThreadMembersUpdated(DiscordClient client, ThreadMembersUpdateEventArgs e)
            => this._threadMembersUpdated.InvokeAsync(client, e);


        /// <summary>
        /// Client_S the sheduled event created.
        /// </summary>
        /// <param name="client">The client.</param>
        /// <param name="e">The events.</param>
        /// <returns>A Task.</returns>
        private Task Client_GuildSheduledEventCreated(DiscordClient client, GuildSheduledEventCreateEventArgs e)
            => this._guildSheduledEventCreated.InvokeAsync(client, e);

        /// <summary>
        /// Client_S the sheduled event updated.
        /// </summary>
        /// <param name="client">The client.</param>
        /// <param name="e">The events.</param>
        /// <returns>A Task.</returns>
        private Task Client_GuildSheduledEventUpdated(DiscordClient client, GuildSheduledEventUpdateEventArgs e)
            => this._guildSheduledEventUpdated.InvokeAsync(client, e);

        /// <summary>
        /// Client_S the sheduled event deleted.
        /// </summary>
        /// <param name="client">The client.</param>
        /// <param name="e">The events.</param>
        /// <returns>A Task.</returns>
        private Task Client_GuildSheduledEventDeleted(DiscordClient client, GuildSheduledEventDeleteEventArgs e)
            => this._guildSheduledEventDeleted.InvokeAsync(client, e);

        #endregion
    }
}
