// solution configuration : Debug
// visual studio version: 11.0
// dll import file: libzmq-v110-mt-4_0_4.dll
// template file: ZeroMQ.tt
// customToolNamespace: 
// rootNamespace: Scraps
// see also:
//   http://api.zeromq.org/4-0:_start
//   C:\Program Files (x86)\ZeroMQ 4.0.4\include\zmq.h
// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable FieldCanBeMadeReadOnly.Global
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace Scraps
{
    /// <summary>
    /// 0MQ Bindings for version 4.0.4
    /// </summary>
    public static class zmq
    {
        #region 0MQ version

        public const int ZMQ_VERSION_MAJOR = 4;
        public const int ZMQ_VERSION_MINOR = 0;
        public const int ZMQ_VERSION_PATCH = 4;

        public const string dll = "libzmq-v110-mt-4_0_4.dll";

        /// <summary>
        /// Run-time API version detection
        /// </summary>
        [DllImport(dll, CallingConvention = CallingConvention.Cdecl)]
        public static extern void zmq_version(out int major, out int minor, out int patch);

        #endregion

        #region 0MQ errors

        /// <summary>
        /// A number random enough not to collide with different errno ranges on
        /// different OSes. The assumption is that error_t is at least 32-bit type.
        /// </summary>
        public const int ZMQ_HAUSNUMERO = 156384712;

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void check(this Result result)
        {
            if (result != Result.OK) error();
        }

        public static void error()
        {
            var errno = zmq_errno();
            var message = errno.zmq_strerror().ToString();
            error(string.Format("{0}: {1}", errno, message));
        }

        public static void error(string message)
        {
            throw new zmq_exception(message);
        }

        /// <summary>
		/// This function retrieves the errno as it is known to 0MQ library. The goal
		/// of this function is to make the code 100% portable, including where 0MQ
		/// compiled with certain CRT library (on Windows) is linked to an
		/// application that uses different CRT library.
        /// </summary>
        [DllImport(dll, CallingConvention = CallingConvention.Cdecl)]
        public static extern Error zmq_errno();

        /// <summary>
        /// Resolves system errors and 0MQ errors to human-readable string.
        /// </summary>
        [DllImport(dll, CallingConvention = CallingConvention.Cdecl)]
        public static extern text zmq_strerror(this Error errnum);

        #endregion

        #region 0MQ infrastructure

        /// <summary>
        /// creates a new 0MQ context.
        /// </summary>
        [DllImport(dll, CallingConvention = CallingConvention.Cdecl)]
        public static extern context zmq_ctx_new();

        /// <summary>
        /// Destroy a 0MQ context
        /// </summary>
        [DllImport(dll, CallingConvention = CallingConvention.Cdecl)]
        public static extern Result zmq_ctx_term(IntPtr context);

        /// <summary>
        /// Shutdown a 0MQ context
        /// </summary>
        [DllImport(dll, CallingConvention = CallingConvention.Cdecl)]
        public static extern Result zmq_ctx_shutdown(this context context);

        [DllImport(dll, CallingConvention = CallingConvention.Cdecl)]
        public static extern Result zmq_ctx_set(this context context, ContextOption option, int optval);

        [DllImport(dll, CallingConvention = CallingConvention.Cdecl)]
        public static extern int zmq_ctx_get(this context context, ContextOption option);

        #endregion

        #region 0MQ messages

        public const int ZMQ_MESSAGE_SIZE = 32;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void FreeMessageDataCallback(IntPtr data, IntPtr hint);

        [DllImport(dll, CallingConvention = CallingConvention.Cdecl)]
        public static extern Result zmq_msg_init(this message msg);

        [DllImport(dll, CallingConvention = CallingConvention.Cdecl)]
        public static extern Result zmq_msg_init_size(this message msg, int size);

        /// <summary>
        /// The zmq_msg_init_data() function shall initialise the message object referenced
        /// by msg to represent the content referenced by the buffer located at address data,
        /// size bytes long. No copy of data shall be performed and ØMQ shall take ownership
        /// of the supplied buffer.
        /// http://api.zeromq.org/4-0:zmq-msg-init-data
        /// </summary>
        [DllImport(dll, CallingConvention = CallingConvention.Cdecl)]
        public static extern Result zmq_msg_init_data(this message msg, IntPtr data, int size, FreeMessageDataCallback ffn, IntPtr hint);

        [DllImport(dll, CallingConvention = CallingConvention.Cdecl)]
        public static extern Result zmq_msg_send(this message msg, socket socket, SendOption flags);

        [DllImport(dll, CallingConvention = CallingConvention.Cdecl)]
        public static extern Result zmq_msg_recv(this message msg, socket socket, RecvOption flags);

        [DllImport(dll, CallingConvention = CallingConvention.Cdecl)]
        public static extern Result zmq_msg_close(this message msg);

        [DllImport(dll, CallingConvention = CallingConvention.Cdecl)]
        public static extern Result zmq_msg_move(this message dest, message src);

        [DllImport(dll, CallingConvention = CallingConvention.Cdecl)]
        public static extern Result zmq_msg_copy(this message dest, message src);

        [DllImport(dll, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr zmq_msg_data(this message msg);

        /// <summary>
        /// The zmq_msg_init_size() function shall allocate any resources required
        /// to store a message size bytes long and initialise the message object
        /// referenced by msg to represent the newly allocated message.
        /// <remarks>http://api.zeromq.org/4-0:zmq-msg-init-size</remarks>
        /// </summary>
        [DllImport(dll, CallingConvention = CallingConvention.Cdecl)]
        public static extern int zmq_msg_size(this message msg);

        /// <summary>
        /// The zmq_msg_more() function indicates whether this is part of a multi-part message,
        /// and there are further parts to receive. This method can safely be called after zmq_msg_close().
        /// This method is identical to zmq_msg_get() with an argument of ZMQ_MORE.
        /// </summary>
        /// <returns>The zmq_msg_more() function shall return zero if this is the final part
        /// of a multi-part message, or the only part of a single-part message.
        /// It shall return 1 if there are further parts to receive.</returns>
        [DllImport(dll, CallingConvention = CallingConvention.Cdecl)]
        public static extern int zmq_msg_more(this message msg);

        [DllImport(dll, CallingConvention = CallingConvention.Cdecl)]
        public static extern int zmq_msg_get(this message msg, MessageOption option);

        [DllImport(dll, CallingConvention = CallingConvention.Cdecl)]
        public static extern Result zmq_msg_set(this message msg, MessageOption option, int optval);

        #endregion

        #region 0MQ sockets

        [DllImport(dll, CallingConvention = CallingConvention.Cdecl)]
        public static extern socket zmq_socket(this context context, SocketType type);

        [DllImport(dll, CallingConvention = CallingConvention.Cdecl)]
        public static extern Result zmq_close(IntPtr socket);

        [DllImport(dll, CallingConvention = CallingConvention.Cdecl)]
        public static extern Result zmq_setsockopt(this socket socket, SocketOption option_name, [MarshalAs(UnmanagedType.LPArray)] byte[] option_value, int option_len);

        [DllImport(dll, CallingConvention = CallingConvention.Cdecl)]
        public static extern Result zmq_setsockopt(this socket socket, SocketOption option_name, [MarshalAs(UnmanagedType.LPStr)] string option_value, int option_len);

        [DllImport(dll, CallingConvention = CallingConvention.Cdecl)]
        public static extern Result zmq_setsockopt(this socket socket, SocketOption option_name, ref int option_value, int option_len);

        [DllImport(dll, CallingConvention = CallingConvention.Cdecl)]
        public static extern Result zmq_setsockopt(this socket socket, SocketOption option_name, ref uint option_value, int option_len);

        [DllImport(dll, CallingConvention = CallingConvention.Cdecl)]
        public static extern Result zmq_setsockopt(this socket socket, SocketOption option_name, ref long option_value, int option_len);

        [DllImport(dll, CallingConvention = CallingConvention.Cdecl)]
        public static extern Result zmq_setsockopt(this socket socket, SocketOption option_name, ref ulong option_value, int option_len);

        [DllImport(dll, CallingConvention = CallingConvention.Cdecl)]
        public static extern Result zmq_getsockopt(this socket socket, SocketOption option_name, IntPtr option_value, ref int option_len);

        [DllImport(dll, CallingConvention = CallingConvention.Cdecl)]
        public static extern Result zmq_getsockopt(this socket socket, SocketOption option_name, [Out, MarshalAs(UnmanagedType.LPArray)] byte[] option_value, ref int option_len);

        [DllImport(dll, CallingConvention = CallingConvention.Cdecl)]
        public static extern Result zmq_getsockopt(this socket socket, SocketOption option_name, [In, Out, MarshalAs(UnmanagedType.LPStr)] StringBuilder option_value, ref int option_len);

        [DllImport(dll, CallingConvention = CallingConvention.Cdecl)]
        public static extern Result zmq_getsockopt(this socket socket, SocketOption option_name, ref int option_value, ref int option_len);

        [DllImport(dll, CallingConvention = CallingConvention.Cdecl)]
        public static extern Result zmq_getsockopt(this socket socket, SocketOption option_name, ref uint option_value, ref int option_len);

        [DllImport(dll, CallingConvention = CallingConvention.Cdecl)]
        public static extern Result zmq_getsockopt(this socket socket, SocketOption option_name, ref long option_value, ref int option_len);

        [DllImport(dll, CallingConvention = CallingConvention.Cdecl)]
        public static extern Result zmq_getsockopt(this socket socket, SocketOption option_name, ref ulong option_value, ref int option_len);

        [DllImport(dll, CallingConvention = CallingConvention.Cdecl)]
        public static extern Result zmq_bind(this socket socket, [MarshalAs(UnmanagedType.LPStr)] string addr);

        [DllImport(dll, CallingConvention = CallingConvention.Cdecl)]
        public static extern Result zmq_connect(this socket socket, [MarshalAs(UnmanagedType.LPStr)] string addr);

        [DllImport(dll, CallingConvention = CallingConvention.Cdecl)]
        public static extern Result zmq_unbind(this socket socket, [MarshalAs(UnmanagedType.LPStr)] string addr);

        [DllImport(dll, CallingConvention = CallingConvention.Cdecl)]
        public static extern Result zmq_disconnect(this socket socket, [MarshalAs(UnmanagedType.LPStr)] string addr);

        [DllImport(dll, CallingConvention = CallingConvention.Cdecl)]
        public static extern int zmq_send(this socket socket, [MarshalAs(UnmanagedType.LPArray)] byte[] buf, int len, SendOption flags = 0);

        [DllImport(dll, CallingConvention = CallingConvention.Cdecl)]
        public static extern int zmq_send(this socket socket, [MarshalAs(UnmanagedType.LPStr)] string buf, int len, SendOption flags = 0);

        [DllImport(dll, CallingConvention = CallingConvention.Cdecl)]
        public static extern int zmq_send_const(this socket socket, [MarshalAs(UnmanagedType.LPArray)] byte[] buf, int len, SendOption flags = 0);

        [DllImport(dll, CallingConvention = CallingConvention.Cdecl)]
        public static extern int zmq_recv(this socket socket, [MarshalAs(UnmanagedType.LPArray)] byte[] buf, int len, RecvOption flags = 0);

        [DllImport(dll, CallingConvention = CallingConvention.Cdecl)]
        public static extern int zmq_recv(this socket socket, [In, Out, MarshalAs(UnmanagedType.LPStr)] StringBuilder buf, int len, RecvOption flags = 0);

        [DllImport(dll, CallingConvention = CallingConvention.Cdecl)]
        public static extern Result zmq_socket_monitor(this socket socket, [MarshalAs(UnmanagedType.LPStr)] string addr, SocketTransportEvent events);

        [DllImport(dll, CallingConvention = CallingConvention.Cdecl)]
        public static extern int zmq_sendmsg(this socket socket, IntPtr msg, SendOption flags = 0);

        [DllImport(dll, CallingConvention = CallingConvention.Cdecl)]
        public static extern int zmq_recvmsg(this socket socket, IntPtr msg, RecvOption flags = 0);

        #endregion

        #region 0MQ i/o multiplexing.

        /// <summary>
		/// Block on the current thread until one of the referenced sockets raise an event or a timeout expires
		/// </summary>
        [DllImport(dll, CallingConvention = CallingConvention.Cdecl)]
        public static extern int zmq_poll([In, Out] zmq_pollitem_t[] items, int nitems, int timeout);

        /// <summary>
		/// Built-in message proxy (3-way)
		/// </summary>
        [DllImport(dll, CallingConvention = CallingConvention.Cdecl)]
        public static extern int zmq_proxy(IntPtr frontend, IntPtr backend, IntPtr capture);

        #endregion

        #region 0MQ utilities

        [DllImport(dll, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr zmq_z85_encode([MarshalAs(UnmanagedType.LPStr)] StringBuilder dest, [MarshalAs(UnmanagedType.LPArray)] byte[] data, int size);

        [DllImport(dll, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr zmq_z85_decode([MarshalAs(UnmanagedType.LPArray)] byte[] dest, [MarshalAs(UnmanagedType.LPStr)] string data, int size);

        [DllImport(dll, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr zmq_stopwatch_start();

        /// <summary>
        /// Stops the stopwatch. Returns the number of microseconds elapsed since
        /// the stopwatch was started.
        /// </summary>
        /// <returns>microseconds elapsed</returns>
        [DllImport(dll, CallingConvention = CallingConvention.Cdecl)]
        public static extern uint zmq_stopwatch_stop(IntPtr watch);

        [DllImport(dll, CallingConvention = CallingConvention.Cdecl)]
        public static extern void zmq_sleep(int seconds);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void zmq_thread_fn(IntPtr data);

        [DllImport(dll, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr zmq_threadstart(zmq_thread_fn func, IntPtr arg);

        [DllImport(dll, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr zmq_threadclose(IntPtr thread);

        #endregion

        #region 0MQ handles

        public abstract class handle : SafeHandle
        {
            protected handle()
                : base(IntPtr.Zero, true)
            {
            }

            public override bool IsInvalid
            {
                get { return handle == IntPtr.Zero; }
            }

            public override string ToString()
            {
                return string.Format("{0}:{1:X8}", GetType().Name, handle.ToInt32());
            }
        }

        public class context : handle
        {
            protected override bool ReleaseHandle()
            {
                var result = zmq_ctx_term(handle) == Result.OK;
                if (result) handle = IntPtr.Zero;
                return result;
            }
        }

        public class socket : handle
        {
            protected override bool ReleaseHandle()
            {
                var result = zmq_close(handle) == Result.OK;
                if (result) handle = IntPtr.Zero;
                return result;
            }
        }

        public class message : handle
        {
            public message()
            {
                handle = Marshal.AllocHGlobal(ZMQ_MESSAGE_SIZE);
            }

            protected override bool ReleaseHandle()
            {
                if (handle != IntPtr.Zero) Marshal.FreeHGlobal(handle);
                handle = IntPtr.Zero;
                return true;
            }
        }

		public class text : handle
        {
            protected override bool ReleaseHandle()
            {
                handle = IntPtr.Zero;
                return true;
            }

            public override string ToString()
            {
                return Marshal.PtrToStringAnsi(handle);
            }
        }

        #endregion

        #region 0MQ types

        /// <summary>
        /// Socket event data
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 2)]
        public struct zmq_event_t
        {
            public SocketTransportEvent Event;
            public int Value;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct zmq_pollitem_t
        {
            public IntPtr Socket;
            public IntPtr FileDescriptor;
            public PollEvents Events;
            public short ReadyEvents;
        }

        #endregion

		public class zmq_exception : Exception
		{
			[DebuggerStepThrough]
			public zmq_exception(string message)
				: base(message)
			{
			}
		}
    }

    #region 0MQ Enumerations

    public enum Result
    {
        OK = 0,
        ERROR = -1
    }

    public enum Error
    {
        OK = 0,

        // Error Codes
        EPERM = 1,
        ENOENT = 2,
        ESRCH = 3,
        EINTR = 4,
        EIO = 5,
        ENXIO = 6,
        E2BIG = 7,
        ENOEXEC = 8,
        EBADF = 9,
        ECHILD = 10,
        EAGAIN = 11,
        ENOMEM = 12,
        EACCES = 13,
        EFAULT = 14,
        EBUSY = 16,
        EEXIST = 17,
        EXDEV = 18,
        ENODEV = 19,
        ENOTDIR = 20,
        EISDIR = 21,
        ENFILE = 23,
        EMFILE = 24,
        ENOTTY = 25,
        EFBIG = 27,
        ENOSPC = 28,
        ESPIPE = 29,
        EROFS = 30,
        EMLINK = 31,
        EPIPE = 32,
        EDOM = 33,
        EDEADLK = 36,
        ENAMETOOLONG = 38,
        ENOLCK = 39,
        ENOSYS = 40,
        ENOTEMPTY = 41,

        // Error codes used in the Secure CRT functions
        EINVAL = 22,
        ERANGE = 34,
        EILSEQ = 42,
        STRUNCATE = 80,

        // POSIX SUPPLEMENT,
        EADDRINUSE = 100,
        EADDRNOTAVAIL = 101,
        EAFNOSUPPORT = 102,
        EALREADY = 103,
        EBADMSG = 104,
        ECANCELED = 105,
        ECONNABORTED = 106,
        ECONNREFUSED = 107,
        ECONNRESET = 108,
        EDESTADDRREQ = 109,
        EHOSTUNREACH = 110,
        EIDRM = 111,
        EINPROGRESS = 112,
        EISCONN = 113,
        ELOOP = 114,
        EMSGSIZE = 115,
        ENETDOWN = 116,
        ENETRESET = 117,
        ENETUNREACH = 118,
        ENOBUFS = 119,
        ENODATA = 120,
        ENOLINK = 121,
        ENOMSG = 122,
        ENOPROTOOPT = 123,
        ENOSR = 124,
        ENOSTR = 125,
        ENOTCONN = 126,
        ENOTRECOVERABLE = 127,
        ENOTSOCK = 128,
        ENOTSUP = 129,
        EOPNOTSUPP = 130,
        EOTHER = 131,
        EOVERFLOW = 132,
        EOWNERDEAD = 133,
        EPROTO = 134,
        EPROTONOSUPPORT = 135,
        EPROTOTYPE = 136,
        ETIME = 137,
        ETIMEDOUT = 138,
        ETXTBSY = 139,
        EWOULDBLOCK = 140,

        // Native 0MQ error codes.
        EFSM = 156384763,
        ENOCOMPATPROTO = 156384764,
        ETERM = 156384765,
        EMTHREAD = 156384766
    }

    public enum ContextOption
    {
        ZMQ_IO_THREADS = 1,
        ZMQ_MAX_SOCKETS = 2
    }

    public enum SocketType
    {
        ZMQ_PAIR = 0,
        ZMQ_PUB = 1,
        ZMQ_SUB = 2,
        ZMQ_REQ = 3,
        ZMQ_REP = 4,
        ZMQ_DEALER = 5,
        ZMQ_ROUTER = 6,
        ZMQ_PULL = 7,
        ZMQ_PUSH = 8,
        ZMQ_XPUB = 9,
        ZMQ_XSUB = 10,
        ZMQ_STREAM = 11
    }

    public enum SocketOption
    {
        ZMQ_AFFINITY = 4,
        ZMQ_IDENTITY = 5,
        ZMQ_SUBSCRIBE = 6,
        ZMQ_UNSUBSCRIBE = 7,
        ZMQ_RATE = 8,
        ZMQ_RECOVERY_IVL = 9,
        ZMQ_SNDBUF = 11,
        ZMQ_RCVBUF = 12,
        ZMQ_RCVMORE = 13,
        ZMQ_FD = 14,
        ZMQ_EVENTS = 15,
        ZMQ_TYPE = 16,
        ZMQ_LINGER = 17,
        ZMQ_RECONNECT_IVL = 18,
        ZMQ_BACKLOG = 19,
        ZMQ_RECONNECT_IVL_MAX = 21,
        ZMQ_MAXMSGSIZE = 22,
        ZMQ_SNDHWM = 23,
        ZMQ_RCVHWM = 24,
        ZMQ_MULTICAST_HOPS = 25,
        ZMQ_RCVTIMEO = 27,
        ZMQ_SNDTIMEO = 28,
        ZMQ_LAST_ENDPOINT = 32,
        ZMQ_ROUTER_MANDATORY = 33,
        ZMQ_TCP_KEEPALIVE = 34,
        ZMQ_TCP_KEEPALIVE_CNT = 35,
        ZMQ_TCP_KEEPALIVE_IDLE = 36,
        ZMQ_TCP_KEEPALIVE_INTVL = 37,
        ZMQ_TCP_ACCEPT_FILTER = 38,
        ZMQ_IMMEDIATE = 39,
        ZMQ_XPUB_VERBOSE = 40,
        ZMQ_ROUTER_RAW = 41,
        ZMQ_IPV6 = 42,
        ZMQ_MECHANISM = 43,
        ZMQ_PLAIN_SERVER = 44,
        ZMQ_PLAIN_USERNAME = 45,
        ZMQ_PLAIN_PASSWORD = 46,
        ZMQ_CURVE_SERVER = 47,
        ZMQ_CURVE_PUBLICKEY = 48,
        ZMQ_CURVE_SECRETKEY = 49,
        ZMQ_CURVE_SERVERKEY = 50,
        ZMQ_PROBE_ROUTER = 51,
        ZMQ_REQ_CORRELATE = 52,
        ZMQ_REQ_RELAXED = 53,
        ZMQ_CONFLATE = 54,
        ZMQ_ZAP_DOMAIN = 55,
        ZMQ_ROUTER_HANDOVER = 56,
        ZMQ_TOS = 57,
        ZMQ_IPC_FILTER_PID = 58,
        ZMQ_IPC_FILTER_UID = 59,
        ZMQ_IPC_FILTER_GID = 60,
        ZMQ_CONNECT_RID = 61,
        ZMQ_GSSAPI_SERVER = 62,
        ZMQ_GSSAPI_PRINCIPAL = 63,
        ZMQ_GSSAPI_SERVICE_PRINCIPAL = 64,
        ZMQ_GSSAPI_PLAINTEXT = 65,
        ZMQ_HANDSHAKE_IVL = 66,
        ZMQ_IDENTITY_FD = 67,
        ZMQ_SOCKS_PROXY = 68
    }

    public enum MessageOption
    {
        ZMQ_MORE = 1,
        ZMQ_SRCFD = 2,
        ZMQ_SHARED = 3
    }

    public enum SendOption
    {
        ZMQ_DONTWAIT = 1,
        ZMQ_SNDMORE = 2
    }

    public enum RecvOption
    {
        ZMQ_DONTWAIT = 1
    }

    public enum SecurityMechanism
    {
        ZMQ_NULL = 0,
        ZMQ_PLAIN = 1,
        ZMQ_CURVE = 2
    }

    [Flags]
    public enum SocketTransportEvent : ushort
    {
        ZMQ_EVENT_CONNECTED = 0x0001,
        ZMQ_EVENT_CONNECT_DELAYED = 0x0002,
        ZMQ_EVENT_CONNECT_RETRIED = 0x0004,

        ZMQ_EVENT_LISTENING = 0x0008,
        ZMQ_EVENT_BIND_FAILED = 0x0010,

        ZMQ_EVENT_ACCEPTED = 0x0020,
        ZMQ_EVENT_ACCEPT_FAILED = 0x0040,

        ZMQ_EVENT_CLOSED = 0x0080,
        ZMQ_EVENT_CLOSE_FAILED = 0x0100,
        ZMQ_EVENT_DISCONNECTED = 0x0200,
        ZMQ_EVENT_MONITOR_STOPPED = 0x0400,

        ZMQ_EVENT_ALL = 0xFFFF
    }

    [Flags]
    public enum PollEvents : short
    {
        ZMQ_POLLIN = 0x1,
        ZMQ_POLLOUT = 0x2,
        ZMQ_POLLERR = 0x4
    }

    #endregion

}
