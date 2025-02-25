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
    /// Represents a directory entrys primary category type.
    /// </summary>
    public enum DirectoryCategory : int
    {
        /// <summary>
        /// Indicates that this entry falls under the category Clubs.
        /// </summary>
        Clubs = 1,

        /// <summary>
        /// Indicates that this entry falls under the category Classes.
        /// </summary>
        Classes = 2,

        /// <summary>
        /// Indicates that this entry falls under the category Social and Study.
        /// </summary>
        SocialAndStudy = 3,

        /// <summary>
        /// Indicates that this entry falls under the category Majors and Subjects.
        /// </summary>
        MajorsAndSubjects = 4,

        /// <summary>
        /// Indicates that this entry falls under the category Miscellaneous.
        /// </summary>
        Miscellaneous = 5,
        
        /// <summary>
        /// Indicates unknown category type.
        /// </summary>
        Unknown = int.MaxValue
    }
}
