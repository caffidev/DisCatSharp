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

namespace DisCatSharp
{
    /// <summary>
    /// Represents base for all DSharpPlus extensions. To implement your own extension, extend this class, and implement its abstract members.
    /// </summary>
    public abstract class BaseExtension
    {
        /// <summary>
        /// Gets the instance of <see cref="DiscordClient"/> this extension is attached to.
        /// </summary>
        public DiscordClient Client { get; protected set; }

        /// <summary>
        /// Initializes this extension for given <see cref="DiscordClient"/> instance.
        /// </summary>
        /// <param name="client">Discord client to initialize for.</param>
        protected internal abstract void Setup(DiscordClient client);
    }
}
