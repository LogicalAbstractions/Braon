using System;
using System.Runtime.InteropServices;
using Braon.Format;
using Braon.Text;

namespace Braon
{
#if SIZE_OPTIMIZED
    using PropertyIdType = System.Int16;
#else
    using PropertyIdType = System.Int32;
#endif
    
    public unsafe struct BraonObjectReader
    {
        private byte* pointer;
        private int metadataEntryPosition;
        
        private PropertyIdType propertyCount;

        internal BraonObjectReader(byte * pointer,PropertyIdType propertyCount,int metadataEntryPosition)
        {
            this.pointer                   = pointer;
            this.propertyCount             = propertyCount;
            this.metadataEntryPosition     = metadataEntryPosition;
        }

        public struct PropertyIdEnumerator
        {
            private byte* pointer;
            private int metadataEntryPosition;
            private PropertyIdType propertyCount;
            private PropertyIdType index;

            internal PropertyIdEnumerator(byte * pointer, PropertyIdType propertyCount,int metadataEntryPosition)
            {
                this.pointer = pointer;
                this.metadataEntryPosition = metadataEntryPosition;
                this.propertyCount = propertyCount;
                this.index = -1;
            }
            
            public bool MoveNext()
            {
                index++;

                return index < propertyCount;
            }

            public void Reset()
            {
                index = -1;
            }

            public int Current => ((ObjectPropertyMetadata*) &pointer[metadataEntryPosition])[index].propertyId;

            public void Dispose()
            {
            }
        }

        public PropertyIdType Count => propertyCount;

        public PropertyIdType GetPropertyId(int index)
        {
            return ((ObjectPropertyMetadata*) &pointer[metadataEntryPosition])[index].propertyId;
        }

        public PropertyIdEnumerator GetEnumerator()
        {
            return new PropertyIdEnumerator(pointer,propertyCount,metadataEntryPosition);
        }
        
        public BraonValueType GetPropertyType(PropertyIdType id)
        {
            if (TryFindPropertyEntry(id, out var entry))
            {
                return entry.valueType;
            }

            ThrowPropertyNotFound(id);
            return BraonValueType.Undefined;
        }
        
        public bool HasProperty(PropertyIdType id)
        {
            return TryFindPropertyEntry(id, out var _);
        }

        public byte ReadPropertyValueAsByte(PropertyIdType id)
        {
            if (TryFindPropertyPointer(id, out var ptr))
            {
                return ptr[0];
            }

            ThrowPropertyNotFound(id);
            return 0;
        }

        public short ReadPropertyValueAsShort(PropertyIdType id)
        {
            if (TryFindPropertyPointer(id, out var ptr))
            {
                return ((short*) ptr)[0];
            }

            ThrowPropertyNotFound(id);
            return 0;
        }

        public int ReadPropertyValueAsInt(PropertyIdType id)
        {
            if (TryFindPropertyPointer(id, out var ptr))
            {
                return ((int*) ptr)[0];
            }

            ThrowPropertyNotFound(id);
            return 0;
        }
        
        public long ReadPropertyValueAsLong(PropertyIdType id)
        {
            if (TryFindPropertyPointer(id, out var ptr))
            {
                return ((long*) ptr)[0];
            }

            ThrowPropertyNotFound(id);
            return 0;
        }
        
        public float ReadPropertyValueAsFloat(PropertyIdType id)
        {
            if (TryFindPropertyPointer(id, out var ptr))
            {
                return ((float*) ptr)[0];
            }

            ThrowPropertyNotFound(id);
            return 0.0f;
        }
        
        public double ReadPropertyValueAsDouble(PropertyIdType id)
        {
            if (TryFindPropertyPointer(id, out var ptr))
            {
                return ((double*) ptr)[0];
            }

            ThrowPropertyNotFound(id);
            return 0.0;
        }
        
        public Utf8String ReadPropertyValueAsString(PropertyIdType id)
        {
            if (TryFindPropertyPosition(id,out var position))
            {
                return BraonReadingUtilities.ReadString(pointer,position);
            }

            ThrowPropertyNotFound(id);
            return Utf8String.Empty;
        }

        public bool ReadPropertyValueAsBool(PropertyIdType id)
        {
            return ReadPropertyValueAsByte(id) > 0;
        }

        public BraonBlobData ReadPropertyValueAsBlob(PropertyIdType id)
        {
            if (TryFindPropertyPosition(id, out var position))
            {
                return BraonReadingUtilities.ReadBlob(pointer,position);
            }

            ThrowPropertyNotFound(id);
            return new BraonBlobData();
        }

        public BraonObjectReader ReadPropertyValueAsObject(PropertyIdType id)
        {
            if (TryFindPropertyEntry(id, out var entry))
            {
                return BraonReadingUtilities.ReadAsObject(pointer,
                    metadataEntryPosition, entry.offset);
            }

            ThrowPropertyNotFound(id);
            return new BraonObjectReader();
        }

        public BraonArrayReader ReadPropertyValueAsArray(PropertyIdType id)
        {
            if (TryFindPropertyEntry(id, out var entry))
            {
                return BraonReadingUtilities.ReadAsArray(pointer,
                    metadataEntryPosition, entry.offset);
            }
            
            ThrowPropertyNotFound(id);
            return new BraonArrayReader();
        }

        private bool TryFindPropertyPosition(PropertyIdType propertyId, out int position)
        {
            if (TryFindPropertyEntry(propertyId, out var entry))
            {
                #if SIZE_OPTIMIZED
                position = metadataEntryPosition - 2 - entry.offset;
                #else
                position = metadataEntryPosition - 4 - entry.offset;
                #endif

                return true;
            }

            position = 0;
            return false;
        }

        private bool TryFindPropertyPointer(PropertyIdType propertyId, out byte * ptr)
        {
            if (TryFindPropertyEntry(propertyId, out var entry))
            {
                #if SIZE_OPTIMIZED
                 ptr = &pointer[metadataEntryPosition - 2 - entry.offset];      
                #else
                ptr = &pointer[metadataEntryPosition - 4 - entry.offset];       
                #endif

                return true;
            }

            ptr = null;
            return false;
        }
        
        private bool TryFindPropertyEntry(PropertyIdType propertyId,out ObjectPropertyMetadata metadata)
        {
            var entryPointer = (ObjectPropertyMetadata*)&pointer[metadataEntryPosition];
 
            var minIndex = 0;
            var maxIndex = propertyCount - 1;
            var found = false;

            while (!found)
            {
                var guessIndex = (maxIndex + minIndex) / 2;
                var candidatePropertyId = entryPointer[guessIndex].propertyId;
                
                if (candidatePropertyId == propertyId)
                {
                    metadata = entryPointer[guessIndex];
                    return true;
                }
                
                minIndex = candidatePropertyId < propertyId ? guessIndex + 1 : minIndex;
                maxIndex = candidatePropertyId > propertyId ? guessIndex - 1 : maxIndex;

                if (minIndex >= propertyCount || maxIndex < 0)
                {
                    break;
                }
            }
            
            metadata = new ObjectPropertyMetadata();
            return false;
        }

        private void ThrowPropertyNotFound(PropertyIdType id)
        {
            throw new BraonException($"Property with id {id} not found");
        }
    }
}