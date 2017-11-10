using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Braon.Text
{
    public sealed class Utf8StringSource : IDisposable
    {
        private static readonly Encoding encoding = Encoding.UTF8;
        
        private readonly string sourceValue;
        private readonly byte[] utf8ByteArray;
        private readonly GCHandle handle;
            
        public Utf8StringSource(string sourceValue)
        {
            this.sourceValue = sourceValue;
            this.utf8ByteArray = encoding.GetBytes(sourceValue);
            this.handle = GCHandle.Alloc(utf8ByteArray,GCHandleType.Pinned);
        }

        public void Dispose()
        {
            ReleaseUnmanagedResources();
            GC.SuppressFinalize(this);
        }

        public override string ToString()
        {
            return sourceValue;
        }

        public static unsafe implicit operator Utf8String(Utf8StringSource source)
        {
            return new Utf8String((byte*) source.handle.AddrOfPinnedObject(), source.utf8ByteArray.Length);
        }

        ~Utf8StringSource()
        {
            ReleaseUnmanagedResources();
        }

        private void ReleaseUnmanagedResources()
        {
            handle.Free();
        }
    }
}