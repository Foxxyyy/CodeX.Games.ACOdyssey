using CodeX.Core.Utilities;
using CodeX.Games.ACOdyssey.FORGE;
using System.Numerics;

namespace CodeX.Games.ACOdyssey.Resources
{
    public class ForgeWorld
    {
        public ForgeBaseObjectPtr BaseObjectPtrGrid { get; set; }
        public int NumGridsDesc { get; set; }
        public GridDescription[] GridsDesc { get; set; }
        public int NumEntities { get; set; }
        public ForgeEntity[] TerrainShadowProxies { get; set; }
        public ForgeFileReference FakeEntitiesManager { get; set; }
        public ForgeFileReference DefaultTransitionPortal { get; set; }
        public ForgeFileReference DynamicTransitionPortal { get; set; }
        public int NumWorldPortals { get; set; }
        public ForgeFileReference[] WorldTransitionPortals { get; set; }
        public ForgeFileReference WorldDataLayerManager { get; set; }
        public WorldGraphicData WorldGraphicData { get; set; }
        public WorldEngineData WorldEngineData { get; set; }
        public RegionLayoutManager RegionLayoutManager { get; set; }

        public ForgeWorld()
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

            BaseObjectPtrGrid = new ForgeBaseObjectPtr();
            BaseObjectPtrGrid.Read(reader);

            NumGridsDesc = reader.ReadInt32();
            GridsDesc = new GridDescription[NumGridsDesc];

            for (int i = 0; i < NumGridsDesc; i++)
            {
                GridsDesc[i] = new GridDescription();
                GridsDesc[i].Read(reader);
            }

            NumEntities = reader.ReadInt32();
            TerrainShadowProxies = new ForgeEntity[NumEntities];

            for (int i = 0; i < NumEntities; i++)
            {
                TerrainShadowProxies[i] = new ForgeEntity();
                TerrainShadowProxies[i].Read(reader, true);
            }

            FakeEntitiesManager = new ForgeFileReference();
            FakeEntitiesManager.Read(reader);

            DefaultTransitionPortal = new ForgeFileReference();
            DefaultTransitionPortal.Read(reader);

            DynamicTransitionPortal = new ForgeFileReference();
            DynamicTransitionPortal.Read(reader);

            NumWorldPortals = reader.ReadInt32();
            WorldTransitionPortals = new ForgeFileReference[NumWorldPortals];

            for (int i = 0; i < NumWorldPortals; i++)
            {
                WorldTransitionPortals[i] = new ForgeFileReference();
                WorldTransitionPortals[i].Read(reader);
            }

            WorldDataLayerManager = new ForgeFileReference();
            WorldDataLayerManager.Read(reader);

            WorldGraphicData = new WorldGraphicData();
            WorldGraphicData.Read(reader);

            WorldEngineData = new WorldEngineData();
            WorldEngineData.Read(reader);

