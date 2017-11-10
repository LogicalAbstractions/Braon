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
    
    public unsafe struct BraonArrayWriter
    {
        private byte* pointer;
        private int currentOffset;
        private List<ObjectPropertyMetadata> metadata;
        private List<ObjectPropertyMetadata> parentMetadata;
        private int propertyIdOrIndex;

        internal BraonArrayWriter(byte* pointer, 
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
        
        public void WriteElementValue(byte value)
        {
            pointer[currentOffset] = value;
            
            AddMetadata(currentOffset,BraonValueType.Byte);
            
            currentOffset++;
        }
        
        public void WriteElementValue(short value)
        {
            var writePtr = (short*)&pointer[currentOffset];
            writePtr[0] = value;
            
            AddMetadata(currentOffset,BraonValueType.Short);
            currentOffset += 2;
        }
        
        public void WriteElementValue(int value)
        {
            var writePtr = (int*)&pointer[currentOffset];
            writePtr[0] = value;
            
            AddMetadata(currentOffset,BraonValueType.Int);
            currentOffset += 4;
        }

        public void WriteElementValue(long value)
        {
            var writePtr = (long*)&pointer[currentOffset];
            writePtr[0] = value;
            
            AddMetadata(currentOffset,BraonValueType.Long);
            currentOffset += 8;
        }

        public void WriteElementValue(float value)
        {
            var writePtr = (float*)&pointer[currentOffset];
            writePtr[0] = value;
            
            AddMetadata(currentOffset,BraonValueType.Float);
            currentOffset += 4;
        }

        public void WriteElementValue(double value)
        {
            var writePtr = (double*)&pointer[currentOffset];
            writePtr[0] = value;
            
            AddMetadata(currentOffset,BraonValueType.Double);
            currentOffset += 8;
        }

        public void WriteElementValue(Utf8String value)
        {
            WriteElementValue(ref value);
        }
        
        public void WriteElementValue(ref Utf8String value)
        {
            var bytesWritten = BraonWritingUtilities.Write(&pointer[currentOffset], ref value);
            
            AddMetadata(currentOffset,BraonValueType.String);

            currentOffset += bytesWritten;
        }

        public void WriteElementValue(byte* ptr, int count)
        {
            ((int*) pointer)[0] = count;
            
            BraonWritingUtilities.CopyBytes(ptr,pointer + 4,count);

            currentOffset += 4 + count;
        }
        
        public void WriteElementValue(PropertyIdType id, bool value)
        {
            WriteElementValue((byte)(value ? 1: 0));
        }

        public BraonObjectWriter WritePropertyObject()
        {
            return new BraonObjectWriter(pointer,currentOffset,new List<ObjectPropertyMetadata>(),metadata,metadata.Count);
        }

        public BraonArrayWriter WritePropertyArray(PropertyIdType id)
        {
            return new BraonArrayWriter(pointer,currentOffset,new List<ObjectPropertyMetadata>(),metadata,metadata.Count);
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
            
#if SIZE_OPTIMIZED       
            ((short*) &pointer[currentOffset])[0] = (short)metadata.Count;
            currentOffset += 2;
#else
            ((int*) &pointer[currentOffset])[0] = metadata.Count;
            currentOffset += 4;
#endif
           
            var metadataPtr = (ArrayElementMetadata*) &pointer[currentOffset];
            
            // Write out properties:
            for (var i = 0; i < metadata.Count; ++i)
            {
                var metadataEntry = metadata[i];

                metadataPtr[i].offset = startPosition - metadataEntry.offset;
                metadataPtr[i].valueType = metadataEntry.valueType;
            }
        }
        
        private void AddMetadata(int offset, BraonValueType valueType)
        {
            metadata.Add(new ObjectPropertyMetadata()
            {
                offset = offset,
                propertyId = (PropertyIdType)metadata.Count,
                valueType = valueType
            } );
        }
    }
}