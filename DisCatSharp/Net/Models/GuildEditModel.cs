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
using System.IO;
using DisCatSharp.Entities;

namespace DisCatSharp.Net.Models
{
    /// <summary>
    /// Represents a guild edit model.
    /// </summary>
    public class GuildEditModel : BaseEditModel
    {
        /// <summary>
        /// The new guild name.
        /// </summary>
        public Optional<string> Name { internal get; set; }

        /// <summary>
        /// The new guild voice region.
        /// </summary>
        [Obsolete("Channels use their own voice regions. Guild region is deprecated. Please use the newer one on the voice channels.", false)]
        public Optional<DiscordVoiceRegion> Region { internal get; set; }

        /// <summary>
        /// The new guild icon.
        /// </summary>
        public Optional<Stream> Icon { internal get; set; }

        /// <summary>
        /// The new guild verification level.
        /// </summary>
        public Optional<VerificationLevel> VerificationLevel { internal get; set; }

        /// <summary>
        /// The new guild default message notification level.
        /// </summary>
        public Optional<DefaultMessageNotifications> DefaultMessageNotifications { internal get; set; }

        /// <summary>
        /// The new guild MFA level.
        /// </summary>
        public Optional<MfaLevel> MfaLevel { internal get; set; }

        /// <summary>
        /// The new guild explicit content filter level.
        /// </summary>
        public Optional<ExplicitContentFilter> ExplicitContentFilter { internal get; set; }

        /// <summary>
        /// The new AFK voice channel.
        /// </summary>
        public Optional<DiscordChannel> AfkChannel { internal get; set; }

        /// <summary>
        /// The new AFK timeout time in seconds.
        /// </summary>
        public Optional<int> AfkTimeout { internal get; set; }

        /// <summary>
        /// The new guild owner.
        /// </summary>
        public Optional<DiscordMember> Owner { internal get; set; }

        /// <summary>
        /// The new guild splash.
        /// </summary>
        public Optional<Stream> Splash { internal get; set; }

        /// <summary>
        /// The new guild system channel.
        /// </summary>
        public Optional<DiscordChannel> SystemChannel { internal get; set; }

        /// <summary>
        /// The guild system channel flags.
        /// </summary>
        public Optional<SystemChannelFlags> SystemChannelFlags { internal get; set; }

        /// <summary>
        /// The guild description.
        /// </summary>
        public Optional<string> Description { internal get; set; }

        /// <summary>
        /// The new guild rules channel.
        /// </summary>
        public Optional<DiscordChannel> RulesChannel { internal get; set; }

        /// <summary>
        /// The new guild public updates channel.
        /// </summary>
        public Optional<DiscordChannel> PublicUpdatesChannel { internal get; set; }

        /// <summary>
        /// The new guild preferred locale.
        /// </summary>
        public Optional<string> PreferredLocale { internal get; set; }

        /// <summary>
        /// The new banner of the guild
        /// </summary>
        public Optional<Stream> Banner { get; set; }

        /// <summary>
        /// The new discorvery splash image of the guild
        /// </summary>
        public Optional<Stream> DiscoverySplash { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="GuildEditModel"/> class.
        /// </summary>
        internal GuildEditModel()
        {

        }
    }
}
