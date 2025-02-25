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

namespace DisCatSharp
{
    /// <summary>
    /// Represents permission methods.
    /// </summary>
    public static class PermissionMethods
    {
        /// <summary>
        /// Gets the full permissions enum (long).
        /// </summary>
        internal static Permissions FULL_PERMS { get; } = (Permissions)1099511627775L;

        /// <summary>
        /// Calculates whether this permission set contains the given permission.
        /// </summary>
        /// <param name="p">The permissions to calculate from</param>
        /// <param name="permission">permission you want to check</param>
        /// <returns></returns>
        public static bool HasPermission(this Permissions p, Permissions permission)
            => p.HasFlag(Permissions.Administrator) || (p & permission) == permission;

        /// <summary>
        /// Grants permissions.
        /// </summary>
        /// <param name="p">The permissions to add to.</param>
        /// <param name="grant">Permission to add.</param>
        /// <returns></returns>
        public static Permissions Grant(this Permissions p, Permissions grant) => p | grant;

        /// <summary>
        /// Revokes permissions.
        /// </summary>
        /// <param name="p">The permissions to take from.</param>
        /// <param name="revoke">Permission to take.</param>
        /// <returns></returns>
        public static Permissions Revoke(this Permissions p, Permissions revoke) => p & ~revoke;
    }

    /// <summary>
    /// Whether a permission is allowed, denied or unset
    /// </summary>
    public enum PermissionLevel
    {
        /// <summary>
        /// Said permission is Allowed
        /// </summary>
        Allowed,

        /// <summary>
        /// Said permission is Denied
        /// </summary>
        Denied,

        /// <summary>
        /// Said permission is Unset
        /// </summary>
        Unset
    }

    /// <summary>
    /// Bitwise permission flags.
    /// </summary>
    [Flags]
    public enum Permissions : long
    {
        /// <summary>
        /// Indicates no permissions given.
        /// </summary>
        [PermissionString("No permissions")]
        None = 0x0000000000000000,

        /// <summary>
        /// Indicates all permissions are granted
        /// </summary>
        [PermissionString("All permissions")]
        All = 1099511627775,

        /// <summary>
        /// Allows creation of instant channel invites.
        /// </summary>
        [PermissionString("Create instant invites")]
        CreateInstantInvite = 0x0000000000000001,

        /// <summary>
        /// Allows kicking members.
        /// </summary>
        [PermissionString("Kick members")]
        KickMembers = 0x0000000000000002,

        /// <summary>
        /// Allows banning and unbanning members.
        /// </summary>
        [PermissionString("Ban members")]
        BanMembers = 0x0000000000000004,

        /// <summary>
        /// Enables full access on a given guild. This also overrides other permissions.
        /// </summary>
        [PermissionString("Administrator")]
        Administrator = 0x0000000000000008,

        /// <summary>
        /// Allows managing channels.
        /// </summary>
        [PermissionString("Manage channels")]
        ManageChannels = 0x0000000000000010,

        /// <summary>
        /// Allows managing the guild.
        /// </summary>
        [PermissionString("Manage guild")]
        ManageGuild = 0x0000000000000020,

        /// <summary>
        /// Allows adding reactions to messages.
        /// </summary>
        [PermissionString("Add reactions")]
        AddReactions = 0x0000000000000040,

        /// <summary>
        /// Allows viewing audit log entries.
        /// </summary>
        [PermissionString("View audit log")]
        ViewAuditLog = 0x0000000000000080,

        /// <summary>
        /// Allows the use of priority speaker.
        /// </summary>
        [PermissionString("Use priority speaker")]
        PrioritySpeaker = 0x0000000000000100,

        /// <summary>
        /// Allows accessing text and voice channels. Disabling this permission hides channels.
        /// </summary>
        [PermissionString("Read messages")]
        AccessChannels = 0x0000000000000400,

        /// <summary>
        /// Allows sending messages (does not allow sending messages in threads).
        /// </summary>
        [PermissionString("Send messages")]
        SendMessages = 0x0000000000000800,

        /// <summary>
        /// Allows sending text-to-speech messages.
        /// </summary>
        [PermissionString("Send TTS messages")]
        SendTtsMessages = 0x0000000000001000,

        /// <summary>
        /// Allows managing messages of other users.
        /// </summary>
        [PermissionString("Manage messages")]
        ManageMessages = 0x0000000000002000,

        /// <summary>
        /// Allows embedding content in messages.
        /// </summary>
        [PermissionString("Use embeds")]
        EmbedLinks = 0x0000000000004000,

        /// <summary>
        /// Allows uploading files.
        /// </summary>
        [PermissionString("Attach files")]
        AttachFiles = 0x0000000000008000,

        /// <summary>
        /// Allows reading message history.
        /// </summary>
        [PermissionString("Read message history")]
        ReadMessageHistory = 0x0000000000010000,

