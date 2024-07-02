using CodeX.Core.Engine;
using CodeX.Core.Numerics;
using CodeX.Core.Utilities;
using CodeX.Games.ACOdyssey.Files;
using CodeX.Games.ACOdyssey.Resources;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;

namespace CodeX.Games.ACOdyssey.FORGE
{
    public class ForgeFileManager : FileManager
    {
        public override string ArchiveTypeName => "FORGE";
        public override string ArchiveExtension => ".forge";

        public ForgeDataFileMgr DataFileMgr { get; set; }

        public ForgeFileManager(Game game) : base(game)
        {
        }

        public override bool Init()
        {
            if (ForgeCompression.Init(Folder))
            {
                LoadStartupCache(); //9s with cache vs 25s without
                return true;
            }
            return false;
        }

        public override void InitFileTypes() //Extensions were added manually
        {
            InitGenericFileTypes();
            InitFileType(".forge", "Archive", FileTypeIcon.Archive);
            InitFileType(".mesh", "Mesh", FileTypeIcon.Piece, FileTypeAction.ViewModels);
            InitFileType(".texture", "Texture", FileTypeIcon.Image, FileTypeAction.ViewTextures);
            InitFileType(".material", "Material", FileTypeIcon.TextFile, FileTypeAction.ViewText);
            InitFileType(".lodselector", "LOD Selector", FileTypeIcon.TextFile, FileTypeAction.ViewText);
            InitFileType(".entity", "Entity", FileTypeIcon.TextFile, FileTypeAction.ViewText);
            InitFileType(".entitygroup", "Entity Group", FileTypeIcon.Piece, FileTypeAction.ViewModels);
            InitFileType(".gridcelldata", "Cell Data", FileTypeIcon.TextFile, FileTypeAction.ViewText);
            InitFileType(".world", "World", FileTypeIcon.TextFile, FileTypeAction.ViewText);
        }

        public override void InitArchives(string[] files)
        {
            foreach (var path in files)
            {
                var relpath = path.Replace(Folder + "\\", "");
                var filepathl = path.ToLowerInvariant();
                var isFile = File.Exists(path);
                Core.Engine.Console.Write("ForgeFileManager", Game.GamePathPrefix + relpath + "...");

                if (isFile)
                {
                    if (IsArchive(filepathl))
                    {
                        var archive = GetArchive(path, relpath);
                        if (archive?.AllEntries == null)
                            continue;

                        RootArchives.Add(archive);
                        var queue = new Queue<GameArchive>();
                        queue.Enqueue(archive);

                        while (queue.Count > 0)
                        {
                            var a = queue.Dequeue();
                            if (a.Children != null)
                            {
                                foreach (var ca in a.Children)
                                {
                                    queue.Enqueue(ca);
                                }
                            }
                            AllArchives.Add(a);
                        }
                    }
                }
            }
        }

        public override void InitArchivesComplete()
        {
            foreach (var archive in AllArchives)
            {
                if (archive.AllEntries != null)
                {
                    ArchiveDict[archive.Path] = archive;
                    foreach (var entry in archive.AllEntries)
                    {
                        if (entry is ForgeEntry fe)
                        {
                            EntryDict[fe.Path] = fe;
                            JenkIndex.Ensure(fe.ShortNameLower, "ACOdyssey");
                        }
                    }
                }
            }

            InitGameFiles();
            if (StartupCacheDirty)
            {
                SaveStartupCache();
            }
        }

        private void InitGameFiles()
        {
            Core.Engine.Console.Write("ACOdyssey.InitGameFiles", "Initialising ACOdyssey...");
            DataFileMgr ??= new ForgeDataFileMgr(this);
            DataFileMgr.Init();
            Core.Engine.Console.Write("ACOdyssey.InitGameFiles", "ACOdyssey Initialised.");
        }

        public override bool IsArchive(string filename)
        {
            return filename.EndsWith(ArchiveExtension);
        }

        public override GameArchive GetArchive(string path, string relpath)
        {
            if ((StartupCache != null) && StartupCache.TryGetValue(path, out GameArchive archive))
            {
                return archive;
            }
            var rpf = new ForgeFile(path, relpath);
            rpf?.ReadStructure();
            return rpf;
        }

        public override void InitCreateInfos()
        {
            //TODO
        }

