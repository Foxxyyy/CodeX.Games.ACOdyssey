using CodeX.Core.Engine;
using CodeX.Core.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace CodeX.Games.ACOdyssey.FORGE
{
    public class ForgeFile : GameArchive
    {
        //Header
        public long StartPos { get; set; }
        public ulong Version { get; set; } //Identifier
        public byte Padding1 { get; set; }
        public int FileVersion { get; set; }
        public long HeaderOffset { get; set; }

        //SubHeader
        public int EntryCount { get; set; }
        public int Unknown_4h { get; set; }
        public int Unknown_8h { get; set; }
        public int Unknown_Ch { get; set; }
        public int Unknown_10h { get; set; }
        public long Unknown_14h { get; set; }
        public int MaximumEntryCount { get; set; }
        public int Unknown_20h { get; set; }
        public long DataOffset { get; set; }

        //DataHeader
        public int IndexCount { get; set; }
        public int Padding2 { get; set; }
        public long TableOffset { get; set; }
        public long NextDataOffset { get; set; }
        public int IndexStart { get; set; }
        public int IndexEnd { get; set; }
        public long NameTableOffset { get; set; }
        public long Padding3 { get; set; }

        public ForgeFile(string fpath, string relpath)
        {
            var fi = new FileInfo(fpath);
            Name = fi.Name;
            Path = relpath.ToLowerInvariant();
            FilePath = fpath;
            Size = fi.Length;
        }

        private void ReadHeader(BinaryReader br)
        {
            StartPos = br.BaseStream.Position;
            Version = br.ReadUInt64(); //'scimitar'
            Padding1 = br.ReadByte();
            FileVersion = br.ReadInt32();
            HeaderOffset = br.ReadInt64();

            if (Version != 8241996789220729715)
            {
                var verbytes = BitConverter.GetBytes(Version);
                var versionstr = BitConverter.ToString(verbytes);
                throw new Exception("Invalid FORGE archive - found \"" + versionstr + "\" instead.");
            }
        }

        private void ReadSubHeader(BinaryReader br)
        {
            br.BaseStream.Position = HeaderOffset;
            EntryCount = br.ReadInt32();
            Unknown_4h = br.ReadInt32();
            Unknown_8h = br.ReadInt32();
            Unknown_Ch = br.ReadInt32();
            Unknown_10h = br.ReadInt32();
            Unknown_14h = br.ReadInt64();
            MaximumEntryCount = br.ReadInt32();
            Unknown_20h = br.ReadInt32();
            DataOffset = br.ReadInt64();
        }

        private void ReadDataHeader(BinaryReader br)
        {
            br.BaseStream.Position = DataOffset;
            IndexCount = br.ReadInt32();
            Padding2 = br.ReadInt32();
            TableOffset = br.ReadInt64();
            NextDataOffset = br.ReadInt64();
            IndexStart = br.ReadInt32();
            IndexEnd = br.ReadInt32();
            NameTableOffset = br.ReadInt64();
            Padding3 = br.ReadInt64();

            byte[] entriesdata = br.ReadBytes(20 * IndexCount);
            var entries = new BinaryReader(new MemoryStream(entriesdata));
            AllEntries = new List<GameArchiveEntry>();

            for (uint i = 0; i < EntryCount; i++)
            {
                var entry = new ForgeEntry();
                entry.ReadEntry(entries, (int)i);
                AllEntries.Add(entry);
            } 
        }

        private void ReadNameTable(BinaryReader br)
        {
            br.BaseStream.Position = NameTableOffset;
            byte[] entriesdata = br.ReadBytes(192 * IndexCount);
            var nametable = new BinaryReader(new MemoryStream(entriesdata));

            for (int i = 0; i < AllEntries.Count; i++)
            {
                var table = new ForgeNameTable();
                table.Read(nametable);

                ((ForgeEntry)AllEntries[i]).NameTable = table;
                ((ForgeEntry)AllEntries[i]).ResourceType = table.ResourceIdentifier.ToString();
                AllEntries[i].Name = table.Name;
                AllEntries[i].Path = Path + "\\" + table.Name;
            }
        }

        private void CreateDirectories()
        {
            Root = new ForgeDirectoryEntry
            {
                Archive = this,
                Name = Name,
                Path = Path
            };

            var dirs = new Dictionary<string, ForgeDirectoryEntry>();
            dirs[Root.Path] = (ForgeDirectoryEntry)Root;

            foreach (var e in AllEntries)
            {
                e.Archive = this;
                ((ForgeDirectoryEntry)Root).Files.Add((GameArchiveFileInfo)e);
            }
        }

        public override void ReadStructure(BinaryReader br)
        {
            ReadHeader(br);
            ReadSubHeader(br);
            ReadDataHeader(br);
            ReadNameTable(br);
            CreateDirectories();
            Children = new List<GameArchive>();
        }

        public override bool EnsureEditable(Func<string, string, bool> confirm)
        {
            throw new NotImplementedException();
        }

        public override byte[] ExtractFile(GameArchiveFileInfo f, bool compressed = false)
        {
            try
            {
                using BinaryReader br = new BinaryReader(File.OpenRead(GetPhysicalFilePath()));
                return ExtractFileResource((ForgeEntry)f, br, compressed);
            }
            catch
            {
                return null;
            }
        }

        public byte[] ExtractFileResource(ForgeEntry entry, BinaryReader br, bool compressed = false)
        {
            byte[] data = new byte[entry.Size];
            br.BaseStream.Seek(entry.Offset, SeekOrigin.Begin);
            data = br.ReadBytes((int)entry.Size);

            if (data != null)
            {
                data = ForgeCrypto.DecompressData(data);
            }
            return data;
        }

        public static GameArchiveEntry FindEntryByFileID(List<GameArchive> archives, long fileID)
        {
            foreach (var archive in archives)
            {
                var entry = archive.AllEntries.FirstOrDefault(e => ((ForgeEntry)e).FileID == fileID);
                if (entry != null)
                    return entry;
            }
            return null;
        }

        public void ReadStartupCache(BinaryReader br)
        {
            StartPos = br.ReadInt64();
            Version = br.ReadUInt64();
            Padding1 = br.ReadByte();
            FileVersion = br.ReadInt32();
            HeaderOffset = br.ReadInt64();
            EntryCount = br.ReadInt32();
            AllEntries = new List<GameArchiveEntry>();

            var entrydict = new Dictionary<string, GameArchiveFileInfo>();
            for (int i = 0; i < EntryCount; i++)
            {
                var entry = new ForgeEntry
                {
                    Name = br.ReadStringNullTerminated(),
                    Size = br.ReadInt64(),
                };
                entry.NameTable = new ForgeNameTable()
                {
                    Name = entry.Name,
                    ResourceIdentifier = (ForgeResourceType)br.ReadUInt32()
                };
                entry.ResourceType = entry.NameTable.ResourceIdentifier.ToString();
                entry.Path = Path + "\\" + entry.Name;
                entry.ReadEntry(br, i);

                switch (entry.NameTable.ResourceIdentifier)
                {
                    case ForgeResourceType.MESH:
                        entry.Name += ".mesh";
                        break;
                    case ForgeResourceType.TEXTURE_MAP:
                        entry.Name += ".texture";
                        break;
                    case ForgeResourceType.LOD_SELECTOR:
                        entry.Name += ".lodselector";
                        break;
                    case ForgeResourceType.CELL_DATA_BLOCK:
                        entry.Name += ".gridcelldata";
                        break;
                    case ForgeResourceType.MATERIAL:
                        entry.Name += ".material";
                        break;
                    case ForgeResourceType.ENTITY:
                        entry.Name += ".entity";
                        break;
                    case ForgeResourceType.ENTITY_GROUP:
                        entry.Name += ".entitygroup";
                        break;
                    case ForgeResourceType.WORLD:
                        entry.Name += ".world";
                        break;
                }
                AllEntries.Add(entry);

                if ((entry is GameArchiveFileInfo finfo) && finfo.IsArchive)
                {
                    entrydict[finfo.Path.ToLowerInvariant()] = finfo;
                }
            }
            CreateDirectories();
        }

        public void WriteStartupCache(BinaryWriter bw)
        {
            bw.Write(StartPos);
            bw.Write(Version);
            bw.Write(Padding1);
            bw.Write(FileVersion);
            bw.Write(HeaderOffset);
            bw.Write(EntryCount);

            for (int i = 0; i < EntryCount; i++)
            {
                if (AllEntries[i] is ForgeEntry entry)
                {
                    if (entry == null)
                        continue;

                    string name = entry.Name.Contains('.') ? entry.Name[..entry.Name.LastIndexOf('.')] : entry.Name;
                    bw.WriteStringNullTerminated(name);
                    bw.Write(entry.Size);
                    bw.Write((uint)(entry.NameTable?.ResourceIdentifier ?? 0));
                    entry.WriteEntry(bw);
                }
            }
        }
    }

    public class ForgeEntry : GameArchiveEntryBase, GameArchiveFileInfo
    {
        public ForgeNameTable NameTable { get; set; }
        public long Offset { get; set; }
        public long FileID { get; set; }
        public string ResourceType { get; set; } = "File";

        private string _Attributes;
        public override string Attributes
        {
            get
            {
                if (_Attributes == null)
                {
                    if (uint.TryParse(ResourceType, out uint type))
                    {
                        ResourceType = "UNKNOWN";
                    }
                    _Attributes = "Resource [" + ResourceType + "]";
                }
                return _Attributes;
            }
        }

        public bool IsArchive => false;

        public void ReadEntry(BinaryReader br, int index)
        {
            Offset = br.ReadInt64();
            FileID = br.ReadInt64();
            Size = br.ReadInt32();
        }

        public void WriteEntry(BinaryWriter bw)
        {
            bw.Write(Offset);
            bw.Write(FileID);
            bw.Write((int)Size);
        }

        public string GetFilePath()
        {
            return Name;
        }

        public override string ToString()
        {
            return Path;
        }
    }

    public class ForgeDirectoryEntry : ForgeEntry, GameArchiveDirectory
    {
        public List<GameArchiveDirectory> Directories { get; set; } = new List<GameArchiveDirectory>();
        public List<GameArchiveFileInfo> Files { get; set; } = new List<GameArchiveFileInfo>();

        public override string ToString()
        {
            return "Directory: " + Path;
        }
    }

    public class ForgeNameTable
    {
        public int DataSize { get; set; }
        public long FileDataID { get; set; }
        public int Unknown_Ch { get; set; }
        public ForgeResourceType ResourceIdentifier { get; set; }
        public int Unknown_1Ch { get; set; }
        public int Unknown_20h { get; set; }
        public int NextFileCount { get; set; }
        public int PreviousFileCount { get; set; }
        public int Unknown_28h { get; set; }
        public int Timestamp { get; set; }
        public string Name { get; set; } = string.Empty; //Actually char[128]
        public int Unknown_ACh { get; set; }
        public int Unknown_B0h { get; set; }
        public int Unknown_B4h { get; set; }
        public int Unknown_B8h { get; set; }
        public int Unknown_BCh { get; set; }

        public void Read(BinaryReader br)
        {
            DataSize = br.ReadInt32(); //4
            FileDataID = br.ReadInt64(); //12
            Unknown_Ch = br.ReadInt32(); //16
            ResourceIdentifier = (ForgeResourceType)br.ReadUInt32(); //20
            Unknown_1Ch = br.ReadInt32(); //24
            Unknown_20h = br.ReadInt32(); //28
            NextFileCount = br.ReadInt32(); //32
            PreviousFileCount = br.ReadInt32(); //36
            Unknown_28h = br.ReadInt32(); //40
            Timestamp = br.ReadInt32(); //44
            Name = new string(br.ReadChars(128)); //172
            Unknown_ACh = br.ReadInt32(); //176
            Unknown_B0h = br.ReadInt32(); //180
            Unknown_B4h = br.ReadInt32(); //184
            Unknown_B8h = br.ReadInt32(); //188
            Unknown_BCh = br.ReadInt32(); //192
            Name = Regex.Replace(Name, @"[^\u0020-\u007E]", string.Empty);

            switch (ResourceIdentifier)
            {
                case ForgeResourceType.MESH:
                    Name += ".mesh";
                    break;
                case ForgeResourceType.TEXTURE_MAP:
                    Name += ".texture";
                    break;
                case ForgeResourceType.LOD_SELECTOR:
                    Name += ".lodselector";
                    break;
                case ForgeResourceType.CELL_DATA_BLOCK:
                    Name += ".gridcelldata";
                    break;
                case ForgeResourceType.MATERIAL:
                    Name += ".material";
                    break;
                case ForgeResourceType.ENTITY:
                    Name += ".entity";
                    break;
                case ForgeResourceType.ENTITY_GROUP:            
                    Name += ".entitygroup";
                    break;
                case ForgeResourceType.WORLD:
                    Name += ".world";
                    break;
            }
        }

        public override string ToString()
        {
            return Name;
        }
    }

    public class ForgeDataHeader
    {
        public ForgeResourceType ResourceIdentifier { get; set; }
        public int FileSize { get; set; }
        public int FileNameSize { get; set; }
        public string FileName { get; set; } = string.Empty;

        public ForgeDataHeader()
        {
        }

        public ForgeDataHeader(ForgeResourceType resourceIdentifier, int fileSize, int fileNameSize, string fileName)
        {
            ResourceIdentifier = resourceIdentifier;
            FileSize = fileSize;
            FileNameSize = fileNameSize;
            FileName = fileName;
        }

        public ForgeDataHeader(ForgeDataHeader header)
        {
            ResourceIdentifier = header.ResourceIdentifier;
            FileSize = header.FileSize;
            FileNameSize = header.FileNameSize;
            FileName = header.FileName;
        }
    }

    public class ForgeFileReference //0xA
    {
        public byte Num { get; set; }
        public byte IsManaged { get; set; }
        public long FileID { get; set; }

        public ForgeFileReference()
        {
        }

        public ForgeFileReference(ForgeFileReference reference)
        {
            Num = reference.Num;
            IsManaged = reference.IsManaged;
            FileID = reference.FileID;
        }

        public void Read(DataReader reader, bool check = false)
        {
            Num = reader.ReadByte();
            if (check && Num != 0)
            {
                return;
            }
            IsManaged = reader.ReadByte();
            FileID = reader.ReadInt64();
        }
    }

    public class ForgeBaseObject //0xC
    {
        public uint BaseID { get; set; }
        public uint Unknown_4h { get; set; }
        public uint Unknown_8h { get; set; }

        public ForgeBaseObject()
        {
        }

        public void Read(DataReader reader)
        {
            BaseID = reader.ReadUInt32();
            Unknown_4h = reader.ReadUInt32();
            Unknown_8h = reader.ReadUInt32();
        }
    }

    public class ForgeObject : ForgeBaseObject //0xD
    {
        public byte Num { get; set; }

        public ForgeObject()
        {
        }

        public void Read(DataReader reader, bool check = false)
        {
            Num = reader.ReadByte();
            if (check && Num == 3)
            {
                return;
            }
            base.Read(reader);
        }
    }

    public class ForgeBaseObjectPtr : ForgeFileReference //0xE
    {
        public uint FileType { get; set; }

        public ForgeBaseObjectPtr()
        {
        }

        public ForgeBaseObjectPtr(ForgeBaseObjectPtr objectPtr)
        {
            Num = objectPtr.Num;
            IsManaged = objectPtr.IsManaged;
            FileID = objectPtr.FileID;
            FileType = objectPtr.FileType;
        }

        public void Read(DataReader reader, bool check = false, long newOffset = 0)
        {
            base.Read(reader, check);
            if (check && base.Num != 0)
            {
                reader.BaseStream.Position += newOffset;
                return;
            }
            FileType = reader.ReadUInt32();
        }
    }

    public class ForgeObjectPtr //0x9
    {
        public byte Num { get; set; }
        public bool IsManagedObject { get; set; }
        public long FileID { get; set; }

        public ForgeObjectPtr()
        {
        }

        public void Read(DataReader reader, bool check = false, long newOffset = 0)
        {
            Num = reader.ReadByte();
            if (check)
            {
                switch (Num)
                {
                    case 0:
                        reader.BaseStream.Position += newOffset;
                        return;
                    case 3:
                        return;
                }
            }
            FileID = reader.ReadInt64();
        }
    }

    public enum ForgeResourceType : uint
    {
        ACCOMPLISHMENT = 0x60B663FB,
        ACCOMPLISHMENT_CONFIG = 0x022F146B,
        ACCOMPLISHMENT_MANAGER = 0x79210545,
        ACE_FIGHT_SETTINGS = 0xB523F455,
        AGENDA_TREE = 0xE80D4CAD,
        AGENT_CONTEXT_DATA = 0xA3602709,
        AGENT_CONTEXT_TRANSITION_TREE = 0x05F998A9,
        AI_CONTEXT_BODYGUARD_REACTION_HANDLER_LOGIC_DESCRIPTOR = 0x3D6A155E,
        AI_FIGHT_STAGING_SETTINGS = 0xE1F97746,
        AI_ROLE = 0xF63AB81A,
        AI_ROLE_DEFINITION = 0xF7FE1FFA,
        AMBIANCE_ZONE = 0x5766A70C,
        AMBIANCE_ZONES_SETTINGS_CELL_DATA = 0xDAE744C7,
        AMBIENCE_ZONE_SOUND_DATA = 0xA4DD63BE,
        AMBIENT_SPAWN_DIRECTOR = 0xEF97722C,
        ANIMAL_ACTOR_DATA_WRAPPER = 0x1589F53A,
        ANIMAL_CONTEXT_DATA_SET = 0x5C9ACBA0,
        ANIMAL_INCAPACITATE_SETTINGS = 0x662BB5EC,
        ANIMAL_SWARM_ANIMATION_DESCRIPTOR = 0x0ECC3F99,
        ANIMATED_TEXTURE = 0x201429AB,
        ANIMATION = 0x0FA3067F,
        ANIMATION_SETTINGS = 0x174B8004,
        ANIM_TRACK_BUILTIN_TRACKS_TABLE = 0x46A37798,
        ANVIL_SCRIPT = 0x198B47F1,
        ANVIL_SCRIPT_PROJECT = 0xDB7D7B5D,
        ARCHER_VOLLEY_SOUND_SET = 0x82BEC695,
        ARCHETYPE_SCHEDULES = 0x488D7C1E,
        ARENA_ENCOUNTER_DEFINITION = 0xDC16BD0D,
        ARMOR_SETTINGS = 0xBBF663AC,
        ASSASSIN_BLACKBOARD_KEY_REPOSITORY = 0x321CDCCB,
        ASSASSIN_QUEST_UI_ICON_SETTINGS = 0xDE7DF9CB,
        ASSASSIN_SOUND_SETTINGS = 0xFD9CF1E7,
        ASSASSIN_STORY_SETTINGS = 0xAC52E01A,
        ATOM_REPLACE_SET = 0xCD4F18F6,
        AC2_ASSASSINATION_TARGET_TRACKER = 0x441D6851,
        ACD_GLOBAL_MUSIC_SETTINGS = 0x3B4F5451,
        BALLISTIC_PROJECTILE_SOUND_SET = 0x8FED0345,
        BIG_BATTLE_DATA_CONTAINER = 0x644A8F92,
        BIG_BATTLE_PARAMETERS = 0xD201873B,
        BIG_BATTLE_PHALANX_FORMATION = 0xFC2BD79B,
        BOAT_CAPTAIN_FACIAL_GROUP_SET = 0x52C7F9F4,
        BOAT_CONTEXT_DATA_SET = 0x647C80CB,
        BOAT_RANK_INFO = 0x5BEAAA80,
        BOAT_SECOND_IN_COMMAND_FACIAL_GROUP_SET = 0xD510F10E,
        BOAT_SHAPE = 0x7807A27F,
        BOAT_VEHICLE_ADDON = 0x64548654,
        BOAT_VEHICLE_BEHAVIOR_DATA = 0x786374A5,
        BOAT_VEHICLE_BOARDING_DATA = 0xAE56845E,
        BOAT_VEHICLE_BRACING_DATA = 0x4D6EEA6F,
        BOAT_VEHICLE_F_X_DATA = 0xE78062F8,
        BOAT_VEHICLE_HANDLING_DATA = 0xEB233CFA,
        BOAT_VEHICLE_RAMMING_DATA = 0xDCBFF569,
        BOAT_VEHICLE_SETTINGS = 0x250A6FA5,
        BOAT_VEHICLE_SOUND_SET = 0x12FD3552,
        BODY_PART_MAPPING = 0xB3ADF542,
        BODY_PART_TEMPLATE = 0x02251ED8,
        BONE_PRESET_SETTINGS = 0xCC5D5D9E,
        BOSS_FIGHT_ACTIVITY_PACK = 0x9328902D,
        BOUNTY_GATES_HANDLER = 0x769B5A0C,
        BOW_SOUND_SET = 0x1F6BA72E,
        BOX_SHAPE = 0x4EC68E98,
        BUILD_TABLE = 0x22ECBE63,
        BUILD_TAGS_TO_COLUMN_MASKS = 0xF8C05D2F,
        BULK_ACTION_DESCRIPTOR = 0xD52B642F,
        BULK_ACTION_KIT = 0xE2D7120E,
        BULK_ACTION_TYPE = 0x78B0A1F2,
        BULK_MESH = 0x4107267C,
        BULK_SKELETAL_ANIMATION = 0x0E0DBB0F,
        CAMERA_MANAGER = 0x7AAB07A6,
        CAMERA_SELECTOR_BLENDER_NODE = 0x01A0D270,
        CAMERA_SELECTOR_NODE = 0xD536E933,
        CAPSULE = 0x0DD5981F,
        CAPSULE_SHAPE = 0xB8599052,
        CARD_REPOSITORY = 0xE7536E8C,
        CARRIABLE_OBJECT_SOUND_SET = 0x2B3B6499,
        CELL_DATA_BLOCK = 0xAC2BBF68,
        CHARACTER_SETUP_CONFIGURATION = 0x24640CDA,
        CHEAT_MANAGER = 0x2A06D2F0,
        CINEMATIC_COMMUNITY_MEMBER = 0x1AB816BC,
        CINEMATIC_MANAGER = 0x558FBA35,
        CLIMATE_SETTINGS = 0x94FB04BC,
        CLOTH = 0xE33044BA,
        CLUTTER_RENDERING_DISTANCE_REGION_CELL_DATA = 0xE17A8ADA,
        COLLECTION = 0xB31066E2,
        COLLISION_MATERIAL = 0x74F7311D,
        COMPILED_MATERIAL_TEMPLATE = 1000043639,
        COMPILED_MATERIAL_TEMPLATE_CONTAINER = 0x7AF98A66,
        COMPILED_MESH = 4238218645,
        COMPILED_MESH_INSTANCE = 1130893339,
        COMPILED_MIP = 0x1D4B87A3,
        COMPILED_TEXTURE_MAP = 321093609,
        COMPRESSED_LOCALIZATION_DATA = 3531835829,
        CONE_CAPSULE_SHAPE = 0x6AAD6AFA,
        CONFLICT_MUSIC_SETTINGS = 0x39E461FA,
        CONSTANT_PROPERTY_CONTROLLER_DATA = 0x43F0EEBF,
        CONTACT_MATERIAL_SETTINGS = 0x2AF75776,
        CONTACT_TABLE = 0x5A03895E,
        CONTEXTUAL_ACTION_SETTINGS = 0x06A44366,
        CONTEXTUAL_COLLISION_SEARCH_PARAMETERS = 0x7B2672C0,
        CONTROLS_MANAGER = 0xB39AD60E,
        CONTROL_MAPPING_SET = 0x18C2C097,
        CONTROL_PLATFORM_IMAGE = 0xDD4E76DD,
        CONVEX_EXTRUDED_SHAPE = 0x73556CDA,
        CONVEX_MESH_SHAPE = 0x79410E1F,
        COST_SETTINGS = 0x36247029,
        CREW_CATEGORY = 0xA3728D8B,
        CREW_CROWD_LIFE_POINT_GROUP = 0xA9986CB0,
        CREW_DESCRIPTOR = 0xFE6623EE,
        CREW_INVENTORY_ITEM_SETTINGS = 0x8C3EBF12,
        CREW_LIFE_STATION_SET = 0x686983BB,
        CROWD_DUTY_REGION = 0x5B8B9990,
        CROWD_STATION_DESCRIPTION_LABEL = 0x2929B3C5,
        CSS_MANAGER = 0xAE849E18,
        CSS_PRESET = 0x52B36CC4,
        CUSTOM_PLATFORM_IMAGE_V2 = 0x6A594F43,
        CUSTOM_WEAPON_ACTION_PACK = 0x23226A61,
        CYLINDER_SHAPE = 0x445B37F9,
        DAMAGE_RULES = 0x1A0FDFBE,
        DATA_DRIVEN_ENUM_DEBUG_MANAGER = 0x37CFF500,
        DATA_DRIVEN_ENUM_DEFINITION = 0xAE00BCDA,
        DATA_EXCLUSION_REGION_CELL_DATA = 0xA4DCAD02,
        DATA_LAYER = 0x41813744,
        DECAL_SETTINGS = 0xF6E4569A,
        DECISION_TREE = 0x763A3D0E,
        DEIMOS_COMMUNITY_MEMBER = 0x98E29B46,
        DESYNC_PROMPT_PAGE_FACTORY_SETTINGS = 0xB5845172,
        DIALOGUE_CAMERA_BLEND = 0xB3852142,
        DIALOGUE_CAMERA_HANDHELD_NOISE = 0xB5BFD71D,
        DIALOGUE_CAMERA_IN_TRANSITION = 0x713B595B,
        DIALOGUE_CAMERA_LENS = 0x83CDE514,
        DIALOGUE_CAMERA_OUT_TRANSITION = 0x52CC5BF1,
        DIALOGUE_FACIAL_PUNCTUATION_GROUP = 0xF4A7303E,
        DIALOGUE_IDLE_STATE = 0x054BA23E,
        DIALOGUE_LIGHT_RIG = 0xB2BD6051,
        DIALOGUE_LIVE_CAMERA = 0x2AA901BA,
        DIALOGUE_LOCATION_PROFILE = 0xC50434A1,
        DIALOGUE_SMART_CAMERA = 0x1671C445,
        DIALOGUE_STAGE = 0xE7388BE2,
        DIALOGUE_UNIVERSE_COMPONENT = 0x9A2536FD,
        DISMANTLING_DATA = 0x5B24052A,
        DISMANTLING_SETTINGS = 0x28B7BCE1,
        DISTRICT_REGION_CELL_DATA = 0x3306CD8E,
        DOMINO_SCRIPT_DEFINITION = 0xE802B9DA,
        DOOR_SOUND_SET = 0x3BC3DBBD,
        ECONOMIC_SYSTEM_SETTINGS = 0xC2F2E93E,
        ELEVATOR_SOUND_SET = 0xE06B3C16,
        ELITE_BOAT_RANK_INFO_TEMPLATE = 0x8382FB22,
        ELITE_RANK_INFO_TEMPLATE = 0xD58F597D,
        ENEMY_RANK_INFO = 0x9B0EC1F1,
        ENGINE_LOCALIZATION_REPOSITORY = 0x6A43BCD4,
        ENGINE_OPTIONS = 0xB4E69FA1,
        ENTITY = 0x0984415E,
        ENTITY_BUILDER = 0x971A842E,
        ENTITY_GROUP = 0x3F742D26,
        ENTITY_GROUP_BUILDER = 0xAADF2263,
        ENTITY_LABEL = 0x5C13390B,
        ENTITY_POSITIONS_TEMPLATE = 0xE1DF65C3,
        EPHEMERAL_STATION_HOST_PRESET = 0x1A4D3B35,
        EQUIPMENT_RARITY_SWITCH_SET = 0x5E8A141E,
        EQUIPMENT_SOUND_SET = 0x0754840C,
        EXPLORATION_MANAGER_DATA = 0x02AE8BAF,
        EXPRESSION_PACK = 0x7D4BD0B7,
        FAKE_DISTANCE_CULLING_FACTOR_REGION_CELL_DATA = 0xB3C38C09,
        FAKE_GRAPHIC_OBJECT = 0xD31EB8DB,
        FAR_SHADOWS_SETTINGS = 0xC0B85C92,
        FAST_TRAVEL_ENTRY = 0x9FF4770F,
        FAST_TRAVEL_MANAGER = 0x125CFF33,
        FIGHT_LOGIC_ACTION_STATE_ATTACK = 0x9B427871,
        FIGHT_LOGIC_ACTION_STATE_HIT_REACT = 0xE7A04B3C,
        FIGHT_LOGIC_ACTION_STATE_MOVEMENT = 0xE54A4B81,
        FIGHT_LOGIC_ACTION_STATE_PAIRED = 0x813E99C1,
        FIGHT_LOGIC_ACTION_STATE_RANGED = 0x4BCC61E8,
        FIGHT_LOGIC_ACTION_STATE_SPECIAL_ABILITY = 0xD4FB4035,
        FIGHT_LOGIC_ACTION_STATE_SPECIAL_ABILITY_PAIRED = 0x8DB30BA0,
        FIGHT_LOGIC_REPOSITORY = 0x39A52B1E,
        FIGHT_LOGIC_SET = 0x3BE32838,
        FIGHT_LOGIC_STANCE_STATE = 0x24349DF3,
        FIGHT_STAGE_SET_COLLECTION_DATA = 0x1EB2B1FF,
        FIRE_DAMAGE = 0x10B32D38,
        FIRE_ENERGY = 0x966A00C5,
        FIRE_FX_TEMPLATE = 0x594E0612,
        FIRE_PROPAGATION_CONDITIONS = 0xBAB4FF48,
        FIRE_SETTINGS = 0x124BF076,
        FONT_FILE = 0xC46B4618,
        FONT_MANAGER = 0x61D83BBA,
        FX_MATERIAL_OVERLAY_PROPERTY_MAPPING = 0x71704EA6,
        FBR_DATA_FILE = 0x2A8F0073,
        GAMEPLAY_POSITIONS_CONTAINER = 0x4FFDE80D,
        GAME_BOOTSTRAP = 0xE5A83560,
        GAME_FIX = 0x2CC42429,
        GAME_FLOW_IMPL = 0x4005ED29,
        GAME_OBJECT_GROUP_FACTORY = 0x9755C0A5,
        GAME_OBJECT_QUERY_DEFINITION = 0xF0497042,
        GAME_PLAY_SETTINGS = 0x91831427,
        GAME_POINT_TAG = 0x9D60DD0D,
        GAME_POINT_TAG_DICTIONARY = 0x1EBF7C6F,
        GAME_SETTING = 0x52534498,
        GENERAL_SCRIPT_DEFINITION = 0xC01E5105,
        GENERIC_NPC_COMMUNITY_MEMBER = 0xABE3061A,
        GENERIC_REGION_CELL_DATA = 0x44F7C7A6,
        GI_DATABLOCK = 0xF7D22B3C,
        GI_LOADER = 0x9ED472D4,
        GI_SETTINGS_REGION = 0xA5457642,
        GI_TIME_OF_DAY_SEGMENTS = 0x1767D3F8,
        GLOBAL_FIGHT_STAGE_CONDITION_CONTAINER = 0xCF74BDB4,
        GLOBAL_FIGHT_STAGE_MOVEMENT_SETTINGS_DEFINITION_CONTAINER = 0xDC0E5630,
        GLOBAL_FIGHT_STAGE_SET_DEFINITION_CONTAINER = 0x871FF875,
        GLOBAL_FIGHT_STAGE_WRAPPER_CONTAINER = 0x59227F24,
        GLOBAL_HURT_SEQUENCE_MAPPING_WRAPPER = 0x88F76017,
        GLOBAL_LIGHTING_SCALE_CURVE = 0x3B979A5C,
        GLOBAL_NAMED_FIGHT_LOGIC_CONDITION = 0x7EA406FE,
        GLOBAL_NAMED_FIGHT_LOGIC_OPERATION = 0xDAF328D0,
        GOAP_GOAL_DEFINITION = 0x89DA528D,
        GRAPHICS_CONFIG = 0xB1420AD1,
        GOAP_ACTION_DEFINITION = 0xFF4573C6,
        GOAP_PLANNER_DEFINITION = 0xD51B1AC3,
        HAIR_MESH = 2121000489,
        HAY_STACK_SOUND_SET = 0xA67CBF25,
        HTML_ENTITY = 0xC82C1794,
        HUDAFS_MODULE = 0x5D6E7C64,
        HUD_LOOT_MODULE = 0x86492EFC,
        HUD_VISIBILITY_MANAGER_GAME = 0x3DCB7278,
        HUMAN_FX_PACK = 0xBA42B58E,
        HURT_BOX_CATEGORY = 0x3E222091,
        HURT_SEQUENCE_MAPPING = 0x4C8287C2,
        IMPACT_DATA = 0x032F4C94,
        IMPOSTOR = 0xDFCD6E51,
        IMPOSTOR_SHADOW_DISTANCE_REGION_CELL_DATA = 0x08E0AF91,
        INQUIRY = 0x95843A6C,
        INVENTORY_ITEM_LEVEL_VALUE_MODIFIER = 0x103BB309,
        INVENTORY_ITEM_SETTINGS = 0xC69075AB,
        INVENTORY_SETTINGS = 0x07DCB7E8,
        INVESTIGATION_CLUE = 0xFDA4333C,
        INVESTIGATION_SETTINGS = 0x2CDBAEAA,
        ITEM_PERK_AI_CONDITION_CHALLENGE = 0xA7216802,
        ITEM_PERK_BOOK_CHALLENGE = 0x75798493,
        ITEM_PERK_CHALLENGE_GROUP = 0x9D2C7B46,
        ITEM_SET = 0x65CACDCE,
        KEYBOARD_PLATFORM_IMAGE_V2 = 0xDEED9BC9,
        KINO_BOOT_STRAP_DATA = 0x0549696C,
        KINO_EXTERNAL_RESOURCE_HOLDER = 0xDFC831E3,
        KINO_EXTERNAL_STATE_DATA = 0xB19D4FCD,
        KINO_GRAPH_DATA = 0x3527D1AE,
        KINO_MARKUP_DICTIONARY_DATA = 0xBBA2898E,
        KINO_REPLACE_FAMILY_DATA = 0x28941138,
        KINO_REPLACE_SET_DATA = 0x3F49D5C9,
        KINO_REPLACE_SYSTEM_DATA = 0x235C0560,
        KINO_REPLACE_TREE_DATA = 0x2C1329D2,
        KINO_RUNTIME_DATA = 0x149E0A31,
        KINO_TAG_CATEGORY_DATA = 0xC76EED91,
        KINO_TAG_SYSTEM_DATA = 0x92F31FA2,
        LAYERED_SKY_SETTINGS = 0x984B79C1,
        LIGHT_INTENSITY_GROUP_TIME_OF_DAY_SEGMENTS = 0x3D4BBCBC,
        LIGHT_OPTIMIZED_ATTRIBUTE = 0xBEEE5EB1,
        LIST_SHAPE = 0x86EBFD8D,
        LITE_RAGDOLL = 0x891043D5,
        LOADING_RANGE_FACTOR_REGION_CELL_DATA = 0x5F7EE5EB,
        LOADING_RANGE_FACTOR_REGION_LAYOUT = 0x73FC7A69,
        LOCALIZATION_COLLECTION = 0xDF503E1F,
        LOCALIZATION_MANAGER = 1248910915,
        LOCALIZATION_PACKAGE = 0x6E3C9C6F,
        LOCAL_CUBE_MAP = 0xEEBB2443,
        LOCAL_VISUAL_AMBIANCE_MODIFIER = 0xCD9FD300,
        LOCATE_GAMEPLAY_SETTINGS = 0xB4F81BD5,
        LOCATE_SETTINGS = 0xCDA51B83,
        LOD_MESH_SHAPE_SETTINGS = 0x60043875,
        LOD_SELECTOR = 0x51DC6B80,
        LOOK_AT_DATA_SKELETON_DEFINITION = 0xD86B9F28,
        LOOT_FX_PACK = 0x32F0C478,
        LOOT_GLOBAL_SETTINGS = 0x50A3159E,
        LOD_MESH_SHAPE = 0xC87F5C22,
        MAIN_BEHAVIOR_TARGET_TRACKER = 0x07894C47,
        MAIN_SKILL_CONTAINER = 0xA4ED7571,
        MASK_16 = 2461622132,
        MATERIAL = 0x85C817C3,
        MATERIAL_INFO = 2560476850,
        MATERIAL_SOUNDS_MANAGER = 0xE9E772C7,
        MATERIAL_TEMPLATE = 0xBCFB3C7A,
        MENU_MANAGER_GAME = 0x3DDA14EE,
        MENU_PAGE_REPOSITORY = 0xFC609F9F,
        MENU_PAGE_STATE = 0x8BAD589E,
        MENU_PAGE_STATE_REPOSITORY = 0xBE12437C,
        MESH = 0x415D9568,
        MESH_DATA = 105229237,
        MESH_DECAL_SETTINGS = 0x943A5618,
        MESH_INSTANCE_DATA = 1399756347,
        MESH_PRIMITIVE = 2775812079,
        MESH_SHAPE = 0xB22B3E61,
        MESH_SHAPE_TRIANGLE_MATERIAL_DATA = 2407545263,
        MESH_SOURCE = 3981995930,
        META_AI_AVOIDANCE = 0x7A31D05D,
        META_AI_BEHAVIOUR_AMBUSH = 0x22543BF1,
        META_AI_BEHAVIOUR_ANIMAL_RETURN_TO_HABITAT = 0x1AE28989,
        META_AI_BEHAVIOUR_ANIMAL_SATISFY_NEEDS = 0x287293B6,
        META_AI_BEHAVIOUR_ANIMAL_SETUP_DEBUG_CONTEXT = 0x70651A43,
        META_AI_BEHAVIOUR_ATTACK = 0xF2328E06,
        META_AI_BEHAVIOUR_BOAT_FOLLOW_LEADER = 0xAE232D43,
        META_AI_BEHAVIOUR_BOAT_STAND_STILL = 0x7A1482F3,
        META_AI_BEHAVIOUR_DEFEATED = 0xC0726347,
        META_AI_BEHAVIOUR_DISMOUNT_RIDEABLE = 0x2560ED8C,
        META_AI_BEHAVIOUR_FLEE_TO_AI_LOCATION = 0x28F7E112,
        META_AI_BEHAVIOUR_GOTO = 0x3C7A5836,
        META_AI_BEHAVIOUR_HERD_FOLLOW = 0x3C66F19E,
        META_AI_BEHAVIOUR_HERD_SATISFY_NEEDS = 0xC3CFA70F,
        META_AI_BEHAVIOUR_IMPRISONED = 0x12DD8985,
        META_AI_BEHAVIOUR_INJURED = 0x6322BF0F,
        META_AI_BEHAVIOUR_NAVAL_ATTACK = 0x5CDDE146,
        META_AI_BEHAVIOUR_NAVAL_GUARD_ZONE = 0x44DFE287,
        META_AI_BEHAVIOUR_NAVAL_PATROL_ZONE = 0xFD220312,
        META_AI_BEHAVIOUR_PATROL_LOCATIONS = 0x4BD28A06,
        META_AI_BEHAVIOUR_PET = 0x9550CA12,
        META_AI_BEHAVIOUR_RAID = 0x399D6705,
        META_AI_BEHAVIOUR_RETURN_TO_SPAWNER = 0x15F12BC3,
        META_AI_BEHAVIOUR_SCRIPTED_GOTO = 0xD0381284,
        META_AI_BEHAVIOUR_SEARCH = 0x0102789A,
        META_AI_BEHAVIOUR_STAND_STILL = 0x124C623B,
        META_AI_BEHAVIOUR_TRANSPORTING_DEAD_BODY = 0x0637F5F3,
        META_AI_BEHAVIOUR_UNARMED_GOTO = 0xAAD001E8,
        META_AI_BEHAVIOUR_UNCONSCIOUS = 0x78289F9C,
        META_AI_BEHAVIOUR_WANDER_ON_AI_NETWORK = 0x3B068631,
        META_AI_COMPONENT_SETTINGS = 0x72ACA389,
        META_AI_CONTEXT_TABLE = 0x7F0FAEEA,
        META_AI_DEAD_BODY_DATA = 0xCF629B29,
        META_AI_FACTION_DATA = 0x350B1328,
        META_AI_GROUP_DATA_CONTEXT_TABLE = 0xE05CB077,
        META_AI_OBJECTIVE_AMBUSH = 0x8AD0D3F1,
        META_AI_OBJECTIVE_ANIMAL = 0x77DD6822,
        META_AI_OBJECTIVE_ANIMAL_HERD = 0xD026BAD0,
        META_AI_OBJECTIVE_ANIMAL_LOCATION = 0x41B0BF1A,
        META_AI_OBJECTIVE_ANIMAL_SPAWNER = 0x090908DA,
        META_AI_OBJECTIVE_BASE = 0xFAF0455F,
        META_AI_OBJECTIVE_BOAT_VEHICLE = 0xD33412F4,
        META_AI_OBJECTIVE_BOUNTY_SPONSOR_SPAWNER = 0x281DA5D6,
        META_AI_OBJECTIVE_BRAZIER = 0xDE8D757A,
        META_AI_OBJECTIVE_CAGE = 0x6CE2F56F,
        META_AI_OBJECTIVE_FIGHT_POCKET = 0x52940403,
        META_AI_OBJECTIVE_HUMAN = 0x64398CE5,
        META_AI_OBJECTIVE_MILITARY_FORMATION = 0xC97DBAD8,
        META_AI_OBJECTIVE_NAVAL_PATROL_ROUTE = 0x4D9F388B,
        META_AI_OBJECTIVE_PERSISTENT_CHARACTER_SPAWNER = 0x6DE7BDC4,
        META_AI_OBJECTIVE_RIDEABLE_ANIMAL = 0x8A7BFE04,
        META_AI_OBJECTIVE_SMALL_WATER_VEHICLE = 0x7106A32C,
        META_AI_OBJECTIVE_SPAWNER = 0xA5C98D53,
        META_AI_OBJECTIVE_STATIC = 0x9DB66318,
        META_AI_OBJECTIVE_STATIC_CAGE = 0x15890DA1,
        META_AI_OBJECTIVE_WEAPON_RACK = 0x162E20AF,
        META_AI_REACTION_BEHAVIOUR_SUMMONED_RIDEABLE = 0xB5EBC133,
        META_AI_RESTRICTION_ZONE_LIST = 0x68BD22A6,
        META_AI_SCHEDULE = 0x47171155,
        META_AI_SCRIPTED_DATA_SETTINGS = 0xF173125D,
        META_AI_VIRTUAL_DESCRIPTOR_DYNAMIC_LABEL = 0x9D3DC730,
        META_AI_VIRTUAL_DESCRIPTOR_LOCATION_LABEL = 0xB03ECA17,
        META_AI_ZONE_CELL_DATA = 0x021D77CE,
        MIPMAP = 491489187,
        MOTION_SOFT_BODY = 0x9895FF0A,
        MULTI_DECAL = 0x519A86F6,
        NAVAL_BLACKBOARD_KEY_REPOSITORY = 0x1093CE1E,
        NAVAL_CREW_AGENDA_TREE = 0xD399A75C,
        NAVAL_GAME_PLAY_SETTINGS = 0x5A07FB28,
        NAVAL_GROUP_DATA = 0xBECA6AAC,
        NAVAL_OBJECT_MODIFIERS_CATEGORY = 0xBC6E3CF1,
        NAVAL_PROGRESSION_SETTINGS = 0x15C566F8,
        NAVIGATION_PARAMETERS = 0xFD04FB88,
        NAVIGATION_SELECTOR_BLENDER_NODE = 0x5E7D0E5B,
        NAVIGATION_SELECTOR_NODE = 0xC75A8DF2,
        NAV_MESH_MANAGER = 0x0B4CE0E0,
        NEW_GAME_PLUS_SETTINGS = 0x8239DC20,
        NPC_COMMUNITY = 0xD02C1538,
        NPC_COMMUNITY_DLC_ADDON = 0x8EA17C62,
        NPC_SCRIPT_DEFINITION = 0x076C3170,
        NPC_CONTEXT_DATA_SET = 0xD40226D8,
        OASIS_VOICE_TABLE_ITEM = 0x3A250224,
        OBJECT_MODIFIER_DEFINITION = 0xAC2EB537,
        OBJECT_MODIFIER_REPOSITORY = 0x98E0CD22,
        OBJECT_MODIFIER_REPOSITORY_DLC_ADDON = 0xF55FD1E4,
        OBJECT_MODIFIER_TIER_CONTAINER = 0x63B8DB6A,
        OBJECT_MODIFIER_TRAIT_SETTINGS = 0x06A16E30,
        OBJECT_PACK = 0xF29157B2,
        OCCLUSION_MESH = 0xF1D48F2B,
        OFFLINE_GLYPHS = 0xA50273B2,
        OMNI_COOKIE = 0xC6D6F7E4,
        OMNI_LIGHT = 0x344780D6,
        OUT_OF_BOUNDS_SETTINGS = 0x71CD9E46,
        OUT_OF_COMMISSION_NPC_DATA = 0xBC2B1688,
        PAD_PLATFORM_IMAGE_V2 = 0x36D8BF51,
        PARKOUR_SETTINGS = 0xBFF852C8,
        PERCEPTION_SETTINGS = 0x0CF87690,
        PERSISTABLE_OPTIONS_PROFILE = 0x8F07EDFF,
        PERSISTENT_BACKGROUND_BANK = 0x2EBF8BC0,
        PERSISTENT_CHARACTER_SETTINGS = 0xF784A8B7,
        PHOTO_MODE_ADDONS_MANAGER = 0xF388BE76,
        PHOTO_MODE_FACIAL_EXPRESSION_ITEM = 0xBA154A00,
        PHOTO_MODE_FILTER_PRESET = 0x4FDB2F0C,
        PHOTO_MODE_FRAME_OVERLAY = 0x3D7948E0,
        PHOTO_MODE_GRID = 0x35739538,
        PHOTO_MODE_MANAGER = 0x5FD662D8,
        PHOTO_MODE_OPTION_ITEM = 0xE1A2A8DD,
        PILOT_CONFIG_DATA = 0x0889691F,
        PLANNER_ACTION_TAG = 0x70FD5525,
        PLANNER_ACTION_TAG_CATEGORY = 0x6F9F3BBD,
        PLATFORM_IMAGE_MANAGER = 0x70609AC4,
        PLAYER_PROGRESSION_MANAGER = 0x9713A15F,
        PLAYER_SETTINGS = 0xC4EBA473,
        POINT_OF_INTEREST_COMPLETION_SETTINGS = 0xFDDEA1FB,
        POISON_TEMPLATE = 0xB271782D,
        POSITION_LABEL = 0x32A56A5C,
        PREMADE_PERSISTENT_UNIT = 0x3FAEF0A5,
        PROBABILITY_BAG = 0x58D4FC40,
        PROBABILITY_TABLE = 0x1850AAAC,
        PROCEDURAL_ENTITY = 0x95905827,
        PROCEDURAL_ENTITY_GROUP = 0x207C7937,
        PROCEDURAL_MESH = 0xE99D3578,
        PROCEDURAL_QUEST_DATABASE = 0x9CEBDA4E,
        PROCEDURAL_QUEST_EXPRESSION = 0x32A8F6E1,
        PROCEDURAL_QUEST_MOTIVATION = 0x0AF8D30D,
        PROCEDURAL_QUEST_OBJECTIVE_PRESET = 0xF5711C5F,
        PROCEDURAL_QUEST_QUEST_GIVER_EPHEMERAL_NPC = 0xEB69C2C1,
        PROCEDURAL_QUEST_SCHEDULER_QUEST_GROUP = 0x563267B2,
        PROCEDURAL_QUEST_TARGET_EPHEMERAL_STATIC_ENTITY = 0x8978F8F6,
        PROCEDURAL_QUEST_TASK = 0x3608016C,
        PROJECTILES_SETTINGS = 0xA3E78AC6,
        PROTOTYPE_SCRIPT_DEFINITION = 0x6A59F76E,
        PUSHABLE_OBJECT_SOUND_SET = 0x6FBF5034,
        QUEST_DEFINITION = 0x4469BE53,
        QUEST_FLOW = 0xEC4C221F,
        QUEST_FLOW_INSTANCE = 0xD3413873,
        QUEST_GAMEPLAY_DATA = 0x207EE531,
        QUEST_HISTORY_PRESET = 0x6FFEA3B9,
        QUEST_INSTANCE = 0x2857C44B,
        QUEST_LABEL = 0xD314A0D5,
        QUEST_MANAGER = 0x2D7142A8,
        QUEST_MUSIC_SETTINGS = 0x3B0217D3,
        QUEST_UI_CATEGORY = 0x379AA683,
        RAGDOLL_SKELETON = 0x4BE653CF,
        RAIN_BLOCKER_SETTINGS_REGION = 0x122E3BFB,
        RANK_MANAGER = 0xC4A59FC7,
        REACH_HIGH_POINT_SOUND_SET = 0x146CB00F,
        REACTION_DATA_TREE = 0x4A37269A,
        REACTION_MANAGER = 0xF85C3B4F,
        REACTION_PACK = 0x2EE12657,
        REACTION_TREE = 0x0E0126A8,
        REACTION_TREE_ADDON = 0x4A0BDD77,
        REFERENCE_LIST_SHAPE = 0x2D675BA2,
        REGION_LAYOUT = 0x17FB5AA8,
        REWARD_CATEGORY = 0xACA11FCA,
        REWARD_DEFINITION = 0x62A7F9CB,
        REWARD_MANAGER = 0x1D6E71D7,
        RFX_SOUND_INSTANCE = 0x2E27628B,
        RIDEABLE_TARGET_TRACKER = 0x26529FF6,
        SANDBOX_MUSIC_SETTINGS = 0x56C35203,
        SAVE_GAME_DATA_MANAGER = 0x575C803B,
        SHADER_CUSTOM_OPERATOR_DEFINITION = 0xD01DA6C7,
        SHADOWS_SETTINGS = 0x680456EA,
        SHADOW_RIGID_BODY = 0x1CBDE084,
        SHOP_LABEL = 0xD1406B2F,
        SKELETON = 0x24AECB7C,
        SKILL = 0x9FFCCB73,
        SKILL_LOAD_ON_DEMAND_SETTINGS = 0x105BD7A9,
        SKILL_SYSTEM = 0xE76DE4CA,
        SKILL_SYSTEM_ADDONS = 0xD40F9637,
        SMART_OBJECT_DEFINITION = 0xA9EE606C,
        SOFT_BODY_SETTINGS = 0xF7A7E4DF,
        SOFT_BODY_SETTINGS_OVERRIDE_BY_TYPE = 0xCECBE216,
        SOUND_BANKS_LOAD_ON_DEMAND = 0x49F387B1,
        SOUND_DAMPING_METHOD = 0x039A8532,
        SOUND_FWALLA_SETTINGS = 0x192C036F,
        SOUND_INIT_SETTINGS = 0x8DDA228D,
        SOUND_MATCHING_PACKAGE = 0xA807AF51,
        SOUND_MATCHING_PACKAGE_COLLECTION = 0xA05A52B9,
        SOUND_NPC_INFO = 0x6E6AF3F9,
        SOUND_OCCLUSION_PRESET = 0x7564FD67,
        SOUND_PORTAL_OBSTRUCTION_PRESET = 0x92B0BB7C,
        SOUND_RFX_DATA = 0xACBCAA50,
        SOUND_TRALLA_GROUP = 0xB90B50AE,
        SOUND_VOICES_SETTINGS = 0x7A4D89B1,
        SOUND_VOICE_ACTIVE_STATE_SETTINGS = 0x20B8F41A,
        SOUND_VOICE_NPC_TYPE = 0x1122101C,
        SOUND_WALLA_LOCATION_DATA = 0xE7B83453,
        SOURCE_MUSIC_SETTINGS = 0xB5C43265,
        SPECIAL_ABILITY_REPOSITORY = 0x08B71F46,
        SPECIAL_ABILITY_SETTINGS = 0x283EDECE,
        SPHERE_SHAPE = 0xFA3F7A18,
        SPLASH_FX = 0x755ACE14,
        SPLASH_VECTOR_FIELD_DATA = 0x3126E5EE,
        SPOT_LIGHT = 0x80320FB8,
        STATIC_PERMUTATION = 0xBED73BE9,
        STATUS_EFFECT_PACK = 0x3DC6D919,
        STRING_REPLACEMENT_COLLECTION = 0xA5CD7166,
        SUB_QUEST_FLOW_DEFINITION = 0x8F3980B1,
        SUMMONED_RIDEABLE_SETTINGS = 0x092689F8,
        SUMMONED_RIDEABLE_SETTINGS_DLC_ADDON = 0xF9111953,
        SYSTEMIC_FIRE_SOUND_SET = 0xD56E8C60,
        TAG_DICTIONNARY = 0x0196529F,
        TAG_RULES = 0x02071BC3,
        TERRAIN = 130771501,
        TERRAIN_MATERIAL_COMPILED_DATA = 0x24C84277,
        TERRAIN_NODE_DATA = 0x4E4353C1,
        TEST_SCRIPT_DEFINITION = 0x6BB1DC5E,
        TEXTURE_BASE = 535104481,
        TEXTURE_FILE = 0x53CEC390,
        TEXTURE_GRADIENT = 610229413,
        TEXTURE_MAP = 0xA2B7E917,
        TEXTURE_MAP_SELECTOR = 0x828AB4C6,
        TEXTURE_MAP_SPEC = 0x989DC6B2,
        TEXTURE_SET = 0xD70E6670,
        TEXTURE_SOURCE = 2849914618,
        THEATER_CAMERA_MANAGER = 0x0569FB9E,
        THEATER_CAMERA_SELECTOR_NODE = 0xB25B947C,
        THEATER_CINEMATIC = 0x28435F00,
        THEATER_CONFIGURATION = 0x695E121A,
        THEATER_DATA = 0x84EC653F,
        THEATER_NAVIGATION_MANAGER = 0xB98E4068,
        THEATER_SCRIPT_DEFINITION = 0x9543E286,
        THREAT_LEVEL_MUSIC_SETTINGS = 0x0E830CA4,
        THREAT_VALUE_TABLE = 0xF740B947,
        TIME_OF_DAY_PROPERTY_CONTROLLER_DATA = 0xF0C85018,
        TIME_SAVER_MAP_LOCATION_LIST = 0xB99078B9,
        TOKEN_SETTINGS = 0x350A7198,
        TRAVERSABLE_CRACK_SOUND_SET = 0x6ED380C9,
        TUTORIAL = 0x3F1D9DBF,
        TUTORIAL_MANAGER = 0x72649D35,
        TUTORIAL_SETTING = 0x17340014,
        UI_CATEGORY = 0x549E34D8,
        UI_ENUM_BINDING = 0x2E772CD8,
        UI_FACTION_INFO = 0x7046ED88,
        UI_ICON_SETTINGS = 0xE1AB221D,
        UI_ICON_SETTINGS_LEGEND_ITEM = 0x9EDE9DCC,
        UI_ICON_SETTINGS_MANAGER = 0x10C0CDCC,
        UI_INFO_CONTEXT_CONTAINER = 0xB39EE3B6,
        UI_INVENTORY_CATEGORY = 0xD57C042B,
        UI_ITEM_INFO = 0x9317D1B8,
        UI_ITEM_INFO_OVERRIDE = 0xFBA7CEB3,
        UI_LIGHTING_SETUP = 0x225C4723,
        UI_LOCALIZATION_REPOSITORY2 = 0xE93750FC,
        UI_LOCATION_CONTAINER = 0xA5CC3107,
        UI_MANAGER_GAME = 0x2A6590B7,
        UI_PRESENT_DAY_AUDIO_FILE_ENTRY = 0x27CC6824,
        UI_PRESENT_DAY_DOCUMENT_FILE_ENTRY = 0x2F69EA3C,
        UI_PRESENT_DAY_FILE_IMAGE_CONTENT_ENTRY = 0x797D8033,
        UI_SOUNDS = 0xEE3AB005,
        UNIQUE_ANIMAL_COMMUNITY_MEMBER = 0xB72BB0E4,
        UNIQUE_BOAT_COMMUNITY_MEMBER = 0x21784D2C,
        UNIQUE_NPC_COMMUNITY_MEMBER = 0x25FD8E9D,
        UNIQUE_RIDEABLE_ANIMAL_COMMUNITY_MEMBER = 0xCD123EF9,
        UNIT_TRAIT_MANAGER = 0x7E09EB0B,
        UNIVERSE = 0x98435A63,
        VALIDATION_SCRIPT_MANAGER = 0x75C605FD,
        VEHICLE_DAMAGE_THRESHOLD_TABLE = 0x84E08BE7,
        VIRTUAL_ENTITY_DEFINITION = 0x7565942C,
        VIRTUAL_ENTITY_NAME_TAG = 0xAA0E379D,
        VISION_MODE = 0x52AD75E9,
        VISUAL_AMBIANCE_REGION_CELL_DATA = 0x7BDC0DCF,
        VISUAL_BOW_SPLASH_COMPONENT_DATA = 0xB35053D0,
        VISUAL_DAMAGE_SETUP = 0x9A29D794,
        VOICE_MANAGER = 0x135DA422,
        VOLLEY_CONTACT_MATERIAL_DATA_PACK = 0x937482A8,
        WATER_BOAT_BEHAVIOR_DATA = 0x7DBA7FDE,
        WATER_BOAT_HANDLING_DATA = 0xEEFA3781,
        WATER_BOAT_SETTINGS = 0x9FBA51D1,
        WATER_BOAT_SOUND_SET = 0xA84D0B26,
        WATER_BOAT_VISUAL_DATA = 0x2369697B,
        WATER_FLOATING_OBJECT_SETTINGS = 0x8CCCE798,
        WATER_MESH = 0x4F6E0B3E,
        WATER_OBJECT_SPLASH_SETTINGS = 0x3861A620,
        WATER_VEHICLE_DESTRUCTIBLE_POINTS_CONTAINER = 0xDD512BAB,
        WATER_VEHICLE_HEALTH_STATE_CONTAINER = 0xEFA54A58,
        WATER_VEHICLE_SEPARATED_DESTRUCTIBLE_POINT_STATE = 0x8A5B94DF,
        WEAPON_SOUND_SET = 0x37DF4185,
        WEATHER_PROPERTY_CONTROLLER_DATA = 0x887004DC,
        WEIGHTED_TABLE = 0xA75E1E80,
        WIND_PROPERTY_CONTROLLER_DATA = 0xBC116237,
        WORLD = 0xFBB63E47,
        WORLD_BOUNTY_SETTINGS = 0xD4CDE33E,
        WORLD_DATA_LAYER_MANAGER = 0xECEF6DDB,
        WORLD_LOGIC_SETTINGS = 0x58132076,
        WORLD_MAP_MANAGER = 0x2B0695D0,
        WORLD_PATH_MANAGER = 0x3B9EE6E6,
        WORLD_PATH_NAVIGATION_SETUP = 0x90AE4959,
        WORLD_STATE_PACKAGE = 0x50BF2581,
        WORLD_TRANSITION_PORTAL = 0x7C00F6AF,
        ZIP_LINE_SOUND_SET = 0x2FF673C7,
        _NONE = 0xFFC5A970,
        NONE = 0x0
    }
}
