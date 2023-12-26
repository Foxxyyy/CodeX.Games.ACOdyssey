using CodeX.Core.Utilities;
using System.IO;
using System.Numerics;

namespace CodeX.Games.ACOdyssey.FORGE
{
    public class ForgeDataReader : DataReader
    {
        public ForgeDataReader(Stream input, DataEndianess endianess = DataEndianess.LittleEndian) : base(input, endianess)
        {

        }

        public Vector2 ReadVector2AsInt16()
        {
            var v = new Vector2
            {
                X = ReadInt16(),
                Y = ReadInt16()
            };
            return v;
        }

        public Vector3 ReadVector3AsInt16()
        {
            var v = new Vector3
            {
                X = ReadInt16(),
                Y = ReadInt16(),
                Z = ReadInt16()
            };
            return v;
        }

        public Vector4 ReadVector4AsByte()
        {
            var v = new Vector4
            {
                X = ReadByte(),
                Y = ReadByte(),
                Z = ReadByte(),
                W = ReadByte()
            };
            return v;
        }
    }
}