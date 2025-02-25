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
using System.Linq;
using DisCatSharp.Enums;
using Newtonsoft.Json;

namespace DisCatSharp.Entities
{
    /// <summary>
    /// Represents a command that is registered to an application.
    /// </summary>
    public sealed class DiscordApplicationCommand : SnowflakeObject, IEquatable<DiscordApplicationCommand>
    {
        /// <summary>
        /// Gets the type of this application command.
        /// </summary>
        [JsonProperty("type")]
        public ApplicationCommandType Type { get; internal set; }

        /// <summary>
        /// Gets the unique ID of this command's application.
        /// </summary>
        [JsonProperty("application_id")]
        public ulong ApplicationId { get; internal set; }

        /// <summary>
        /// Gets the name of this command.
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; internal set; }

        /// <summary>
        /// Gets the description of this command.
        /// </summary>
        [JsonProperty("description")]
        public string Description { get; internal set; }

        /// <summary>
        /// Gets the potential parameters for this command.
        /// </summary>
        [JsonProperty("options", NullValueHandling = NullValueHandling.Ignore)]
        public IReadOnlyCollection<DiscordApplicationCommandOption> Options { get; internal set; }

        /// <summary>
        /// Gets whether the command is enabled by default when the app is added to a guild.
        /// </summary>
        [JsonProperty("default_permission", NullValueHandling = NullValueHandling.Ignore)]
        public bool DefaultPermission { get; internal set; }

        /// <summary>
        /// Gets the version number for this command. 
        /// </summary>
        [JsonProperty("version")]
        public ulong Version { get; internal set; }

        /// <summary>
        /// Creates a new instance of a <see cref="DiscordApplicationCommand"/>.
        /// </summary>
        /// <param name="name">The name of the command.</param>
        /// <param name="description">The description of the command.</param>
        /// <param name="options">Optional parameters for this command.</param>
        /// <param name="default_permission">Optional default permission for this command.</param>
        /// <param name="type">The type of the command. Defaults to ChatInput.</param>
        public DiscordApplicationCommand(string name, string description, IEnumerable<DiscordApplicationCommandOption> options = null, bool default_permission = true, ApplicationCommandType type = ApplicationCommandType.ChatInput)
        {
            if (type == ApplicationCommandType.ChatInput)
            {
                if (!Utilities.IsValidSlashCommandName(name))
                throw new ArgumentException("Invalid slash command name specified. It must be below 32 characters and not contain any whitespace.", nameof(name));
                if (name.Any(ch => char.IsUpper(ch)))
                    throw new ArgumentException("Slash command name cannot have any upper case characters.", nameof(name));
                if (description.Length > 100)
                    throw new ArgumentException("Slash command description cannot exceed 100 characters.", nameof(description));
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(description))
                    throw new ArgumentException("Context menus do not support descriptions.");
                if (options?.Any() ?? false)
                    throw new ArgumentException("Context menus do not support options.");
                description = string.Empty;
            }

            var optionsList = options != null ? new ReadOnlyCollection<DiscordApplicationCommandOption>(options.ToList()) : null;

            this.Type = type;
            this.Name = name;
            this.Description = description;
            this.Options = optionsList;
            this.DefaultPermission = default_permission;
        }

        /// <summary>
        /// Checks whether this <see cref="DiscordApplicationCommand"/> object is equal to another object.
        /// </summary>
        /// <param name="other">The command to compare to.</param>
        /// <returns>Whether the command is equal to this <see cref="DiscordApplicationCommand"/>.</returns>
        public bool Equals(DiscordApplicationCommand other)
            => this.Id == other.Id;

        /// <summary>
        /// Determines if two <see cref="DiscordApplicationCommand"/> objects are equal.
        /// </summary>
        /// <param name="e1">The first command object.</param>
        /// <param name="e2">The second command object.</param>
        /// <returns>Whether the two <see cref="DiscordApplicationCommand"/> objects are equal.</returns>
        public static bool operator ==(DiscordApplicationCommand e1, DiscordApplicationCommand e2)
            => e1.Equals(e2);

        /// <summary>
        /// Determines if two <see cref="DiscordApplicationCommand"/> objects are not equal.
        /// </summary>
        /// <param name="e1">The first command object.</param>
        /// <param name="e2">The second command object.</param>
        /// <returns>Whether the two <see cref="DiscordApplicationCommand"/> objects are not equal.</returns>
        public static bool operator !=(DiscordApplicationCommand e1, DiscordApplicationCommand e2)
            => !(e1 == e2);

        /// <summary>
        /// Determines if a <see cref="object"/> is equal to the current <see cref="DiscordApplicationCommand"/>.
        /// </summary>
        /// <param name="other">The object to compare to.</param>
        /// <returns>Whether the two <see cref="DiscordApplicationCommand"/> objects are not equal.</returns>
        public override bool Equals(object other)
            => other is DiscordApplicationCommand dac && this.Equals(dac);

        /// <summary>
        /// Gets the hash code for this <see cref="DiscordApplicationCommand"/>.
        /// </summary>
        /// <returns>The hash code for this <see cref="DiscordApplicationCommand"/>.</returns>
        public override int GetHashCode()
            => this.Id.GetHashCode();
    }
}
