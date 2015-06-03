using System;

namespace Scraps
{
    public class DisposeEventArgs : EventArgs
    {
        public readonly bool Disposing;

        public DisposeEventArgs(bool disposing)
        {
            Disposing = disposing;
        }
    }

    // ReSharper disable MemberCanBePrivate.Global

    /// <summary>
    /// Base class for a <see cref="T:System.IDisposable" /> class.
    /// </summary>
    public abstract class DisposeBase : IDisposable
    {
        /// <summary>
        /// Occurs when this instance is fully disposed.
        /// </summary>
        public event EventHandler<DisposeEventArgs> Disposed;

        /// <summary>
        /// Occurs when this instance is starting to be disposed.
        /// </summary>
        public event EventHandler<DisposeEventArgs> Disposing;

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        private void CheckAndDispose(bool disposing)
        {
            if (!IsDisposed)
            {
                var eventArgs = new DisposeEventArgs(disposing);
                if (Disposing != null)
                {
                    Disposing(this, eventArgs);
                }
                Dispose(disposing);
                if (disposing)
                {
                    GC.SuppressFinalize(this);
                }
                IsDisposed = true;
                if (Disposed != null)
                {
                    Disposed(this, eventArgs);
                }
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            CheckAndDispose(true);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        //protected abstract void Dispose(bool disposing);
        protected abstract void Dispose(bool disposing);

        /// <summary>
        /// Releases unmanaged resources and performs other cleanup operations before the
        /// <see cref="T:Cauldron.Core.DisposeBase" /> is reclaimed by garbage collection.
        /// </summary>
        ~DisposeBase()
        {
            CheckAndDispose(false);
        }

        /// <summary>
        /// Gets a value indicating whether this instance is disposed.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is disposed; otherwise, <c>false</c>.
        /// </value>
        public bool IsDisposed { get; private set; }
    }

    // ReSharper restore MemberCanBePrivate.Global
}