using CodeX.Core.Engine;
using CodeX.Core.Utilities;
using CodeX.Games.ACOdyssey.FORGE;
using CodeX.Games.ACOdyssey.Resources;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CodeX.Games.ACOdyssey.Files
{
    public class CellDataFile
    {
        public ForgeCellData CellData;
        public JenkHash Hash;
        public string Name;

        public CellDataFile(ForgeEntry entry)
        {
            CellData = new ForgeCellData();
            Name = entry.NameLower;
            Hash = JenkHash.GenHash(entry.NameLower);
        }

        public void Load(byte[] data)
        {
            var reader = new DataReader(new MemoryStream(data));
            CellData.Read(reader);
        }

        public string ToString(List<GameArchive> archives)
        {
            var sb = new StringBuilder();
            if (CellData.Objects != null)
            {
                sb.AppendLine(string.Format("NumberOfObjectsToActivate: {0}", CellData.NumberOfObjectsToActivate));
                sb.AppendLine(string.Format("OwnerRelatedIndex: {0}", CellData.OwnerRelatedIndex));
                sb.AppendLine(string.Format("\nObjects:", CellData.NumberOfObjects));

                foreach (var data in CellData.Objects)
                {
                    var templateEntry = ForgeFile.FindEntryByFileID(archives, data.FileID);
                    sb.AppendLine(string.Format("   - Resource ID: 0x{0} ({1})", data.FileID.ToString("X"), templateEntry != null ? templateEntry.Name : "Unknown"));
                }
            }
            return sb.ToString();
        }
    }
}