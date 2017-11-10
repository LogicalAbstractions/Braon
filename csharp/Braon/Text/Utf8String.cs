using System;
using System.Text;

namespace Braon.Text
{
    public unsafe struct Utf8String : IEquatable<Utf8String>
    {
        private static readonly Encoding encoding = Encoding.UTF8;
        
        private byte* pointer;
        private int length;
        private int charLength;
        private string value;
        
        public Utf8String(byte * pointer,  int length,string value = null)
        {
            this.pointer = pointer;
            this.length = length;
            this.charLength = value?.Length ?? -1;
            this.value = value;
        }
        
        public static Utf8String Empty => new Utf8String(null,0);

        internal int ByteLength => length;

        internal byte* Pointer => pointer;

        public int Length
        {
            get
            {
                if (charLength < 0)
                {
                    charLength = encoding.GetCharCount(pointer, length);
                }

                return charLength;
            }
        }
        
        public bool Equals(Utf8String other)
        {
            if (length == other.length)
            {
                if (pointer == other.pointer)
                {
                    return true;
                }

                for (var i = 0; i < length; ++i)
                {
                    if (pointer[i] != other.pointer[i])
                    {
                        return false;
                    }
                }

                return true;
            }

            return false;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is Utf8String && Equals((Utf8String) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                
                
                return ((int)pointer * 397) ^ length;
            }
        }

        public override string ToString()
        {
            if (value == null)
            {
                value = encoding.GetString(pointer, length);
            }

            return value;
        }

        public static bool operator ==(Utf8String left, Utf8String right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Utf8String left, Utf8String right)
        {
            return !left.Equals(right);
        }     
    }
}