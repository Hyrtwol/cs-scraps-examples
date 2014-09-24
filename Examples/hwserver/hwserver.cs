using System;
using System.Threading;

namespace Scraps.Examples
{
    internal static class hwserver
    {
        private static void Main()
        {
            Console.Title = "Hello World Server";
            //  Socket to talk to clients
            using (var context = zmq.zmq_ctx_new())
            using (var responder = context.zmq_socket(SocketType.ZMQ_REP))
            {
                responder.zmq_bind("tcp://*:5555").check();

                while (true)
                {
                    var buffer = new byte[10];
                    int res = responder.zmq_recv(buffer, 10);
                    Console.WriteLine("Received Hello");
                    Thread.Sleep(500); //sleep(1); //  Do some 'work'
                    res = responder.zmq_send("World", 5);
                }
            }
        }
    }
}
