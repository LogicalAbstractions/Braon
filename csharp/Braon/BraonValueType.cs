namespace Braon
{
    public enum BraonValueType 
#if SIZE_OPTIMIZED
   : byte
#else
   : int
#endif
    {
        Undefined = 0x00,
        Null      = 0x01,
        
        // Integer types
        Byte      = 0x02,
        Short     = 0x03,
        Int       = 0x04,
        Long      = 0x05,
        
        // Float types
        Float     = 0x06,
        Double    = 0x07,

        // String types
        String    = 0x08,
        
        // Additional types
        Boolean   = 0x09,
        Blob      = 0x10,
        
        // Complex types
        Array     = 0x11,
        Object    = 0x12,
        Document  = 0x13
    }
}