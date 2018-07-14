using System.IO;

namespace spnode.world.data.Utils.Guid
{
    public class BitWriter
    {
        private readonly BinaryWriter byteWriter;
        private int bitBuffer;
        private int bitBufferSize;

        public BitWriter(BinaryWriter byteWriter)
        {
            this.byteWriter = byteWriter;
        }

        public void Write(int bits, int bitCount)   //bitCount can't be more than 16!
        {
            this.bitBuffer <<= bitCount;
            this.bitBuffer |= (bits & this.BitMask(bitCount));
            this.bitBufferSize += bitCount;

            if (this.bitBufferSize < 8) 
                return;

            var nextByte = (byte)(this.bitBuffer >> (this.bitBufferSize - 8));
            this.byteWriter.Write(nextByte);

            this.bitBufferSize -= 8;
            this.bitBuffer &= this.BitMask(this.bitBufferSize);
        }
        
        private int BitMask(int count)
            => ~(0xFFFFFFF << count);
    }
}