using System;
using System.Text;

namespace Scraps.Examples
{
    static class wuclient
    {
        static void Main(string[] args)
        {
            Console.Title = "Weather Update Client";

            Console.WriteLine("Collecting updates from weather server...");

            // default zipcode is 10001
            string zipcode = "10001 "; // the reason for having a space after 10001 is in case of the message would start with 100012 which we are not interested in

            if (args.Length > 0) zipcode = args[1] + " ";

            using (var context = zmq.zmq_ctx_new())
            {
                //Console.WriteLine("IOThreads:  " + context.IOThreads);
                //Console.WriteLine("MaxSockets: " + context.MaxSockets);
                using (var subscriber = context.zmq_socket(SocketType.ZMQ_SUB))
                {
                    var buf = Encoding.ASCII.GetBytes(zipcode);
                    subscriber.zmq_setsockopt(SocketOption.ZMQ_SUBSCRIBE, buf, buf.Length).check();
                    
                    subscriber.zmq_connect("tcp://localhost:5556");
                    //subscriber.Connect("ipc:///tmp/feeds/0");

                    const int updatesToCollect = 100;
                    int totalTemperature = 0;

                    var buffer = new StringBuilder(100);

                    for (int updateNumber = 0; updateNumber < updatesToCollect; updateNumber++)
                    {
                        int result = subscriber.zmq_recv(buffer, buffer.Capacity);
                        buffer.Length = result;
                        var update = buffer.ToString();
                        totalTemperature += Convert.ToInt32(update.Split()[1]);
                        Console.Write(".");
                    }
                    Console.WriteLine();
                    Console.WriteLine("Average temperature for zipcode {0} was {1}F", zipcode, totalTemperature / updatesToCollect);

                    Console.WriteLine("Press any key to exit.");
                    Console.ReadKey();
                }
            }
        }
    }
}
