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
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DisCatSharp.Entities;
using DisCatSharp.Net;
using Microsoft.Extensions.Logging;
using static System.Net.WebRequestMethods;

namespace DisCatSharp
{
    /// <summary>
    /// Various Discord-related utilities.
    /// </summary>
    public static class Utilities
    {
        /// <summary>
        /// Gets the version of the library
        /// </summary>
        private static string VersionHeader { get; set; }

        /// <summary>
        /// Gets or sets the permission strings.
        /// </summary>
        private static Dictionary<Permissions, string> PermissionStrings { get; set; }

        /// <summary>
        /// Gets the utf8 encoding
        /// </summary>
        internal static UTF8Encoding UTF8 { get; } = new UTF8Encoding(false);

        /// <summary>
        /// Initializes a new instance of the <see cref="Utilities"/> class.
        /// </summary>
        static Utilities()
        {
            PermissionStrings = new Dictionary<Permissions, string>();
            var t = typeof(Permissions);
            var ti = t.GetTypeInfo();
            var vals = Enum.GetValues(t).Cast<Permissions>();

            foreach (var xv in vals)
            {
                var xsv = xv.ToString();
                var xmv = ti.DeclaredMembers.FirstOrDefault(xm => xm.Name == xsv);
                var xav = xmv.GetCustomAttribute<PermissionStringAttribute>();

                PermissionStrings[xv] = xav.String;
            }

            var a = typeof(DiscordClient).GetTypeInfo().Assembly;

            var vs = "";
            var iv = a.GetCustomAttribute<AssemblyInformationalVersionAttribute>();
            if (iv != null)
                vs = iv.InformationalVersion;
            else
            {
                var v = a.GetName().Version;
                vs = v.ToString(3);
            }

            VersionHeader = $"DiscordBot (https://github.com/Aiko-IT-Systems/DisCatSharp, v{vs})";
        }

        /// <summary>
        /// Gets the api base uri.
        /// </summary>
        /// <returns>A string.</returns>
        internal static string GetApiBaseUri(bool canary = false)
            => canary ? "https://canary.discord.com/api/v9" : Endpoints.BASE_URI;

        /// <summary>
        /// Gets the api uri for.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="canary">Use canary</param>
        /// <returns>An Uri.</returns>
        internal static Uri GetApiUriFor(string path, bool canary = false)
            => new($"{GetApiBaseUri(canary)}{path}");

        /// <summary>
        /// Gets the api uri for.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="queryString">The query string.</param>
        /// <param name="canary">Use canary</param>
        /// <returns>An Uri.</returns>
        internal static Uri GetApiUriFor(string path, string queryString, bool canary = false)
            => new($"{GetApiBaseUri(canary)}{path}{queryString}");

        /// <summary>
        /// Gets the api uri builder for.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="canary">Use canary</param>
        /// <returns>A QueryUriBuilder.</returns>
        internal static QueryUriBuilder GetApiUriBuilderFor(string path, bool canary = false)
            => new($"{GetApiBaseUri(canary)}{path}");

        /// <summary>
        /// Gets the formatted token.
        /// </summary>
        /// <param name="client">The client.</param>
        /// <returns>A string.</returns>
        internal static string GetFormattedToken(BaseDiscordClient client) => GetFormattedToken(client.Configuration);

        /// <summary>
        /// Gets the formatted token.
        /// </summary>
        /// <param name="config">The config.</param>
        /// <returns>A string.</returns>
        internal static string GetFormattedToken(DiscordConfiguration config)
        {
            return config.TokenType switch
            {
                TokenType.Bearer => $"Bearer {config.Token}",
                TokenType.Bot => $"Bot {config.Token}",
                _ => throw new ArgumentException("Invalid token type specified.", nameof(config.Token)),
            };
        }

        /// <summary>
        /// Gets the base headers.
        /// </summary>
        /// <returns>A Dictionary.</returns>
        internal static Dictionary<string, string> GetBaseHeaders()
            => new();

        /// <summary>
        /// Gets the user agent.
        /// </summary>
        /// <returns>A string.</returns>
        internal static string GetUserAgent()
            => VersionHeader;

        /// <summary>
        /// Contains the user mentions.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns>A bool.</returns>
        internal static bool ContainsUserMentions(string message)
        {
            var pattern = @"<@(\d+)>";
            var regex = new Regex(pattern, RegexOptions.ECMAScript);
            return regex.IsMatch(message);
        }

        /// <summary>
        /// Contains the nickname mentions.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns>A bool.</returns>
        internal static bool ContainsNicknameMentions(string message)
        {
            var pattern = @"<@!(\d+)>";
            var regex = new Regex(pattern, RegexOptions.ECMAScript);
            return regex.IsMatch(message);
        }

