using BepuUtilities.Memory;
using CodeX.Core.Engine;
using CodeX.Core.Utilities;
using CodeX.Games.ACOdyssey.FORGE;
using System.ComponentModel.DataAnnotations;
using System.Numerics;
using System.Reflection.Metadata;
using System.Reflection;
using CodeX.Core.Numerics;
using System;
using System.Collections.Generic;

namespace CodeX.Games.ACOdyssey.Resources
{
    public class ForgeMaterial
    {
        public ForgeFileReference MaterialTemplate { get; set; }
        public ForgeFileReference TextureSet { get; set; }
        public Mask Mask { get; set; }
        public int BlendMode { get; set; }
        public int AlphaDiplayMode { get; set; }
        public byte AlphaTestValue { get; set; }
        public bool ZWriteDisabledOpaque { get; set; }
        public bool ZWriteDisabledAlpha { get; set; }
        public bool ZTestDisabled { get; set; }
        public bool LayerMaterial { get; set; }
        public bool LayerOverrideAlbedo { get; set; }
        public bool LayerOverrideNormal { get; set; }
        public bool LayerOverrideMetalness { get; set; }
        public bool LayerOverrideGloss { get; set; }
        public bool LayerOverrideWetness { get; set; }
        public bool LayerOverrideEmissive { get; set; }
        public bool PreTransparent { get; set; }
        public bool FlipNormalOnBackFace { get; set; }
        public bool ShadowCasterOpaque { get; set; }
        public bool ShadowCasterAlpha { get; set; }
        public bool IsAtlasMaterial { get; set; }
        public bool IsObjectAtlasMaterial { get; set; }
        public bool SupportRuntimeDisable { get; set; }
        public bool CanBeOccluder { get; set; }
        public bool ZPrePassEnabled { get; set; }
        public bool RenderInLowResolution { get; set; }
        public bool IsWaterMaterial { get; set; }
        public bool IsFakeWaterMaterial { get; set; }
        public bool IsWaterMaterialDetailed { get; set; }
        public bool Wireframe { get; set; }
        public bool HasTextureMapSelector { get; set; }
        public bool AlphaTestEnabled { get; set; }
        public bool TwoSided { get; set; }
        public bool MaterialDisabled { get; set; }
        public bool ExcludeWaterClipMaterial { get; set; }
        public bool IgnoreWaterDepthTest { get; set; }
        public bool WaterClipMaterial { get; set; }
        public bool ForceRenderInFakeBucket { get; set; }
        public bool SSAOOnDiffuse { get; set; }
        public ForgeFileReference BackFaceMaterial { get; set; }
        public float TangentShift1 { get; set; }
        public float TangentShift2 { get; set; }
        public float DiffuseDarkening { get; set; }
        public float SpecularReflectance1 { get; set; }
        public float SpecularReflectance2 { get; set; }
        public Mask16b MaterialMatchMask { get; set; }
        public byte TAADitherFactor { get; set; }
        public int NumProperties { get; set; }
        public DynamicProperty[] DynamicProperties { get; set; }

        public ForgeMaterial()
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
                header.FileName = new(reader.ReadChars(header.FileNameSize));
            }

            reader.BaseStream.Position += 0xE;
            MaterialTemplate = new ForgeFileReference();
            MaterialTemplate.Read(reader);
            
            TextureSet = new ForgeFileReference();
            TextureSet.Read(reader);

            Mask = new Mask();
            Mask.Read(reader);

