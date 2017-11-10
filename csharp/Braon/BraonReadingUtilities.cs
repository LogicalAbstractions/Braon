using Braon.Text;

namespace Braon
{
#if SIZE_OPTIMIZED
    using PropertyIdType = System.Int16;
#else
    using PropertyIdType = System.Int32;
#endif
    internal static unsafe class BraonReadingUtilities
    {
        internal static Utf8String ReadString(byte * ptr,int headerPosition)
        {
            var sourcePtr = &ptr[headerPosition];
            
            // Read the length:
            #if SIZE_OPTIMIZED
                short stringByteCount = ((short*)sourcePtr)[0];
                var stringBytesPtr = &sourcePtr[2];
            #else
                int stringByteCount = ((int*)sourcePtr)[0];
                var stringBytesPtr = &sourcePtr[4];
            #endif

            return new Utf8String(stringBytesPtr, (int)stringByteCount);
        }

        internal static BraonBlobData ReadBlob(byte * ptr,int headerPosition)
        {
            var sourcePtr = &ptr[headerPosition];
            
            // Read the length:         
            var blobByteCount = ((int*) sourcePtr)[0];
            
            return new BraonBlobData(&sourcePtr[4],blobByteCount);
        }

        internal static BraonArrayReader ReadAsArray(
            byte * ptr,
            int metadataEntryPosition,
            int offset)  
        {
            var arrayMetadataEntryPosition = metadataEntryPosition - offset;
            var arrayMetadataPosition = arrayMetadataEntryPosition - 4;

            var sourcePtr = &ptr[arrayMetadataPosition];

            var elementCount = ((int*) sourcePtr)[0];
                
            return new BraonArrayReader(ptr,elementCount,arrayMetadataEntryPosition);
        }
        
        internal static BraonObjectReader ReadAsObject(
            byte * ptr,
            int metadataEntryPosition,
            int offset)
        {
            var objectMetadataEntryPosition = metadataEntryPosition - offset;
                
#if SIZE_OPTIMIZED
             var objectMetadataPosition = objectMetadataEntryPosition - 2;            
                #else
            var objectMetadataPosition = objectMetadataEntryPosition - 4;
#endif

            var sourcePtr = &ptr[objectMetadataPosition];
            var propertyCount = ((PropertyIdType*) sourcePtr)[0];
                
            return new BraonObjectReader(ptr,propertyCount,objectMetadataEntryPosition);
        }
    }
}