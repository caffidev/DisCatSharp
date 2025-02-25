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
using Microsoft.Extensions.DependencyInjection;

namespace DisCatSharp.ApplicationCommands
{
    /// <summary>
    /// A configuration for a <see cref="ApplicationCommandsExtension"/>
    /// </summary>
    public class ApplicationCommandsConfiguration
    {
        /// <summary>
        /// <para>Sets the service provider.</para>
        /// <para>Objects in this provider are used when instantiating application command modules. This allows passing data around without resorting to static members.</para>
        /// <para>Defaults to null.</para>
        /// </summary>
        public IServiceProvider Services { internal get; set; } = new ServiceCollection().BuildServiceProvider(true);
    }
}