            BlendMode = reader.ReadInt32();
            AlphaDiplayMode = reader.ReadInt32();
            AlphaTestValue = reader.ReadByte();
            ZWriteDisabledOpaque = reader.ReadBoolean();
            ZWriteDisabledAlpha = reader.ReadBoolean();
            ZTestDisabled = reader.ReadBoolean();
            LayerMaterial = reader.ReadBoolean();
            LayerOverrideAlbedo = reader.ReadBoolean();
            LayerOverrideNormal = reader.ReadBoolean();
            LayerOverrideMetalness = reader.ReadBoolean();
            LayerOverrideGloss = reader.ReadBoolean();
            LayerOverrideWetness = reader.ReadBoolean();
            LayerOverrideEmissive = reader.ReadBoolean();
            PreTransparent = reader.ReadBoolean();
            FlipNormalOnBackFace = reader.ReadBoolean();
            ShadowCasterOpaque = reader.ReadBoolean();
            ShadowCasterAlpha = reader.ReadBoolean();
            IsAtlasMaterial = reader.ReadBoolean();
            IsObjectAtlasMaterial = reader.ReadBoolean();
            SupportRuntimeDisable = reader.ReadBoolean();
            CanBeOccluder = reader.ReadBoolean();
            ZPrePassEnabled = reader.ReadBoolean();
            RenderInLowResolution = reader.ReadBoolean();
            IsWaterMaterial = reader.ReadBoolean();
            IsFakeWaterMaterial = reader.ReadBoolean();
            IsWaterMaterialDetailed = reader.ReadBoolean();
            Wireframe = reader.ReadBoolean();
            HasTextureMapSelector = reader.ReadBoolean();
            AlphaTestEnabled = reader.ReadBoolean();
            TwoSided = reader.ReadBoolean();
            MaterialDisabled = reader.ReadBoolean();
            ExcludeWaterClipMaterial = reader.ReadBoolean();
            IgnoreWaterDepthTest = reader.ReadBoolean();
            WaterClipMaterial = reader.ReadBoolean();
            ForceRenderInFakeBucket = reader.ReadBoolean();
            SSAOOnDiffuse = reader.ReadBoolean();
            BackFaceMaterial = new ForgeFileReference();
            BackFaceMaterial.Read(reader);
            TangentShift1 = reader.ReadSingle();
            TangentShift2 = reader.ReadSingle();
            DiffuseDarkening = reader.ReadSingle();
            SpecularReflectance1 = reader.ReadSingle();
            SpecularReflectance2 = reader.ReadSingle();

            MaterialMatchMask = new Mask16b();
            MaterialMatchMask.Read(reader);

            TAADitherFactor = reader.ReadByte();

