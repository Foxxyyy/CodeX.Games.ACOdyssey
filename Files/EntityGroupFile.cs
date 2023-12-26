using CodeX.Core.Engine;
using CodeX.Core.Utilities;
using CodeX.Games.ACOdyssey.FORGE;
using CodeX.Games.ACOdyssey.Resources;
using System;
using System.Collections.Generic;
using System.IO;

namespace CodeX.Games.ACOdyssey.Files
{
    public class EntityGroupFile
    {
        public ForgeEntityGroup EntityGroup;
        public List<ForgeModel>? EmbeddedMeshes;
        public List<ForgeMaterial>? EmbeddedMaterials;
        public List<ForgeTextureSet>? EmbeddedTextureSets;
        public List<ForgeLODSelector>? EmbeddedLODSelector;
        public JenkHash Hash;
        public string Name;

        public EntityGroupFile(ForgeEntry entry)
        {
            EntityGroup = new ForgeEntityGroup();
            Name = entry.NameLower;
            Hash = JenkHash.GenHash(entry.NameLower);
        }

        public void Load(byte[] data)
        {
            var reader = new DataReader(new MemoryStream(data));
            EntityGroup.Read(reader);

            EmbeddedMeshes = new List<ForgeModel>();
            EmbeddedMaterials = new List<ForgeMaterial>();
            EmbeddedTextureSets = new List<ForgeTextureSet>();
            EmbeddedLODSelector = new List<ForgeLODSelector>();

            //We might have a embedded resource after that
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
                    case ForgeResourceType.NONE:
                        _ = reader.ReadInt32(); //Padding??
                        break;
                    case ForgeResourceType.MESH:
                        var compiledMesh = new ForgeModel();
                        compiledMesh.Read(reader, true);
                        compiledMesh.Name = header.FileName;
                        EmbeddedMeshes.Add(compiledMesh);
                        break;
                    case ForgeResourceType.MATERIAL:
                        var material = new ForgeMaterial();
                        material.Read(reader, true);
                        EmbeddedMaterials.Add(material);
                        break;
                    case ForgeResourceType.TEXTURE_SET:
                        var textureSet = new ForgeTextureSet();
                        textureSet.Read(reader, true);
                        EmbeddedTextureSets.Add(textureSet);
                        break;
                    case ForgeResourceType.LOD_SELECTOR:
                        var lod = new ForgeLODSelector();
                        lod.Read(reader, true);
                        lod.Header = header;
                        EmbeddedLODSelector.Add(lod);
                        break;
                    default:
                        throw new Exception("Unknown resource block found in EntityGroup...");
                }
            }
        }

        public string ToString(List<GameArchive> archives)
        {
            return string.Empty;
        }
    }
}