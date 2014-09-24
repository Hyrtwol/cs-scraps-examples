using System;

namespace Scraps.Examples
{
    internal static class hwclient
    {
        private static void Main()
        {
            Console.Title = "Hello World Client";
            Console.WriteLine("Connecting to hello world server...");
            using (var context = zmq.zmq_ctx_new())
            using (var requester = context.zmq_socket(SocketType.ZMQ_REQ))
            {
                requester.zmq_connect("tcp://localhost:5555").check();

                var buffer = new byte[10];
                for (int i = 0; i != 10; i++)
                {
                    Console.WriteLine("Sending Hello {0}...", i);
                    requester.zmq_send("Hello", 5);
                    requester.zmq_recv(buffer, 10);
                    Console.WriteLine("Received World {0}", i);
                }
            }
        }
    }
}
