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

using System.Threading;
using System.Threading.Tasks;

namespace DisCatSharp.Common.Utilities
{
    /// <summary>
    /// Represents a thread synchronization event that, when signaled, must be reset manually. Unlike <see cref="ManualResetEventSlim"/>, this event is asynchronous.
    /// </summary>
    public sealed class AsyncManualResetEvent
    {
        /// <summary>
        /// Gets whether this event has been signaled.
        /// </summary>
        public bool IsSet => this._resetTcs?.Task?.IsCompleted == true;

        private volatile TaskCompletionSource<bool> _resetTcs;

        /// <summary>
        /// Creates a new asynchronous synchronization event with initial state.
        /// </summary>
        /// <param name="initialState">Initial state of this event.</param>
        public AsyncManualResetEvent(bool initialState)
        {
            this._resetTcs = new TaskCompletionSource<bool>();
            if (initialState)
                this._resetTcs.TrySetResult(initialState);
        }

        // Spawn a threadpool thread instead of making a task
        // Maybe overkill, but I am less unsure of this than awaits and
        // potentially cross-scheduler interactions
        /// <summary>
        /// Asynchronously signal this event.
        /// </summary>
        /// <returns></returns>
        public Task SetAsync()
            => Task.Run(() => this._resetTcs.TrySetResult(true));

        /// <summary>
        /// Asynchronously wait for this event to be signaled.
        /// </summary>
        /// <returns></returns>
        public Task WaitAsync()
            => this._resetTcs.Task;

        /// <summary>
        /// Reset this event's signal state to unsignaled.
        /// </summary>
        public void Reset()
        {
            while (true)
            {
                var tcs = this._resetTcs;
                if (!tcs.Task.IsCompleted || Interlocked.CompareExchange(ref this._resetTcs, new TaskCompletionSource<bool>(), tcs) == tcs)
                    return;
            }
        }
    }
}
