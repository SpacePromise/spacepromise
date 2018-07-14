using System;
using System.ComponentModel;
using System.IO;
using System.Linq;

namespace spnode.world.data.Utils.Guid
{
    [TypeConverter(typeof(MiniGuidTypeConverter))]
    public struct MiniGuid : IEquatable<MiniGuid>
    {
        public MiniGuid(System.Guid guid) {
            this.Guid = guid;
        }
        
        public System.Guid Guid { get; }

        public static MiniGuid NewGuid()
            => System.Guid.NewGuid();
        

        #region Conversion

        public static implicit operator MiniGuid(System.Guid guid)
            => new MiniGuid(guid);

        public static implicit operator System.Guid(MiniGuid miniGuid)
            => miniGuid.Guid;

        public static implicit operator MiniGuid(string @string)
            => Parse(@string);

        public static implicit operator string(MiniGuid miniGuid)
            => miniGuid.ToString();

        #endregion


        #region Equality/Comparison

        public override bool Equals(object obj)
        {
            switch(obj)
            {
                case MiniGuid miniGuid: return this.Equals(miniGuid);
                case System.Guid guid: return this.Guid.Equals(guid);
                case string @string: return @string.Equals(this.ToString());
                default: return false;
            }            
        }

        public bool Equals(MiniGuid other)
            => this.Guid.Equals(other.Guid);

        public override int GetHashCode()
            => this.Guid.GetHashCode() + 1;

        #endregion


        #region Stringifying/Parsing

        private static readonly (char, char?)[] Bin2Char;
        private static readonly int?[] Char2Bin;
        
        static MiniGuid()
        {
            Bin2Char = "abcdefghijklmnopqrstuvwxyzABCDEF".ToCharArray()
                .Zip("GHIJKLMNOPQRSTUVWXYZ".PadRight(32).ToCharArray(),
                    (c1, c2) => (c1, c2 != ' ' ? (char?)c2 : null))
                .ToArray();

            Char2Bin = new int?[256];

            for(int i = 0; i < Bin2Char.Length; i++)
            {
                var (c1, c2) = Bin2Char[i];

                Char2Bin[c1] = i;
                if (c2.HasValue) Char2Bin[c2.Value] = i;
            }
        }
        

        public override string ToString()
        {
            var guidBytes = this.Guid.ToByteArray();
            var guidBitReader = new BitReader(new BinaryReader(new MemoryStream(guidBytes)));

            var chars = new char[26];
            int acc = 0;

            foreach (var b in guidBytes) IncrementAcc(b);

            for (int i = 0; i < 25; i++)
            {
                int chunk = ReadChunk(5);
                chars[i] = ChooseChar(Bin2Char[chunk]);
            }

            {
                int chunk = ReadChunk(3);
                chunk |= (acc & 0x18);  //extends to full 5-bit range, to reach all chars
                chars[25] = ChooseChar(Bin2Char[chunk]);
            }
                        
            return new string(chars);


            int ReadChunk(int bitCount)
            {
                int chunk = guidBitReader.Read(bitCount);
                IncrementAcc(chunk);
                return chunk;
            }

            void IncrementAcc(int v)
            {
                acc = (acc + v) & 0xF;
            }

            char ChooseChar((char, char?) tup)
            {
                var (c1, c2) = tup;
                return c2.HasValue && (acc & 1) == 1
                    ? c2.Value
                    : c1;
            }
        }


        public static bool TryParse(string input, out MiniGuid guid)
        {
            if (input.Length != 26) return false;

            var guidBytes = new byte[16];
            var bitWriter = new BitWriter(new BinaryWriter(new MemoryStream(guidBytes)));

            for(int i = 0; i < input.Length - 1; i++)
            {
                var c = input[i];
                var chunk = Char2Bin[c];
                if (!chunk.HasValue) return false;

                bitWriter.Write(chunk.Value, 5);
            }

            {
                var c = input[25];
                var chunk = Char2Bin[c];
                if (!chunk.HasValue) return false;

                bitWriter.Write(chunk.Value, 3);
            }                                       
            
            guid = new MiniGuid(new System.Guid(guidBytes));

            return true;
        }

        public static MiniGuid Parse(string input)
        {
            if (TryParse(input, out var miniGuid)) return miniGuid;
            else throw new InvalidOperationException();
        }

        #endregion
    }
}