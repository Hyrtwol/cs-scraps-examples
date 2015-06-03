using System;
using System.Threading;

namespace Scraps
{
    public abstract class Worker
    {
        private static readonly ManualResetEvent TerminateEvent = new ManualResetEvent(false);
        private Thread _newThread;

        public virtual string Name { get { return GetType().Name; } }

        public virtual TimeSpan DelayBeforeAbort { get { return TimeSpan.FromSeconds(5.0); } }

		public virtual int ThreadId { get { return _newThread.ManagedThreadId; } }

        protected Worker()
        {
        }

        protected abstract void WorkTick();

        public virtual void Start()
        {
            if (_newThread != null) return;

            _newThread = new Thread(WorkLoop);
            _newThread.Name = Name;
            BeforeThreadStart(_newThread);
            _newThread.Start();
        }

        protected virtual void BeforeThreadStart(Thread thread)
        {
        }

        public virtual void Stop()
        {
            if (_newThread == null) return;

            TerminateEvent.Set();

            if (_newThread.IsAlive)
            {
                if (!_newThread.Join(DelayBeforeAbort))
                {
                    AbortThread();
                    _newThread.Join();
                }
            }
            _newThread = null;
        }

        protected virtual void AbortThread()
        {
            _newThread.Abort();
        }

        public virtual void Dispose()
        {
            if (_newThread != null) Stop();
        }

        protected static bool ShouldExit()
        {
            return TerminateEvent.WaitOne(0);
        }

        protected virtual void WorkLoop()
        {
            while (!ShouldExit())
            {
                WorkTick();
            }
        }
    }
}