            RegionLayoutManager = new RegionLayoutManager();
            RegionLayoutManager.Read(reader);
        }
    }

    public class GridDescription
    {
        public ForgeBaseObject BaseObjectGridDescription { get; set; }
        public ForgeBaseObject BaseObjectGridLayout { get; set; }
        public ushort DefaultLoadingDistance { get; set; }
        public int CellSize { get; set; }
        public int BottomLeftX { get; set; }
        public int BottomLeftY { get; set; }
        public int BottomLeftZ { get; set; }
        public int GridDimensionLevel0 { get; set; }
        public bool IsSingleLevelGrid { get; set; }
        public bool IgnoreCustomLoadingRange { get; set; }
        public bool IgnoreRegionLayoutLoadingRange { get; set; }
        public ForgeFileReference Grid { get; set; }

        public GridDescription()
        {
        }

        public void Read(DataReader reader)
        {
            BaseObjectGridDescription = new ForgeBaseObject();
            BaseObjectGridDescription.Read(reader);

            BaseObjectGridLayout = new ForgeBaseObject();
            BaseObjectGridLayout.Read(reader);

            DefaultLoadingDistance = reader.ReadUInt16();
            CellSize =  reader.ReadInt32();
            BottomLeftX =  reader.ReadInt32();
            BottomLeftY =  reader.ReadInt32();
            BottomLeftZ =  reader.ReadInt32();
            GridDimensionLevel0 =  reader.ReadInt32();
            IsSingleLevelGrid =  reader.ReadBoolean();
            IgnoreCustomLoadingRange =  reader.ReadBoolean();
            IgnoreRegionLayoutLoadingRange =  reader.ReadBoolean();

            Grid = new ForgeFileReference();
            Grid.Read(reader);
        }
    }

    public class WorldGraphicData
    {
        public ForgeObject ObjectWorldGraphicData { get; set; }
        public LayeredSky LayeredSky { get; set; }
        public ForgeFileReference WorldGI { get; set; }
        public LocalCubeMapContainer LocalCubeMapContainer { get; set; }
        public ForgeFileReference FakeEntitiesReservedTexture { get; set; }
        public ForgeFileReference RainBlockerContainerRef { get; set; }
        public bool ClearGBufferColors { get; set; }
        public bool EnableSunlight { get; set; }
        public bool EnableDynamicLights { get; set; }
        public bool EnableReflections { get; set; }
        public bool EnableSunlightShadow { get; set; }
        public float DefaultShadowFactor { get; set; }
        public byte SunDeconstructionGroupMask { get; set; }
        public bool PreTransparentsNeedFullZBuffer { get; set; }
        public bool EnableWater { get; set; }
        public bool EnableShadowCascade1 { get; set; }
        public bool EnableShadowCascade2 { get; set; }
        public bool EnableShadowCascade3 { get; set; }
        public bool EnableShadowCascade4 { get; set; }
        public uint UnderwaterShadowCount { get; set; }
        public bool ShadowCenterOnCamera { get; set; }
        public ForgeObjectPtr WorldParticleSystems { get; set; }
        public float Near { get; set; }
        public float Far { get; set; }
        public bool DeconstructionEnabled { get; set; }
        public Mask DeconstructionGroupMask { get; set; }
        public float DeconstructionStartDistance { get; set; }
        public float DeconstructionSize { get; set; }
        public Vector4 DeconstructionOrigin { get; set; }
        public float UINear { get; set; }
        public float UIFar { get; set; }
        public ForgeFileReference UILightingSetup { get; set; }
        public float WorldMapFogMinHeight { get; set; }
        public float WorldMapFogMaxHeight { get; set; }
        public float WorldMapLODDistanceScale { get; set; }
        public float WorldMapTerrainNodeErrorLimit { get; set; }
        public ForgeFileReference WorldMapFogHeightTexture { get; set; }
        public float ReflectionMaxDistance { get; set; }
        public PhotoMode PhotoMode { get; set; }
        public bool VisibilityQueries { get; set; }
        public float MinGlobalLightingScale { get; set; }
        public float MaxGlobalLightingScale { get; set; }
        public float MinGlobalLightingScaleEV { get; set; }
        public float MaxGlobalLightingScaleEV { get; set; }
        public ForgeFileReference GISegments { get; set; }
        public ForgeFileReference LUTSegments { get; set; }
        public ForgeFileReference LightGroupIntensitySegments { get; set; }
        public CollectionSettings CollectionSettings { get; set; }
        public GraceForceSettings GraceForceSettings { get; set; }
        public float TranslucentSaturation { get; set; }
        public float FarLightsEndDistanceScale { get; set; }
        public float FarLightsSourceEndDistanceScale { get; set; }

        public WorldGraphicData()
        {
        }

        public void Read(DataReader reader)
        {
            ObjectWorldGraphicData = new ForgeObject();
            ObjectWorldGraphicData.Read(reader);

            LayeredSky = new LayeredSky();
            LayeredSky.Read(reader);

            WorldGI = new ForgeFileReference();
            WorldGI.Read(reader);

            LocalCubeMapContainer = new LocalCubeMapContainer();
            LocalCubeMapContainer.Read(reader);

            FakeEntitiesReservedTexture = new ForgeFileReference();
            FakeEntitiesReservedTexture.Read(reader);

            RainBlockerContainerRef = new ForgeFileReference();
            RainBlockerContainerRef.Read(reader);

            ClearGBufferColors = reader.ReadBoolean();
            EnableSunlight = reader.ReadBoolean();
            EnableDynamicLights = reader.ReadBoolean();
            EnableReflections = reader.ReadBoolean();
            EnableSunlightShadow = reader.ReadBoolean();
            DefaultShadowFactor = reader.ReadSingle();
            SunDeconstructionGroupMask = reader.ReadByte();
            PreTransparentsNeedFullZBuffer = reader.ReadBoolean();
            EnableWater = reader.ReadBoolean();
            EnableShadowCascade1 = reader.ReadBoolean();
            EnableShadowCascade2 = reader.ReadBoolean();
            EnableShadowCascade3 = reader.ReadBoolean();
            EnableShadowCascade4 = reader.ReadBoolean();
            UnderwaterShadowCount = reader.ReadUInt32();
            ShadowCenterOnCamera = reader.ReadBoolean();

            WorldParticleSystems = new ForgeObjectPtr();
            WorldParticleSystems.Read(reader, true, 0x3);

            Near = reader.ReadSingle();
            Far = reader.ReadSingle();
            DeconstructionEnabled = reader.ReadBoolean();

            DeconstructionGroupMask = new Mask();
            DeconstructionGroupMask.Read(reader);

            DeconstructionStartDistance = reader.ReadSingle();
            DeconstructionSize = reader.ReadSingle();
            DeconstructionOrigin = reader.ReadVector4();
            UINear = reader.ReadSingle();
            UIFar = reader.ReadSingle();

            UILightingSetup = new ForgeFileReference();
            UILightingSetup.Read(reader);

            WorldMapFogMinHeight = reader.ReadSingle();
            WorldMapFogMaxHeight = reader.ReadSingle();
            WorldMapLODDistanceScale = reader.ReadSingle();
            WorldMapTerrainNodeErrorLimit = reader.ReadSingle();

            WorldMapFogHeightTexture = new ForgeFileReference();
            WorldMapFogHeightTexture.Read(reader);

            ReflectionMaxDistance = reader.ReadSingle();

            PhotoMode = new PhotoMode();
            PhotoMode.Read(reader);

            VisibilityQueries = reader.ReadBoolean();
            MinGlobalLightingScale = reader.ReadSingle();
            MaxGlobalLightingScale = reader.ReadSingle();
            MinGlobalLightingScaleEV = reader.ReadSingle();
            MaxGlobalLightingScaleEV = reader.ReadSingle();

            GISegments = new ForgeFileReference();
            GISegments.Read(reader);

            LUTSegments = new ForgeFileReference();
            LUTSegments.Read(reader);

            LightGroupIntensitySegments = new ForgeFileReference();
            LightGroupIntensitySegments.Read(reader);

            CollectionSettings = new CollectionSettings();
            CollectionSettings.Read(reader);

            GraceForceSettings = new GraceForceSettings();
            GraceForceSettings.Read(reader);

            TranslucentSaturation = reader.ReadSingle();
            FarLightsEndDistanceScale = reader.ReadSingle();
            FarLightsSourceEndDistanceScale = reader.ReadSingle();
        }
    }

    public class WorldEngineData
    {
        public ForgeBaseObject BaseObjectWorldEngineData { get; set; }
        public byte MaxSoundDistance { get; set; }
        public float DeathHeight { get; set; }

        public WorldEngineData()
        {
        }

        public void Read(DataReader reader)
        {
            BaseObjectWorldEngineData = new ForgeBaseObject();
            BaseObjectWorldEngineData.Read(reader);

            MaxSoundDistance = reader.ReadByte();
            DeathHeight = reader.ReadSingle();
        }
    }

    public class RegionLayoutManager
    {
        public ForgeObject ObjectRegionLayoutManager { get; set; }
        public int NumRegions { get; set; }
        public RegionLayout[] Regions { get; set; }
        public int NumComponents { get; set; }
        public WorldComponent[] Components { get; set; }
        public WorldMapParameters MapParameters { get; set; }
        public bool PartialWorld { get; set; }
        public bool HasPartialInstallExtraDataSource { get; set; }

        public RegionLayoutManager()
        {
        }

        public void Read(DataReader reader)
        {
            ObjectRegionLayoutManager = new ForgeObject();
            ObjectRegionLayoutManager.Read(reader);

            NumRegions = reader.ReadInt32();
            Regions = new RegionLayout[NumRegions];

            for (int i = 0; i < Regions.Length; i++)
            {
                Regions[i] = new RegionLayout();
                Regions[i].Read(reader);
            }

            NumComponents = reader.ReadInt32();
            Components = new WorldComponent[NumComponents];

            for (int i = 0; i < NumComponents; i++)
            {
                var componentType = new ForgeBaseObjectPtr();
                componentType.Read(reader);

                var component = WorldComponent.CreateWorldComponent((ComponentType)componentType.FileType);
                component.Read(reader);

                Components[i] = component;
                Components[i].ComponentType = (ComponentType)componentType.FileType;
            }

            MapParameters = new WorldMapParameters();
            MapParameters.Read(reader);

            PartialWorld = reader.ReadBoolean();
            HasPartialInstallExtraDataSource = reader.ReadBoolean();
        }
    }

    public class RegionLayout
    {
        public ForgeFileReference Region { get; set; } //.RegionLayout

        public RegionLayout()
        {
        }

        public void Read(DataReader reader)
        {
            Region = new ForgeFileReference();
            Region.Read(reader);
        }
    }

    public class LayeredSky
    {
        public ForgeBaseObjectPtr BaseObjectPtrLayeredSky { get; set; }
        public byte DescriptorMask { get; set; }
        public bool Generated { get; set; }
        public bool HasHighOverdraw { get; set; }
        public float MaxCullingDistance { get; set; }
        public uint EstimatedMemoryUsage { get; set; }
        public uint CompiledStarCount { get; set; }
        public ForgeFileReference StarCatalogBuffer { get; set; }
        public ForgeFileReference StarShimmerBuffer { get; set; }
        public ForgeFileReference SkyModelLUT { get; set; }
        public ForgeFileReference CloudData { get; set; }

        public LayeredSky()
        {
        }

        public void Read(DataReader reader)
        {
            BaseObjectPtrLayeredSky = new ForgeBaseObjectPtr();
            BaseObjectPtrLayeredSky.Read(reader);

            DescriptorMask = reader.ReadByte();
            Generated = reader.ReadBoolean();
            HasHighOverdraw = reader.ReadBoolean();
            MaxCullingDistance = reader.ReadSingle();
            EstimatedMemoryUsage = reader.ReadUInt32();
            CompiledStarCount = reader.ReadUInt32();

            StarCatalogBuffer = new ForgeFileReference();
            StarCatalogBuffer.Read(reader);

            StarShimmerBuffer = new ForgeFileReference();
            StarShimmerBuffer.Read(reader);

            SkyModelLUT = new ForgeFileReference();
            SkyModelLUT.Read(reader);

            CloudData = new ForgeFileReference();
            CloudData.Read(reader);
        }
    }

    public class LocalCubeMapContainer
    {
        public ForgeObject ObjectLocalCubeMapContainer { get; set; }
        public LocalCubeMapSettings LocalCubeMapSettings { get; set; }
        public int NumLocalCubeMapEntries { get; set; }
        public CubeMapEntry[] LocalCubeMapEntries { get; set; }
        public int NumGlobalCubeMapEntries { get; set; }
        public CubeMapEntry[] GlobalCubeMapEntries { get; set; }

        public LocalCubeMapContainer()
        {
        }

        public void Read(DataReader reader)
        {
            ObjectLocalCubeMapContainer = new ForgeObject();
            ObjectLocalCubeMapContainer.Read(reader);

            LocalCubeMapSettings = new LocalCubeMapSettings();
            LocalCubeMapSettings.Read(reader);

            NumLocalCubeMapEntries = reader.ReadInt32();
            LocalCubeMapEntries = new CubeMapEntry[NumLocalCubeMapEntries];

            for (int i = 0; i < LocalCubeMapEntries.Length; i++)
            {
                LocalCubeMapEntries[i] = new CubeMapEntry();
                LocalCubeMapEntries[i].Read(reader);
            }

            NumGlobalCubeMapEntries = reader.ReadInt32();
            GlobalCubeMapEntries = new CubeMapEntry[NumGlobalCubeMapEntries];

            for (int i = 0; i < GlobalCubeMapEntries.Length; i++)
            {
                GlobalCubeMapEntries[i] = new CubeMapEntry();
                GlobalCubeMapEntries[i].Read(reader);
            }
        }
    }

    public class LocalCubeMapSettings
    {
        public ForgeBaseObject BaseObjectLocalCubeMapSettings { get; set; }
        public bool EnableFog { get; set; }
        public bool EnableShadowsInFog { get; set; }
        public float BlendDuration { get; set; }
        public float GlobalCubeMapBlendDuration { get; set; }
        public float[] LightMapKeyFrames { get; set; }
        public uint CubeMapSize { get; set; }
        public bool AddCloudInSkyCubeMap { get; set; }
        public float CloudCoverStartOffset { get; set; }
        public float CloudCoverEndOffset { get; set; }
        public float Luminosity { get; set; }
        public float Saturation { get; set; }
        public float AdditiveBlue { get; set; }

        public LocalCubeMapSettings()
        {
        }

        public void Read(DataReader reader)
        {
            BaseObjectLocalCubeMapSettings = new ForgeBaseObject();
            BaseObjectLocalCubeMapSettings.Read(reader);

            EnableFog = reader.ReadBoolean();
            EnableShadowsInFog = reader.ReadBoolean();
            BlendDuration = reader.ReadSingle();
            GlobalCubeMapBlendDuration = reader.ReadSingle();
            LightMapKeyFrames = new float[8];

            for (int i = 0; i < LightMapKeyFrames.Length; i++)
            {
                LightMapKeyFrames[i] = reader.ReadSingle();
            }

            CubeMapSize = reader.ReadUInt32();
            AddCloudInSkyCubeMap = reader.ReadBoolean();
            CloudCoverStartOffset = reader.ReadSingle();
            CloudCoverEndOffset = reader.ReadSingle();
            Luminosity = reader.ReadSingle();
            Saturation = reader.ReadSingle();
            AdditiveBlue = reader.ReadSingle();
        }
    }

    public class CubeMapEntry
    {
        public ForgeBaseObject BaseObjectCubeMapEntry { get; set; }
        public ForgeObjectPtr CubeMap { get; set; } //Handle<LocalCubeMap>
        public ForgeObjectPtr Ambiance { get; set; } //Handle<LocalVisualAmbiance>

        public CubeMapEntry()
        {
        }

        public void Read(DataReader reader)
        {
            BaseObjectCubeMapEntry = new ForgeBaseObject();
            BaseObjectCubeMapEntry.Read(reader);

            CubeMap = new ForgeObjectPtr();
            CubeMap.Read(reader);

            Ambiance = new ForgeObjectPtr();
            Ambiance.Read(reader);
        }
    }

    public class PhotoMode
    {
        public ForgeBaseObject BaseObjectPhotoMode { get; set; }

        public PhotoMode()
        {
        }

        public void Read(DataReader reader) //Unknown data block
        {
            BaseObjectPhotoMode = new ForgeBaseObject();
            BaseObjectPhotoMode.Read(reader);
        }
    }

    public class CollectionSettings
    {
        public ForgeObject ObjectCollectionSettings { get; set; }
        public float ClusterFadeVarianceFactor { get; set; }
        public float Unknown_4h { get; set; }
        public float Unknown_8h { get; set; }
        public float Unknown_Ch { get; set; }
        public float Unknown_10h { get; set; }
        public float Unknown_14h { get; set; }
        public float Unknown_18h { get; set; }
        public float Unknown_1Ch { get; set; }
        public float Unknown_20h { get; set; }
        public float GlobalClutterDistanceScale_XB1 { get; set; }
        public float GlobalClutterDistanceScale_XB1Scorpio { get; set; }
        public float GlobalClutterDistanceScale_PS4 { get; set; }
        public float GlobalClutterDistanceScale_PS4Pro { get; set; }
        public float GlobalClutterDistanceScale_PC_1_Low { get; set; }
        public float GlobalClutterDistanceScale_PC_2_Medium { get; set; }
        public float GlobalClutterDistanceScale_PC_3_High { get; set; }
        public float GlobalClutterDistanceScale_PC_4_VeryHigh { get; set; }
        public float MinimumCombineDistanceScale { get; set; }

        public CollectionSettings()
        {
        }

        public void Read(DataReader reader)
        {
            ObjectCollectionSettings = new ForgeObject();
            ObjectCollectionSettings.Read(reader);

            ClusterFadeVarianceFactor = reader.ReadSingle();
            Unknown_4h = reader.ReadSingle();
            Unknown_8h = reader.ReadSingle();
            Unknown_Ch = reader.ReadSingle();
            Unknown_10h = reader.ReadSingle();
            Unknown_14h = reader.ReadSingle();
            Unknown_18h = reader.ReadSingle();
            Unknown_1Ch = reader.ReadSingle();
            Unknown_20h = reader.ReadSingle();
            GlobalClutterDistanceScale_XB1 = reader.ReadSingle();
            GlobalClutterDistanceScale_XB1Scorpio = reader.ReadSingle();
            GlobalClutterDistanceScale_PS4 = reader.ReadSingle();
            GlobalClutterDistanceScale_PS4Pro = reader.ReadSingle();
            GlobalClutterDistanceScale_PC_1_Low = reader.ReadSingle();
            GlobalClutterDistanceScale_PC_2_Medium = reader.ReadSingle();
            GlobalClutterDistanceScale_PC_3_High = reader.ReadSingle();
            GlobalClutterDistanceScale_PC_4_VeryHigh = reader.ReadSingle();
            MinimumCombineDistanceScale = reader.ReadSingle();
        }
    }

    public class GraceForceSettings
    {
        public ForgeObject ObjectGraceForceSettings { get; set; }
        public bool Enabled { get; set; }
        public uint TextureSize { get; set; }
        public float Radius { get; set; }
        public float FadeStart { get; set; }
        public float CapsuleXYDisplacementForce { get; set; }
        public float XYDecayFactor { get; set; }
        public float DisplacementInertia { get; set; }
        public float DynamicForceClamp { get; set; }
        public float CapsuleScalingFactor { get; set; }

        public GraceForceSettings()
        {
        }

        public void Read(DataReader reader)
        {
            ObjectGraceForceSettings = new ForgeObject();
            ObjectGraceForceSettings.Read(reader);

            Enabled = reader.ReadBoolean();
            TextureSize = reader.ReadUInt32();
            Radius = reader.ReadSingle();
            FadeStart = reader.ReadSingle();
            CapsuleXYDisplacementForce = reader.ReadSingle();
            XYDecayFactor = reader.ReadSingle();
            DisplacementInertia = reader.ReadSingle();
            DynamicForceClamp = reader.ReadSingle();
            CapsuleScalingFactor = reader.ReadSingle();
        }
    }
}