        /// <summary>
        /// Allows using @everyone and @here mentions.
        /// </summary>
        [PermissionString("Mention everyone")]
        MentionEveryone = 0x0000000000020000,

        /// <summary>
        /// Allows using emojis from external servers, such as twitch or nitro emojis.
        /// </summary>
        [PermissionString("Use external emojis")]
        UseExternalEmojis = 0x0000000000040000,

        /// <summary>
        /// Allows connecting to voice chat.
        /// </summary>
        [PermissionString("Use voice chat")]
        UseVoice = 0x0000000000100000,

        /// <summary>
        /// Allows speaking in voice chat.
        /// </summary>
        [PermissionString("Speak")]
        Speak = 0x0000000000200000,

        /// <summary>
        /// Allows muting other members in voice chat.
        /// </summary>
        [PermissionString("Mute voice chat members")]
        MuteMembers = 0x0000000000400000,

        /// <summary>
        /// Allows deafening other members in voice chat.
        /// </summary>
        [PermissionString("Deafen voice chat members")]
        DeafenMembers = 0x0000000000800000,

        /// <summary>
        /// Allows moving voice chat members.
        /// </summary>
        [PermissionString("Move voice chat members")]
        MoveMembers = 0x0000000001000000,

        /// <summary>
        /// Allows using voice activation in voice chat. Revoking this will usage of push-to-talk.
        /// </summary>
        [PermissionString("Use voice activity detection")]
        UseVoiceDetection = 0x0000000002000000,

        /// <summary>
        /// Allows changing of own nickname.
        /// </summary>
        [PermissionString("Change own nickname")]
        ChangeNickname = 0x0000000004000000,

        /// <summary>
        /// Allows managing nicknames of other members.
        /// </summary>
        [PermissionString("Manage nicknames")]
        ManageNicknames = 0x0000000008000000,

        /// <summary>
        /// Allows managing roles in a guild.
        /// </summary>
        [PermissionString("Manage roles")]
        ManageRoles = 0x0000000010000000,

        /// <summary>
        /// Allows managing webhooks in a guild.
        /// </summary>
        [PermissionString("Manage webhooks")]
        ManageWebhooks = 0x0000000020000000,

        /// <summary>
        /// Allows managing guild emojis and stickers.
        /// </summary>
        [PermissionString("Manage emojis & stickers")]
        ManageEmojisAndStickers = 0x0000000040000000,

        /// <summary>
        /// Allows the user to go live.
        /// </summary>
        [PermissionString("Allow stream")]
        Stream  = 0x0000000000000200,

        /// <summary>
        /// Allows the user to use slash commands.
        /// </summary>
        [PermissionString("Use application commands")]
        UseApplicationCommands = 0x0000000080000000,

        /// <summary>
        /// Allows for requesting to speak in stage channels.
        /// </summary>
        [PermissionString("Request to speak")]
        RequestToSpeak = 0x0000000100000000,

        /// <summary>
        /// Allows managing guild events.
        /// </summary>
        [PermissionString("Manage Events")]
        ManageEvents = 0x0000000200000000,

        /// <summary>
        /// Allows for deleting and archiving threads, and viewing all private threads.
        /// </summary>
        [PermissionString("Manage Threads")]
        ManageThreads = 0x0000000400000000,

        /// <summary>
        /// Allows for creating threads.
        /// </summary>
        [PermissionString("Create Public Threads")]
        CreatePublicThreads = 0x0000000800000000,

        /// <summary>
        /// Allows for creating private threads.
        /// </summary>
        [PermissionString("Create Private Threads")]
        CreatePrivateThreads = 0x0000001000000000,

        /// <summary>
        /// Allows the usage of custom stickers from other servers.
        /// </summary>
        [PermissionString("Use external Stickers")]
        UseExternalStickers = 0x0000002000000000,

        /// <summary>
        /// Allows for sending messages in threads.
        /// </summary>
        [PermissionString("Send messages in Threads")]
        SendMessagesInThreads = 0x0000004000000000,

        /// <summary>
        /// Allows for launching activities (applications with the `EMBEDDED` flag) in a voice channel.     
        /// </summary>
        [PermissionString("Start Embedded Activities")]
        StartEmbeddedActivities = 0x0000008000000000
    }
    
    /// <summary>
    /// Defines a readable name for this permission.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public sealed class PermissionStringAttribute : Attribute
    {
        /// <summary>
        /// Gets the readable name for this permission.
        /// </summary>
        public string String { get; }

        /// <summary>
        /// Defines a readable name for this permission.
        /// </summary>
        /// <param name="str">Readable name for this permission.</param>
        public PermissionStringAttribute(string str)
        {
            this.String = str;
        }
    }
}
