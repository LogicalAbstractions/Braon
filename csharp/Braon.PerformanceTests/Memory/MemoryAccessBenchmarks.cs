using System;
using System.Drawing;
using System.Runtime.InteropServices;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

namespace Braon.PerformanceTests.Memory
{
    [BenchmarkSuite(Name = "MemoryAccess")]
    public unsafe class MemoryAccessBenchmarks
    {
        public const int Size = 1024 * 1024;
        
        private readonly int[] arrayData = new int[Size];
        private readonly GCHandle arrayHandle = new GCHandle();
        private readonly int* arrayPtr;
        private readonly IntPtr unmanagedHandle = Marshal.AllocHGlobal(Size * 4);
        private readonly int* unmanagedPtr;
        
        public MemoryAccessBenchmarks()
        {
            arrayHandle = GCHandle.Alloc(arrayData,GCHandleType.Pinned);
            arrayPtr = (int*) arrayHandle.AddrOfPinnedObject().ToPointer();
            unmanagedPtr = (int*)unmanagedHandle.ToPointer();
        }
        
        [Benchmark()]
        public void ManagedArrayAccess()
        {
            for (var i = 0; i < Size; ++i)
            {
                if (i % 2 == 0)
                {
                    arrayData[i] = arrayData[i] + 1;
                }
                else
                {
                    arrayData[i] = arrayData[i] - 1;
                }
            }
        }

        [Benchmark]
        public unsafe void ManagedArrayPointerAccess()
        {
            for (var i = 0; i < Size; ++i)
            {
                if (i % 2 == 0)
                {
                    arrayPtr[i] = arrayPtr[i] + 1;
                }
                else
                {
                    arrayPtr[i] = arrayPtr[i] - 1;
                }
            }
        }

        [Benchmark]
        public unsafe void ManagedArraySpanAccess()
        {
            var span = new Span<int>(arrayData);
            
            for (var i = 0; i < Size; ++i)
            {
                if (i % 2 == 0)
                {
                    span[i] = span[i] + 1;
                }
                else
                {
                    span[i] = span[i] - 1;
                }
            }
        }
        
        [Benchmark]
        public unsafe void ManagedArrayPointerSpanAccess()
        {
            var span = new Span<int>(arrayPtr,Size * 4);
            
            for (var i = 0; i < Size; ++i)
            {
                if (i % 2 == 0)
                {
                    span[i] = span[i] + 1;
                }
                else
                {
                    span[i] = span[i] - 1;
                }
            }
        }
        
        [Benchmark]
        public unsafe void UnmanagedArrayPointerAccess()
        {
            for (var i = 0; i < Size; ++i)
            {
                if (i % 2 == 0)
                {
                    unmanagedPtr[i] = unmanagedPtr[i] + 1;
                }
                else
                {
                    unmanagedPtr[i] = unmanagedPtr[i] - 1;
                }
            }
        }
        
        [Benchmark]
        public unsafe void UnmanagedArrayPointerSpanAccess()
        {
            var span = new Span<int>(unmanagedPtr,Size * 4);
            
            for (var i = 0; i < Size; ++i)
            {
                if (i % 2 == 0)
                {
                    span[i] = span[i] + 1;
                }
                else
                {
                    span[i] = span[i] - 1;
                }
            }
        }
    }
}