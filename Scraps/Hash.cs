using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;

namespace Scraps
{
    /// <summary>
    /// This struct represents a hash code of 16 bytes.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Size = 16)]
    public struct Hash : IEquatable<Hash>
    {
        const int SizeOfHashInBytes = 16;
        const int SizeOfHashInChars = SizeOfHashInBytes * 2;

        static readonly Random Random = new Random();

        public static unsafe Hash NewHash(byte[] bytes)
        {
            if (bytes == null) throw new ArgumentNullException("bytes");
            if (bytes.Length != SizeOfHashInBytes) throw new ArgumentOutOfRangeException("bytes", "Length must be " + SizeOfHashInBytes + " bytes");
            fixed (byte* pSource = bytes)
            {
                return *((Hash*)(pSource));
            }
        }

        public static unsafe Hash NewHash(Guid guid)
        {
            // since Guid and Hash are both value-types and is the same size (16 bytes) a simple pointer cast will do the trick
            // take the address of guid, cast it to a Hash pointer, take the value of the Hash pointer address
            return *((Hash*)(&guid));
        }

        public static Hash NewUniqueHash()
        {
            return NewHash(Guid.NewGuid());
        }

        public static Hash NewRandomHash()
        {
            var bytes = new byte[16];
            Random.NextBytes(bytes);
            return NewHash(bytes);
        }

        public unsafe byte[] ToByteArray()
        {
            var bytes = new byte[SizeOfHashInBytes];
            fixed (byte* pByte = bytes)
            {
                *((Hash*)(pByte)) = this;
            }
            return bytes;
        }

        public string ToHexString()
        {
            return string.Concat(ToByteArray().Select(b => b.ToString("x2")));
        }

        public override string ToString()
        {
            return ToHexString();
        }

        public static explicit operator string(Hash hash)
        {
            return hash.ToHexString();
        }

        public static explicit operator byte[](Hash hash)
        {
            return hash.ToByteArray();
        }

        public static explicit operator Hash(byte[] bytes)
        {
            return NewHash(bytes);
        }

        public static explicit operator Hash(Guid guid)
        {
            return NewHash(guid);
        }

        public static explicit operator Guid(Hash hash)
        {
            return new Guid(hash.ToByteArray());
        }

        public static Hash Parse(string value)
        {
            if (value == null) throw new ArgumentNullException("value");
            if (value.Length != SizeOfHashInChars) throw new ArgumentOutOfRangeException("value", "Length must be " + SizeOfHashInChars + " chars");
            var bytes = new byte[SizeOfHashInBytes];
            for (int i = 0; i < SizeOfHashInBytes; i++)
            {
                bytes[i] = byte.Parse(value.Substring(i * 2, 2), NumberStyles.HexNumber);
            }
            return NewHash(bytes);
        }

        public static bool TryParse(string value, out Hash hash)
        {
            try
            {
                hash = Parse(value);
                return true;
            }
            catch (Exception)
            {
                hash = new Hash();
                return false;
            }
        }

        public unsafe bool Equals(Hash other)
        {
            fixed (Hash* pThis = &this)
            {
                long* pt = (long*)pThis, po = (long*)&other;
                return (pt[0] == po[0]) & (pt[1] == po[1]);
            }
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is Hash && Equals((Hash)obj);
        }

        public override unsafe int GetHashCode()
        {
            fixed (Hash* pSource = &this)
            {
                return *((int*)(pSource));
            }
        }

        public static Hash ComputeMD5(byte[] input)
        {
            if (input == null) throw new ArgumentNullException("input");
            using (var md5 = MD5.Create())
            {
                return NewHash(md5.ComputeHash(input));
            }
        }

        public static Hash ComputeMD5FromFile(string path)
        {
            if (path == null) throw new ArgumentNullException("path");
            using (var stream = File.OpenRead(path))
            {
                return ComputeMD5(stream);
            }
        }

        public static Hash ComputeMD5(FileInfo input)
        {
            if (input == null) throw new ArgumentNullException("input");
            using (var stream = input.OpenRead())
            {
                return ComputeMD5(stream);
            }
        }

        public static Hash ComputeMD5(Stream stream)
        {
            if (stream == null) throw new ArgumentNullException("stream");
            using (var md5 = MD5.Create())
            {
                var bytes = md5.ComputeHash(stream);
                return (Hash)bytes;
            }
        }
    }
}
