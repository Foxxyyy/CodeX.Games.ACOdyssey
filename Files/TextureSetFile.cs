using CodeX.Core.Utilities;
using CodeX.Games.ACOdyssey.FORGE;
using CodeX.Games.ACOdyssey.Resources;
using System.IO;

namespace CodeX.Games.ACOdyssey.Files
{
    public class TextureSetFile
    {
        public ForgeTextureSet TextureSet;
        public JenkHash Hash;
        public string Name;

        public TextureSetFile(ForgeEntry file)
        {
            TextureSet = new ForgeTextureSet();
            Name = file.NameLower;
            Hash = JenkHash.GenHash(file.NameLower);
        }

        public void Load(byte[] data)
        {
            var reader = new DataReader(new MemoryStream(data));
            TextureSet.Read(reader);
        }
    }
}