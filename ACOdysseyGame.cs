using CodeX.Games.ACOdyssey.FORGE;
using CodeX.Core.Engine;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace CodeX.Games.ACOdyssey
{
    public class ACOdysseyGame : Game
    {
        public override string Name => "Assassin's Creed Odyssey";
        public override string ShortName => "ACOdyssey";
        public override string GameFolder { get => GameFolderSetting.GetString(); set => GameFolderSetting.Set(value); }
        public override string GamePathPrefix => "ACOdyssey\\";
        public override bool GameFolderOk => Directory.Exists(GameFolder);
        public override bool RequiresGameFolder => true;
        public override bool Enabled { get => GameEnabledSetting.GetBool(); set => GameEnabledSetting.Set(value); }
        public override bool EnableMapView => true;
        public override FileTypeIcon Icon => FileTypeIcon.Hotdog;
        public override string HashAlgorithm => "None";

        public static Setting GameFolderSetting = Settings.Register("ACOdyssey.GameFolder", SettingType.String, "C:\\Program Files (x86)\\Ubisoft\\Ubisoft Game Launcher\\games\\Assassin's Creed Odyssey");
        public static Setting GameEnabledSetting = Settings.Register("ACOdyssey.Enabled", SettingType.Bool, true);

        public override bool CheckGameFolder(string folder)
        {
            return Directory.Exists(folder) && File.Exists(folder + "\\ACOdyssey.exe");
        }

        public override bool AutoDetectGameFolder(out string source)
        {
            if (AutoDetectFolder(out Dictionary<string, string> matches))
            {
                var match = matches.First();
                source = match.Key;
                GameFolder = match.Value.Trim().TrimEnd('/', '\\', ' ');
                return true;
            }
            source = null;
            return false;
        }

        public override FileManager CreateFileManager()
        {
            return new ForgeFileManager(this);
        }

        public override Level GetMapLevel()
        {
            return null;
        }

        public override Setting[] GetMapSettings()
        {
            return null;
        }

        private bool AutoDetectFolder(out Dictionary<string, string> matches)
        {
            matches = new Dictionary<string, string>();
            if (CheckGameFolder(GameFolder))
            {
                matches.Add("Current CodeX Folder", GameFolder);
            }
            return matches.Count > 0;
        }
    }
}