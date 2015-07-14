using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using NUnit.Framework;

namespace Scraps.Test
{
    [TestFixture]
    public class ZMQInteropTests
    {
        [Test]
        public void VerityStructSizes()
        {
            Assert.AreEqual(6, Marshal.SizeOf(typeof(zmq.zmq_event_t)));
            Assert.AreEqual(12, Marshal.SizeOf(typeof(zmq.zmq_pollitem_t)));
            //Assert.AreEqual(32, Marshal.SizeOf(typeof(ZMQ.zmq_msg_t)));
        }

        [Test]
        public void CreateContext()
        {
            zmq.context context;
            using (context = zmq.zmq_ctx_new())
            {
                Assert.IsFalse(context.IsInvalid);
                Assert.IsFalse(context.IsClosed);

                var result = context.zmq_ctx_get(ContextOption.ZMQ_IO_THREADS);
                Debug.Print("ZMQ_IO_THREADS={0}", result);
                Assert.AreEqual(1, result);

                using (var socket = context.zmq_socket(SocketType.ZMQ_REP))
                {
                    Assert.IsFalse(socket.IsInvalid);
                    Assert.IsFalse(socket.IsClosed);

                    int optionValue = 0;
                    int optionLen = 4;
                    socket.zmq_getsockopt(SocketOption.ZMQ_RCVHWM, ref optionValue, ref optionLen).check();
                    Debug.Print("ZMQ_RCVHWM={0} ({1})", optionValue, optionLen);

                    Assert.AreEqual(4, optionLen);
                    Assert.AreEqual(1000, optionValue);

                    Debug.Print("context={0}", context);
                    Debug.Print("socket={0}", socket);
                }
            }
            Debug.Print("context={0}", context);
            Assert.IsTrue(context.IsClosed);
            Assert.IsTrue(context.IsInvalid);
        }

        [Test]
        public void Constants()
        {
            Assert.AreEqual("libzmq-v110-mt-4_0_4.dll", zmq.dll);
            Assert.AreEqual(32, zmq.ZMQ_MESSAGE_SIZE);
        }

        [Test]
        public void GetErrNo()
        {
            var error = zmq.zmq_errno();
            Debug.WriteLine(error);
            Assert.AreEqual(Error.OK, error);
        }

        [Test]
        public void GetErrStr()
        {
            Assert.AreEqual("No error", zmq.zmq_strerror(0).ToString());

            Assert.AreEqual("Operation cannot be accomplished in current state", Error.EFSM.zmq_strerror().ToString());
            Assert.AreEqual("The protocol is not compatible with the socket type", Error.ENOCOMPATPROTO.zmq_strerror().ToString());
            Assert.AreEqual("Context was terminated", Error.ETERM.zmq_strerror().ToString());
            Assert.AreEqual("No thread available", Error.EMTHREAD.zmq_strerror().ToString());

            var script = new Action<int>(idx =>
                {
                    for (int i = 0; i < 200; i++)
                    {
                        var error = (Error) (idx);
                        var msg = error.zmq_strerror().ToString();
                        if (msg != "Unknown error")
                        {
                            Debug.Print("{0,-16} '{1}'", error, msg);
                        }
                        idx++;
                    }
                });
            script(0);
            script(zmq.ZMQ_HAUSNUMERO);
        }

        [Test]
        public void GetVersion()
        {
            int major, minor, patch;
            zmq.zmq_version(out major, out minor, out patch);
            Debug.WriteLine(string.Join(".", major, minor, patch));
            Assert.AreEqual(zmq.ZMQ_VERSION_MAJOR, major);
            Assert.AreEqual(zmq.ZMQ_VERSION_MINOR, minor);
            Assert.AreEqual(zmq.ZMQ_VERSION_PATCH, patch);
        }

        [Test]
        public void ContextHandle()
        {
            zmq.context handle;
            using (handle = new zmq.context())
            {
                Debug.Print("handle={0}", handle);
                Assert.IsNotNull(handle);
                Assert.IsTrue(handle.IsInvalid);
                Assert.IsFalse(handle.IsClosed);
            }
            Assert.IsTrue(handle.IsClosed);
            Assert.IsTrue(handle.IsInvalid);
        }

        [Test]
        public void MessageHandle()
        {
            zmq.message handle;
            using (handle = new zmq.message())
            {
                Debug.Print("handle={0}", handle);
                Assert.IsNotNull(handle);
                Assert.IsFalse(handle.IsInvalid);
                Assert.IsFalse(handle.IsClosed);
            }
            Assert.IsTrue(handle.IsClosed);
            Assert.IsTrue(handle.IsInvalid);
        }

        [Test]
        public void SocketHandle()
        {
            zmq.socket handle;
            using (handle = new zmq.socket())
            {
                Debug.Print("handle={0}", handle);
                Assert.IsNotNull(handle);
                Assert.IsTrue(handle.IsInvalid);
                Assert.IsFalse(handle.IsClosed);
            }
            Assert.IsTrue(handle.IsClosed);
            Assert.IsTrue(handle.IsInvalid);
        }

        [Test]
        public void Z85Decode()
        {
            const string data = "8vP8N";

            var size = data.Length;
            var destSize = size*4/5;
            Debug.Print("size={0}", size);
            Debug.Print("destSize={0}", destSize);

            var dest = new byte[destSize];
            var result = zmq.zmq_z85_decode(dest, data, size);
            Assert.AreNotEqual(IntPtr.Zero, result);
            Debug.WriteLine(string.Join(", ", dest));
        }

        [Test]
        public void Z85Encode()
        {
            var data = new byte[] {26, 12, 70, 111};
            Debug.WriteLine(string.Join(", ", data));

            var size = data.Length;
            var destSize = size*5/4;
            Debug.Print("size={0}", size);
            Debug.Print("destSize={0}", destSize);

            var dest = new StringBuilder(destSize);
            var result = zmq.zmq_z85_encode(dest, data, size);
            Assert.AreNotEqual(IntPtr.Zero, result);
            Debug.Print("'{0}'", dest);
        }

        [Test]
        public void Misc()
        {
            Debug.WriteLine(Path.GetFileNameWithoutExtension("blabla\\benny.txt"));
        }
    }
}