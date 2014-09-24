using System;
using System.Threading;

namespace Scraps.Examples
{
    static class wuserver
    {
        private static readonly ManualResetEvent TerminateEvent = new ManualResetEvent(false);

        static void Main()
        {
            Console.Title = "Weather Update Server";

            using (var context = zmq.zmq_ctx_new())
            {
                using (var publisher = context.zmq_socket(SocketType.ZMQ_PUB))
                {
                    publisher.zmq_bind("tcp://*:5556");
                    //publisher.Bind("ipc:///tmp/feeds/0");

                    var newThread = new Thread(DoWork);
                    newThread.Name = "wuserver";

                    Console.WriteLine("Press any key to continue.");
                    newThread.Start(publisher);

                    Console.ReadKey();
                    TerminateEvent.Set();
                    newThread.Abort();
                    newThread.Join();

                    Console.WriteLine("Closing socket");
                }
            }

            Console.WriteLine("Press any key to exit.");
            Console.ReadKey();
        }

        private static void DoWork(object obj)
        {
            var publisher = (zmq.socket)obj;
            var randomizer = new Random(DateTime.Now.Millisecond);

            while (!TerminateEvent.WaitOne(0))
            {
                //  Get values that will fool the boss
                int zipcode = randomizer.Next(0, 20000);
                int temperature = randomizer.Next(-80, 135);
                int relativeHumidity = randomizer.Next(10, 60);

                string update = string.Format("{0} {1} {2}", zipcode, temperature, relativeHumidity);

                //  Send message to 0..N subscribers via a pub socket
                publisher.zmq_send(update, update.Length);
            }
        }
    }
}
