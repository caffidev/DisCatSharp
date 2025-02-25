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
using System.Threading;
using System.Threading.Tasks;
using DisCatSharp.Entities;
using DisCatSharp.EventArgs;

namespace DisCatSharp.Interactivity.EventHandling
{
    /// <summary>
    /// Represents a match that is being waited for.
    /// </summary>
    internal class ComponentMatchRequest
    {
        /// <summary>
        /// The id to wait on. This should be uniquely formatted to avoid collisions.
        /// </summary>
        public DiscordMessage Message { get; private set; }

        /// <summary>
        /// The completion source that represents the result of the match.
        /// </summary>
        public TaskCompletionSource<ComponentInteractionCreateEventArgs> Tcs { get; private set; } = new();

        protected readonly CancellationToken _cancellation;
        protected readonly Func<ComponentInteractionCreateEventArgs, bool> _predicate;

        /// <summary>
        /// Initializes a new instance of the <see cref="ComponentMatchRequest"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="predicate">The predicate.</param>
        /// <param name="cancellation">The cancellation token.</param>
        public ComponentMatchRequest(DiscordMessage message, Func<ComponentInteractionCreateEventArgs, bool> predicate, CancellationToken cancellation)
        {
            this.Message = message;
            this._predicate = predicate;
            this._cancellation = cancellation;
            this._cancellation.Register(() => this.Tcs.TrySetResult(null)); // TrySetCancelled would probably be better but I digress ~Velvet //
        }

        /// <summary>
        /// Whether it is a match.
        /// </summary>
        /// <param name="args">The arguments.</param>
        public bool IsMatch(ComponentInteractionCreateEventArgs args) => this._predicate(args);
    }
}
