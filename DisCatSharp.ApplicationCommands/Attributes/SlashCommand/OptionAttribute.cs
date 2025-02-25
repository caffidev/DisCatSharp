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

namespace DisCatSharp.ApplicationCommands
{
    /// <summary>
    /// Marks this parameter as an option for a slash command
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false)]
    public class OptionAttribute : Attribute
    {
        /// <summary>
        /// Gets the name of this option
        /// </summary>
        public string Name;

        /// <summary>
        /// Gets the description of this option
        /// </summary>
        public string Description;

        /// <summary>
        /// Gets the optional allowed channel types.
        /// </summary>
        public IReadOnlyCollection<ChannelType> ChannelTypes { get; }

        /// <summary>
        /// Marks this parameter as an option for a slash command.
        /// </summary>
        /// <param name="name">The name of the option.</param>
        /// <param name="description">The description of the option.</param>
        /// <param name="channeltypes">The optional selectable channel types.</param>
        public OptionAttribute(string name, string description, params ChannelType[] channeltypes)
        {
            if(name.Length > 32)
                throw new ArgumentException("Slash command option names cannot go over 32 characters.");
            else if (description.Length > 100)
                throw new ArgumentException("Slash command option descriptions cannot go over 100 characters.");

            ReadOnlyCollection<ChannelType> channelTypes = channeltypes.Any() ? new(channeltypes) : null;
            Name = name.ToLower();
            Description = description;
            ChannelTypes = channelTypes;
        }
    }
}
