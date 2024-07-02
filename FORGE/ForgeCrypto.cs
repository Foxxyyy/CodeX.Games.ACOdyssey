using CodeX.Core.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;

namespace CodeX.Games.ACOdyssey.FORGE
{
    public class ForgeCrypto
    {
        public static byte[] DecompressData(byte[] rawData)
        {
            using var stream = new MemoryStream(rawData);
            using var reader = new BinaryReader(stream);

            var identifierOffsets = LocateRawDataIdentifier(reader);
            if (identifierOffsets.Length < 2)
                return rawData;

            var x = identifierOffsets[1];
            var decompressed = rawData;

            //Skip to this Raw Data Block's offset
            reader.BaseStream.Seek(x, SeekOrigin.Begin);

            //Header Block
            var identifier = reader.ReadInt64();
            var version = reader.ReadInt16();
            var compression = (CompressionType)Enum.ToObject(typeof(CompressionType), reader.ReadByte());

            //Skip 4 bytes
            reader.BaseStream.Seek(4, SeekOrigin.Current);

            //Block indices
            var blockCount = reader.ReadInt32();
            var indices = new Block[blockCount];
            var chunks = new DataChunk[blockCount];

            for (int i = 0; i < blockCount; i++)
            {
                indices[i] = new Block
                {
                    UncompressedSize = reader.ReadInt32(),
                    CompressedSize = reader.ReadInt32()
                };
            }

            //Data chunks
            for (int i = 0; i < blockCount; i++)
            {
                chunks[i] = new DataChunk
                {
                    Checksum = reader.ReadInt32(),
                    Data = reader.ReadBytes(indices[i].CompressedSize)
                };

                //If the compressed and uncompressed size do not match, decompress data
                chunks[i].UncompressedData = (indices[i].CompressedSize == indices[i].UncompressedSize) ? chunks[i].Data : ForgeCompression.Decompress(chunks[i].Data, indices[i].UncompressedSize);
            }

            //Concatenate all uncompressed data chunks
            using var outputStream = new MemoryStream();
            foreach (var chunk in chunks)
            {
                outputStream.Write(chunk.UncompressedData, 0, chunk.UncompressedData.Length);
            }
            return outputStream.ToArray();
        }

        public static byte[] GetBytes(Vector2 v)
        {
            var x = BitConverter.GetBytes(v.X);
            var y = BitConverter.GetBytes(v.Y);
            return x.Concat(y).ToArray();
        }

        public static byte[] GetBytes(Vector3 v)
        {
            byte[] x = BitConverter.GetBytes(v.X);
            byte[] y = BitConverter.GetBytes(v.Y);
            byte[] z = BitConverter.GetBytes(v.Z);
            return x.Concat(y).Concat(z).ToArray();
        }

        public static long[] LocateRawDataIdentifier(BinaryReader reader)
        {
            var offsets = new List<long>();
            var originalPos = reader.BaseStream.Position;
            reader.BaseStream.Position = 0;

            while ((reader.BaseStream.Position <= reader.BaseStream.Length - 8) && offsets.Count < 2)
            {
                if (reader.ReadInt64() == 1154322941026740787)
                    offsets.Add(reader.BaseStream.Position - 8);
                else
                    reader.BaseStream.Position -= 0x7;
            }

            reader.BaseStream.Position = originalPos;
            return offsets.ToArray();
        }

        /// <summary>
        /// Returns the offsets of where find can be found. The long holds the original position of the BinaryReader.
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="find"></param>
        /// <returns></returns>
        public static Tuple<int[], long> LocateBytes(BinaryReader reader, byte[] find)
        {
            List<int> offs = new List<int>();
            long pos = reader.BaseStream.Position;
            reader.BaseStream.Seek(0, SeekOrigin.Begin);
            byte[] allData = reader.ReadBytes((int)reader.BaseStream.Length);
            foreach (Tuple<int, int> tuple in RecurringIndexes(allData, find, 4))
            {
                offs.Add(tuple.Item1);
            }
            return new Tuple<int[], long>(offs.ToArray(), pos);
        }

