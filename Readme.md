# 0MQ C# Examples

Examples for [ZeroMQ](http://zeromq.org/) C# Bindings.

## Links

* [0MQ](http://zeromq.org/)
* [0MQ API Reference](http://api.zeromq.org/)
* [Submodule](cszmq)

### zmq.tt

	<#
	    var settings = new Dictionary<string, object>()
	    {
	        {"Version", new Version("4.0.4")},
	        {"UseException", true},
	        {"Visibility", "public"},
	        {"DeclareException", true},
	        {"ClassName", "zmq"}
	    };
	#>
	<#@ include file="..\cs-scraps\ZeroMQ.tt" #>

### hwclient.cs

Converted from [hwclient.c](https://github.com/imatix/zguide/blob/master/examples/C/hwclient.c)

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

### hwserver.cs

Converted from [hwserver.c](https://github.com/imatix/zguide/blob/master/examples/C/hwserver.c)

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
