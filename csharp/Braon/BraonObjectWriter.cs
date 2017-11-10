using System;
using System.Collections.Generic;
using Braon.Format;
using Braon.Text;

namespace Braon
{
#if SIZE_OPTIMIZED
    using PropertyIdType = System.Int16;
#else
    using PropertyIdType = System.Int32;
#endif
    
    public unsafe struct BraonObjectWriter : IDisposable
    {
        private byte* pointer;
        private int currentOffset;
        private List<ObjectPropertyMetadata> metadata;
        private List<ObjectPropertyMetadata> parentMetadata;
        private int propertyIdOrIndex;

        internal BraonObjectWriter(byte* pointer, 
            int currentOffset, 
            List<ObjectPropertyMetadata> metadata, 
            List<ObjectPropertyMetadata> parentMetadata,
            int propertyIdOrIndex)
        {
            this.pointer = pointer;
            this.currentOffset = currentOffset;
            this.metadata = metadata;
            this.parentMetadata = parentMetadata;
            this.propertyIdOrIndex = propertyIdOrIndex;
        }

        public void WritePropertyValue(PropertyIdType id,byte value)
        {
            pointer[currentOffset] = value;
            
            AddMetadata(id,currentOffset,BraonValueType.Byte);
            
            currentOffset++;
        }
        
        public void WritePropertyValue(PropertyIdType id,short value)
        {
            var writePtr = (short*)&pointer[currentOffset];
            writePtr[0] = value;
            
            AddMetadata(id,currentOffset,BraonValueType.Short);
            currentOffset += 2;
        }
        
        public void WritePropertyValue(PropertyIdType id,int value)
        {
            var writePtr = (int*)&pointer[currentOffset];
            writePtr[0] = value;
            
            AddMetadata(id,currentOffset,BraonValueType.Int);
            currentOffset += 4;
        }

        public void WritePropertyValue(PropertyIdType id, long value)
        {
            var writePtr = (long*)&pointer[currentOffset];
            writePtr[0] = value;
            
            AddMetadata(id,currentOffset,BraonValueType.Long);
            currentOffset += 8;
        }

        public void WritePropertyValue(PropertyIdType id, float value)
        {
            var writePtr = (float*)&pointer[currentOffset];
            writePtr[0] = value;
            
            AddMetadata(id,currentOffset,BraonValueType.Float);
            currentOffset += 4;
        }

        public void WritePropertyValue(PropertyIdType id, double value)
        {
            var writePtr = (double*)&pointer[currentOffset];
            writePtr[0] = value;
            
            AddMetadata(id,currentOffset,BraonValueType.Double);
            currentOffset += 8;
        }

        public void WritePropertyValue(PropertyIdType id, Utf8String value)
        {
            WritePropertyValue(id,ref value);
        }
        
        public void WritePropertyValue(PropertyIdType id, ref Utf8String value)
        {
            var bytesWritten = BraonWritingUtilities.Write(&pointer[currentOffset], ref value);
            
            AddMetadata(id,currentOffset,BraonValueType.String);

            currentOffset += bytesWritten;
        }

        public void WritePropertyValue(PropertyIdType id, byte* ptr, int count)
        {
            ((int*) pointer)[0] = count;
            
            BraonWritingUtilities.CopyBytes(ptr,pointer + 4,count);

            currentOffset += 4 + count;
        }
        
        public void WritePropertyValue(PropertyIdType id, bool value)
        {
            WritePropertyValue(id,(byte)(value ? 1: 0));
        }

        public BraonObjectWriter WritePropertyObject(PropertyIdType id)
        {
            return new BraonObjectWriter(pointer,currentOffset,new List<ObjectPropertyMetadata>(),metadata,id);
        }

        public BraonArrayWriter WritePropertyArray(PropertyIdType id)
        {
            return new BraonArrayWriter(pointer,currentOffset,new List<ObjectPropertyMetadata>(),metadata,id);
        }

        public void Dispose()
        {
            var startPosition = currentOffset;
            
            // Write parent entry:
            parentMetadata.Add(new ObjectPropertyMetadata()
            {
                propertyId = propertyIdOrIndex,
                offset = currentOffset,
                valueType = BraonValueType.Object
            });
            
            metadata.Sort();
            
#if SIZE_OPTIMIZED       
            ((short*) &pointer[currentOffset])[0] = (short)metadata.Count;
            currentOffset += 2;
#else
            ((int*) &pointer[currentOffset])[0] = metadata.Count;
            currentOffset += 4;
#endif
            var metadataPtr = (ObjectPropertyMetadata*) &pointer[currentOffset];
            
            // Write out properties:
            for (var i = 0; i < metadata.Count; ++i)
            {
                var metadataEntry = metadata[i];

                metadataEntry.offset = startPosition - metadataEntry.offset;

                metadataPtr[i] = metadataEntry;
            }
        }
        
        private void AddMetadata(PropertyIdType propertyId, int offset, BraonValueType valueType)
        {
            metadata.Add(new ObjectPropertyMetadata()
            {
                offset = offset,
                propertyId = propertyId,
                valueType = valueType
            } );
        }
    }
}