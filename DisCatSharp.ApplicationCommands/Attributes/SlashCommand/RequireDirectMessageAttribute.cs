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
using System.Threading.Tasks;
using DisCatSharp.Entities;

namespace DisCatSharp.ApplicationCommands.Attributes
{
    /// <summary>
    /// Defines that this application command is only usable within a direct message channel.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public sealed class ApplicationCommandRequireDirectMessageAttribute : SlashCheckBaseAttribute
    {
        /// <summary>
        /// Defines that this command is only usable within a direct message channel.
        /// </summary>
        public ApplicationCommandRequireDirectMessageAttribute()
        { }

        /// <summary>
        /// Runs checks.
        /// </summary>
        public override Task<bool> ExecuteChecksAsync(InteractionContext ctx)
            => Task.FromResult(ctx.Channel is DiscordDmChannel);
    }
}