        public override GameArchive CreateArchive(string gamefolder, string relpath)
        {
            throw new NotImplementedException();
        }

        public override GameArchive CreateArchive(GameArchiveDirectory dir, string name)
        {
            throw new NotImplementedException();
        }

        public override GameArchiveDirectory CreateDirectory(GameArchiveDirectory dir, string name)
        {
            throw new NotImplementedException();
        }

        public override GameArchiveFileInfo CreateFile(GameArchiveDirectory dir, string name, byte[] data, bool overwrite = true)
        {
            throw new NotImplementedException();
        }

        public override GameArchiveFileInfo CreateFileEntry(string name, string path, ref byte[] data)
        {
            return null;
        }

        public override void Defragment(GameArchive file, Action<string, float> progress = null)
        {
            throw new NotImplementedException();
        }

        public override void DeleteEntry(GameArchiveEntry entry)
        {
            throw new NotImplementedException();
        }

        public override string GetXmlFormatName(string filename, out int trimlength)
        {
            throw new NotImplementedException();
        }

        public override byte[] ConvertFromText(string text, string filename)
        {
            throw new NotImplementedException();
        }

        public override byte[] ConvertFromXml(string xml, string filename, string folder = "")
        {
            throw new NotImplementedException();
        }

        public override string ConvertToText(GameArchiveFileInfo file, byte[] data, out string newfilename)
        {
            newfilename = file.Name;
            data = EnsureFileData(file, data);

            if (data == null || file is not ForgeEntry entry)
            {
                return string.Empty;
            }

            if (file.Name.EndsWith(".celldata"))
            {

                var cellmap = new CellDataFile(entry);
                cellmap.Load(data);

                if (cellmap != null)
                {
                    return cellmap.ToString(AllArchives);
                }
            }
            else if (file.Name.EndsWith(".material"))
            {
                var material = new MaterialFile(entry);
                material.Load(data);

                if (material != null)
                {
                    return material.ToString(AllArchives);
                }
            }
            else if (file.Name.EndsWith(".entity"))
            {
                var entity = new EntityFile(entry);
                entity.Load(data);

                if (entity != null)
                {
                    return entity.ToString(AllArchives);
                }
            }
            else if (file.Name.EndsWith(".world"))
            {
                var world = new WorldFile(entry);
                world.Load(data);

                if (world != null)
                {
                    return world.ToString(AllArchives);
                }
            }
            else if (file.Name.EndsWith(".lodselector"))
            {
                var lod = new LODSelectorFile(entry);
                lod.Load(data);

                if (lod != null)
                {
                    return lod.ToString(AllArchives);
                }
            }
            return TextUtil.GetUTF8Text(data);
        }

        public override string ConvertToXml(GameArchiveFileInfo file, byte[] data, out string newfilename, out object infoObject, string folder = "")
        {
            throw new NotImplementedException();
        }

        public override AudioPack LoadAudioPack(GameArchiveFileInfo file, byte[] data = null)
        {
            throw new NotImplementedException();
        }

        public override DataBag2 LoadMetadata(GameArchiveFileInfo file, byte[] data = null)
        {
            throw new NotImplementedException();
        }

        public override T LoadMetaNode<T>(GameArchiveFileInfo file, byte[] data = null)
        {
            throw new NotImplementedException();
        }

        public override PiecePack LoadPiecePack(GameArchiveFileInfo file, byte[] data = null, bool loadDependencies = false)
        {
            data = EnsureFileData(file, data);

            if (data == null || file is not ForgeEntry entry)
                return null;

            if (file.NameLower.EndsWith(".mesh"))
            {
                var mesh = new MeshFile(entry);
                mesh.Load(data);

                if (loadDependencies)
                    LoadDependencies(mesh.Model);
                return mesh;
            }
            else if (file.Name.EndsWith(".entitygroup"))
            {
                var entGroup = new EntityGroupFile(entry);
                entGroup.Load(data);

                var group = new List<PiecePack>();
                var objects = GetEntitiesFromEntityGroup(entGroup);

                for (int i = 0; i < objects.Item1?.Length; i++)
                {
                    if (objects.Item1[i] is ForgeEntry entry1)
                    {
                        var piece = LoadPiecePack(entry1, objects.Item2?[i], false);
                        group.Add(piece);
                    }
                    else if (objects.Item1[i] is ForgeModel piece)
                    {
                        var temp = new MeshFile(piece.Name);
                        temp.ConstructPiecePack(piece);
                        group.Add(temp);
                    }
                }

                var pieces = InstanciatePieces(group, entGroup, objects.Item2!);
                var model = new MeshGroupFile(pieces, entGroup);
                model.Load(data);

                return model;
            }
            return null;
        }

