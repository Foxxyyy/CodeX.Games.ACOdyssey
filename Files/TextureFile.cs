using CodeX.Core.Engine;
using CodeX.Core.Utilities;
using CodeX.Games.ACOdyssey.FORGE;
using CodeX.Games.ACOdyssey.Resources;
using System.Collections.Generic;
using System.IO;

namespace CodeX.Games.ACOdyssey.Files
{
    public class TextureFile : TexturePack
    {
        public ForgeTexture? Texture;

        public TextureFile(ForgeEntry file) : base(file)
        {

        }

        public override void Load(byte[] data)
        {
            var r = new DataReader(new MemoryStream(data));

            Texture = new ForgeTexture();
            Texture.Read(r);
            Textures = new Dictionary<string, Texture>();

            if (Texture != null)
            {
                Textures[Texture.Name] = Texture;
                Texture.Pack = this;
            }
        }
    }
}