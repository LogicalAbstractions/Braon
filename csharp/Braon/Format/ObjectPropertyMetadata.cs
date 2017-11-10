using System;
using System.Runtime.InteropServices;

namespace Braon.Format
{
#if SIZE_OPTIMIZED
    using PropertyIdType = System.Int16;
#else
    using PropertyIdType = System.Int32;
#endif
    [StructLayout(LayoutKind.Sequential)]
    internal struct ObjectPropertyMetadata : IEquatable<ObjectPropertyMetadata>,IComparable<ObjectPropertyMetadata>
    {   
#if SIZE_OPTIMIZED
        internal const int Size = 4 + 1;
    
        internal short offset;
#else
        internal const int Size = 8 + 4;
        
        internal int offset;
#endif
        internal PropertyIdType propertyId;
        internal BraonValueType valueType;

        public bool Equals(ObjectPropertyMetadata other)
        {
            return propertyId == other.propertyId;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is ObjectPropertyMetadata && Equals((ObjectPropertyMetadata) obj);
        }

        public override int GetHashCode()
        {
            return propertyId;
        }

        public int CompareTo(ObjectPropertyMetadata other)
        {
            if (propertyId < other.propertyId) return -1;
            if (propertyId > other.propertyId) return 1;
            return 0;
        }
    }
}