        /// <summary>
        /// Searches master for toFind with length precision. Returns a List of Tuples
        /// </summary>
        /// <param name="master"></param>
        /// <param name="toFind"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static IList<Tuple<int, int>> RecurringIndexes(byte[] master, byte[] toFind, int length)
        {
            List<Tuple<int, int>> result = new List<Tuple<int, int>>();

            // Let's return empty list ... or throw appropriate exception
            if (ReferenceEquals(null, master))
                return result;
            else if (ReferenceEquals(null, toFind))
                return result;
            else if (length < 0)
                return result;
            else if (length > toFind.Length)
                return result;

            byte[] subRegion = new byte[length];

            for (int i = 0; i <= toFind.Length - length; ++i)
            {
                for (int j = 0; j < length; ++j)
                    subRegion[j] = toFind[j + i];

                for (int j = 0; j < master.Length - length + 1; ++j)
                {
                    bool counterExample = false;

                    for (int k = 0; k < length; ++k)
                        if (master[j + k] != subRegion[k])
                        {
                            counterExample = true;
                            break;
                        }

                    if (counterExample)
                        continue;

                    result.Add(new Tuple<int, int>(j, j + length - 1));
                }
            }
            return result;
        }
    }

    public class ForgeCompression
    {
        public static bool Init(string folder)
        {
            var dllname = "oo2core_4_win64.dll";
            var dir = StartupUtil.GetFolder();
            var file = Path.Combine(dir, dllname);

            if (File.Exists(file))
            {
                return true;
            }

            var dll = Path.Combine(folder, dllname);
            if (File.Exists(dll) && Directory.Exists(dir))
            {
                File.Copy(dll, file, true);
                if (File.Exists(file))
                {
                    return true;
                }
            }
            return false;
        }

        //from: https://github.com/Tamely/Oodle-Tools/blob/main/OodleTools/Oodle.cs
        /// <summary>
        /// Compresses a byte[] using Oodle.
        /// </summary>
        /// <param name="data">byte[]: The decompressed data you want to compress</param>
        /// <returns>byte[]: The compressed data</returns>
        public static byte[] Compress(byte[] data, int level = 9)
        {
            var maxSize = GetCompressedBounds((uint)data.Length);
            var compressedData = new byte[maxSize];

            var compressedSize = Compress(data, (uint)data.Length, ref compressedData, maxSize, OodleFormat.Kraken, (OodleCompressionLevel)level);

            byte[] result = new byte[compressedSize];
            Buffer.BlockCopy(compressedData, 0, result, 0, (int)compressedSize);

            return result;
        }

        /// <summary>
        /// Decompresses a byte[] using Oodle.
        /// </summary>
        /// <param name="data">byte[]: The compressed data</param>
        /// <param name="decompressedSize">int: The expected size of the decompressed data</param>
        /// <returns>byte[]: The decompressed data</returns>
        /// <exception cref="Exception">Gets thrown when "decompressedSize" doesn't match with what Oodle returns</exception>
        public static byte[] Decompress(byte[] data, int decompressedSize)
        {
            byte[] decompressedData = new byte[decompressedSize];
            var verificationSize = Decompress(data, (uint)data.Length, ref decompressedData, (uint)decompressedSize);

            if (verificationSize != decompressedSize)
            {
                throw new Exception("Decompression failed. Verification size does not match given size.");
            }
            return decompressedData;
        }

        private static uint Compress(byte[] buffer, uint bufferSize, ref byte[] OutputBuffer, uint OutputBufferSize, OodleFormat format, OodleCompressionLevel level)
        {
            if (buffer.Length > 0 && bufferSize > 0 && OutputBuffer.Length > 0 && OutputBufferSize > 0)
            {
                return (uint)OodleLZ_Compress(format, buffer, bufferSize, OutputBuffer, level, 0, 0, 0);
            }
            return 0;
        }

