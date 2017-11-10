using System.Runtime.InteropServices;

namespace Braon.Format
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct ArrayElementMetadata
    {
#if SIZE_OPTIMIZED
        internal const int Size = 2 + 1;
    
        internal short offset;   
#else
        internal const int Size = 4 + 4;

        internal int offset;
#endif
        internal BraonValueType valueType;
    }
}