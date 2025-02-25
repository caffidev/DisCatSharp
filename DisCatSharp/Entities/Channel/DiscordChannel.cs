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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DisCatSharp.Exceptions;
using DisCatSharp.Net.Abstractions;
using DisCatSharp.Net.Models;
using Newtonsoft.Json;

namespace DisCatSharp.Entities
{
    /// <summary>
    /// Represents a discord channel.
    /// </summary>
    public class DiscordChannel : SnowflakeObject, IEquatable<DiscordChannel>
    {
        /// <summary>
        /// Gets ID of the guild to which this channel belongs.
        /// </summary>
        [JsonProperty("guild_id", NullValueHandling = NullValueHandling.Ignore)]
        public ulong? GuildId { get; internal set; }

        /// <summary>
        /// Gets ID of the category that contains this channel.
        /// </summary>
        [JsonProperty("parent_id", NullValueHandling = NullValueHandling.Include)]
        public ulong? ParentId { get; internal set; }

        /// <summary>
        /// Gets the category that contains this channel.
        /// </summary>
        [JsonIgnore]
        public DiscordChannel Parent
            => this.ParentId.HasValue ? this.Guild.GetChannel(this.ParentId.Value) : null;

        /// <summary>
        /// Gets the name of this channel.
        /// </summary>
        [JsonProperty("name", NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; internal set; }

        /// <summary>
        /// Gets the type of this channel.
        /// </summary>
        [JsonProperty("type", NullValueHandling = NullValueHandling.Ignore)]
        public ChannelType Type { get; internal set; }

        /// <summary>
        /// Gets the position of this channel.
        /// </summary>
        [JsonProperty("position", NullValueHandling = NullValueHandling.Ignore)]
        public int Position { get; internal set; }

        /// <summary>
        /// Gets the maximum available position to move the channel to.
        /// This can contain outdated informations.
        /// </summary>
        public int GetMaxPosition()
        {
            var channels = this.Guild.Channels.Values;
            return this.ParentId != null
                ? this.Type == ChannelType.Text || this.Type == ChannelType.News
                    ? channels.Where(xc => xc.ParentId == this.ParentId && (xc.Type == ChannelType.Text || xc.Type == ChannelType.News)).OrderBy(xc => xc.Position).ToArray().Last().Position
                    : this.Type == ChannelType.Voice || this.Type == ChannelType.Stage
                        ? channels.Where(xc => xc.ParentId == this.ParentId && (xc.Type == ChannelType.Voice || xc.Type == ChannelType.Stage)).OrderBy(xc => xc.Position).ToArray().Last().Position
                        : channels.Where(xc => xc.ParentId == this.ParentId && xc.Type == this.Type).OrderBy(xc => xc.Position).ToArray().Last().Position
                : channels.Where(xc => xc.ParentId == null && xc.Type == this.Type).OrderBy(xc => xc.Position).ToArray().Last().Position;
        }

        /// <summary>
        /// Gets the minimum available position to move the channel to.
        /// </summary>
        public int GetMinPosition()
        {
            var channels = this.Guild.Channels.Values;
            return this.ParentId != null
                ? this.Type == ChannelType.Text || this.Type == ChannelType.News
                    ? channels.Where(xc => xc.ParentId == this.ParentId && (xc.Type == ChannelType.Text || xc.Type == ChannelType.News)).OrderBy(xc => xc.Position).ToArray().First().Position
                    : this.Type == ChannelType.Voice || this.Type == ChannelType.Stage
                        ? channels.Where(xc => xc.ParentId == this.ParentId && (xc.Type == ChannelType.Voice || xc.Type == ChannelType.Stage)).OrderBy(xc => xc.Position).ToArray().First().Position
                        : channels.Where(xc => xc.ParentId == this.ParentId && xc.Type == this.Type).OrderBy(xc => xc.Position).ToArray().First().Position
                : channels.Where(xc => xc.ParentId == null && xc.Type == this.Type).OrderBy(xc => xc.Position).ToArray().First().Position;
        }

        /// <summary>
        /// Gets whether this channel is a DM channel.
        /// </summary>
        [JsonIgnore]
        public bool IsPrivate
            => this.Type == ChannelType.Private || this.Type == ChannelType.Group;

        /// <summary>
        /// Gets whether this channel is a channel category.
        /// </summary>
        [JsonIgnore]
        public bool IsCategory
            => this.Type == ChannelType.Category;

        /// <summary>
        /// Gets whether this channel is a stage channel.
        /// </summary>
        [JsonIgnore]
        public bool IsStage
            => this.Type == ChannelType.Stage;

        /// <summary>
        /// Gets the guild to which this channel belongs.
        /// </summary>
        [JsonIgnore]
        public DiscordGuild Guild
            => this.GuildId.HasValue && this.Discord.Guilds.TryGetValue(this.GuildId.Value, out var guild) ? guild : null;

        /// <summary>
        /// Gets a collection of permission overwrites for this channel.
        /// </summary>
        [JsonIgnore]
        public IReadOnlyList<DiscordOverwrite> PermissionOverwrites
            => this._permissionOverwritesLazy.Value;

        [JsonProperty("permission_overwrites", NullValueHandling = NullValueHandling.Ignore)]
        internal List<DiscordOverwrite> _permissionOverwrites = new();
        [JsonIgnore]
        private readonly Lazy<IReadOnlyList<DiscordOverwrite>> _permissionOverwritesLazy;

        /// <summary>
        /// Gets the channel's topic. This is applicable to text channels only.
        /// </summary>
        [JsonProperty("topic", NullValueHandling = NullValueHandling.Ignore)]
        public string Topic { get; internal set; }

        /// <summary>
        /// Gets the ID of the last message sent in this channel. This is applicable to text channels only.
        /// </summary>
        [JsonProperty("last_message_id", NullValueHandling = NullValueHandling.Ignore)]
        public ulong? LastMessageId { get; internal set; }

        /// <summary>
        /// Gets this channel's bitrate. This is applicable to voice channels only.
        /// </summary>
        [JsonProperty("bitrate", NullValueHandling = NullValueHandling.Ignore)]
        public int? Bitrate { get; internal set; }

        /// <summary>
        /// Gets this channel's user limit. This is applicable to voice channels only.
        /// </summary>
        [JsonProperty("user_limit", NullValueHandling = NullValueHandling.Ignore)]
        public int? UserLimit { get; internal set; }

        /// <summary>
        /// <para>Gets the slow mode delay configured for this channel.</para>
        /// <para>All bots, as well as users with <see cref="Permissions.ManageChannels"/> or <see cref="Permissions.ManageMessages"/> permissions in the channel are exempt from slow mode.</para>
        /// </summary>
        [JsonProperty("rate_limit_per_user")]
        public int? PerUserRateLimit { get; internal set; }

        /// <summary>
        /// Gets this channel's video quality mode. This is applicable to voice channels only.
        /// </summary>
        [JsonProperty("video_quality_mode", NullValueHandling = NullValueHandling.Ignore)]
        public VideoQualityMode? QualityMode { get; internal set; }

        /// <summary>
        /// Gets when the last pinned message was pinned.
        /// </summary>
        [JsonIgnore]
        public DateTimeOffset? LastPinTimestamp
            => !string.IsNullOrWhiteSpace(this.LastPinTimestampRaw) && DateTimeOffset.TryParse(this.LastPinTimestampRaw, CultureInfo.InvariantCulture, DateTimeStyles.None, out var dto) ?
                dto : null;

        /// <summary>
        /// Gets when the last pinned message was pinned as raw string.
        /// </summary>
        [JsonProperty("last_pin_timestamp", NullValueHandling = NullValueHandling.Ignore)]
        internal string LastPinTimestampRaw { get; set; }

        /// <summary>
        /// Gets this channel's default duration for newly created threads, in minutes, to automatically archive the thread after recent activity.
        /// </summary>
        [JsonProperty("default_auto_archive_duration", NullValueHandling = NullValueHandling.Ignore)]
        public ThreadAutoArchiveDuration? DefaultAutoArchiveDuration { get; internal set; }

        /// <summary>
        /// Gets this channel's mention string.
        /// </summary>
        [JsonIgnore]
        public string Mention
            => Formatter.Mention(this);

        /// <summary>
        /// Gets this channel's children. This applies only to channel categories.
        /// </summary>
        [JsonIgnore]
        public IReadOnlyList<DiscordChannel> Children
        {
            get
            {
                return !this.IsCategory
                    ? throw new ArgumentException("Only channel categories contain children.")
                    : this.Guild._channels.Values.Where(e => e.ParentId == this.Id).ToList();
            }
        }

        /// <summary>
        /// Gets the list of members currently in the channel (if voice channel), or members who can see the channel (otherwise).
        /// </summary>
        [JsonIgnore]
        public virtual IReadOnlyList<DiscordMember> Users
        {
            get
            {
                return this.Guild == null
                    ? throw new InvalidOperationException("Cannot query users outside of guild channels.")
                    : this.Type == ChannelType.Voice || this.Type == ChannelType.Stage
                    ? this.Guild.Members.Values.Where(x => x.VoiceState?.ChannelId == this.Id).ToList()
                    : this.Guild.Members.Values.Where(x => (this.PermissionsFor(x) & Permissions.AccessChannels) == Permissions.AccessChannels).ToList();
            }
        }

        /// <summary>
        /// Gets whether this channel is an NSFW channel.
        /// </summary>
        [JsonProperty("nsfw")]
        public bool IsNSFW { get; internal set; }

        /// <summary>
        /// Gets this channel's region id (if voice channel).
        /// </summary>
        [JsonProperty("rtc_region", NullValueHandling = NullValueHandling.Ignore)]
        internal string RtcRegionId { get; set; }

        /// <summary>
        /// Gets this channel's region override (if voice channel).
        /// </summary>
        [JsonIgnore]
        public DiscordVoiceRegion RtcRegion
            => this.RtcRegionId != null ? this.Discord.VoiceRegions[this.RtcRegionId] : null;


        /// <summary>
        /// Only sent on the resolved channels of interaction responses for application commands. Gets the permissions of the user in this channel who invoked the command.
        /// </summary>
        [JsonProperty("permissions")]
        public Permissions? UserPermissions { get; internal set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="DiscordChannel"/> class.
        /// </summary>
        internal DiscordChannel()
        {
            this._permissionOverwritesLazy = new Lazy<IReadOnlyList<DiscordOverwrite>>(() => new ReadOnlyCollection<DiscordOverwrite>(this._permissionOverwrites));
        }

        #region Methods

        /// <summary>
        /// Sends a message to this channel.
        /// </summary>
        /// <param name="content">Content of the message to send.</param>
        /// <returns>The sent message.</returns>
        /// <exception cref="UnauthorizedException">Thrown when the client does not have the <see cref="Permissions.SendMessages"/> permission if TTS is true and <see cref="Permissions.SendTtsMessages"/> if TTS is true.</exception>
        /// <exception cref="NotFoundException">Thrown when the channel does not exist.</exception>
        /// <exception cref="BadRequestException">Thrown when an invalid parameter was provided.</exception>
        /// <exception cref="ServerErrorException">Thrown when Discord is unable to process the request.</exception>
        public Task<DiscordMessage> SendMessageAsync(string content)
        {
            return this.Type != ChannelType.Text && this.Type != ChannelType.Private && this.Type != ChannelType.Group && this.Type != ChannelType.News
                ? throw new ArgumentException("Cannot send a text message to a non-text channel.")
                : this.Discord.ApiClient.CreateMessageAsync(this.Id, content, null, sticker: null, replyMessageId: null, mentionReply: false, failOnInvalidReply: false);
        }

        /// <summary>
        /// Sends a message to this channel.
        /// </summary>
        /// <param name="embed">Embed to attach to the message.</param>
        /// <returns>The sent message.</returns>
        /// <exception cref="UnauthorizedException">Thrown when the client does not have the <see cref="Permissions.SendMessages"/> permission and <see cref="Permissions.SendTtsMessages"/> if TTS is true.</exception>
        /// <exception cref="NotFoundException">Thrown when the channel does not exist.</exception>
        /// <exception cref="BadRequestException">Thrown when an invalid parameter was provided.</exception>
        /// <exception cref="ServerErrorException">Thrown when Discord is unable to process the request.</exception>
        public Task<DiscordMessage> SendMessageAsync(DiscordEmbed embed)
        {
            return this.Type != ChannelType.Text && this.Type != ChannelType.Private && this.Type != ChannelType.Group && this.Type != ChannelType.News
                ? throw new ArgumentException("Cannot send a text message to a non-text channel.")
                : this.Discord.ApiClient.CreateMessageAsync(this.Id, null, embed != null ? new[] { embed } : null, sticker: null, replyMessageId: null, mentionReply: false, failOnInvalidReply: false);
        }

        /// <summary>
        /// Sends a message to this channel.
        /// </summary>
        /// <param name="embed">Embed to attach to the message.</param>
        /// <param name="content">Content of the message to send.</param>
        /// <returns>The sent message.</returns>
        /// <exception cref="UnauthorizedException">Thrown when the client does not have the <see cref="Permissions.SendMessages"/> permission if TTS is true and <see cref="Permissions.SendTtsMessages"/> if TTS is true.</exception>
        /// <exception cref="NotFoundException">Thrown when the channel does not exist.</exception>
        /// <exception cref="BadRequestException">Thrown when an invalid parameter was provided.</exception>
        /// <exception cref="ServerErrorException">Thrown when Discord is unable to process the request.</exception>
        public Task<DiscordMessage> SendMessageAsync(string content, DiscordEmbed embed)
        {
            return this.Type != ChannelType.Text && this.Type != ChannelType.Private && this.Type != ChannelType.Group && this.Type != ChannelType.News
                ? throw new ArgumentException("Cannot send a text message to a non-text channel.")
                : this.Discord.ApiClient.CreateMessageAsync(this.Id, content, embed != null ? new[] { embed } : null, sticker: null, replyMessageId: null, mentionReply: false, failOnInvalidReply: false);
        }

        /// <summary>
        /// Sends a message to this channel.
        /// </summary>
        /// <param name="builder">The builder with all the items to send.</param>
        /// <returns>The sent message.</returns>
        /// <exception cref="UnauthorizedException">Thrown when the client does not have the <see cref="Permissions.SendMessages"/> permission TTS is true and <see cref="Permissions.SendTtsMessages"/> if TTS is true.</exception>
        /// <exception cref="NotFoundException">Thrown when the channel does not exist.</exception>
        /// <exception cref="BadRequestException">Thrown when an invalid parameter was provided.</exception>
        /// <exception cref="ServerErrorException">Thrown when Discord is unable to process the request.</exception>
        public Task<DiscordMessage> SendMessageAsync(DiscordMessageBuilder builder)
            => this.Discord.ApiClient.CreateMessageAsync(this.Id, builder);

        /// <summary>
        /// Sends a message to this channel.
        /// </summary>
        /// <param name="action">The builder with all the items to send.</param>
        /// <returns>The sent message.</returns>
        /// <exception cref="UnauthorizedException">Thrown when the client does not have the <see cref="Permissions.SendMessages"/> permission TTS is true and <see cref="Permissions.SendTtsMessages"/> if TTS is true.</exception>
        /// <exception cref="NotFoundException">Thrown when the channel does not exist.</exception>
        /// <exception cref="BadRequestException">Thrown when an invalid parameter was provided.</exception>
        /// <exception cref="ServerErrorException">Thrown when Discord is unable to process the request.</exception>
        public Task<DiscordMessage> SendMessageAsync(Action<DiscordMessageBuilder> action)
        {
            var builder = new DiscordMessageBuilder();
            action(builder);


            return this.Discord.ApiClient.CreateMessageAsync(this.Id, builder);
        }

        /// <summary>
        /// Deletes a guild channel
        /// </summary>
        /// <param name="reason">Reason for audit logs.</param>

        /// <exception cref="UnauthorizedException">Thrown when the client does not have the <see cref="Permissions.ManageChannels"/> permission.</exception>
        /// <exception cref="NotFoundException">Thrown when the channel does not exist.</exception>
        /// <exception cref="BadRequestException">Thrown when an invalid parameter was provided.</exception>
        /// <exception cref="ServerErrorException">Thrown when Discord is unable to process the request.</exception>
        public Task DeleteAsync(string reason = null)
            => this.Discord.ApiClient.DeleteChannelAsync(this.Id, reason);

        /// <summary>
        /// Clones this channel. This operation will create a channel with identical settings to this one. Note that this will not copy messages.
        /// </summary>
        /// <param name="reason">Reason for audit logs.</param>
        /// <returns>Newly-created channel.</returns>
        /// <exception cref="UnauthorizedException">Thrown when the client does not have the <see cref="Permissions.ManageChannels"/> permission.</exception>
        /// <exception cref="NotFoundException">Thrown when the channel does not exist.</exception>
        /// <exception cref="BadRequestException">Thrown when an invalid parameter was provided.</exception>
        /// <exception cref="ServerErrorException">Thrown when Discord is unable to process the request.</exception>
        public async Task<DiscordChannel> CloneAsync(string reason = null)
        {
            if (this.Guild == null)
                throw new InvalidOperationException("Non-guild channels cannot be cloned.");

            var ovrs = new List<DiscordOverwriteBuilder>();
            foreach (var ovr in this._permissionOverwrites)
#pragma warning disable CS0618 // Type or member is obsolete
                ovrs.Add(await new DiscordOverwriteBuilder().FromAsync(ovr).ConfigureAwait(false));
#pragma warning restore CS0618 // Type or member is obsolete

            var bitrate = this.Bitrate;
            var userLimit = this.UserLimit;
            Optional<int?> perUserRateLimit = this.PerUserRateLimit;

            if (this.Type != ChannelType.Voice)
            {
                bitrate = null;
                userLimit = null;
            }
            if (this.Type != ChannelType.Text)
            {
                perUserRateLimit = Optional.FromNoValue<int?>();
            }

            return await this.Guild.CreateChannelAsync(this.Name, this.Type, this.Parent, this.Topic, bitrate, userLimit, ovrs, this.IsNSFW, perUserRateLimit, this.QualityMode, reason).ConfigureAwait(false);
        }

        /// <summary>
        /// Returns a specific message
        /// </summary>
        /// <param name="id">The id of the message</param>

        /// <exception cref="UnauthorizedException">Thrown when the client does not have the <see cref="Permissions.ReadMessageHistory"/> permission.</exception>
        /// <exception cref="NotFoundException">Thrown when the channel does not exist.</exception>
        /// <exception cref="BadRequestException">Thrown when an invalid parameter was provided.</exception>
        /// <exception cref="ServerErrorException">Thrown when Discord is unable to process the request.</exception>
        public async Task<DiscordMessage> GetMessageAsync(ulong id)
        {
            return this.Discord.Configuration.MessageCacheSize > 0
                && this.Discord is DiscordClient dc
                && dc.MessageCache != null
                && dc.MessageCache.TryGet(xm => xm.Id == id && xm.ChannelId == this.Id, out var msg)
                ? msg
                : await this.Discord.ApiClient.GetMessageAsync(this.Id, id).ConfigureAwait(false);
        }

        /// <summary>
        /// Modifies the current channel.
        /// </summary>
        /// <param name="action">Action to perform on this channel</param>

        /// <exception cref="UnauthorizedException">Thrown when the client does not have the <see cref="Permissions.ManageChannels"/>.</exception>
        /// <exception cref="NotSupportedException">Thrown when the client does not have the correct <see cref="PremiumTier"/> for modifying the <see cref="ThreadAutoArchiveDuration"/>.</exception>
        /// <exception cref="NotFoundException">Thrown when the channel does not exist.</exception>
        /// <exception cref="BadRequestException">Thrown when an invalid parameter was provided.</exception>
        /// <exception cref="ServerErrorException">Thrown when Discord is unable to process the request.</exception>
        public Task ModifyAsync(Action<ChannelEditModel> action)
        {
            var mdl = new ChannelEditModel();
            action(mdl);

            if (mdl.DefaultAutoArchiveDuration.HasValue)
            {
                if (!Utilities.CheckThreadAutoArchiveDurationFeature(this.Guild, mdl.DefaultAutoArchiveDuration.Value))
                    throw new NotSupportedException($"Cannot modify DefaultAutoArchiveDuration. Guild needs boost tier {(mdl.DefaultAutoArchiveDuration.Value == ThreadAutoArchiveDuration.ThreeDays ? "one" : "two")}.");
            }

            return this.Discord.ApiClient.ModifyChannelAsync(this.Id, mdl.Name, mdl.Position, mdl.Topic, mdl.Nsfw,
                mdl.Parent.HasValue ? mdl.Parent.Value?.Id : default(Optional<ulong?>), mdl.Bitrate, mdl.Userlimit, mdl.PerUserRateLimit, mdl.RtcRegion.IfPresent(r => r?.Id),
                mdl.QualityMode, mdl.DefaultAutoArchiveDuration, mdl.Type, mdl.PermissionOverwrites, mdl.AuditLogReason);
        }

        /// <summary>
        /// Updates the channel position when it doesn't have a category.
        /// 
        /// Use <see cref="ModifyParentAsync"/> for moving to other categories.
        /// Use <see cref="RemoveParentAsync"/> to move out of a category.
        /// Use <see cref="ModifyPositionInCategoryAsync"/> for moving within a category.
        /// </summary>
        /// <param name="position">Position the channel should be moved to.</param>
        /// <param name="reason">Reason for audit logs.</param>
        /// <exception cref="UnauthorizedException">Thrown when the client does not have the <see cref="Permissions.ManageChannels"/> permission.</exception>
        /// <exception cref="NotFoundException">Thrown when the channel does not exist.</exception>
        /// <exception cref="BadRequestException">Thrown when an invalid parameter was provided.</exception>
        /// <exception cref="ServerErrorException">Thrown when Discord is unable to process the request.</exception>
        [Obsolete("This will be replaced by ModifyPositionInCategoryAsync. Use it instead.")]
        public Task ModifyPositionAsync(int position, string reason = null)
        {
            if (this.Guild == null)
                throw new ArgumentException("Cannot modify order of non-guild channels.");
            if (this.ParentId != null)
                throw new ArgumentException("Cannot modify order of channels within a category. Use ModifyPositionInCategoryAsync instead.");

            var chns = this.Guild._channels.Values.Where(xc => xc.Type == this.Type).OrderBy(xc => xc.Position).ToArray();
            var pmds = new RestGuildChannelReorderPayload[chns.Length];
            for (var i = 0; i < chns.Length; i++)
            {
                pmds[i] = new RestGuildChannelReorderPayload
                {
                    ChannelId = chns[i].Id,
                };

                pmds[i].Position = chns[i].Id == this.Id ? position : chns[i].Position >= position ? chns[i].Position + 1 : chns[i].Position;
            }

            return this.Discord.ApiClient.ModifyGuildChannelPositionAsync(this.Guild.Id, pmds, reason);
        }

        /// <summary>
        /// Updates the channel position within it's own category.
        ///
        /// Use <see cref="ModifyParentAsync"/> for moving to other categories.
        /// Use <see cref="RemoveParentAsync"/> to move out of a category.
        /// Use <see cref="ModifyPositionAsync"/> to move channels outside a category.
        /// </summary>
        /// <param name="position">The position.</param>
        /// <param name="reason">The reason.</param>
        /// <exception cref="UnauthorizedException">Thrown when the client does not have the <see cref="Permissions.ManageChannels"/> permission.</exception>
        /// <exception cref="NotFoundException">Thrown when the channel does not exist.</exception>
        /// <exception cref="BadRequestException">Thrown when an invalid parameter was provided.</exception>
        /// <exception cref="ServerErrorException">Thrown when Discord is unable to process the request.</exception>
        /// <exception cref="IndexOutOfRangeException">Thrown when <paramref name="position"/> is out of range.</exception>
        /// <exception cref="ArgumentException">Thrown when function is called on a channel without a parent channel.</exception>
        public async Task ModifyPositionInCategoryAsync(int position, string reason = null)
        {
            //if (this.ParentId == null)
            //    throw new ArgumentException("You can call this function only on channels in categories.");

            var isUp = position > this.Position;

            var channels = await this.InternalRefreshChannelsAsync();

            var chns = this.ParentId != null
                ? this.Type == ChannelType.Text || this.Type == ChannelType.News
                    ? channels.Where(xc => xc.ParentId == this.ParentId && (xc.Type == ChannelType.Text || xc.Type == ChannelType.News))
                    : this.Type == ChannelType.Voice || this.Type == ChannelType.Stage
                        ? channels.Where(xc => xc.ParentId == this.ParentId && (xc.Type == ChannelType.Voice || xc.Type == ChannelType.Stage))
                        : channels.Where(xc => xc.ParentId == this.ParentId && xc.Type == this.Type)
                : this.Type == ChannelType.Text || this.Type == ChannelType.News
                    ? channels.Where(xc => xc.ParentId == null && (xc.Type == ChannelType.Text || xc.Type == ChannelType.News))
                    : this.Type == ChannelType.Voice || this.Type == ChannelType.Stage
                        ? channels.Where(xc => xc.ParentId == null && (xc.Type == ChannelType.Voice || xc.Type == ChannelType.Stage))
                        : channels.Where(xc => xc.ParentId == null && xc.Type == this.Type);

            var ochns = chns.OrderBy(xc => xc.Position).ToArray();
            var min = ochns.First().Position;
            var max = ochns.Last().Position;

            if (position > max || position < min)
                throw new IndexOutOfRangeException($"Position is not in range. {position} is {(position > max ? "greater then the maximal" : "lower then the minimal")} position.");

            var pmds = new RestGuildChannelReorderPayload[ochns.Length];
            for (var i = 0; i < ochns.Length; i++)
            {
                pmds[i] = new RestGuildChannelReorderPayload
                {
                    ChannelId = ochns[i].Id,
                };

                if (ochns[i].Id == this.Id)
                {
                    pmds[i].Position = position;
                }
                else
                {
                    if (isUp)
                    {
                        if (ochns[i].Position <= position && ochns[i].Position > this.Position)
                        {
                            pmds[i].Position = ochns[i].Position - 1;
                        }
                        else if (ochns[i].Position < this.Position || ochns[i].Position > position)
                        {
                            pmds[i].Position = ochns[i].Position;
                        }
                    }
                    else
                    {
                        if (ochns[i].Position >= position && ochns[i].Position < this.Position)
                        {
                            pmds[i].Position = ochns[i].Position + 1;
                        }
                        else if (ochns[i].Position > this.Position || ochns[i].Position < position)
                        {
                            pmds[i].Position = ochns[i].Position;
                        }
                    }
                }
            }

            await this.Discord.ApiClient.ModifyGuildChannelPositionAsync(this.Guild.Id, pmds, reason).ConfigureAwait(false);
        }

        /// <summary>
        /// Internaly refreshes the channel list.
        /// </summary>
        private async Task<IReadOnlyList<DiscordChannel>> InternalRefreshChannelsAsync()
        {
            await this.RefreshPositionsAsync();
            return this.Guild.Channels.Values.ToList().AsReadOnly();
        }

        /// <summary>
        /// Refreshes the positions.
        /// </summary>
        public async Task RefreshPositionsAsync()
        {
            var channels = await this.Discord.ApiClient.GetGuildChannelsAsync(this.Guild.Id);
            this.Guild._channels.Clear();
            foreach (var channel in channels.ToList())
            {
                channel.Discord = this.Discord;
                foreach (var xo in channel._permissionOverwrites)
                {
                    xo.Discord = this.Discord;
                    xo._channel_id = channel.Id;
                }
                this.Guild._channels[channel.Id] = channel;
            }
        }

        /// <summary>
        /// Updates the channel position within it's own category.
        /// Valid modes: '+' or 'down' to move a channel down | '-' or 'up' to move a channel up.
        /// 
        /// Use <see cref="ModifyParentAsync"/> for moving to other categories.
        /// Use <see cref="RemoveParentAsync"/> to move out of a category.
        /// Use <see cref="ModifyPositionAsync"/> to move channels outside a category.
        /// </summary>
        /// <param name="mode">The mode. Valid: '+' or 'down' to move a channel down | '-' or 'up' to move a channel up</param>
        /// <param name="position">The position.</param>
        /// <param name="reason">The reason.</param>
        /// <exception cref="UnauthorizedException">Thrown when the client does not have the <see cref="Permissions.ManageChannels"/> permission.</exception>
        /// <exception cref="NotFoundException">Thrown when the channel does not exist.</exception>
        /// <exception cref="BadRequestException">Thrown when an invalid parameter was provided.</exception>
        /// <exception cref="ServerErrorException">Thrown when Discord is unable to process the request.</exception>
        /// <exception cref="IndexOutOfRangeException">Thrown when <paramref name="position"/> is out of range.</exception>
        /// <exception cref="ArgumentException">Thrown when function is called on a channel without a parent channel, a wrong mode is givven or given position is zero.</exception>
        public Task ModifyPositionInCategorySmartAsync(string mode, int position, string reason = null)
        {
            if (mode != "+" && mode != "-" && mode != "down" && mode != "up")
                throw new ArgumentException("Error with the selected mode: Valid is '+' or 'down' to move a channel down and '-' or 'up' to move a channel up");

            var positive = mode == "+" || mode == "positive" || mode == "down";
            var negative = mode == "-" || mode == "negative" || mode == "up";
            return positive
                ? position < this.GetMaxPosition()
                    ? this.ModifyPositionInCategoryAsync(this.Position + position, reason)
                    : throw new IndexOutOfRangeException($"Position is not in range of category.")
                : negative
                    ? position > this.GetMinPosition()
                        ? this.ModifyPositionInCategoryAsync(this.Position - position, reason)
                        : throw new IndexOutOfRangeException($"Position is not in range of category.")
                    : throw new ArgumentException("You can only modify with +X or -X. 0 is not valid.");
        }

        /// <summary>
        /// Updates the channel parent, moving the channel to the bottom of the new category.
        /// </summary>
        /// <param name="newParent">New parent for channel. Will move out of parent if null.</param>
        /// <param name="lock_permissions">Sync permissions with parent. Defaults to null.</param>
        /// <param name="reason">Reason for audit logs.</param>
        /// <exception cref="UnauthorizedException">Thrown when the client does not have the <see cref="Permissions.ManageChannels"/> permission.</exception>
        /// <exception cref="NotFoundException">Thrown when the channel does not exist.</exception>
        /// <exception cref="BadRequestException">Thrown when an invalid parameter was provided.</exception>
        /// <exception cref="ServerErrorException">Thrown when Discord is unable to process the request.</exception>
#pragma warning disable CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
        public Task ModifyParentAsync(DiscordChannel? newParent = null, bool? lock_permissions = null, string reason = null)
#pragma warning restore CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
        {
            if (this.Guild == null)
                throw new ArgumentException("Cannot modify parent of non-guild channels.");
            if (this.IsCategory)
                throw new ArgumentException("Cannot modify parent of category channels.");
            if (newParent.Type is not ChannelType.Category)
                throw new ArgumentException("Only category type channels can be parents.");


            var position = this.Guild._channels.Values.Where(xc => xc.Type == this.Type && xc.ParentId == newParent.Id) // gets list same type channels in parent
                            .Select(xc => xc.Position).DefaultIfEmpty(-1).Max() + 1; // returns highest position of list +1, default val: 0

            var chns = this.Guild._channels.Values.Where(xc => xc.Type == this.Type)
                            .OrderBy(xc => xc.Position).ToArray();

            var pmds = new RestGuildChannelNewParentPayload[chns.Length];

            for (var i = 0; i < chns.Length; i++)
            {
                pmds[i] = new RestGuildChannelNewParentPayload
                {
                    ChannelId = chns[i].Id,
                    Position = chns[i].Position >= position ? chns[i].Position + 1 : chns[i].Position,
                };
                if (chns[i].Id == this.Id)
                {
                    pmds[i].Position = position;
                    pmds[i].ParentId = newParent is not null ? newParent.Id : null;
                    pmds[i].LockPermissions = lock_permissions;
                }
            }

            return this.Discord.ApiClient.ModifyGuildChannelParentAsync(this.Guild.Id, pmds, reason);
        }

        /// <summary>
        /// Moves the channel out of a category.
        /// </summary>
        /// <param name="reason">Reason for audit logs.</param>

        /// <exception cref="UnauthorizedException">Thrown when the client does not have the <see cref="Permissions.ManageChannels"/> permission.</exception>
        /// <exception cref="NotFoundException">Thrown when the channel does not exist.</exception>
        /// <exception cref="BadRequestException">Thrown when an invalid parameter was provided.</exception>
        /// <exception cref="ServerErrorException">Thrown when Discord is unable to process the request.</exception>
        public Task RemoveParentAsync(string reason = null)
        {
            if (this.Guild == null)
                throw new ArgumentException("Cannot modify parent of non-guild channels.");
            if (this.IsCategory)
                throw new ArgumentException("Cannot modify parent of category channels.");

            var position = this.Guild._channels.Values.Where(xc => xc.Type == this.Type && xc.Parent is null) //gets list of same type channels with no parent
                            .Select(xc => xc.Position).DefaultIfEmpty(-1).Max() + 1; // returns highest position of list +1, default val: 0

            var chns = this.Guild._channels.Values.Where(xc => xc.Type == this.Type)
                .OrderBy(xc => xc.Position).ToArray();

            var pmds = new RestGuildChannelNoParentPayload[chns.Length];

            for (var i = 0; i < chns.Length; i++)
            {
                pmds[i] = new RestGuildChannelNoParentPayload
                {
                    ChannelId = chns[i].Id,
                };
                if (chns[i].Id == this.Id)
                {
                    pmds[i].Position = 1;
                    pmds[i].ParentId = null;
                }
                else
                {
                    pmds[i].Position = chns[i].Position < this.Position ? chns[i].Position + 1 : chns[i].Position;
                }
            }

            return this.Discord.ApiClient.DetachGuildChannelParentAsync(this.Guild.Id, pmds, reason);
        }

        /// <summary>
        /// Returns a list of messages before a certain message.
        /// <param name="limit">The amount of messages to fetch.</param>
        /// <param name="before">Message to fetch before from.</param>
        /// </summary>
        /// <exception cref="UnauthorizedException">Thrown when the client does not have the <see cref="Permissions.AccessChannels"/> permission.</exception>
        /// <exception cref="NotFoundException">Thrown when the channel does not exist.</exception>
        /// <exception cref="BadRequestException">Thrown when an invalid parameter was provided.</exception>
        /// <exception cref="ServerErrorException">Thrown when Discord is unable to process the request.</exception>
        public Task<IReadOnlyList<DiscordMessage>> GetMessagesBeforeAsync(ulong before, int limit = 100)
            => this.GetMessagesInternalAsync(limit, before, null, null);

        /// <summary>
        /// Returns a list of messages after a certain message.
        /// <param name="limit">The amount of messages to fetch.</param>
        /// <param name="after">Message to fetch after from.</param>
        /// </summary>
        /// <exception cref="UnauthorizedException">Thrown when the client does not have the <see cref="Permissions.AccessChannels"/> permission.</exception>
        /// <exception cref="NotFoundException">Thrown when the channel does not exist.</exception>
        /// <exception cref="BadRequestException">Thrown when an invalid parameter was provided.</exception>
        /// <exception cref="ServerErrorException">Thrown when Discord is unable to process the request.</exception>
        public Task<IReadOnlyList<DiscordMessage>> GetMessagesAfterAsync(ulong after, int limit = 100)
            => this.GetMessagesInternalAsync(limit, null, after, null);

        /// <summary>
        /// Returns a list of messages around a certain message.
        /// <param name="limit">The amount of messages to fetch.</param>
        /// <param name="around">Message to fetch around from.</param>
        /// </summary>
        /// <exception cref="UnauthorizedException">Thrown when the client does not have the <see cref="Permissions.AccessChannels"/> permission.</exception>
        /// <exception cref="NotFoundException">Thrown when the channel does not exist.</exception>
        /// <exception cref="BadRequestException">Thrown when an invalid parameter was provided.</exception>
        /// <exception cref="ServerErrorException">Thrown when Discord is unable to process the request.</exception>
        public Task<IReadOnlyList<DiscordMessage>> GetMessagesAroundAsync(ulong around, int limit = 100)
            => this.GetMessagesInternalAsync(limit, null, null, around);

        /// <summary>
        /// Returns a list of messages from the last message in the channel.
        /// <param name="limit">The amount of messages to fetch.</param>
        /// </summary>
        /// <exception cref="UnauthorizedException">Thrown when the client does not have the <see cref="Permissions.AccessChannels"/> permission.</exception>
        /// <exception cref="NotFoundException">Thrown when the channel does not exist.</exception>
        /// <exception cref="BadRequestException">Thrown when an invalid parameter was provided.</exception>
        /// <exception cref="ServerErrorException">Thrown when Discord is unable to process the request.</exception>
        public Task<IReadOnlyList<DiscordMessage>> GetMessagesAsync(int limit = 100) =>
            this.GetMessagesInternalAsync(limit, null, null, null);

        /// <summary>
        /// Returns a list of messages
        /// </summary>
        /// <param name="limit">How many messages should be returned.</param>
        /// <param name="before">Get messages before snowflake.</param>
        /// <param name="after">Get messages after snowflake.</param>
        /// <param name="around">Get messages around snowflake.</param>
        private async Task<IReadOnlyList<DiscordMessage>> GetMessagesInternalAsync(int limit = 100, ulong? before = null, ulong? after = null, ulong? around = null)
        {
            if (this.Type != ChannelType.Text && this.Type != ChannelType.Private && this.Type != ChannelType.Group && this.Type != ChannelType.News)
                throw new ArgumentException("Cannot get the messages of a non-text channel.");

            if (limit < 0)
                throw new ArgumentException("Cannot get a negative number of messages.");

            if (limit == 0)
                return Array.Empty<DiscordMessage>();

            //return this.Discord.ApiClient.GetChannelMessagesAsync(this.Id, limit, before, after, around);
            if (limit > 100 && around != null)
                throw new InvalidOperationException("Cannot get more than 100 messages around the specified ID.");

            var msgs = new List<DiscordMessage>(limit);
            var remaining = limit;
            ulong? last = null;
            var isAfter = after != null;

            int lastCount;
            do
            {
                var fetchSize = remaining > 100 ? 100 : remaining;
                var fetch = await this.Discord.ApiClient.GetChannelMessagesAsync(this.Id, fetchSize, !isAfter ? last ?? before : null, isAfter ? last ?? after : null, around).ConfigureAwait(false);

                lastCount = fetch.Count;
                remaining -= lastCount;

                if (!isAfter)
                {
                    msgs.AddRange(fetch);
                    last = fetch.LastOrDefault()?.Id;
                }
                else
                {
                    msgs.InsertRange(0, fetch);
                    last = fetch.FirstOrDefault()?.Id;
                }
            }
            while (remaining > 0 && lastCount > 0);

            return new ReadOnlyCollection<DiscordMessage>(msgs);
        }

        /// <summary>
        /// Deletes multiple messages if they are less than 14 days old.  If they are older, none of the messages will be deleted and you will receive a <see cref="BadRequestException"/> error.
        /// </summary>
        /// <param name="messages">A collection of messages to delete.</param>
        /// <param name="reason">Reason for audit logs.</param>

        /// <exception cref="UnauthorizedException">Thrown when the client does not have the <see cref="Permissions.ManageMessages"/> permission.</exception>
        /// <exception cref="NotFoundException">Thrown when the channel does not exist.</exception>
        /// <exception cref="BadRequestException">Thrown when an invalid parameter was provided.</exception>
        /// <exception cref="ServerErrorException">Thrown when Discord is unable to process the request.</exception>
        public async Task DeleteMessagesAsync(IEnumerable<DiscordMessage> messages, string reason = null)
        {
            // don't enumerate more than once
            var msgs = messages.Where(x => x.Channel.Id == this.Id).Select(x => x.Id).ToArray();
            if (messages == null || !msgs.Any())
                throw new ArgumentException("You need to specify at least one message to delete.");

            if (msgs.Count() < 2)
            {
                await this.Discord.ApiClient.DeleteMessageAsync(this.Id, msgs.Single(), reason).ConfigureAwait(false);
                return;
            }

            for (var i = 0; i < msgs.Count(); i += 100)
                await this.Discord.ApiClient.DeleteMessagesAsync(this.Id, msgs.Skip(i).Take(100), reason).ConfigureAwait(false);
        }

        /// <summary>
        /// Deletes a message
        /// </summary>
        /// <param name="message">The message to be deleted.</param>
        /// <param name="reason">Reason for audit logs.</param>

        /// <exception cref="UnauthorizedException">Thrown when the client does not have the <see cref="Permissions.ManageMessages"/> permission.</exception>
        /// <exception cref="NotFoundException">Thrown when the channel does not exist.</exception>
        /// <exception cref="BadRequestException">Thrown when an invalid parameter was provided.</exception>
        /// <exception cref="ServerErrorException">Thrown when Discord is unable to process the request.</exception>
        public Task DeleteMessageAsync(DiscordMessage message, string reason = null)
            => this.Discord.ApiClient.DeleteMessageAsync(this.Id, message.Id, reason);

        /// <summary>
        /// Returns a list of invite objects
        /// </summary>

        /// <exception cref="UnauthorizedException">Thrown when the client does not have the <see cref="Permissions.CreateInstantInvite"/> permission.</exception>
        /// <exception cref="NotFoundException">Thrown when the channel does not exist.</exception>
        /// <exception cref="BadRequestException">Thrown when an invalid parameter was provided.</exception>
        /// <exception cref="ServerErrorException">Thrown when Discord is unable to process the request.</exception>
        public Task<IReadOnlyList<DiscordInvite>> GetInvitesAsync()
        {
            return this.Guild == null
                ? throw new ArgumentException("Cannot get the invites of a channel that does not belong to a guild.")
                : this.Discord.ApiClient.GetChannelInvitesAsync(this.Id);
        }

        /// <summary>
        /// Create a new invite object
        /// </summary>
        /// <param name="max_age">Duration of invite in seconds before expiry, or 0 for never.  Defaults to 86400.</param>
        /// <param name="max_uses">Max number of uses or 0 for unlimited.  Defaults to 0</param>

        /// <param name="temporary">Whether this invite only grants temporary membership.  Defaults to false.</param>
        /// <param name="unique">If true, don't try to reuse a similar invite (useful for creating many unique one time use invites)</param>
        /// <param name="target_type">Target type of invite for the channel.  Defaults to Streaming</param>
        /// <param name="target_application">Target application of invite for the channel.  Defaults to None</param>
        /// <param name="reason">Reason for audit logs.</param>

        /// <exception cref="UnauthorizedException">Thrown when the client does not have the <see cref="Permissions.CreateInstantInvite"/> permission.</exception>
        /// <exception cref="NotFoundException">Thrown when the channel does not exist.</exception>
        /// <exception cref="BadRequestException">Thrown when an invalid parameter was provided.</exception>
        /// <exception cref="ServerErrorException">Thrown when Discord is unable to process the request.</exception>
        public Task<DiscordInvite> CreateInviteAsync(int max_age = 86400, int max_uses = 0, bool temporary = false, bool unique = false, TargetType? target_type = null, TargetActivity? target_application = null, string reason = null)
            => this.Discord.ApiClient.CreateChannelInviteAsync(this.Id, max_age, max_uses, target_type, target_application, temporary, unique, reason);

        #region Stage

        /// <summary>
        /// Opens a stage.
        /// </summary>
        /// <param name="topic">Topic of the stage.</param>
        /// <param name="send_start_notification">Whether @everyone should be notified.</param>
        /// <param name="privacy_level">Privacy level of the stage (Defaults to <see cref="StagePrivacyLevel.GUILD_ONLY"/>.</param>
        /// <param name="reason">Audit log reason.</param>
        /// <returns>Stage instance</returns>
        /// <exception cref="UnauthorizedException">Thrown when the client does not have the <see cref="Permissions.ManageChannels"/> permission.</exception>
        /// <exception cref="NotFoundException">Thrown when the channel does not exist.</exception>
        /// <exception cref="BadRequestException">Thrown when an invalid parameter was provided.</exception>
        /// <exception cref="ServerErrorException">Thrown when Discord is unable to process the request.</exception>
        public async Task<DiscordStageInstance> OpenStageAsync(string topic, bool send_start_notification = false, StagePrivacyLevel privacy_level = StagePrivacyLevel.GUILD_ONLY, string reason = null)
            => await this.Discord.ApiClient.CreateStageInstanceAsync(this.Id, topic, send_start_notification, privacy_level, reason);

        /// <summary>
        /// Modifies a stage topic.
        /// </summary>
        /// <param name="topic">New topic of the stage.</param>
        /// <param name="privacy_level">New privacy level of the stage.</param>
        /// <param name="reason">Audit log reason.</param>
        /// <exception cref="UnauthorizedException">Thrown when the client does not have the <see cref="Permissions.ManageChannels"/> permission.</exception>
        /// <exception cref="NotFoundException">Thrown when the channel does not exist.</exception>
        /// <exception cref="BadRequestException">Thrown when an invalid parameter was provided.</exception>
        /// <exception cref="ServerErrorException">Thrown when Discord is unable to process the request.</exception>
        public async Task ModifyStageAsync(Optional<string> topic, Optional<StagePrivacyLevel> privacy_level, string reason = null)
            => await this.Discord.ApiClient.ModifyStageInstanceAsync(this.Id, topic, privacy_level, reason);

        /// <summary>
        /// Closes a stage.
        /// </summary>
        /// <param name="reason">Audit log reason.</param>
        /// <exception cref="UnauthorizedException">Thrown when the client does not have the <see cref="Permissions.ManageChannels"/> permission.</exception>
        /// <exception cref="NotFoundException">Thrown when the channel does not exist.</exception>
        /// <exception cref="BadRequestException">Thrown when an invalid parameter was provided.</exception>
        /// <exception cref="ServerErrorException">Thrown when Discord is unable to process the request.</exception>
        public async Task CloseStageAsync(string reason = null)
            => await this.Discord.ApiClient.DeleteStageInstanceAsync(this.Id, reason);

        /// <summary>
        /// Gets a stage.
        /// </summary>
        /// <returns>The requested stage.</returns>
        /// <exception cref="UnauthorizedException">Thrown when the client does not have the <see cref="Permissions.AccessChannels"/> or <see cref="Permissions.UseVoice"/> permission.</exception>
        /// <exception cref="NotFoundException">Thrown when the channel does not exist.</exception>
        /// <exception cref="BadRequestException">Thrown when an invalid parameter was provided.</exception>
        /// <exception cref="ServerErrorException">Thrown when Discord is unable to process the request.</exception>
        public async Task<DiscordStageInstance> GetStageAsync()
            => await this.Discord.ApiClient.GetStageInstanceAsync(this.Id);

        #endregion

        #region Threads

        /// <summary>
        /// Creates a thread.
        /// Depending on whether it is created inside an <see cref="ChannelType.News"/> or an <see cref="ChannelType.Text"/> it is either an <see cref="ChannelType.NewsThread"/> or an <see cref="ChannelType.PublicThread"/>.
        /// Depending on whether the <see cref="ChannelType"/> is set to <see cref="ChannelType.PrivateThread"/> it is either an <see cref="ChannelType.PrivateThread"/> or an <see cref="ChannelType.PublicThread"/> (default).
        /// </summary>
        /// <param name="name">The name of the thread.</param>
        /// <param name="auto_archive_duration"><see cref="ThreadAutoArchiveDuration"/> till it gets archived. Defaults to <see cref="ThreadAutoArchiveDuration.OneHour"/>.</param>
        /// <param name="type">Can be either an <see cref="ChannelType.PrivateThread"/>, <see cref="ChannelType.NewsThread"/> or an <see cref="ChannelType.PublicThread"/>.</param>
        /// <param name="reason">Audit log reason.</param>
        /// <returns>The created thread.</returns>
        /// <exception cref="UnauthorizedException">Thrown when the client does not have the <see cref="Permissions.CreatePublicThreads"/> or <see cref="Permissions.SendMessagesInThreads"/> or if creating a private thread the <see cref="Permissions.CreatePrivateThreads"/> permission.</exception>
        /// <exception cref="NotFoundException">Thrown when the guild hasn't enabled threads atm.</exception>
        /// <exception cref="BadRequestException">Thrown when an invalid parameter was provided.</exception>
        /// <exception cref="ServerErrorException">Thrown when Discord is unable to process the request.</exception>
        /// <exception cref="NotSupportedException">Thrown when the <see cref="ThreadAutoArchiveDuration"/> cannot be modified. This happens, when the guild hasn't reached a certain boost <see cref="PremiumTier"/>. Or if <see cref="GuildFeatures.CanCreatePrivateThreads"/> is not enabled for guild. This happens, if the guild does not have <see cref="PremiumTier.Tier_2"/></exception>
        public async Task<DiscordThreadChannel> CreateThreadAsync(string name, ThreadAutoArchiveDuration auto_archive_duration = ThreadAutoArchiveDuration.OneHour, ChannelType type = ChannelType.PublicThread, string reason = null)
        {
            return (type != ChannelType.NewsThread && type != ChannelType.PublicThread && type != ChannelType.PrivateThread)
                ? throw new NotSupportedException("Wrong thread type given.")
                : (this.Type != ChannelType.News && this.Type != ChannelType.Text)
                ? throw new NotSupportedException("Parent channel is no text or news channel")
                : type == ChannelType.PrivateThread
                ? Utilities.CheckThreadPrivateFeature(this.Guild)
                ? Utilities.CheckThreadAutoArchiveDurationFeature(this.Guild, auto_archive_duration)
                    ? await this.Discord.ApiClient.CreateThreadWithoutMessageAsync(this.Id, name, auto_archive_duration, type, reason)
                    : throw new NotSupportedException($"Cannot modify ThreadAutoArchiveDuration. Guild needs boost tier {(auto_archive_duration == ThreadAutoArchiveDuration.ThreeDays ? "one" : "two")}.")
                : throw new NotSupportedException($"Cannot create a private thread. Guild needs to be boost tier two.")
                : Utilities.CheckThreadAutoArchiveDurationFeature(this.Guild, auto_archive_duration)
                    ? await this.Discord.ApiClient.CreateThreadWithoutMessageAsync(this.Id, name, auto_archive_duration, this.Type == ChannelType.News ? ChannelType.NewsThread : ChannelType.PublicThread, reason)
                : throw new NotSupportedException($"Cannot modify ThreadAutoArchiveDuration. Guild needs boost tier {(auto_archive_duration == ThreadAutoArchiveDuration.ThreeDays ? "one" : "two")}.");
        }


        /// <summary>
        /// Gets joined archived private threads. Can contain more threads.
        /// If the result's value 'HasMore' is true, you need to recall this function to get older threads.
        /// </summary>
        /// <param name="before">Get threads created before this thread id.</param>
        /// <param name="limit">Defines the limit of returned <see cref="DiscordThreadResult"/>.</param>
        /// <exception cref="UnauthorizedException">Thrown when the client does not have the <see cref="Permissions.ReadMessageHistory"/> permission.</exception>
        /// <exception cref="NotFoundException">Thrown when the channel does not exist.</exception>
        /// <exception cref="BadRequestException">Thrown when an invalid parameter was provided.</exception>
        /// <exception cref="ServerErrorException">Thrown when Discord is unable to process the request.</exception>
        public async Task<DiscordThreadResult> GetJoinedPrivateArchivedThreadsAsync(ulong? before, int? limit)
            => await this.Discord.ApiClient.GetJoinedPrivateArchivedThreadsAsync(this.Id, before, limit);

        /// <summary>
        /// Gets archived public threads. Can contain more threads.
        /// If the result's value 'HasMore' is true, you need to recall this function to get older threads.
        /// </summary>
        /// <param name="before">Get threads created before this thread id.</param>
        /// <param name="limit">Defines the limit of returned <see cref="DiscordThreadResult"/>.</param>
        /// <exception cref="UnauthorizedException">Thrown when the client does not have the <see cref="Permissions.ReadMessageHistory"/> permission.</exception>
        /// <exception cref="NotFoundException">Thrown when the channel does not exist.</exception>
        /// <exception cref="BadRequestException">Thrown when an invalid parameter was provided.</exception>
        /// <exception cref="ServerErrorException">Thrown when Discord is unable to process the request.</exception>
        public async Task<DiscordThreadResult> GetPublicArchivedThreadsAsync(ulong? before, int? limit)
            => await this.Discord.ApiClient.GetPublicArchivedThreadsAsync(this.Id, before, limit);

        /// <summary>
        /// Gets archived private threads. Can contain more threads.
        /// If the result's value 'HasMore' is true, you need to recall this function to get older threads.
        /// </summary>
        /// <param name="before">Get threads created before this thread id.</param>
        /// <param name="limit">Defines the limit of returned <see cref="DiscordThreadResult"/>.</param>
        /// <exception cref="UnauthorizedException">Thrown when the client does not have the <see cref="Permissions.ManageThreads"/> or <see cref="Permissions.ReadMessageHistory"/> permission.</exception>
        /// <exception cref="NotFoundException">Thrown when the channel does not exist.</exception>
        /// <exception cref="BadRequestException">Thrown when an invalid parameter was provided.</exception>
        /// <exception cref="ServerErrorException">Thrown when Discord is unable to process the request.</exception>
        public async Task<DiscordThreadResult> GetPrivateArchivedThreadsAsync(ulong? before, int? limit)
            => await this.Discord.ApiClient.GetPrivateArchivedThreadsAsync(this.Id, before, limit);

        #endregion

        /// <summary>
        /// Adds a channel permission overwrite for specified role.
        /// </summary>
        /// <param name="role">The role to have the permission added.</param>
        /// <param name="allow">The permissions to allow.</param>
        /// <param name="deny">The permissions to deny.</param>
        /// <param name="reason">Reason for audit logs.</param>

        /// <exception cref="UnauthorizedException">Thrown when the client does not have the <see cref="Permissions.ManageRoles"/> permission.</exception>
        /// <exception cref="NotFoundException">Thrown when the channel does not exist.</exception>
        /// <exception cref="BadRequestException">Thrown when an invalid parameter was provided.</exception>
        /// <exception cref="ServerErrorException">Thrown when Discord is unable to process the request.</exception>
        public Task AddOverwriteAsync(DiscordRole role, Permissions allow = Permissions.None, Permissions deny = Permissions.None, string reason = null)
            => this.Discord.ApiClient.EditChannelPermissionsAsync(this.Id, role.Id, allow, deny, "role", reason);


        /// <summary>
        /// Adds a channel permission overwrite for specified member.
        /// </summary>
        /// <param name="member">The member to have the permission added.</param>
        /// <param name="allow">The permissions to allow.</param>
        /// <param name="deny">The permissions to deny.</param>
        /// <param name="reason">Reason for audit logs.</param>

        /// <exception cref="UnauthorizedException">Thrown when the client does not have the <see cref="Permissions.ManageRoles"/> permission.</exception>
        /// <exception cref="NotFoundException">Thrown when the channel does not exist.</exception>
        /// <exception cref="BadRequestException">Thrown when an invalid parameter was provided.</exception>
        /// <exception cref="ServerErrorException">Thrown when Discord is unable to process the request.</exception>
        public Task AddOverwriteAsync(DiscordMember member, Permissions allow = Permissions.None, Permissions deny = Permissions.None, string reason = null)
            => this.Discord.ApiClient.EditChannelPermissionsAsync(this.Id, member.Id, allow, deny, "member", reason);

        /// <summary>
        /// Deletes a channel permission overwrite for specified member.
        /// </summary>
        /// <param name="member">The member to have the permission deleted.</param>
        /// <param name="reason">Reason for audit logs.</param>

        /// <exception cref="UnauthorizedException">Thrown when the client does not have the <see cref="Permissions.ManageRoles"/> permission.</exception>
        /// <exception cref="NotFoundException">Thrown when the channel does not exist.</exception>
        /// <exception cref="BadRequestException">Thrown when an invalid parameter was provided.</exception>
        /// <exception cref="ServerErrorException">Thrown when Discord is unable to process the request.</exception>
        public Task DeleteOverwriteAsync(DiscordMember member, string reason = null)
            => this.Discord.ApiClient.DeleteChannelPermissionAsync(this.Id, member.Id, reason);

        /// <summary>
        /// Deletes a channel permission overwrite for specified role.
        /// </summary>
        /// <param name="role">The role to have the permission deleted.</param>
        /// <param name="reason">Reason for audit logs.</param>

        /// <exception cref="UnauthorizedException">Thrown when the client does not have the <see cref="Permissions.ManageRoles"/> permission.</exception>
        /// <exception cref="NotFoundException">Thrown when the channel does not exist.</exception>
        /// <exception cref="BadRequestException">Thrown when an invalid parameter was provided.</exception>
        /// <exception cref="ServerErrorException">Thrown when Discord is unable to process the request.</exception>
        public Task DeleteOverwriteAsync(DiscordRole role, string reason = null)
            => this.Discord.ApiClient.DeleteChannelPermissionAsync(this.Id, role.Id, reason);

        /// <summary>
        /// Post a typing indicator
        /// </summary>

        /// <exception cref="NotFoundException">Thrown when the channel does not exist.</exception>
        /// <exception cref="BadRequestException">Thrown when an invalid parameter was provided.</exception>
        /// <exception cref="ServerErrorException">Thrown when Discord is unable to process the request.</exception>
        public Task TriggerTypingAsync()
        {
            return this.Type != ChannelType.Text && this.Type != ChannelType.Private && this.Type != ChannelType.Group && this.Type != ChannelType.News
                ? throw new ArgumentException("Cannot start typing in a non-text channel.")
                : this.Discord.ApiClient.TriggerTypingAsync(this.Id);
        }

        /// <summary>
        /// Returns all pinned messages
        /// </summary>

        /// <exception cref="UnauthorizedException">Thrown when the client does not have the <see cref="Permissions.AccessChannels"/> permission.</exception>
        /// <exception cref="NotFoundException">Thrown when the channel does not exist.</exception>
        /// <exception cref="BadRequestException">Thrown when an invalid parameter was provided.</exception>
        /// <exception cref="ServerErrorException">Thrown when Discord is unable to process the request.</exception>
        public Task<IReadOnlyList<DiscordMessage>> GetPinnedMessagesAsync()
        {
            return this.Type != ChannelType.Text && this.Type != ChannelType.Private && this.Type != ChannelType.Group && this.Type != ChannelType.News
                ? throw new ArgumentException("A non-text channel does not have pinned messages.")
                : this.Discord.ApiClient.GetPinnedMessagesAsync(this.Id);
        }

        /// <summary>
        /// Create a new webhook
        /// </summary>
        /// <param name="name">The name of the webhook.</param>
        /// <param name="avatar">The image for the default webhook avatar.</param>
        /// <param name="reason">Reason for audit logs.</param>

        /// <exception cref="UnauthorizedException">Thrown when the client does not have the <see cref="Permissions.ManageWebhooks"/> permission.</exception>
        /// <exception cref="NotFoundException">Thrown when the channel does not exist.</exception>
        /// <exception cref="BadRequestException">Thrown when an invalid parameter was provided.</exception>
        /// <exception cref="ServerErrorException">Thrown when Discord is unable to process the request.</exception>
        public async Task<DiscordWebhook> CreateWebhookAsync(string name, Optional<Stream> avatar = default, string reason = null)
        {
            var av64 = Optional.FromNoValue<string>();
            if (avatar.HasValue && avatar.Value != null)
                using (var imgtool = new ImageTool(avatar.Value))
                    av64 = imgtool.GetBase64();
            else if (avatar.HasValue)
                av64 = null;

            return await this.Discord.ApiClient.CreateWebhookAsync(this.Id, name, av64, reason).ConfigureAwait(false);
        }

        /// <summary>
        /// Returns a list of webhooks
        /// </summary>

        /// <exception cref="UnauthorizedException">Thrown when the client does not have the <see cref="Permissions.ManageWebhooks"/> permission.</exception>
        /// <exception cref="NotFoundException">Thrown when the channel does not exist.</exception>
        /// <exception cref="ServerErrorException">Thrown when Discord is unable to process the request.</exception>
        public Task<IReadOnlyList<DiscordWebhook>> GetWebhooksAsync()
            => this.Discord.ApiClient.GetChannelWebhooksAsync(this.Id);

        /// <summary>
        /// Moves a member to this voice channel
        /// </summary>
        /// <param name="member">The member to be moved.</param>

        /// <exception cref="UnauthorizedException">Thrown when the client does not have the <see cref="Permissions.MoveMembers"/> permission.</exception>
        /// <exception cref="NotFoundException">Thrown when the channel does not exists or if the Member does not exists.</exception>
        /// <exception cref="BadRequestException">Thrown when an invalid parameter was provided.</exception>
        /// <exception cref="ServerErrorException">Thrown when Discord is unable to process the request.</exception>
        public async Task PlaceMemberAsync(DiscordMember member)
        {
            if (this.Type != ChannelType.Voice && this.Type != ChannelType.Stage)
                throw new ArgumentException("Cannot place a member in a non-voice channel!"); // be a little more angry, let em learn!!1

            await this.Discord.ApiClient.ModifyGuildMemberAsync(this.Guild.Id, member.Id, default, default, default,
                default, this.Id, null).ConfigureAwait(false);
        }

        /// <summary>
        /// Follows a news channel
        /// </summary>
        /// <param name="targetChannel">Channel to crosspost messages to</param>
        /// <exception cref="ArgumentException">Thrown when trying to follow a non-news channel</exception>
        /// <exception cref="UnauthorizedException">Thrown when the current user doesn't have <see cref="Permissions.ManageWebhooks"/> on the target channel</exception>
        public Task<DiscordFollowedChannel> FollowAsync(DiscordChannel targetChannel)
        {
            return this.Type != ChannelType.News
                ? throw new ArgumentException("Cannot follow a non-news channel.")
                : this.Discord.ApiClient.FollowChannelAsync(this.Id, targetChannel.Id);
        }

        /// <summary>
        /// Publishes a message in a news channel to following channels
        /// </summary>
        /// <param name="message">Message to publish</param>
        /// <exception cref="ArgumentException">Thrown when the message has already been crossposted</exception>
        /// <exception cref="UnauthorizedException">
        ///     Thrown when the current user doesn't have <see cref="Permissions.ManageWebhooks"/> and/or <see cref="Permissions.SendMessages"/>
        /// </exception>
        public Task<DiscordMessage> CrosspostMessageAsync(DiscordMessage message)
        {
            return (message.Flags & MessageFlags.Crossposted) == MessageFlags.Crossposted
                ? throw new ArgumentException("Message is already crossposted.")
                : this.Discord.ApiClient.CrosspostMessageAsync(this.Id, message.Id);
        }

        /// <summary>
        /// Updates the current user's suppress state in this channel, if stage channel.
        /// </summary>
        /// <param name="suppress">Toggles the suppress state.</param>
        /// <param name="requestToSpeakTimestamp">Sets the time the user requested to speak.</param>
        /// <exception cref="ArgumentException">Thrown when the channel is not a stage channel.</exception>
        public async Task UpdateCurrentUserVoiceStateAsync(bool? suppress, DateTimeOffset? requestToSpeakTimestamp = null)
        {
            if (this.Type != ChannelType.Stage)
                throw new ArgumentException("Voice state can only be updated in a stage channel.");

            await this.Discord.ApiClient.UpdateCurrentUserVoiceStateAsync(this.GuildId.Value, this.Id, suppress, requestToSpeakTimestamp).ConfigureAwait(false);
        }

        /// <summary>
        /// Calculates permissions for a given member.
        /// </summary>
        /// <param name="mbr">Member to calculate permissions for.</param>
        /// <returns>Calculated permissions for a given member.</returns>
        public Permissions PermissionsFor(DiscordMember mbr)
        {
            // future note: might be able to simplify @everyone role checks to just check any role ... but I'm not sure
            // xoxo, ~uwx
            //
            // you should use a single tilde
            // ~emzi

            // user > role > everyone
            // allow > deny > undefined
            // =>
            // user allow > user deny > role allow > role deny > everyone allow > everyone deny
            // thanks to meew0

            if (this.IsPrivate || this.Guild == null)
                return Permissions.None;

            if (this.Guild.OwnerId == mbr.Id)
                return PermissionMethods.FULL_PERMS;

            Permissions perms;

            // assign @everyone permissions
            var everyoneRole = this.Guild.EveryoneRole;
            perms = everyoneRole.Permissions;

            // roles that member is in
            var mbRoles = mbr.Roles.Where(xr => xr.Id != everyoneRole.Id).ToArray();

            // assign permissions from member's roles (in order)
            perms |= mbRoles.Aggregate(Permissions.None, (c, role) => c | role.Permissions);

            // Adminstrator grants all permissions and cannot be overridden
            if ((perms & Permissions.Administrator) == Permissions.Administrator)
                return PermissionMethods.FULL_PERMS;

            // channel overrides for roles that member is in
            var mbRoleOverrides = mbRoles
                .Select(xr => this._permissionOverwrites.FirstOrDefault(xo => xo.Id == xr.Id))
                .Where(xo => xo != null)
                .ToList();

            // assign channel permission overwrites for @everyone pseudo-role
            var everyoneOverwrites = this._permissionOverwrites.FirstOrDefault(xo => xo.Id == everyoneRole.Id);
            if (everyoneOverwrites != null)
            {
                perms &= ~everyoneOverwrites.Denied;
                perms |= everyoneOverwrites.Allowed;
            }

            // assign channel permission overwrites for member's roles (explicit deny)
            perms &= ~mbRoleOverrides.Aggregate(Permissions.None, (c, overs) => c | overs.Denied);
            // assign channel permission overwrites for member's roles (explicit allow)
            perms |= mbRoleOverrides.Aggregate(Permissions.None, (c, overs) => c | overs.Allowed);

            // channel overrides for just this member
            var mbOverrides = this._permissionOverwrites.FirstOrDefault(xo => xo.Id == mbr.Id);
            if (mbOverrides == null) return perms;

            // assign channel permission overwrites for just this member
            perms &= ~mbOverrides.Denied;
            perms |= mbOverrides.Allowed;

            return perms;
        }

        /// <summary>
        /// Returns a string representation of this channel.
        /// </summary>
        /// <returns>String representation of this channel.</returns>
        public override string ToString()
        {
            return this.Type == ChannelType.Category
                ? $"Channel Category {this.Name} ({this.Id})"
                : this.Type == ChannelType.Text || this.Type == ChannelType.News
                ? $"Channel #{this.Name} ({this.Id})"
                : !string.IsNullOrWhiteSpace(this.Name) ? $"Channel {this.Name} ({this.Id})" : $"Channel {this.Id}";
        }
        #endregion

        /// <summary>
        /// Checks whether this <see cref="DiscordChannel"/> is equal to another object.
        /// </summary>
        /// <param name="obj">Object to compare to.</param>
        /// <returns>Whether the object is equal to this <see cref="DiscordChannel"/>.</returns>
        public override bool Equals(object obj) => this.Equals(obj as DiscordChannel);

        /// <summary>
        /// Checks whether this <see cref="DiscordChannel"/> is equal to another <see cref="DiscordChannel"/>.
        /// </summary>
        /// <param name="e"><see cref="DiscordChannel"/> to compare to.</param>
        /// <returns>Whether the <see cref="DiscordChannel"/> is equal to this <see cref="DiscordChannel"/>.</returns>
        public bool Equals(DiscordChannel e) => e is not null && (ReferenceEquals(this, e) || this.Id == e.Id);

        /// <summary>
        /// Gets the hash code for this <see cref="DiscordChannel"/>.
        /// </summary>
        /// <returns>The hash code for this <see cref="DiscordChannel"/>.</returns>
        public override int GetHashCode() => this.Id.GetHashCode();

        /// <summary>
        /// Gets whether the two <see cref="DiscordChannel"/> objects are equal.
        /// </summary>
        /// <param name="e1">First channel to compare.</param>
        /// <param name="e2">Second channel to compare.</param>
        /// <returns>Whether the two channels are equal.</returns>
        public static bool operator ==(DiscordChannel e1, DiscordChannel e2)
        {
            var o1 = e1 as object;
            var o2 = e2 as object;

            return (o1 != null || o2 == null) && (o1 == null || o2 != null) && ((o1 == null && o2 == null) || e1.Id == e2.Id);
        }

        /// <summary>
        /// Gets whether the two <see cref="DiscordChannel"/> objects are not equal.
        /// </summary>
        /// <param name="e1">First channel to compare.</param>
        /// <param name="e2">Second channel to compare.</param>
        /// <returns>Whether the two channels are not equal.</returns>
        public static bool operator !=(DiscordChannel e1, DiscordChannel e2)
            => !(e1 == e2);
    }
}
