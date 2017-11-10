using Braon.Text;

namespace Braon
{
    internal static unsafe class BraonWritingUtilities
    {
        internal static int Write(byte * writePtr, ref Utf8String value)
        {
            var bytesWritten = 0;
             
            #if SIZE_OPTIMIZED
                var lengthPtr = (short*) writePtr;
                lengthPtr[0] = (short)value.ByteLength;
                var stringBytesPtr = writePtr + 2;
                bytesWritten = 2;
            #else
                var lengthPtr = (int*) writePtr;
                lengthPtr[0] = value.ByteLength;
                var stringBytesPtr = writePtr + 4;
                bytesWritten = 4;
            #endif

            CopyBytes(value.Pointer,stringBytesPtr,value.ByteLength);

            return bytesWritten + value.ByteLength;
        }

        internal static void CopyBytes(byte* sourcePtr, byte* targetPtr, int count)
        {
            for (var i = 0; i < count; ++i)
            {
                targetPtr[i] = sourcePtr[i];
            }
        }
    }
}