using CodeX.Core.Utilities;
using CodeX.Games.ACOdyssey.FORGE;

namespace CodeX.Games.ACOdyssey.Resources
{
    public class ForgeCellData
    {
        public int NumberOfObjects { get; set; }
        public ForgeFileReference[]? Objects { get; set; }
        public int NumberOfObjectsToActivate { get; set; }
        public long OwnerRelatedIndex { get; set; }

        public ForgeCellData()
        {
        }

        public void Read(DataReader reader)
        {
            var header = new ForgeDataHeader()
            {
                ResourceIdentifier = (ForgeResourceType)reader.ReadUInt32(),
                FileSize = reader.ReadInt32(),
                FileNameSize = reader.ReadInt32()
            };
            header.FileName = new(reader.ReadChars(header.FileNameSize));

            reader.BaseStream.Position += 0xE;
            NumberOfObjects = reader.ReadInt32();
            Objects = new ForgeFileReference[NumberOfObjects];

            for (int i = 0; i < NumberOfObjects; i++)
            {
                Objects[i] = new ForgeFileReference();
                Objects[i].Read(reader);
            }

            NumberOfObjectsToActivate = reader.ReadInt32();
            OwnerRelatedIndex = reader.ReadInt64();
        }
    }
}