        /// <summary>
        /// Contains the channel mentions.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns>A bool.</returns>
        internal static bool ContainsChannelMentions(string message)
        {
            var pattern = @"<#(\d+)>";
            var regex = new Regex(pattern, RegexOptions.ECMAScript);
            return regex.IsMatch(message);
        }

        /// <summary>
        /// Contains the role mentions.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns>A bool.</returns>
        internal static bool ContainsRoleMentions(string message)
        {
            var pattern = @"<@&(\d+)>";
            var regex = new Regex(pattern, RegexOptions.ECMAScript);
            return regex.IsMatch(message);
        }

        /// <summary>
        /// Contains the emojis.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns>A bool.</returns>
        internal static bool ContainsEmojis(string message)
        {
            var pattern = @"<a?:(.*):(\d+)>";
            var regex = new Regex(pattern, RegexOptions.ECMAScript);
            return regex.IsMatch(message);
        }

        /// <summary>
        /// Gets the user mentions.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns>A list of ulong.</returns>
        internal static IEnumerable<ulong> GetUserMentions(DiscordMessage message)
        {
            var regex = new Regex(@"<@!?(\d+)>", RegexOptions.ECMAScript);
            var matches = regex.Matches(message.Content);
            foreach (Match match in matches)
                yield return ulong.Parse(match.Groups[1].Value, CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Gets the role mentions.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns>A list of ulong.</returns>
        internal static IEnumerable<ulong> GetRoleMentions(DiscordMessage message)
        {
            var regex = new Regex(@"<@&(\d+)>", RegexOptions.ECMAScript);
            var matches = regex.Matches(message.Content);
            foreach (Match match in matches)
                yield return ulong.Parse(match.Groups[1].Value, CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Gets the channel mentions.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns>A list of ulong.</returns>
        internal static IEnumerable<ulong> GetChannelMentions(DiscordMessage message)
        {
            var regex = new Regex(@"<#(\d+)>", RegexOptions.ECMAScript);
            var matches = regex.Matches(message.Content);
            foreach (Match match in matches)
                yield return ulong.Parse(match.Groups[1].Value, CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Gets the emojis.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns>A list of ulong.</returns>
        internal static IEnumerable<ulong> GetEmojis(DiscordMessage message)
        {
            var regex = new Regex(@"<a?:([a-zA-Z0-9_]+):(\d+)>", RegexOptions.ECMAScript);
            var matches = regex.Matches(message.Content);
            foreach (Match match in matches)
                yield return ulong.Parse(match.Groups[2].Value, CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Are the valid slash command name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>A bool.</returns>
        internal static bool IsValidSlashCommandName(string name)
        {
            var regex = new Regex(@"^[\w-]{1,32}$", RegexOptions.ECMAScript);
            return regex.IsMatch(name);
        }

        /// <summary>
        /// Checks the thread auto archive duration feature.
        /// </summary>
        /// <param name="guild">The guild.</param>
        /// <param name="taad">The taad.</param>
        /// <returns>A bool.</returns>
        internal static bool CheckThreadAutoArchiveDurationFeature(DiscordGuild guild, ThreadAutoArchiveDuration taad)
        {
            return taad == ThreadAutoArchiveDuration.ThreeDays
                ? (guild.PremiumTier.HasFlag(PremiumTier.Tier_1) || guild.Features.CanSetThreadArchiveDurationThreeDays)
                : taad != ThreadAutoArchiveDuration.OneWeek || guild.PremiumTier.HasFlag(PremiumTier.Tier_2) || guild.Features.CanSetThreadArchiveDurationSevenDays;
        }

        /// <summary>
        /// Checks the thread private feature.
        /// </summary>
        /// <param name="guild">The guild.</param>
        /// <returns>A bool.</returns>
        internal static bool CheckThreadPrivateFeature(DiscordGuild guild) => guild.PremiumTier.HasFlag(PremiumTier.Tier_2) || guild.Features.CanCreatePrivateThreads;

        /// <summary>
        /// Have the message intents.
        /// </summary>
        /// <param name="intents">The intents.</param>
        /// <returns>A bool.</returns>
        internal static bool HasMessageIntents(DiscordIntents intents)
            => intents.HasIntent(DiscordIntents.GuildMessages) || intents.HasIntent(DiscordIntents.DirectMessages);

        /// <summary>
        /// Have the reaction intents.
        /// </summary>
        /// <param name="intents">The intents.</param>
        /// <returns>A bool.</returns>
        internal static bool HasReactionIntents(DiscordIntents intents)
            => intents.HasIntent(DiscordIntents.GuildMessageReactions) || intents.HasIntent(DiscordIntents.DirectMessageReactions);

        /// <summary>
        /// Have the typing intents.
        /// </summary>
        /// <param name="intents">The intents.</param>
        /// <returns>A bool.</returns>
        internal static bool HasTypingIntents(DiscordIntents intents)
            => intents.HasIntent(DiscordIntents.GuildMessageTyping) || intents.HasIntent(DiscordIntents.DirectMessageTyping);

        // https://discord.com/developers/docs/topics/gateway#sharding-sharding-formula
        /// <summary>
        /// Gets a shard id from a guild id and total shard count.
        /// </summary>
        /// <param name="guildId">The guild id the shard is on.</param>
        /// <param name="shardCount">The total amount of shards.</param>
        /// <returns>The shard id.</returns>
        public static int GetShardId(ulong guildId, int shardCount)
            => (int)(guildId >> 22) % shardCount;

        /// <summary>
        /// Helper method to create a <see cref="DateTimeOffset"/> from Unix time seconds for targets that do not support this natively.
        /// </summary>
        /// <param name="unixTime">Unix time seconds to convert.</param>
        /// <param name="shouldThrow">Whether the method should throw on failure. Defaults to true.</param>
        /// <returns>Calculated <see cref="DateTimeOffset"/>.</returns>
        public static DateTimeOffset GetDateTimeOffset(long unixTime, bool shouldThrow = true)
        {
            try
            {
                return DateTimeOffset.FromUnixTimeSeconds(unixTime);
            }
            catch (Exception)
            {
                if (shouldThrow)
                    throw;

                return DateTimeOffset.MinValue;
            }
        }

        /// <summary>
        /// Helper method to create a <see cref="DateTimeOffset"/> from Unix time milliseconds for targets that do not support this natively.
        /// </summary>
        /// <param name="unixTime">Unix time milliseconds to convert.</param>
        /// <param name="shouldThrow">Whether the method should throw on failure. Defaults to true.</param>
        /// <returns>Calculated <see cref="DateTimeOffset"/>.</returns>
        public static DateTimeOffset GetDateTimeOffsetFromMilliseconds(long unixTime, bool shouldThrow = true)
        {
            try
            {
                return DateTimeOffset.FromUnixTimeMilliseconds(unixTime);
            }
            catch (Exception)
            {
                if (shouldThrow)
                    throw;

                return DateTimeOffset.MinValue;
            }
        }

        /// <summary>
        /// Helper method to calculate Unix time seconds from a <see cref="DateTimeOffset"/> for targets that do not support this natively.
        /// </summary>
        /// <param name="dto"><see cref="DateTimeOffset"/> to calculate Unix time for.</param>
        /// <returns>Calculated Unix time.</returns>
        public static long GetUnixTime(DateTimeOffset dto)
            => dto.ToUnixTimeMilliseconds();

        /// <summary>
        /// Computes a timestamp from a given snowflake.
        /// </summary>
        /// <param name="snowflake">Snowflake to compute a timestamp from.</param>
        /// <returns>Computed timestamp.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static DateTimeOffset GetSnowflakeTime(this ulong snowflake)
            => DiscordClient._discordEpoch.AddMilliseconds(snowflake >> 22);

        /// <summary>
        /// Converts this <see cref="Permissions"/> into human-readable format.
        /// </summary>
        /// <param name="perm">Permissions enumeration to convert.</param>
        /// <returns>Human-readable permissions.</returns>
        public static string ToPermissionString(this Permissions perm)
        {
            if (perm == Permissions.None)
                return PermissionStrings[perm];

            perm &= PermissionMethods.FULL_PERMS;

            var strs = PermissionStrings
                .Where(xkvp => xkvp.Key != Permissions.None && (perm & xkvp.Key) == xkvp.Key)
                .Select(xkvp => xkvp.Value);

            return string.Join(", ", strs.OrderBy(xs => xs));
        }

        /// <summary>
        /// Checks whether this string contains given characters.
        /// </summary>
        /// <param name="str">String to check.</param>
        /// <param name="characters">Characters to check for.</param>
        /// <returns>Whether the string contained these characters.</returns>
        public static bool Contains(this string str, params char[] characters)
        {
            foreach (var xc in str)
                if (characters.Contains(xc))
                    return true;

            return false;
        }

        /// <summary>
        /// Logs the task fault.
        /// </summary>
        /// <param name="task">The task.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="level">The level.</param>
        /// <param name="eventId">The event id.</param>
        /// <param name="message">The message.</param>
        internal static void LogTaskFault(this Task task, ILogger logger, LogLevel level, EventId eventId, string message)
        {
            if (task == null)
                throw new ArgumentNullException(nameof(task));

            if (logger == null)
                return;

            task.ContinueWith(t => logger.Log(level, eventId, t.Exception, message), TaskContinuationOptions.OnlyOnFaulted);
        }

        /// <summary>
        /// Deconstructs the.
        /// </summary>
        /// <param name="kvp">The kvp.</param>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        internal static void Deconstruct<TKey, TValue>(this KeyValuePair<TKey, TValue> kvp, out TKey key, out TValue value)
        {
            key = kvp.Key;
            value = kvp.Value;
        }
    }
}
