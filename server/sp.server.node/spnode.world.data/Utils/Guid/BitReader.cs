using System;
using System.IO;

namespace spnode.world.data.Utils.Guid
{
    public class BitReader
    {
        private readonly BinaryReader byteReader;
        private readonly byte[] byteBuffer = new byte[1];

        private int bitBuffer;
        private int bitBufferLength;
        
        public BitReader(BinaryReader byteReader)
        {
            this.byteReader = byteReader;
        }

        public int Read(int count)
        {
            var output = 0;
            var bitsToRead = count;

            while (bitsToRead > 0)
            {
                if (this.bitBufferLength == 0)
                {
                    var bytesRead = this.byteReader.Read(this.byteBuffer, 0, 1);
                    if (bytesRead == 0) throw new EndOfStreamException();

                    this.bitBuffer = this.byteBuffer[0];
                    this.bitBufferLength = 8;
                }

                var chunkSize = Math.Min(this.bitBufferLength, bitsToRead);
                this.bitBufferLength -= chunkSize;

                var chunk = this.bitBuffer >> this.bitBufferLength;
                this.bitBuffer &= this.BitMask(this.bitBufferLength);
                
                chunk <<= (bitsToRead - chunkSize);
                output |= chunk;
                
                bitsToRead -= chunkSize;                
            }

            return output;
        }


        private int BitMask(int count)
            => ~(0xFFFFFFF << count);
    }
}