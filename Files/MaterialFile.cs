using CodeX.Core.Engine;
using CodeX.Core.Utilities;
using CodeX.Games.ACOdyssey.FORGE;
using CodeX.Games.ACOdyssey.Resources;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CodeX.Games.ACOdyssey.Files
{
    public class MaterialFile
    {
        public ForgeMaterial Material;
        public List<ForgeTexture> EmbeddedTextures;
        public List<ForgeTextureSet> EmbeddedTextureSets;
        public JenkHash Hash;
        public string Name;

        public MaterialFile(ForgeEntry file)
        {
            Material = new ForgeMaterial();
            Name = file.NameLower;
            Hash = JenkHash.GenHash(file.NameLower);
        }

        public void Load(byte[] data)
        {
            var reader = new DataReader(new MemoryStream(data));
            Material.Read(reader);

            EmbeddedTextures = new List<ForgeTexture>();
            EmbeddedTextureSets = new List<ForgeTextureSet>();

            //We might have a embedded texture after that
            while (reader.BaseStream.Position < reader.BaseStream.Length)
            {
                var header = new ForgeDataHeader()
                {
                    ResourceIdentifier = (ForgeResourceType)reader.ReadUInt32(),
                    FileSize = reader.ReadInt32(),
                    FileNameSize = reader.ReadInt32()
                };
                header.FileName = new(reader.ReadChars(header.FileNameSize));

                switch (header.ResourceIdentifier)
                {
                    case ForgeResourceType.TEXTURE_MAP:
                        var texture = new ForgeTexture();
                        texture.Read(reader, true);
                        texture.Name = header.FileName;
                        EmbeddedTextures.Add(texture);
                        break;
                    case ForgeResourceType.TEXTURE_SET:
                        var textureSet = new ForgeTextureSet();
                        textureSet.Read(reader, true);
                        EmbeddedTextureSets.Add(textureSet);
                        break;
                    default:
                        throw new Exception("Unknown resource block found in Material...");
                }
            }
        }

        public string ToString(List<GameArchive> archives)
        {
            var sb = new StringBuilder();
            if (Material != null)
            {
                if (Material.MaterialTemplate != null)
                {
                    var templateEntry = ForgeFile.FindEntryByFileID(archives, Material.MaterialTemplate.FileID);
                    sb.AppendLine(string.Format("Material Template ID: 0x{0} ({1})", Material.MaterialTemplate.FileID.ToString("X"), templateEntry != null ? templateEntry.Name : "Unknown"));
                }

                if (Material.TextureSet != null)
                {
                    var textureSetEntry = ForgeFile.FindEntryByFileID(archives, Material.TextureSet.FileID);
                    sb.AppendLine(string.Format("Texture Set ID: 0x{0} ({1})", Material.TextureSet.FileID.ToString("X"), textureSetEntry != null ? textureSetEntry.Name : "Unknown"));
                }

                sb.AppendLine(string.Format("BlendMode: {0}", Material.BlendMode));
                sb.AppendLine(string.Format("AlphaDiplayMode: {0}", Material.AlphaDiplayMode));
                sb.AppendLine(string.Format("AlphaTestValue: {0}", Material.AlphaTestValue));
                sb.AppendLine(string.Format("ZWriteDisabledOpaque: {0}", Material.ZWriteDisabledOpaque));
                sb.AppendLine(string.Format("ZWriteDisabledAlpha: {0}", Material.ZWriteDisabledAlpha));
                sb.AppendLine(string.Format("ZTestDisabled: {0}", Material.ZTestDisabled));
                sb.AppendLine(string.Format("LayerMaterial: {0}", Material.LayerMaterial));
                sb.AppendLine(string.Format("LayerOverrideAlbedo: {0}", Material.LayerOverrideAlbedo));
                sb.AppendLine(string.Format("LayerOverrideNormal: {0}", Material.LayerOverrideNormal));
                sb.AppendLine(string.Format("LayerOverrideMetalness: {0}", Material.LayerOverrideMetalness));
                sb.AppendLine(string.Format("LayerOverrideGloss: {0}", Material.LayerOverrideGloss));
                sb.AppendLine(string.Format("LayerOverrideWetness: {0}", Material.LayerOverrideWetness));
                sb.AppendLine(string.Format("LayerOverrideEmissive: {0}", Material.LayerOverrideEmissive));
                sb.AppendLine(string.Format("PreTransparent: {0}", Material.PreTransparent));
                sb.AppendLine(string.Format("FlipNormalOnBackFace: {0}", Material.FlipNormalOnBackFace));
                sb.AppendLine(string.Format("ShadowCasterOpaque: {0}", Material.ShadowCasterOpaque));
                sb.AppendLine(string.Format("ShadowCasterAlpha: {0}", Material.ShadowCasterAlpha));
                sb.AppendLine(string.Format("IsAtlasMaterial: {0}", Material.IsAtlasMaterial));
                sb.AppendLine(string.Format("IsObjectAtlasMaterial: {0}", Material.IsObjectAtlasMaterial));
                sb.AppendLine(string.Format("SupportRuntimeDisable: {0}", Material.SupportRuntimeDisable));
                sb.AppendLine(string.Format("CanBeOccluder: {0}", Material.CanBeOccluder));
                sb.AppendLine(string.Format("ZPrePassEnabled: {0}", Material.ZPrePassEnabled));
                sb.AppendLine(string.Format("RenderInLowResolution: {0}", Material.RenderInLowResolution));
                sb.AppendLine(string.Format("IsWaterMaterial: {0}", Material.IsWaterMaterial));
                sb.AppendLine(string.Format("IsFakeWaterMaterial: {0}", Material.IsFakeWaterMaterial));
                sb.AppendLine(string.Format("IsWaterMaterialDetailed: {0}", Material.IsWaterMaterialDetailed));
                sb.AppendLine(string.Format("Wireframe: {0}", Material.Wireframe));
                sb.AppendLine(string.Format("HasTextureMapSelector: {0}", Material.HasTextureMapSelector));
                sb.AppendLine(string.Format("AlphaTestEnabled: {0}", Material.AlphaTestEnabled));
                sb.AppendLine(string.Format("TwoSided: {0}", Material.TwoSided));
                sb.AppendLine(string.Format("MaterialDisabled: {0}", Material.MaterialDisabled));
                sb.AppendLine(string.Format("ExcludeWaterClipMaterial: {0}", Material.ExcludeWaterClipMaterial));
                sb.AppendLine(string.Format("IgnoreWaterDepthTest: {0}", Material.IgnoreWaterDepthTest));
                sb.AppendLine(string.Format("WaterClipMaterial: {0}", Material.WaterClipMaterial));
                sb.AppendLine(string.Format("ForceRenderInFakeBucket: {0}", Material.ForceRenderInFakeBucket));
                sb.AppendLine(string.Format("SSAOOnDiffuse: {0}", Material.SSAOOnDiffuse));

                if (Material.BackFaceMaterial != null)
                {
                    var backFaceEntry = ForgeFile.FindEntryByFileID(archives, Material.BackFaceMaterial.FileID);
                    sb.AppendLine(string.Format("BackFaceMaterial: 0x{0} ({1})", Material.BackFaceMaterial.FileID.ToString("X"), backFaceEntry != null ? backFaceEntry.Name : "Unknown"));
                }

                sb.AppendLine(string.Format("TangentShift1: {0}", Material.TangentShift1));
                sb.AppendLine(string.Format("TangentShift2: {0}", Material.TangentShift2));
                sb.AppendLine(string.Format("DiffuseDarkening: {0}", Material.DiffuseDarkening));
                sb.AppendLine(string.Format("SpecularReflectance1: {0}", Material.SpecularReflectance1));
                sb.AppendLine(string.Format("SpecularReflectance2: {0}", Material.SpecularReflectance2));
                sb.AppendLine(string.Format("TAADitherFactor: {0}", Material.IsFakeWaterMaterial));
                sb.AppendLine(string.Format("EmbeddedTextures: {0}", EmbeddedTextures?.Count ?? 0));
            }
            return sb.ToString();
        }
    }
}