        public override TexturePack LoadTexturePack(GameArchiveFileInfo file, byte[] data = null)
        {
            data = EnsureFileData(file, data);

            if (data == null || file is not ForgeEntry entry)
                return null;

            if (file.NameLower.EndsWith(".texture") && entry.ResourceType == "TEXTURE_MAP")
            {
                var tex = new TextureFile(entry);
                tex.Load(data);

                //Data is located somewhere else
                if (tex.Texture != null && tex.Texture.Data != null)
                {
                    var mipEntry0 = entry.Archive.AllEntries.FirstOrDefault(e => e.Name == tex.Texture.Name + "_TopMip_0");

                    //Data is in a different archive...
                    if (mipEntry0 == null)
                    {
                        foreach (var archive in AllArchives)
                        {
                            mipEntry0 = archive.AllEntries.FirstOrDefault(e => e.Name == tex.Texture.Name + "_TopMip_0");
                            if (mipEntry0 != null)
                            {
                                break;
                            }
                        }
                    }

                    var entries = DataFileMgr?.StreamEntries?[ForgeResourceType.MIPMAP];
                    if (entries.TryGetValue(JenkHash.GenHash(mipEntry0?.Name.ToLowerInvariant()), out ForgeEntry sEntry0))
                    {
                        var mipData0 = EnsureFileData(sEntry0, null);
                        if (mipData0 != null)
                        {
                            var mipReader = new DataReader(new MemoryStream(mipData0));
                            tex.Texture.Data = tex.Texture.ReadMipData(mipReader);
                        }
                    }               
                }
                return tex;
            }
            return null;
        }

        public List<PiecePack> InstanciatePieces(List<PiecePack> pieces, EntityGroupFile groupFile, List<byte[]> data)
        {
            if (groupFile.EntityGroup.Components == null || data == null)
            {
                return pieces;
            }

            int c, i;
            var instanciatedPieces = new List<PiecePack>();
            for (c = 0; c < groupFile.EntityGroup.Components.Length - 2; c++)
            {
                var piece = (MeshFile)pieces[c];
                if (piece == null)
                {
                    continue;
                }

                var matrices = groupFile.EntityGroup.Components[c].InstanceData?.InstanceMatrices;
                if (matrices != null)
                {
                    if (matrices.Length == 1)
                    {
                        foreach (var p in piece?.Pieces ?? Enumerable.Empty<KeyValuePair<JenkHash, Piece>>())
                        {
                            SetTransformationMatrice(matrices[0], p.Value);
                        }
                    }
                    else
                    {
                        for (i = 1; i < matrices?.Length; i++)
                        {
                            if (data[c] == null)
                            {
                                continue;
                            }

                            var newPiece = new MeshFile((ForgeEntry)piece.FileInfo);
                            newPiece.Load(data[c]!);

                            foreach (var p in newPiece?.Pieces ?? Enumerable.Empty<KeyValuePair<JenkHash, Piece>>())
                            {
                                SetTransformationMatrice(matrices[i], p.Value);
                            }
                            instanciatedPieces.Add(newPiece);
                        }
                    }
                }
            }
            instanciatedPieces.AddRange(pieces);
            return instanciatedPieces.OrderBy(piece => (piece as MeshFile)?.Name).ToList();
        }

