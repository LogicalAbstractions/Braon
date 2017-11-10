using System.Collections.Generic;
using Braon.Text;

namespace Braon
{
#if SIZE_OPTIMIZED
    using PropertyIdType = System.Int16;
#else
    using PropertyIdType = System.Int32;
#endif
    
    public unsafe class BraonPropertyNameWriter
    {
        private readonly List<Utf8String> propertyIds;
        private readonly Dictionary<Utf8String, PropertyIdType> propertyNames;

        public BraonPropertyNameWriter()
            : this(new List<Utf8String>())
        {
        }

        public BraonPropertyNameWriter(List<Utf8String> propertyIds)
        {
            this.propertyIds = propertyIds;
        }
        
        public bool TryGetPropertyId(ref Utf8String name, out PropertyIdType propertyId)
        {
            
        }
    }
}