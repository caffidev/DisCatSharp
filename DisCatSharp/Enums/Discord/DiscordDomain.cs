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
using System.Linq;

namespace DisCatSharp.Enums.Discord
{
    /// <summary>
    /// Core Domains
    /// </summary>
    public enum CoreDomain
    {
        /// <summary>
        /// dis.gd
        /// </summary>
        [DomainHelp("Marketing URL shortner", "dis.gd")]
        DiscordMarketing = 1,

        /// <summary>
        /// discord.co
        /// </summary>
        [DomainHelp("Admin panel, internal tools", "discord.co")]
        DiscordAdmin = 2,

        /// <summary>
        /// discord.com
        /// </summary>
        [DomainHelp("New app, marketing website, API host", "discord.com")]
        Discord = 3,

        /// <summary>
        /// discord.design
        /// </summary>
        [DomainHelp("Dribbble profile shortlink", "discord.design")]
        DiscordDesign = 4,

        /// <summary>
        /// discord.dev
        /// </summary>
        [DomainHelp("Developer site shortlinks", "discord.dev")]
        DiscordDev = 5,

        /// <summary>
        /// discord.gg
        /// </summary>
        [DomainHelp("Invite shortlinks", "discord.gg")]
        DiscordShortlink = 6,

        /// <summary>
        /// discord.gift
        /// </summary>
        [DomainHelp("Gift shortlinks", "discord.gift")]
        DiscordGift = 7,

        /// <summary>
        /// discord.media
        /// </summary>
        [DomainHelp("Voice servers", "discord.media")]
        DiscordMedia = 8,

        /// <summary>
        /// discord.new
        /// </summary>
        [DomainHelp("Template shortlinks", "discord.new")]
        DiscordTemplate = 9,

        /// <summary>
        /// discord.store
        /// </summary>
        [DomainHelp("Merch store", "discord.store")]
        DiscordMerch = 10,

        /// <summary>
        /// discord.tools
        /// </summary>
        [DomainHelp("Internal tools", "discord.tools")]
        DiscordTools = 11,

        /// <summary>
        /// discordapp.com
        /// </summary>
        [DomainHelp("Old app, marketing website, and API; CDN", "discordapp.com")]
        DiscordAppOld = 12,

        /// <summary>
        /// discordapp.net
        /// </summary>
        [DomainHelp("Media Proxy", "discordapp.net")]
        DiscordAppMediaProxy = 13,

        /// <summary>
        /// discordmerch.com
        /// </summary>
        [DomainHelp("Merch store", "discordmerch.com")]
        DiscordMerchOld = 14,

        /// <summary>
        /// discordpartygames.com
        /// </summary>
        [DomainHelp("Voice channel activity API host", "discordpartygames.com")]
        DiscordActivityAlt = 15,

        /// <summary>
        /// discord-activities.com
        /// </summary>
        [DomainHelp("Voice channel activity API host", "discord-activities.com")]
        DiscordActivityAlt2 = 16,

        /// <summary>
        /// discordsays.com
        /// </summary>
        [DomainHelp("Voice channel activity host", "discordsays.com")]
        DiscordActivity = 17,

        /// <summary>
        /// discordstatus.com
        /// </summary>
        [DomainHelp("Status page", "discordstatus.com")]
        DiscordStatus = 18,

        /// <summary>
        /// cdn.discordapp.com
        /// </summary>
        [DomainHelp("CDN", "cdn.discordapp.com")]
        DiscordCdn = 19,

    }

    /// <summary>
    /// Other Domains
    /// </summary>
    public enum OtherDomain
    {
        /// <summary>
        /// airhorn.solutions
        /// </summary>
        [DomainHelp("API implementation example", "airhorn.solutions")]
        Airhorn = 1,

        /// <summary>
        /// airhornbot.com
        /// </summary>
        [DomainHelp("API implementation example", "airhornbot.com")]
        AirhornAlt = 2,

        /// <summary>
        /// bigbeans.solutions
        /// </summary>
        [DomainHelp("April Fools 2017", "bigbeans.solutions")]
        AprilFools = 3,

        /// <summary>
        /// watchanimeattheoffice.com
        /// </summary>
        [DomainHelp("HypeSquad form placeholder/meme", "watchanimeattheoffice.com")]
        HypeSquadMeme = 4
    }

    /// <summary>
    /// Core Domains
    /// </summary>
    public enum UnusedDomain
    {
        /// <summary>
        /// discordapp.io
        /// </summary>
        [Obsolete("Not in use", false)]
        [DomainHelp("IO domain for discord", "discordapp.io")]
        DiscordAppIo = 1,

        /// <summary>
        /// discordcdn.com
        /// </summary>
        [Obsolete("Not in use", false)]
        [DomainHelp("Alternative CDN domain", "discordcdn.com")]
        DiscordCdnCom = 2
    }

    /// <summary>
    /// Represents a discord domain.
    /// </summary>
    public static class DiscordDomain
    {
        /// <summary>
        /// Gets a domain.
        /// Valid types: <see cref="CoreDomain"/>, <see cref="OtherDomain"/> and <see cref="UnusedDomain"/>.
        /// </summary>
        /// <param name="DomainEnum">The domain type.</param>
        /// <returns>A DomainHelpAttribute.</returns>
        public static DomainHelpAttribute GetDomain(Enum DomainEnum)
        {
            if (DomainEnum is not CoreDomain && DomainEnum is not OtherDomain && DomainEnum is not UnusedDomain)
                throw new NotSupportedException($"Invalid type. Found: {DomainEnum.GetType()} Expected: CoreDomain or OtherDomain or UnusedDomain");

            if (DomainEnum is CoreDomain domain && (domain == CoreDomain.DiscordAdmin || domain == CoreDomain.DiscordTools))
                throw new UnauthorizedAccessException("You don't have access to this domains");

            var memberInfo = DomainEnum.GetType().GetMember(DomainEnum.ToString()).FirstOrDefault();
            if (memberInfo != null)
            {
                var attribute = (DomainHelpAttribute)memberInfo.GetCustomAttributes(typeof(DomainHelpAttribute), false).FirstOrDefault();
                return attribute;
            }

            return null;
        }
    }

    /// <summary>
    /// Defines a description and url for this domain.
    /// </summary>
    [AttributeUsage(AttributeTargets.All, AllowMultiple = false)]
    public class DomainHelpAttribute : Attribute
    {
        /// <summary>
        /// Gets the Description for this domain.
        /// </summary>
        public string Description { get; }

        /// <summary>
        /// Gets the Uri for this domain.
        /// </summary>
        public Uri Uri { get; }

        /// <summary>
        /// Gets the Domain for this domain.
        /// </summary>
        public string Domain { get; }

        /// <summary>
        /// Gets the Url for this domain.
        /// </summary>
        public string Url { get; }

        /// <summary>
        /// Defines a description and URIs for this domain.
        /// </summary>
        /// <param name="desc">Description for this domain.</param>
        /// <param name="domain">Url for this domain.</param>
        public DomainHelpAttribute(string desc, string domain)
        {
            this.Description = desc;
            this.Domain = domain;
            var url = $"https://{domain}";
            this.Url = url;
            this.Uri = new(url);
        }
    }
}