            NumProperties = reader.ReadInt32();
            DynamicProperties = new DynamicProperty[NumProperties];
            for (int i = 0; i < DynamicProperties.Length; i++)
            {
                DynamicProperties[i] = new DynamicProperty();
                DynamicProperties[i].Read(reader);
            }
        }
    }

    public class DynamicProperty
    {
        public uint Hash { get; set; }
        public uint DataType { get; set; }
        public uint Type { get; set; }
        public uint Unknown1 { get; set; }
        public Type PropertyType { get; set; }
        public byte Num { get; set; }
        public long ClassID { get; set; }
        public uint DataType2 { get; set; }
        public bool IsManaged { get; set; }
        public object Property { get; set; }

        public DynamicProperty()
        {
        }

        public void Read(DataReader reader)
        {
            Hash = reader.ReadUInt32();
            DataType = reader.ReadUInt32();
            Type = reader.ReadUInt32();
            Unknown1 = reader.ReadUInt32();

            PropertyType = PropertyRegistry.GetType(Type, DataType);
            if (PropertyType == null)
            {
                return;
            }

            if (PropertyType.Equals(typeof(ForgeObject)))
            {
                Num = reader.ReadByte();
                ClassID = reader.ReadInt64();
                DataType2 = reader.ReadUInt32();

                if (DataType != DataType2)
                {
                    IsManaged = reader.ReadBoolean();
                }

                Property = new TextureSelector();
                ((TextureSelector)Property).Read(reader);
            }
            else if (PropertyType.Equals(typeof(float)))
                Property = reader.ReadSingle();
            else if (PropertyType.Equals(typeof(Vector4)))
                Property = reader.ReadVector4();
            else if (PropertyType.Equals(typeof(uint)))
                Property = reader.ReadUInt32();
            else if (PropertyType.Equals(typeof(int)))
                Property = reader.ReadInt32();
            else
                throw new Exception("Unknown DynamicProperty");
        }
    }

    public class TextureSelector
    {
        public uint TextureSpecificationMethod { get; set; }
        public ForgeTextureMapType MapType { get; set; }
        public uint FrameNumber { get; set; }
        public ForgeFileReference TextureSet { get; set; }
        public ForgeFileReference Texture { get; set; }

        public TextureSelector()
        {
        }

        public void Read(DataReader reader)
        {
            TextureSpecificationMethod = reader.ReadUInt32();
            MapType = (ForgeTextureMapType)reader.ReadUInt32();
            FrameNumber = reader.ReadUInt32();
            
            TextureSet = new ForgeFileReference();
            TextureSet.Read(reader);

            Texture = new ForgeFileReference();
            Texture.Read(reader);
        }
    }

    public class Mask
    {
        public ForgeBaseObject BaseObjectGroupMask { get; set; }
        public byte Bit1 { get; set; }
        public byte Bit2 { get; set; }
        public byte Bit3 { get; set; }
        public byte Bit4 { get; set; }
        public byte Bit5 { get; set; }
        public byte Bit6 { get; set; }
        public byte Bit7 { get; set; }
        public byte Bit8 { get; set; }

        public Mask()
        {
        }

        public void Read(DataReader reader)
        {
            BaseObjectGroupMask = new ForgeBaseObject();
            BaseObjectGroupMask.Read(reader);

            Bit1 = reader.ReadByte();
            Bit2 = reader.ReadByte();
            Bit3 = reader.ReadByte();
            Bit4 = reader.ReadByte();
            Bit5 = reader.ReadByte();
            Bit6 = reader.ReadByte();
            Bit7 = reader.ReadByte();
            Bit8 = reader.ReadByte();
        }
    }

    public class Mask16b
    {
        public ForgeBaseObject BaseObjectMask16b { get; set; }
        public byte Mask1 { get; set; }
        public byte Mask2 { get; set; }
        public byte Mask3 { get; set; }
        public byte Mask4 { get; set; }
        public byte Mask5 { get; set; }
        public byte Mask6 { get; set; }
        public byte Mask7 { get; set; }
        public byte Mask8 { get; set; }
        public byte Mask9 { get; set; }
        public byte Mask10 { get; set; }
        public byte Mask11 { get; set; }
        public byte Mask12 { get; set; }
        public byte Mask13 { get; set; }
        public byte Mask14 { get; set; }
        public byte Mask15 { get; set; }
        public byte Mask16 { get; set; }

        public Mask16b()
        {
        }

        public void Read(DataReader reader)
        {
            BaseObjectMask16b = new ForgeBaseObject();
            BaseObjectMask16b.Read(reader);

            Mask1 = reader.ReadByte();
            Mask2 = reader.ReadByte();
            Mask3 = reader.ReadByte();
            Mask4 = reader.ReadByte();
            Mask5 = reader.ReadByte();
            Mask6 = reader.ReadByte();
            Mask7 = reader.ReadByte();
            Mask8 = reader.ReadByte();
            Mask9 = reader.ReadByte();
            Mask10 = reader.ReadByte();
            Mask11 = reader.ReadByte();
            Mask12 = reader.ReadByte();
            Mask13 = reader.ReadByte();
            Mask14 = reader.ReadByte();
            Mask15 = reader.ReadByte();
            Mask16 = reader.ReadByte();
        }
    }

    public static class PropertyRegistry
    {
        public static Dictionary<uint, Type> ParameterTypes { get; } = new Dictionary<uint, Type>
        {
            { 0U, typeof(bool) },
            { 65536U, typeof(char) },
            { 131072U, typeof(sbyte) },
            { 196608U, typeof(byte) },
            { 262144U, typeof(short) },
            { 327680U, typeof(ushort) },
            { 393216U, typeof(int) },
            { 458752U, typeof(uint) },
            { 524288U, typeof(long) },
            { 589824U, typeof(ulong) },
            { 655360U, typeof(float) },
            { 720896U, typeof(Vector2) },
            { 786432U, typeof(Vector3) },
            { 851968U, typeof(Vector4) },
            { 917504U, typeof(Quaternion) },
            { 983040U, typeof(Matrix3x3) },
            { 1048576U, typeof(Matrix4x4) },
            { 1114112U, typeof(ulong) }, //ObjectID
            { 1179648U, typeof(ForgeObjectPtr) }, //Handle
            { 1245184U, typeof(ForgeObject) },
            { 1310720U, typeof(ForgeObjectPtr) },
            { 1376256U, typeof(ForgeBaseObjectPtr) },
            { 1441792U, typeof(ForgeBaseObject) },
            { 1638400U, typeof(uint) }, //Enum
            { 1703936U, typeof(string) }, //String32
            { 1769472U, typeof(string) }, //LString
            { 1835008U, typeof(ForgeFileReference) }
        };

        public static Type GetType(uint value, uint value2)
        {
            return ParameterTypes.TryGetValue(value, out var type1) ? type1
                : ParameterTypes.TryGetValue(value2, out var type2) ? type2
                : null;
        }
    }
}