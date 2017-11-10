using System.Runtime.InteropServices.ComTypes;
using Braon.Format;
using Braon.Text;

namespace Braon
{
#if SIZE_OPTIMIZED
    using PropertyIdType = System.Int16;
#else
    using PropertyIdType = System.Int32;
#endif
    
    public unsafe interface IPropertyNamesReader
    {
        void Initialize(byte * pointer, int propertyNamesPosition, int propertyNamesCount);
           
        bool TryGetPropertyId(Utf8String name, out PropertyIdType id);

        bool TryGetPropertyId(Utf8String name,
            BraonObjectReader.PropertyIdEnumerator candidatePropertyIdEnumerator,
            out PropertyIdType id);
          
        bool TryGetPropertyName(PropertyIdType id, out Utf8String name);
    }
}