        private static uint Decompress(byte[] buffer, uint bufferSize, ref byte[] outputBuffer, uint outputBufferSize)
        {
            if (buffer.Length > 0 && bufferSize > 0 && outputBuffer.Length > 0 && outputBufferSize > 0)
            {
                return (uint)OodleLZ_Decompress(buffer, bufferSize, outputBuffer, outputBufferSize, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
            }
            return 0;
        }

        private enum OodleFormat : uint
        {
            LZH = 0,
            LZHLW = 1,
            LZNIB = 2,
            None = 3,
            LZB16 = 4,
            LZBLW = 5,
            LZA = 6,
            LZNA = 7,
            Kraken = 8,
            Mermaid = 9,
            BitKnit = 10,
            Selkie = 11,
            Hydra = 12,
            Leviathan = 13
        }

        private enum OodleCompressionLevel : uint
        {
            None = 0,
            SuperFast = 1,
            VeryFast = 2,
            Fast = 3,
            Normal = 4,
            Optimal1 = 5,
            Optimal2 = 6,
            Optimal3 = 7,
            Optimal4 = 8,
            Optimal5 = 9
        }

        private static uint GetCompressedBounds(uint BufferSize)
        {
            return BufferSize + 274 * ((BufferSize + 0x3FFFF) / 0x400000);
        }

        /// <summary>
        /// Compress data using Oodle
        /// </summary>
        /// <param name="Format">OodleFormat: The compression format used.</param>
        /// <param name="Buffer">byte[]: The decompressed data.</param>
        /// <param name="BufferSize">long: The size of the decompressed data.</param>
        /// <param name="OutputBuffer">ref byte[]: Where the compressed data will output to.</param>
        /// <param name="Level">OodleCompressionLevel: The compression level used.</param>
        /// <param name="a">uint: unused</param>
        /// <param name="b">uint: unused</param>
        /// <param name="c">uint: unused</param>
        /// <returns>int: The length of the compressed data.</returns>
        [DllImport("oo2core_4_win64.dll")]
        private static extern int OodleLZ_Compress(OodleFormat Format, byte[] Buffer, long BufferSize, byte[] OutputBuffer, OodleCompressionLevel Level, uint a, uint b, uint c);


        /// <summary>
        /// Decompress data using Oodle
        /// </summary>
        /// <param name="Buffer">byte[]: The compressed data.</param>
        /// <param name="BufferSize">long: The size of the compressed data.</param>
        /// <param name="OutputBuffer">ref byte[]: Where the decompressed data will output to.</param>
        /// <param name="OutputBufferSize">long: The size of the decompressed data.</param>
        /// <param name="a">uint: unused</param>
        /// <param name="b">uint: unused</param>
        /// <param name="c">uint: unused</param>
        /// <param name="d">uint: unused</param>
        /// <param name="e">uint: unused</param>
        /// <param name="f">uint: unused</param>
        /// <param name="g">uint: unused</param>
        /// <param name="h">uint: unused</param>
        /// <param name="i">uint: unused</param>
        /// <param name="ThreadModule">int: not really used, pass nullptr/void* in cpp or just 0 in C#</param>
        /// <returns>int: The length of the decompressed data.</returns>
        [DllImport("oo2core_4_win64.dll")]
        internal static extern int OodleLZ_Decompress(byte[] Buffer, long BufferSize, byte[] OutputBuffer, long OutputBufferSize, uint a, uint b, uint c, uint d, uint e, uint f, uint g, uint h, uint i, int ThreadModule);
    }

    public struct Block
    {
        public int UncompressedSize { get; internal set; }
        public int CompressedSize { get; internal set; }
    }

    public struct DataChunk
    {
        public int Checksum { get; internal set; }
        public byte[] Data { get; internal set; }
        public byte[] UncompressedData { get; internal set; }
    }

    public enum CompressionType
    {
        LZO1X = 0x0,
        LZO1X_ = 0x1,
        LZO2A = 0x2,
        OODLE = 0x4,
        LZO1C = 0x5,
        OODLE_ = 0x7,
        OODLE__ = 0x8
    }
}