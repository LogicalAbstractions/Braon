using System;
using Braon.Format;
using Braon.Text;

namespace Braon
{
#if SIZE_OPTIMIZED
    using PropertyIdType = System.Int16;
#else
    using PropertyIdType = System.Int32;
#endif
    
    public unsafe struct BraonArrayReader
    {
        private byte* pointer; 
        private int metadataEntryPosition;
        private int elementCount;

        internal BraonArrayReader(byte * pointer, int elementCount, int metadataEntryPosition)
        {
            this.pointer = pointer;
            this.elementCount = elementCount;
            this.metadataEntryPosition = metadataEntryPosition;
        }

        public int Count => elementCount;

        public BraonValueType GetElementType(int index)
        {
            return GetElementEntry(index).valueType;
        }
        
        public byte GetElementValueAsByte(int index)
        {
            return GetElementPointer(index)[0];
        }

        public short GetElementValueAsShort(int index)
        {
            return ((short*) GetElementPointer(index))[0];
        }

        public int GetElementValueAsInt(int index)
        {
            return ((int*) GetElementPointer(index))[0];
        }
        
        public long GetElementValueAsLong(int index)
        {
            return ((long*) GetElementPointer(index))[0];
        }
        
        public float GetElementValueAsFloat(int index)
        {
            return ((float*) GetElementPointer(index))[0];
        }
        
        public double GetElementValueAsDouble(int index)
        {
            return ((double*) GetElementPointer(index))[0];
        }
        
        public Utf8String GetElementValueAsString(int index)
        {
            var position = GetElementPosition(index);

            return BraonReadingUtilities.ReadString(pointer,position);
        }

        public bool GetElementValueAsBool(int index)
        {
            return GetElementValueAsByte(index) > 0;
        }

        public BraonBlobData GetElementValueAsBlob(int index)
        {
            var position = GetElementPosition(index);
            
            return BraonReadingUtilities.ReadBlob(pointer,position);
        }

        public BraonObjectReader GetElementValueAsObject(int index)
        {
            var entry = GetElementEntry(index);
            return BraonReadingUtilities.ReadAsObject(pointer,
                metadataEntryPosition, entry.offset);
        }

        public BraonArrayReader GetElementValueAsArray(int index)
        {
            var entry = GetElementEntry(index);

            return BraonReadingUtilities.ReadAsArray(pointer,
                metadataEntryPosition, entry.offset);
        }

        private ArrayElementMetadata GetElementEntry(int index)
        {
            return ((ArrayElementMetadata*)&pointer[metadataEntryPosition])[index];
        }

        private byte* GetElementPointer(int index)
        {
            var entry = GetElementEntry(index);

            var position = metadataEntryPosition - 4 - entry.offset;

            return &pointer[position];
        }

        private int GetElementPosition(int index)
        {
            return metadataEntryPosition - 4 - GetElementEntry(index).offset;
        }
    }
}