        public void SetTransformationMatrice(Matrix4x4 matrice, Piece piece)
        {
            foreach (var model in piece.AllModels)
            {
                foreach (var mesh in model.Meshes)
                {
                    var matrix = new Matrix3x4
                    {
                        Row1 = new Vector4(matrice.M11, matrice.M21, matrice.M31, matrice.M41),
                        Row2 = new Vector4(matrice.M12, matrice.M22, matrice.M32, matrice.M42),
                        Row3 = new Vector4(matrice.M13, matrice.M23, matrice.M33, matrice.M43),
                        //Translation = new Vector3(matrice.M14, matrice.M24, matrice.M34),
                        //Orientation = new Quaternion(matrice.M11, matrice.M22, matrice.M33, matrice.M44)
                    };

                    mesh.MeshTransformMode = 1u;
                    mesh.MeshTransform = matrix;
                    mesh.ShaderInputs.SetUInt32(0x14AB52EB, 1u); //MeshTransformMode
                    mesh.ShaderInputs.SetFloat3x4(0x467A3F64, matrix); //MeshTransform
                    mesh.UpdateBounds();
                }
            }
            piece.UpdateAllModels();
            piece.UpdateBounds();
            piece.BoundingSphere = new BoundingSphere(piece.BoundingBox.Center, piece.BoundingBox.Size.Length() * 0.5f);
        }

        public (object[], List<byte[]>) GetEntitiesFromEntityGroup(EntityGroupFile groupFile)
        {
            if (groupFile.EntityGroup == null || groupFile.EntityGroup.Components == null)
            {
                return (null, null);
            }

            var entities = (new List<object>(), new List<byte[]>());
            for (int i = 0; i < groupFile.EntityGroup.Components.Length; i++)
            {
                var comp = groupFile.EntityGroup.Components[i];
                if (comp == null || comp.GraphicObject == null || comp.GraphicObject.FileID == 0)
                {
                    continue;
                }

                var lod = LoadLODSelector(comp.GraphicObject.FileID);
                if (lod.Item1 == null)
                {
                    //We have to look through the EntityGroup file and see if it stores this LODSelector
                    foreach (var eLod in groupFile.EmbeddedLODSelector ?? Enumerable.Empty<ForgeLODSelector>())
                    {
                        if (eLod.BaseObjectPtrLODSelector?.FileID == comp.GraphicObject.FileID)
                        {
                            lod = LoadLODSelector(0, eLod);
                            break;
                        }
                    }
                }

                if (lod.Item1 != null)
                {
                    entities.Item1.Add(lod.Item1); //Entry
                    entities.Item2.Add(lod.Item2 ?? null); //Data
                }
            }
            return (entities.Item1.ToArray(), entities.Item2);
        }

        public (object, byte[]) LoadLODSelector(long fileID, ForgeLODSelector lod = null)
        {
            LODSelectorFile lodSelector = null;
            if (fileID != 0)
            {
                var data = GetEntryFromAllArchives(fileID);
                if (data.Item1 == null || data.Item2 == null)
                {
                    return (null, null);
                }

                lodSelector = new LODSelectorFile(data.Item1);
                lodSelector.Load(data.Item2);
            }
            else if (lod != null) //Got a embbeded LOD
            {
                lodSelector = new LODSelectorFile(lod.Header?.FileName ?? string.Empty)
                {
                    LODSelector = lod
                };
            }


            if (lodSelector != null && lodSelector.LODSelector != null)
            {
                for (int i = 0; i < lodSelector.LODSelector.LODDescs?.Length; i++)
                {
                    var obj = lodSelector.LODSelector.LODDescs[i].StreamObjectHandle;
                    if (obj != null && obj.FileID != 0)
                    {
                        return GetEntryFromAllArchives(obj.FileID); //Returns the highest LOD mesh entry + data that was found
                    }
                    else
                    {
                        var obj2 = lodSelector.LODSelector.LODDescs[i].Object;
                        if (obj2 != null && obj2.FileID != 0)
                        {
                            var entry = GetEntryFromAllArchives(obj2.FileID); //Returns the highest LOD mesh entry + data that was found
                            if (entry.Item1 != null)
                            {
                                return entry;
                            }

                            var m = lodSelector.EmbeddedMeshes;
                            if (m != null && m.Count > 0)
                            {
                                return (m.Keys.ToList()[0], m.Values.ToList()[0]); //Returns the embedded mesh if nothing was found
                            }
                        }
                    }
                }
            }
            return (null, null);
        }

        public (ForgeEntry, byte[]) GetEntryFromAllArchives(long fileID)
        {
            foreach (var archive in AllArchives)
            {
                if (archive.AllEntries.FirstOrDefault(e => ((ForgeEntry)e).FileID == fileID) is ForgeEntry entry)
                {
                    byte[] data = EnsureFileData(entry, null);
                    if (data == null)
                    {
                        continue;
                    }
                    return (entry, data);
                }
            }
            return (null, null);
        }

