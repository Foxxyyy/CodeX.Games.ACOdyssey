using CodeX.Core.Engine;
using CodeX.Core.Utilities;
using CodeX.Games.ACOdyssey.FORGE;
using CodeX.Games.ACOdyssey.Resources;
using System.Collections.Generic;
using System.IO;

namespace CodeX.Games.ACOdyssey.Files
{
    public class WorldFile
    {
        public ForgeWorld World;
        public JenkHash Hash;
        public string Name;

        public WorldFile(ForgeEntry entry)
        {
            World = new ForgeWorld();
            Name = entry.NameLower;
            Hash = JenkHash.GenHash(entry.NameLower);
        }

        public void Load(byte[] data)
        {
            var reader = new DataReader(new MemoryStream(data));
            World.Read(reader);
        }

        public string ToString(List<GameArchive> archives)
        {
            return string.Empty;
        }
    }
}