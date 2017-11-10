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
    
    public unsafe struct BraonReader
    {
        private byte* pointer;
        private int size;
        private DocumentMetadata documentMetadata;

        public BraonReader(byte * pointer,int size)
        {
            this.pointer = pointer;
            this.size = size;

            // Read the document metadata:
            this.documentMetadata = ((DocumentMetadata*)&pointer[size - DocumentMetadata.Size])[0];
        }

        public BraonValueType RootValueType => documentMetadata.rootValueType;

        public byte GetRootAsByte()
        {
            return GetRootPointer()[0];
        }

        public short GetRootAsShort()
        {
            return ((short*) GetRootPointer())[0];
        }

        public int GetRootAsInt()
        {
            return ((int*) GetRootPointer())[0];
        }
        
        public long GetRootAsLong()
        {
            return ((long*) GetRootPointer())[0];
        }
        
        public float GetRootAsFloat()
        {
            return ((float*) GetRootPointer())[0];
        }
        
        public double GetRootAsDouble()
        {
            return ((double*) GetRootPointer())[0];
        }
        
        public Utf8String GetRootAsString()
        {
            return BraonReadingUtilities.ReadString(pointer,GetRootPosition());
        }

        public BraonBlobData GetRootAsBlob()
        {
            return BraonReadingUtilities.ReadBlob(pointer,GetRootPosition());
        }

        public bool GetRootAsBool()
        {
            return GetRootAsByte() > 0;
        }
        
        public BraonArrayReader GetRootAsArray()
        {
            var metadataPosition = size - DocumentMetadata.Size - documentMetadata.rootValueOffset;
            var metadataEntryPosition = metadataPosition + 4;

            return BraonReadingUtilities.ReadAsArray(pointer,
                metadataEntryPosition, 0);
        }

        public BraonObjectReader GetRootAsObject()
        {
            var metadataPosition = size - DocumentMetadata.Size - documentMetadata.rootValueOffset;
             
            #if SIZE_OPTIMIZED
                var metadataEntryPosition = metadataPosition + 2;
            #else
                var metadataEntryPosition = metadataPosition + 4;
            #endif

            return BraonReadingUtilities.ReadAsObject(pointer,
                metadataEntryPosition,0);
        }

        private byte* GetRootPointer()
        {
            var metadataPosition = GetRootPosition();
            return &pointer[metadataPosition];
        }

        private int GetRootPosition()
        {
            return size - DocumentMetadata.Size - documentMetadata.rootValueOffset;
        }
    }
}