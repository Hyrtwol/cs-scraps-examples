using System.Collections.Generic;
using System.Threading;

namespace Scraps
{
    public class ProducerConsumer<T> where T : class
    {
        private readonly object _listLock = new object();
        private readonly Queue<T> _queue = new Queue<T>();

        public int QueueCount
        {
            get
            {
                lock (_listLock)
                {
                    return _queue.Count;
                }
            }
        }
        
        public bool Contains(T obj)
        {
            lock (_listLock)
            {
                return _queue.Contains(obj);
            }
        }

        public void Produce(T obj)
        {
            lock (_listLock)
            {
                _queue.Enqueue(obj);

                // We always need to pulse, even if the queue wasn't
                // empty before. Otherwise, if we add several items
                // in quick succession, we may only pulse once, waking
                // a single thread up, even if there are multiple threads
                // waiting for items.            
                Monitor.Pulse(_listLock);
            }
        }

        public T Consume()
        {
            lock (_listLock)
            {
                // If the queue is empty, wait for an item to be added
                // Note that this is a while loop, as we may be pulsed
                // but not wake up before another thread has come in and
                // consumed the newly added object. In that case, we'll
                // have to wait for another pulse.
                while (_queue.Count == 0)
                {
                    // This releases listLock, only reacquiring it
                    // after being woken up by a call to Pulse
                    Monitor.Wait(_listLock);
                }
                return _queue.Dequeue();
            }
        }

        public T Consume(int millisecondsTimeout)
        {
            lock (_listLock)
            {
                // If the queue is empty, wait for an item to be added
                // Note that this is a while loop, as we may be pulsed
                // but not wake up before another thread has come in and
                // consumed the newly added object. In that case, we'll
                // have to wait for another pulse.
                while (_queue.Count == 0)
                {
                    // This releases listLock, only reacquiring it
                    // after being woken up by a call to Pulse
                    Monitor.Wait(_listLock, millisecondsTimeout);
                }
                return _queue.Dequeue();
            }
        }

        public T Fetch()
        {
            lock (_listLock)
            {
                return _queue.Count > 0 ? _queue.Dequeue() : null;
            }
        }

        public T Fetch(int millisecondsTimeout)
        {
            lock (_listLock)
            {
                if (_queue.Count == 0)
                {
                    // This releases listLock, only reacquiring it
                    // after being woken up by a call to Pulse
                    Monitor.Wait(_listLock, millisecondsTimeout);
                }
                return _queue.Count > 0 ? _queue.Dequeue() : null;
            }
        }
    }
}