        public void LoadDependencies(ForgeModel model)
        {
            if (model == null || model.Meshes == null)
                return;

            var materials = LoadMaterials(model);
            if (materials == null || materials.Length == 0)
                return;

            var textureSet = new ForgeTextureSet[materials.Length];
            for (int i = 0; i < materials.Length; i++)
            {
                textureSet[i] = LoadTextureSet(materials[i]);
            }

            var listTex = new List<TexturePack>();
            var listBumpTex = new List<TexturePack>();

            //Initialize sets to keep track of added diffuse and normal IDs
            var addedDiffuseIDs = new HashSet<long>();
            var addedNormalIDs = new HashSet<long>();

            for (int i = 0; i < textureSet.Length; i++)
            {
                if (textureSet[i] == null || textureSet[i]?.DiffuseMap == null || textureSet[i]?.DiffuseMap?.FileID <= 0)
                    continue;

                foreach (var archive in AllArchives)
                {
                    var diffuseID = textureSet[i]?.DiffuseMap?.FileID;
                    var normalID = textureSet[i]?.NormalMap?.FileID;

                    if (diffuseID != null && !addedDiffuseIDs.Contains((long)diffuseID))
                    {
                        var entry = GetEntryFromAllArchives((long)diffuseID);
                        if (entry.Item1 != null)
                        {
                            var texture = LoadTexturePack(entry.Item1, entry.Item2);
                            if (texture == null)
                                break;

                            listTex.Add(texture);
                            addedDiffuseIDs.Add((long)diffuseID);
                        }
                    }
                    if (normalID != null && !addedNormalIDs.Contains((long)normalID))
                    {
                        var entry = GetEntryFromAllArchives((long)normalID);
                        if (entry.Item1 != null)
                        {
                            var texture = LoadTexturePack(entry.Item1, entry.Item2);
                            if (texture == null)
                                break;

                            listBumpTex.Add(texture);
                            addedNormalIDs.Add((long)normalID);
                        }
                    }
                }
            }

            for (int i = 0; i < model.Meshes.Length; i++)
            {
                if (listTex.Count <= i || model.Meshes[i] == null)
                    continue;

                var mesh = model.Meshes[i];
                var textures = listTex[i].Textures.Values.ToArray();

                if (listBumpTex.Count > i && listBumpTex[i] != null)
                {
                    var bumpTextures = listBumpTex[i].Textures.Values;
                    textures = textures.Concat(bumpTextures).Distinct().ToArray();
                }
                mesh.Textures = textures;
            }
        }

        public ForgeMaterial[] LoadMaterials(ForgeModel model)
        {
            var materialsID = new List<long>();
            foreach (var meshData in model.CompiledMesh?.InstancingData ?? Enumerable.Empty<ForgeMeshInstancingData>())
            {
                if (meshData?.Material?.FileID != null)
                {
                    materialsID.Add(meshData.Material.FileID);
                }
            }

            var materials = new List<ForgeMaterial>();
            foreach (var id in materialsID)
            {
                foreach (var archive in AllArchives)
                {
                    if (archive.AllEntries.FirstOrDefault(e => ((ForgeEntry)e).FileID == id) is ForgeEntry materialEntry)
                    {
                        byte[] matData = EnsureFileData(materialEntry, null);
                        if (matData == null)
                        {
                            continue;
                        }

                        var material = new MaterialFile(materialEntry);
                        material.Load(matData);

                        if (material != null)
                            materials.Add(material.Material);
                        break;
                    }
                }
            }
            return materials.ToArray();
        }

        public ForgeTextureSet LoadTextureSet(ForgeMaterial material)
        {
            if (material == null || material.TextureSet == null)
                return null;

            var textureSetID = (long)material.TextureSet.FileID;
            foreach (var archive in AllArchives)
            {
                if (archive.AllEntries.FirstOrDefault(e => ((ForgeEntry)e).FileID == textureSetID) is ForgeEntry textureSetEntry)
                {
                    byte[] tsData = EnsureFileData(textureSetEntry, null);
                    if (tsData == null)
                    {
                        continue;
                    }

                    var textureSet = new TextureSetFile(textureSetEntry);
                    textureSet.Load(tsData);
                    return textureSet.TextureSet;
                }
            }
            return null;
        }

