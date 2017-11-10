namespace Braon
{
    public unsafe struct BraonBlobData
    {
        private byte* pointer;
        private int size;

        public BraonBlobData(byte* pointer, int size)
        {
            this.pointer = pointer;
            this.size = size;
        }

        public byte* Pointer => pointer;

        public int Size => size;
    }
}