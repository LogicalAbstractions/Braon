using System.Runtime.InteropServices;

namespace Braon.Format
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct DocumentMetadata
    {
        #if SIZE_OPTIMIZED
        internal const int Size = 12 + 1;
        #else
        internal const int Size = 16;
        #endif

        internal int             propertyNamesOffset;
        internal int             propertyNamesCount;
        
        internal int             rootValueOffset;
        internal BraonValueType  rootValueType;
    }
}