        public override void LoadStartupCache()
        {
            var file = StartupUtil.GetFilePath("CodeX.Games.ACOdyssey.startup.dat");

            if (File.Exists(file) == false)
            {
                StartupCacheDirty = true;
                return;
            }

            Core.Engine.Console.Write("ForgeFileManager", "Loading ACOdyssey startup cache...");

            var buf = File.ReadAllBytes(file);
            using (var ms = new MemoryStream(buf))
            {
                var br = new BinaryReader(ms);
                var rootcount = br.ReadInt32();
                var roots = new List<GameArchive>();
                StartupCache = new Dictionary<string, GameArchive>();

                for (int i = 0; i < rootcount; i++)
                {
                    var apath = br.ReadStringNullTerminated();
                    var atime = DateTime.FromBinary(br.ReadInt64());
                    var atimetest = File.GetLastWriteTime(apath);
                    var relpath = apath.Replace(Folder + "\\", "");

                    Core.Engine.Console.Write("ForgeFileManager", Game.GamePathPrefix + relpath);

                    var root = new ForgeFile(apath, relpath);
                    root.ReadStartupCache(br);
                    roots.Add(root);

                    if (atime != atimetest)
                        StartupCacheDirty = true; //Don't cache this file since the times don't match
                    else
                        StartupCache[apath] = root;
                }
                DataFileMgr = new ForgeDataFileMgr(this);
            }
        }

        public override void SaveStartupCache()
        {
            var file = StartupUtil.GetFilePath("CodeX.Games.ACOdyssey.startup.dat");
            Core.Engine.Console.Write("ForgeFileManager", "Building ACOdyssey startup cache...");

            using (var ms = new MemoryStream())
            {
                var bw = new BinaryWriter(ms);
                bw.Write(RootArchives.Count);

                foreach (ForgeFile root in RootArchives.Cast<ForgeFile>())
                {
                    var apath = root.FilePath;
                    var atime = File.GetLastWriteTime(apath);
                    bw.WriteStringNullTerminated(apath);
                    bw.Write(atime.ToBinary());
                    root.WriteStartupCache(bw);
                }

                var buf = new byte[ms.Length];
                ms.Position = 0;
                ms.Read(buf, 0, buf.Length);
                File.WriteAllBytes(file, buf);
            }
        }

        public override void RenameArchive(GameArchive file, string newname)
        {
            throw new NotImplementedException();
        }

        public override void RenameEntry(GameArchiveEntry entry, string newname)
        {
            throw new NotImplementedException();
        }
    }

    public class ForgeDataFileMgr
    {
        public ForgeFileManager FileManager;
        public Dictionary<ForgeResourceType, Dictionary<JenkHash, ForgeEntry>> StreamEntries;

        public ForgeDataFileMgr(ForgeFileManager fman)
        {
            FileManager = fman;
            StreamEntries = null;
        }

        public void Init()
        {
            StreamEntries = new Dictionary<ForgeResourceType, Dictionary<JenkHash, ForgeEntry>>();

            foreach (var archive in FileManager.AllArchives)
            {
                foreach (ForgeEntry entry in archive.AllEntries)
                {
                    if (Enum.TryParse(entry.ResourceType, out ForgeResourceType type))
                    {
                        if (!StreamEntries.TryGetValue(type, out var entries))
                        {
                            entries = new Dictionary<JenkHash, ForgeEntry>();
                            StreamEntries[type] = entries;
                        }
                        entries[entry.ShortNameHash] = entry;
                    }
                }
            }
        }

        public ForgeEntry TryGetStreamEntry(JenkHash hash, ForgeResourceType ext)
        {
            if (StreamEntries != null && StreamEntries.TryGetValue(ext, out var entries))
            {
                if (entries.TryGetValue(hash, out var entry))
                {
                    return entry;
                }
            }
            return null;
        }
    }

    public class ForgeDataFileDevice
    {
        public ForgeDataFileMgr DataFileMgr;
        public ForgeFileManager FileManager;
        public string Name;
        public string PhysicalPath;

        public ForgeDataFileDevice(ForgeDataFileMgr dfm, string name, string path)
        {
            DataFileMgr = dfm;
            FileManager = dfm.FileManager;
            Name = name;
            PhysicalPath = FileManager.Folder + "\\" + path;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}