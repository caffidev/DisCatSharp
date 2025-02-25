// This file is part of the DisCatSharp project, a fork of DSharpPlus.
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
using DisCatSharp.Entities;

namespace DisCatSharp.ApplicationCommands
{
    /// <summary>
    /// The application commands permission context.
    /// </summary>
    public class ApplicationCommandsPermissionContext
    {
        /// <summary>
        /// Gets the type.
        /// </summary>
        public Type Type { get; }

        /// <summary>
        /// Gets the name.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the permissions.
        /// </summary>
        public IReadOnlyCollection<DiscordApplicationCommandPermission> Permissions => _permissions;
        private readonly List<DiscordApplicationCommandPermission> _permissions = new();

        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationCommandsPermissionContext"/> class.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="name">The name.</param>
        internal ApplicationCommandsPermissionContext(Type type, string name)
        {
            this.Type = type;
            this.Name = name;
        }

        /// <summary>
        /// Adds a user to the permission system.
        /// </summary>
        /// <param name="userId">The Id of the user to give this permission.</param>
        /// <param name="permission">The permission for the application command. If set to true, they can use the command. If set to false, they can't use the command.</param>
        public void AddUser(ulong userId, bool permission) => _permissions.Add(new DiscordApplicationCommandPermission(userId, ApplicationCommandPermissionType.User, permission));

        /// <summary>
        /// Adds a user to the permission system.
        /// </summary>
        /// <param name="roleId">The Id of the role to give this permission.</param>
        /// <param name="permission">The permission for the application command. If set to true, they can use the command. If set to false, they can't use the command.</param>
        public void AddRole(ulong roleId, bool permission) => _permissions.Add(new DiscordApplicationCommandPermission(roleId, ApplicationCommandPermissionType.Role, permission));
    }
}
