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

namespace DisCatSharp.CommandsNext.Attributes
{
    /// <summary>
    /// Defines a lifespan for this command module.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class ModuleLifespanAttribute : Attribute
    {
        /// <summary>
        /// Gets the lifespan defined for this module.
        /// </summary>
        public ModuleLifespan Lifespan { get; }

        /// <summary>
        /// Defines a lifespan for this command module.
        /// </summary>
        /// <param name="lifespan">Lifespan for this module.</param>
        public ModuleLifespanAttribute(ModuleLifespan lifespan)
        {
            this.Lifespan = lifespan;
        }
    }

    /// <summary>
    /// Defines lifespan of a command module.
    /// </summary>
    public enum ModuleLifespan : int
    {
        /// <summary>
        /// Defines that this module will be instantiated once.
        /// </summary>
        Singleton = 0,

        /// <summary>
        /// Defines that this module will be instantiated every time a containing command is called.
        /// </summary>
        Transient = 1
    }
}
