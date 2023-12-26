using CodeX.Core.Engine;
using CodeX.Core.Utilities;
using CodeX.Games.ACOdyssey.FORGE;
using SharpDX.Direct3D11;
using System;

namespace CodeX.Games.ACOdyssey.Resources
{
    public class ForgeTexture : Texture
    {
        public ForgeBaseObjectPtr? BaseObjectPtrTexture { get; set; }
        public uint ArraySize { get; set; }
        public ForgeTextureFormat FTextureFormat { get; set; }
        public GammaSettings Gamma { get; set; }
        public ForgeTextureMapType MapType { get; set; }
        public bool Dynamic { get; set; }
        public bool Writeable { get; set; }
        public bool IgnoreSkipMips { get; set; }
        public bool Nudity { get; set; }
        public bool AllowHWGenerateMips { get; set; }
        public bool DisableProxy { get; set; }
        public uint UserCategory { get; set; }
        public int TextureDataSize { get; set; }

        public ForgeTexture()
        {

        }

        public void Read(DataReader reader, bool skipHeader = false)
        {
            if (!skipHeader)
            {
                var header = new ForgeDataHeader()
                {
                    ResourceIdentifier = (ForgeResourceType)reader.ReadUInt32(),
                    FileSize = reader.ReadInt32(),
                    FileNameSize = reader.ReadInt32()
                };
                header.FileName = Name = new(reader.ReadChars(header.FileNameSize));

                if (header.ResourceIdentifier != ForgeResourceType.TEXTURE_MAP)
                {
                    throw new Exception("Not a proper texture map data.");
                }
            }

            BaseObjectPtrTexture = new ForgeBaseObjectPtr();
            BaseObjectPtrTexture.Read(reader);

            Width = (ushort)reader.ReadInt32();
            Height = (ushort)reader.ReadInt32();
            Depth = (ushort)reader.ReadInt32();
            ArraySize = reader.ReadUInt32();
            Format = ConvertToFormat((DXT)reader.ReadInt32());
            FTextureFormat = (ForgeTextureFormat)reader.ReadInt32();
            Gamma = (GammaSettings)reader.ReadInt32();
            MipLevels = (byte)reader.ReadInt32();
            MapType = (ForgeTextureMapType)reader.ReadInt32();
            Dynamic = reader.ReadBoolean();
            Writeable = reader.ReadBoolean();
            IgnoreSkipMips = reader.ReadBoolean();
            Nudity = reader.ReadBoolean();
            AllowHWGenerateMips = reader.ReadBoolean();
            DisableProxy = reader.ReadBoolean();
            UserCategory = reader.ReadUInt32();

            //CompiledTopMips
            reader.BaseStream.Position += 0x7;
            long[] fileIDs = new long[2]; //BaseObject<CompiledTopMip>[2]

            for (int i = 0; i < fileIDs.Length; i++)
            {
                uint identifier = reader.ReadUInt32();
                byte padding = reader.ReadByte();
                fileIDs[i] = reader.ReadInt64(); //Handle<CompiledMip>
                long unkValue = reader.ReadInt64();
            }

            //CompiledTextureMap
            reader.BaseStream.Position += 0x6;
            var ct = new ForgeCompiledTexture();
            ct.ReadCompiledTextureMap(reader);

            //We don't have any CompiledMip texture asociated, load internal CompiledTexture data
            if (fileIDs[0] == 0 || skipHeader)
            {
                TextureDataSize = reader.ReadInt32();
                Data = reader.ReadBytes(TextureDataSize);
                Sampler = TextureSampler.Create(TextureSamplerFilter.Anisotropic, TextureAddressMode.Wrap);
            }
            else
            {
                Width = ct.Width;
                Height = ct.Height;
                MipLevels = ct.MipLevels;
                Format = ct.Format;
            }
        }

        public void ReadMipData(DataReader reader) //CompiledMip
        {
            var header = new ForgeDataHeader()
            {
                ResourceIdentifier = (ForgeResourceType)reader.ReadUInt32(),
                FileSize = reader.ReadInt32(),
                FileNameSize = reader.ReadInt32()
            };
            header.FileName = new(reader.ReadChars(header.FileNameSize));

            reader.BaseStream.Position += 0x12;
            Data = reader.ReadBytes(header.FileSize - 0x12);
            Sampler = TextureSampler.Create(TextureSamplerFilter.Anisotropic, TextureAddressMode.Wrap);
            MipLevels = 1;
        }

        public TextureFormat ConvertToFormat(DXT format)
        {
            switch (format)
            {
                case DXT.DXT1:
                case DXT.DXT1A:
                    return TextureFormat.BC1SRGB;
                case DXT.DXT3:
                    return TextureFormat.BC2;
                case DXT.DXT5:
                    return TextureFormat.BC3;
                case DXT.BC6:
                    return TextureFormat.BC6H;
                case DXT.BC7:
                    return TextureFormat.BC7;
                case DXT.RGBA8888:
                    return TextureFormat.A8R8G8B8;
                case DXT.I8:
                    return TextureFormat.L8;
                default:
                    return TextureFormat.BC7;
            }
        }
    }

    public class ForgeCompiledTexture : ForgeTexture
    {
        public uint PlatformVersion { get; set; }
        public uint SDKVersion { get; set; }
        public uint[] TopMipsSizes { get; set; } = new uint[2];
        public uint TotalTextureSize { get; set; }
        public uint Alignment { get; set; }
        public bool SkipDirectGPUMemoryLoad { get; set; }

        public ForgeCompiledTexture()
        {

        }

        public void ReadCompiledTextureMap(DataReader reader)
        {
            PlatformVersion = reader.ReadUInt32();
            SDKVersion = reader.ReadUInt32();
            Width = (ushort)reader.ReadInt32();
            Height = (ushort)reader.ReadInt32();
            Depth = (ushort)reader.ReadInt32();
            ArraySize = reader.ReadUInt32();
            MipLevels = (byte)reader.ReadInt32();
            Format = ConvertToFormat((DXT)reader.ReadInt32());
            FTextureFormat = (ForgeTextureFormat)reader.ReadInt32();
            Gamma = (GammaSettings)reader.ReadInt32();
            TopMipsSizes[0] = reader.ReadUInt32();
            TopMipsSizes[1] = reader.ReadUInt32();
            TotalTextureSize = reader.ReadUInt32();
            Alignment = reader.ReadUInt32();
            UserCategory = reader.ReadUInt32();
            SkipDirectGPUMemoryLoad = reader.ReadBoolean();
        }
    }

    public enum DXT
    {
        RGBA8888,
        RGBA8888Signed,
        DXT1,
        DXT1A,
        DXT3,
        DXT5,
        DXN,
        BC6,
        BC7,
        A8,
        I8,
        I16,
        A8I8,
        R32F,
        RGBA32F,
        RGBA16F,
        RG1616,
        RG1616F,
        AutoSelect
    }

    public enum ForgeTextureMapType
    {
        MapDiffuse,
        MapNormal,
        MapSpecular,
        MapSpecularPower,
        MapOffsetBump,
        MapEmissive,
        MapTransmission,
        MapOcclusion,
        MapMask1,
        MapMask2,
        MapGeneric,
        RVBmask,
        MapMaxNbTypes,
    }

    public enum ForgeTextureFormat
    {
        Texture1D,
        Texture2D,
        TextureCubeMap,
        Texture3D,
        NbTextureFormats,
    }

    public enum GammaSettings
    {
        Gamma_Linear,
        Gamma_sRGB,
    }
}