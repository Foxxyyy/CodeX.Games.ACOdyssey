using CodeX.Core.Engine;
using CodeX.Core.Utilities;
using CodeX.Games.ACOdyssey.FORGE;
using CodeX.Games.ACOdyssey.Resources;
using System;
using System.Collections.Generic;
using System.IO;

namespace CodeX.Games.ACOdyssey.Files
{
    public class LODSelectorFile
    {
        public ForgeEntry Entry;
        public ForgeLODSelector LODSelector;
        public Dictionary<ForgeModel, byte[]> EmbeddedMeshes;
        public List<ForgeMaterial> EmbeddedMaterials;
        public List<ForgeTextureSet> EmbeddedTextureSets;
        public JenkHash Hash;
        public string Name;

        public LODSelectorFile(ForgeEntry entry)
        {
            Entry = entry;
            Name = entry.NameLower;
            Hash = JenkHash.GenHash(entry.NameLower);
        }

        public LODSelectorFile(string name)
        {
            Entry = null;
            Name = name.ToLower();
            Hash = JenkHash.GenHash(name.ToLower());
        }

        public void Load(byte[] data)
        {
            var reader = new DataReader(new MemoryStream(data));

            LODSelector = new ForgeLODSelector();
            LODSelector.Read(reader);

            EmbeddedMeshes = new Dictionary<ForgeModel, byte[]>();
            EmbeddedMaterials = new List<ForgeMaterial>();
            EmbeddedTextureSets = new List<ForgeTextureSet>();

            //We might have a embedded resource after that
            while (reader.BaseStream.Position < reader.BaseStream.Length)
            {
                long temp = reader.BaseStream.Position;
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
                        long temp2 = reader.BaseStream.Position;
                        reader.BaseStream.Position = temp;
                        EmbeddedMeshes.Add(compiledMesh, reader.ReadBytes((int)(temp2 - temp)));
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
                    default:
                        throw new Exception("Unknown resource block found in LODSelector...");
                }
            }
        }

        public string ToString(List<GameArchive> archives)
        {
            return string.Empty;
        }
    }
}