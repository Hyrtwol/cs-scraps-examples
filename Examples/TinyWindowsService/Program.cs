using System;
using System.Threading;
using Scraps;

namespace TinyWindowsService
{
    static class Program
    {
        static void Main(string[] args)
        {
            var service = new TinyService();
            WindowsService.Run(service);
        }
    }

    internal class TinyService : IWindowsService
    {
        private static readonly ManualResetEvent TerminateEvent = new ManualResetEvent(false);
        private Thread _thread;

        public TinyService()
        {
            _thread = null;
        }

        public void Dispose()
        {
            if (_thread != null) Stop();
        }

        public string Name { get { return GetType().Name; } }

        public void Start()
        {
            if (_thread != null) throw new InvalidOperationException("Service already started");
            _thread = new Thread(DoWork);
            _thread.Name = Name + "Thread";
            _thread.Start("some value");
        }

        public void Stop()
        {
            if (_thread == null) return;
            TerminateEvent.Set();
            if (!_thread.Join(TimeSpan.FromSeconds(10)))
            {
                _thread.Abort();
                _thread.Join();
            }
            _thread = null;
        }

        private static void DoWork(object obj)
        {
            Console.WriteLine(obj);
            var randomizer = new Random(DateTime.Now.Millisecond);

            while (!TerminateEvent.WaitOne(0))
            {
                //  Get values that will fool the boss
                int zipcode = randomizer.Next(0, 20000);
                int temperature = randomizer.Next(-80, 135);
                int relativeHumidity = randomizer.Next(10, 60);

                string update = string.Format("{0} {1} {2}", zipcode, temperature, relativeHumidity);

                Console.WriteLine(update);
            }
        }
    }
}
