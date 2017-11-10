using System;
using System.Runtime.InteropServices;
using Xunit;

namespace Braon.Tests
{
    public unsafe class SimpleMemoryCastingTests
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct SampleStruct : IEquatable<SampleStruct>
        {
            public int     intValue;
            public float   floatValue;
            public double  doubleValue;
            public byte    byteValue;

            public bool Equals(SampleStruct other)
            {
                return intValue == other.intValue && floatValue.Equals(other.floatValue) && doubleValue.Equals(other.doubleValue) && byteValue == other.byteValue;
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                return obj is SampleStruct && Equals((SampleStruct) obj);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    var hashCode = intValue;
                    hashCode = (hashCode * 397) ^ floatValue.GetHashCode();
                    hashCode = (hashCode * 397) ^ doubleValue.GetHashCode();
                    hashCode = (hashCode * 397) ^ byteValue.GetHashCode();
                    return hashCode;
                }
            }
        }
        
        [Fact]
        public unsafe void CopyingWorks()
        {
            var byteArray = new byte[Marshal.SizeOf<SampleStruct>() * 2];
            var handle = GCHandle.Alloc(byteArray, GCHandleType.Pinned);
            
            try
            {
                var sampleData = new SampleStruct()
                {
                    intValue = 10,
                    floatValue = 5.0f,
                    doubleValue = 12.0,
                    byteValue = 1
                };

                byte* ptr = (byte*) handle.AddrOfPinnedObject();

                SampleStruct* structPtr = (SampleStruct*) ptr;

                structPtr[0] = sampleData;

                SampleStruct readStruct = structPtr[0];
                
                Assert.Equal(sampleData,readStruct);
            }
            finally
            {
                handle.Free();
            }
        }
    }
}