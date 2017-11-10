using System;
using System.Collections.Generic;
using Braon.Format;
using Braon.Text;

namespace Braon
{
    public unsafe struct BraonWriter : IDisposable
    {
        private byte* pointer;
        private List<ObjectPropertyMetadata> metadata;
        private int currentOffset;

        public BraonWriter(byte* pointer)
        {
            this.pointer = pointer;
            this.metadata = new List<ObjectPropertyMetadata>();
        }
        
        public void WriteRootValue(byte value)
        {
            pointer[currentOffset] = value;
            
            AddMetadata(BraonValueType.Byte);
            
            currentOffset++;
        }
        
        public void WriteRootValue(short value)
        {
            var writePtr = (short*)&pointer[currentOffset];
            writePtr[0] = value;
            
            AddMetadata(BraonValueType.Short);
            currentOffset += 2;
        }
        
        public void WriteRootValue(int value)
        {
            var writePtr = (int*)&pointer[currentOffset];
            writePtr[0] = value;
            
            AddMetadata(BraonValueType.Int);
            currentOffset += 4;
        }

        public void WriteRootValue(long value)
        {
            var writePtr = (long*)&pointer[currentOffset];
            writePtr[0] = value;
            
            AddMetadata(BraonValueType.Long);
            currentOffset += 8;
        }

        public void WriteRootValue(float value)
        {
            var writePtr = (float*)&pointer[currentOffset];
            writePtr[0] = value;
            
            AddMetadata(BraonValueType.Float);
            currentOffset += 4;
        }

        public void WriteRootValue(double value)
        {
            var writePtr = (double*)&pointer[currentOffset];
            writePtr[0] = value;
            
            AddMetadata(BraonValueType.Double);
            currentOffset += 8;
        }

        public void WriteRootValue(Utf8String value)
        {
            WriteRootValue(ref value);
        }
        
        public void WriteRootValue(ref Utf8String value)
        {
            var bytesWritten = BraonWritingUtilities.Write(&pointer[currentOffset], ref value);
            
            AddMetadata(BraonValueType.String);

            currentOffset += bytesWritten;
        }

        public void WriteRootValue(byte* ptr, int count)
        {
            ((int*) pointer)[0] = count;
            
            BraonWritingUtilities.CopyBytes(ptr,pointer + 4,count);
            AddMetadata(BraonValueType.Blob);

            currentOffset += 4 + count;
        }
        
        public void WriteRootValue(bool value)
        {
            WriteRootValue((byte)(value ? 1: 0));
        }

        public BraonObjectWriter WriteRootObject()
        {
            return new BraonObjectWriter(pointer,currentOffset,new List<ObjectPropertyMetadata>(),metadata,metadata.Count);
        }

        public BraonArrayWriter WriteRootArray()
        {
            return new BraonArrayWriter(pointer,currentOffset,new List<ObjectPropertyMetadata>(),metadata,metadata.Count);
        }

        public void Close(IPropertyNamesWriter propertyNamesWriter = null)
        {
            
        }

        public void Dispose()
        {
            Close();
        }
        
        private void AddMetadata(BraonValueType valueType)
        {
            metadata.Add(new ObjectPropertyMetadata()
            {
                offset = 0,
                propertyId = 0,
                valueType = valueType
            } );
        }
    }
}