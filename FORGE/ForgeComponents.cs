using CodeX.Core.Utilities;
using CodeX.Games.ACOdyssey.Resources;
using System;
using System.Numerics;

namespace CodeX.Games.ACOdyssey.FORGE
{
    /***************************************** DefaultComponents *****************************************/
    public class Component
    {
        public ComponentType ComponentType { get; set; }
        public bool Active { get; set; }
        public bool OptimizedForHardwareInstancing { get; set; }
        public ForgeBaseObject BaseObjectComponentLOD { get; set; }
        public byte ComponentLODBitField { get; set; }
        public ForgeFileReference GraphicObject { get; set; }
        public LODSelectorInstance InstanceData { get; set; }
        public bool HasUnshareContext { get; set; }
        public bool OptimizedPackedLOD { get; set; }
        public int DisplayOrder { get; set; }
        public bool AllowCameraDither { get; set; }
        public bool AllowShadowCast { get; set; }
        public VisualAutoResizer AutoResizer { get; set; }
        public VisualShaderConstants ShaderConstants { get; set; }
        public int NumPermutations { get; set; }
        public StaticPermutation[] AllowedStaticPermutations { get; set; }
        public Vector4 BVScale { get; set; }

        public Component()
        {
        }

        public void Read(DataReader reader, bool visual = true)
        {
            Active = reader.ReadBoolean();
            OptimizedForHardwareInstancing = reader.ReadBoolean();

            BaseObjectComponentLOD = new ForgeBaseObject();
            BaseObjectComponentLOD.Read(reader);
            ComponentLODBitField = reader.ReadByte();

            if (!visual) //GuidanceSystem & RigidBodyComponent
            {
                return;
            }

            GraphicObject = new ForgeFileReference();
            GraphicObject.Read(reader);

            reader.BaseStream.Position += 0xD;
            InstanceData = new LODSelectorInstance();
            InstanceData.Read(reader);

            HasUnshareContext = reader.ReadBoolean();
            OptimizedPackedLOD = reader.ReadBoolean();
            DisplayOrder = reader.ReadInt32();
            AllowCameraDither = reader.ReadBoolean();
            AllowShadowCast = reader.ReadBoolean();

            AutoResizer = new VisualAutoResizer();
            AutoResizer.Read(reader);

            NumPermutations = reader.ReadInt32();
            AllowedStaticPermutations = new StaticPermutation[NumPermutations];
            for (int i = 0; i < NumPermutations; i++)
            {
                AllowedStaticPermutations[i] = new StaticPermutation();
                AllowedStaticPermutations[i].Read(reader);
            }

            ShaderConstants = new VisualShaderConstants();
            ShaderConstants.Read(reader);

            BVScale = reader.ReadVector4();
        }
    }

    public class GuidanceSystemComponent : Component
    {
        public GuidanceSystemData DefaultGuidanceData { get; set; }
        public EdgeFilter EdgeFilter { get; set; }
        public bool Dynamic { get; set; }
        public bool IsClear { get; set; }
        public ushort FirstParallelIndex { get; set; }
        public Partitioner Partitioner { get; set; }
        public bool IsOmniDirectionalGuidance { get; set; }

        public GuidanceSystemComponent()
        {
        }

        public void Read(DataReader reader)
        {
            base.Read(reader, false);
            DefaultGuidanceData = new GuidanceSystemData();
            DefaultGuidanceData.Read(reader);

            EdgeFilter = new EdgeFilter();
            EdgeFilter.Read(reader);

            Dynamic = reader.ReadBoolean();
            IsClear = reader.ReadBoolean();
            FirstParallelIndex = reader.ReadUInt16();

            Partitioner = new Partitioner();
            Partitioner.Read(reader);

            IsOmniDirectionalGuidance = reader.ReadBoolean();
        }
    }

    public class RigidBodyComponent : Component
    {
        public int CollisionReportCapacity { get; set; }
        public int Unknown1 { get; set; }
        public int CharacterInteractionMode { get; set; }
        public int VehicleInteractionMode { get; set; }
        public ForgeBaseObjectPtr PlatformingDecriptor { get; set; }
        public int Resistance { get; set; }
        public ForgeBaseObject GPSurfaceNavType { get; set; }
        public RigidBody RigidBody { get; set; }
        public ForgeFileReference ImpactData { get; set; }
        public bool UseGravityWhenKeyFramed { get; set; }
        public bool HasPhysicsInitTransform { get; set; }
        public bool AutoUpdatePhyicsHierarchy { get; set; }
        public bool UpdateEntityBV { get; set; }
        public bool IsRigideBodyGround { get; set; }
        public bool OverrideListShapeData { get; set; }
        public bool DisplayRigidBodyWithOwnerTransform { get; set; }
        public Matrix4x4 EntityInitTranform { get; set; }
        public float MaxLinearVelocity { get; set; }
        public float MaxAngularVelocity { get; set; }
        public Vector4 LinearConstraint { get; set; }
        public Vector4 AngularConstraint { get; set; }
        public bool Unknown2 { get; set; }
        public ForgeObjectPtr GuidanceSystemPtr { get; set; }
        public int NumShapeOverrideData { get; set; }
        public ListShapeOverrideData[] ListShapeOverrideData { get; set; }
        public ForgeObjectPtr ShadowRigidBody { get; set; }
        public int NumSoundOcclusionLinks { get; set; }
        public ForgeObjectPtr[] SoundOcclusionLinks { get; set; }

        public RigidBodyComponent()
        {
        }

        public void Read(DataReader reader)
        {
            base.Read(reader, false);
            CollisionReportCapacity = reader.ReadInt32();
            Unknown1 = reader.ReadInt32();
            CharacterInteractionMode = reader.ReadInt32();
            VehicleInteractionMode = reader.ReadInt32();
            PlatformingDecriptor = new ForgeBaseObjectPtr();
            PlatformingDecriptor.Read(reader, true);
            Resistance = reader.ReadInt32();

            GPSurfaceNavType = new ForgeBaseObject();
            GPSurfaceNavType.Read(reader);

            RigidBody = new RigidBody();
            RigidBody.Read(reader);

            ImpactData = new ForgeFileReference();
            ImpactData.Read(reader);

            UseGravityWhenKeyFramed = reader.ReadBoolean();
            HasPhysicsInitTransform = reader.ReadBoolean();
            AutoUpdatePhyicsHierarchy = reader.ReadBoolean();
            UpdateEntityBV = reader.ReadBoolean();
            IsRigideBodyGround = reader.ReadBoolean();
            OverrideListShapeData = reader.ReadBoolean();
            DisplayRigidBodyWithOwnerTransform = reader.ReadBoolean();
            EntityInitTranform = reader.ReadMatrix4x4();
            MaxLinearVelocity = reader.ReadSingle();
            MaxAngularVelocity = reader.ReadSingle();
            LinearConstraint = reader.ReadVector4();
            AngularConstraint = reader.ReadVector4();
            Unknown2 = reader.ReadBoolean();

            GuidanceSystemPtr = new ForgeObjectPtr();
            GuidanceSystemPtr.Read(reader, true);

            NumShapeOverrideData = reader.ReadInt32();
            ListShapeOverrideData = new ListShapeOverrideData[NumShapeOverrideData];
            for (int i = 0; i < NumShapeOverrideData; i++)
            {
                ListShapeOverrideData[i] = new ListShapeOverrideData();
                ListShapeOverrideData[i].Read(reader);
            }

            ShadowRigidBody = new ForgeObjectPtr();
            ShadowRigidBody.Read(reader);

            NumSoundOcclusionLinks = reader.ReadInt32();
            SoundOcclusionLinks = new ForgeObjectPtr[NumSoundOcclusionLinks];

            for (int i = 0; i < NumSoundOcclusionLinks; i++)
            {
                SoundOcclusionLinks[i] = new ForgeObjectPtr();
                SoundOcclusionLinks[i].Read(reader);
            }
        }
    }

    public class AudioComponent : Component
    {
        public int NumSubEmitters { get; set; }
        public SimpleSoundSubComponent[] SubEmitters { get; set; }
        public int VoiceSubEmitterIndex { get; set; }

        public AudioComponent()
        {
        }

        public void Read(DataReader reader)
        {
            base.Read(reader, false);
            NumSubEmitters = reader.ReadInt32();
            SubEmitters = new SimpleSoundSubComponent[NumSubEmitters];

            for (int i = 0; i < SubEmitters.Length; i++)
            {
                SubEmitters[i] = new SimpleSoundSubComponent();
                SubEmitters[i].Read(reader);
            }
            VoiceSubEmitterIndex = reader.ReadInt32();
        }
    }

    public class FireComponent : Component
    {
        public ForgeFileReference Template { get; set; }
        public int NumParticles { get; set; }
        public FireParticle[] Particles { get; set; }
        public ForgeFileReference FireSettings { get; set; }
        public float Unknown1 { get; set; }
        public ForgeFileReference GlobalFX { get; set; }
        public ForgeFileReference ParticleFX { get; set; }
        public ForgeFileReference FireDamage { get; set; }
        public ForgeFileReference FireEnergy { get; set; }
        public ForgeFileReference FireResistance { get; set; }
        public ForgeFileReference CustomizedFireSoundSet { get; set; }
        public ForgeFileReference FirePropertyZone { get; set; }
        public ForgeFileReference PropagationConditions { get; set; }
        public bool IsPersistent { get; set; }
        public int NumComponentDescriptors { get; set; }
        public uint[] FireComponentDescriptors { get; set; }
        public int NumLightVisuals { get; set; }
        public uint[] LightVisuals { get; set; }
        public bool Unknown2 { get; set; }

        public FireComponent()
        {
        }

        public void Read(DataReader reader)
        {
            base.Read(reader, false);
            Template = new ForgeFileReference();
            Template.Read(reader);

            NumParticles = reader.ReadInt32();
            Particles = new FireParticle[NumParticles];

            for (int i = 0; i < NumParticles; i++)
            {
                Particles[i] = new FireParticle();
                Particles[i].Read(reader);
            }

            FireSettings = new ForgeFileReference();
            FireSettings.Read(reader);

            Unknown1 = reader.ReadSingle();

            GlobalFX = new ForgeFileReference();
            GlobalFX.Read(reader);

            ParticleFX = new ForgeFileReference();
            ParticleFX.Read(reader);

            FireDamage = new ForgeFileReference();
            FireDamage.Read(reader);

            FireEnergy = new ForgeFileReference();
            FireEnergy.Read(reader);

            FireResistance = new ForgeFileReference();
            FireResistance.Read(reader);

            CustomizedFireSoundSet = new ForgeFileReference();
            CustomizedFireSoundSet.Read(reader);

            FirePropertyZone = new ForgeFileReference();
            FirePropertyZone.Read(reader);

            PropagationConditions = new ForgeFileReference();
            PropagationConditions.Read(reader);

            IsPersistent = reader.ReadBoolean();
            NumComponentDescriptors = reader.ReadInt32();
            FireComponentDescriptors = new uint[NumComponentDescriptors];

            for (int i = 0; i < NumComponentDescriptors; i++)
            {
                FireComponentDescriptors[i] = reader.ReadUInt32();
            }

            NumLightVisuals = reader.ReadInt32();
            LightVisuals = new uint[NumComponentDescriptors];

            for (int i = 0; i < NumLightVisuals; i++)
            {
                throw new Exception("LightVisuals data not implemented");
            }

            Unknown2 = reader.ReadBoolean();
        }
    }


    /***************************************** WorldComponents *****************************************/
    public abstract class WorldComponent
    {
        public ComponentType ComponentType { get; set; }

        public abstract void Read(DataReader reader);

        public WorldComponent()
        {
        }

        public static WorldComponent CreateWorldComponent(ComponentType type)
        {
            return type switch
            {
                ComponentType.NAVIGATION_MANAGER => new NavigationComponent(),
                ComponentType.SYSTEMIC_MANAGER => new SystemicComponent(),
                ComponentType.GAMEPLAY_COORDINATOR => new GameplayCoordinatorComponent(false),
                ComponentType.DEFERRED_CAST_MANAGER => new DeferredCastComponent(),
                ComponentType.MISSION_INTRO_ACTIVATOR_REPOSITORY => new MissionIntroActivatorComponent(),
                ComponentType.CROWD_HERDER => new CrowdHerderComponent(),
                ComponentType.VISUAL_AMBIANCE_MANAGER => new VisualAmbianceComponent(),
                ComponentType.GAME_AI_WORLD => new GameAIWorldComponent(),
                ComponentType.ABSTRACT_GAMEPLAY_COORDINATOR => new GameplayCoordinatorComponent(true),
                ComponentType.BULK_WORLD => new BulkWorldComponent(),
                ComponentType.PHYSICS_WORLD => new PhysicsWorldComponent(),
                ComponentType.NAVMESH_LINKING_MANAGER
                or ComponentType.SPAWNING_SOURCE_WORLD
                or ComponentType.FIGHT_DIRECTOR_WORLD
                or ComponentType.NAVMESH_PATCHING_MANAGER
                or ComponentType.AI_CONTEXT_MANAGER
                or ComponentType.CONFLICT_LOOP_MANAGER
                or ComponentType.LOCATE_TARGET_WORLD
                or ComponentType.SOUND_REGION_CELL_MANAGER
                or ComponentType.PARKOUR_DEBUGGING_MANAGER
                or ComponentType.EAGLE_VISION_MANAGER
                or ComponentType.VANISHING_MANAGER
                or ComponentType.GAMEPLAY_CONTROL_MANAGER
                or ComponentType.ATOM_WORLD_MANAGER
                or ComponentType.ROLE_BASED_GROUP_MANAGER
                or ComponentType.SOUND_WALL_MANAGER
                or ComponentType.SOUND_FALL_MANAGER
                or ComponentType.DIALOGUE_WORLD
                or ComponentType.COMMUNITY_WORLD => new UnknownWorldComponent(), // Unknown data
                ComponentType.PERMANENT_ICON_MANAGER => new PermanentIconManagerComponent(),
                ComponentType.SPACE_WORLD => new SpaceWorldComponent(),
                ComponentType.TRANSFORM_PREDICTION_MANAGER => new TransformPredictionComponent(),
                ComponentType.WORLD_BOOKMARK => new WorldBookmarkComponent(),
                ComponentType.AI_NETWORK_GAME_INTERFACE => new AIGameNetworkInterfaceComponent(),
                ComponentType.META_AI => new MetaAIComponent(),
                ComponentType.RIDEABLE_SUMMONING_MANAGER => new RideableSummoningManager(),
                ComponentType.PERSONAL_RIDEABLE_MANAGER => new PersonalRideableManagerComponent(),
                ComponentType.SOUND_RFX_MANAGER => new SoundRfxManagerComponent(),
                ComponentType.PLAYER_ABILITY_RESTRICTION_MANAGER => new PlayerRestrictionManagerComponent(),
                ComponentType.GAME_WORLD_AREA_FOG_MANAGER => new GameWordAreaFogManagerComponent(),
                ComponentType.REF_COUNTED_EVENT_MANAGER => new RefCountedEventManagerComponent(),
                ComponentType.SOUND_BANK_WORLD => new SoundBankComponent(),
                ComponentType.SOUND_RIVER_MANAGER => new SoundRiverManagerComponent(),
                ComponentType.INTERIOR_MANAGER => new InteriorManagerComponent(),
                ComponentType.DEBUG_SETTINGS => new DebugSettingsComponent(),
                ComponentType.UI_WORLD => new UIWorldComponent(),
                ComponentType.BLOB_SETTINGS => new BlobSettingsComponent(),
                ComponentType.SPAWN_SETTINGS => new SpawnSettingsComponent(),
                ComponentType.AC_SOUND_AMBIENCE_MANAGER => new ACSoundAmbienceManagerComponent(),
                ComponentType.DESYNC_MANAGER => new DesyncManagerComponent(),
                ComponentType.CREW_ASSISTANCE_MANAGER => new CrewAssistanceManagerComponent(),
                ComponentType.WORLD_TAG_RULES => new WorldTagRulesComponent(),
                _ => throw new Exception("Unknown component type: 0x" + type.ToString("X")),
            };
        }
    }

    public class UnknownWorldComponent : WorldComponent //We'll use this class for every unknown component
    {
        public UnknownWorldComponent()
        {
        }

        public override void Read(DataReader reader)
        {
        }
    }

    public class NavigationComponent : WorldComponent
    {
        public uint NavMeshGridLayoutCellizeOverride { get; set; }
        public ForgeFileReference FloorHeightData { get; set; }
        public ForgeFileReference PilotConfigData { get; set; }

        public NavigationComponent()
        {
        }

        public override void Read(DataReader reader)
        {
            NavMeshGridLayoutCellizeOverride = reader.ReadUInt32();

            FloorHeightData = new ForgeFileReference();
            FloorHeightData.Read(reader);

            PilotConfigData = new ForgeFileReference();
            PilotConfigData.Read(reader);
        }
    }

    public class SystemicComponent : WorldComponent
    {
        public SystemicComponent()
        {
        }

        public override void Read(DataReader reader) //Unknown data
        {
        }
    }

    public class GameplayCoordinatorComponent : WorldComponent
    {
        public bool AbstractGameplayAlternative { get; set; }
        public bool Active { get; set; }
        public ForgeObjectPtr DebugSpawningSpecification { get; set; }
        public float PreviewRadius { get; set; }
        public ForgeGameplayCoordinator Coordinator { get; set; }

        public GameplayCoordinatorComponent(bool abstractGameplayAlternative)
        {
            AbstractGameplayAlternative = abstractGameplayAlternative;
        }

        public override void Read(DataReader reader)
        {
            Active = reader.ReadBoolean();

            DebugSpawningSpecification = new ForgeObjectPtr();
            DebugSpawningSpecification.Read(reader, true);

            PreviewRadius = reader.ReadSingle();

            Coordinator = new ForgeGameplayCoordinator();
            Coordinator.Read(reader, AbstractGameplayAlternative);
        }
    }

    public class DeferredCastComponent : WorldComponent
    {
        public ulong NbMicroSecondsAllowed { get; set; }

        public DeferredCastComponent()
        {
        }

        public override void Read(DataReader reader)
        {
            NbMicroSecondsAllowed = reader.ReadUInt64();
        }
    }

    public class MissionIntroActivatorComponent : WorldComponent
    {
        public int NumMissionIntroActivator { get; set; }

        public MissionIntroActivatorComponent()
        {
        }

        public override void Read(DataReader reader) //TODO: Add MissionIntroActivator
        {
            NumMissionIntroActivator = reader.ReadInt32();
        }
    }

    public class CrowdHerderComponent : WorldComponent
    {
        public uint CrowdMaxNumNPCs { get; set; }
        public uint Unknown_4h { get; set; }
        public float CrowdDensityPrecipitationModifierThreshold { get; set; }

        public CrowdHerderComponent()
        {
        }

        public override void Read(DataReader reader)
        {
            CrowdMaxNumNPCs = reader.ReadUInt32();
            Unknown_4h = reader.ReadUInt32();
            CrowdDensityPrecipitationModifierThreshold = reader.ReadSingle();
        }
    }

    public class VisualAmbianceComponent : WorldComponent
    {
        public float WindGustCompression { get; set; }
        public Vector4 PrevailingWind { get; set; }
        public SoundRTPC RTPCBeaufortLevel { get; set; }
        public ForgeFileReference WindData { get; set; }
        public float VisualTime { get; set; }
        public float FullCycleTime { get; set; }
        public TimeOfDayTransition[] TimeOfDayTransitions { get; set; }
        public float DefaultStartTime { get; set; }
        public bool EnableLockTimeOfDay { get; set; }
        public float YawLockVelocity { get; set; }
        public float PitchLockVelocity { get; set; }
        public float RollLockVelocity { get; set; }
        public float TranslationLockVelocity { get; set; }
        public float TimelapseUnlockDelay { get; set; }
        public float CameraUnlockDelay { get; set; }
        public ForgeFileReference DefaultGlobalVisualAmbiance { get; set; }
        public ForgeFileReference WeatherVisualAmbianceModifier { get; set; }
        public FCurveFloat WeatherModifierInfluence { get; set; }
        public float BlendDuration { get; set; }

        public VisualAmbianceComponent()
        {
        }

        public override void Read(DataReader reader)
        {
            WindGustCompression = reader.ReadSingle();
            PrevailingWind = reader.ReadVector4();

            RTPCBeaufortLevel = new SoundRTPC();
            RTPCBeaufortLevel.Read(reader);

            WindData = new ForgeFileReference(); //Reference<WindData>
            WindData.Read(reader);

            VisualTime = reader.ReadSingle();
            FullCycleTime = reader.ReadSingle();

            TimeOfDayTransitions = new TimeOfDayTransition[2];
            for (int i = 0; i < TimeOfDayTransitions.Length; i++)
            {
                TimeOfDayTransitions[i] = new TimeOfDayTransition();
                TimeOfDayTransitions[i].Read(reader);
            }

            DefaultStartTime = reader.ReadSingle();
            EnableLockTimeOfDay = reader.ReadBoolean();
            YawLockVelocity = reader.ReadSingle();
            PitchLockVelocity = reader.ReadSingle();
            RollLockVelocity = reader.ReadSingle();
            TranslationLockVelocity = reader.ReadSingle();
            TimelapseUnlockDelay = reader.ReadSingle();
            CameraUnlockDelay = reader.ReadSingle();

            DefaultGlobalVisualAmbiance = new ForgeFileReference();
            DefaultGlobalVisualAmbiance.Read(reader);

            WeatherVisualAmbianceModifier = new ForgeFileReference();
            WeatherVisualAmbianceModifier.Read(reader);

            WeatherModifierInfluence = new FCurveFloat();
            WeatherModifierInfluence.Read(reader);

            BlendDuration = reader.ReadSingle();
        }
    }

    public class GameAIWorldComponent : WorldComponent
    {
        public AmbientEventManager AmbientEventManager { get; set; }
        public ZoneSpawnerManager ZoneSpawnerManager { get; set; }

        public GameAIWorldComponent()
        {
        }

        public override void Read(DataReader reader)
        {
            AmbientEventManager = new AmbientEventManager();
            AmbientEventManager.Read(reader);

            ZoneSpawnerManager = new ZoneSpawnerManager();
            ZoneSpawnerManager.Read(reader);
        }
    }

    public class BulkWorldComponent : WorldComponent
    {
        public uint BulkGraphicBufferize { get; set; }

        public BulkWorldComponent()
        {
        }

        public override void Read(DataReader reader)
        {
            BulkGraphicBufferize = reader.ReadUInt32();
        }
    }

    public class PhysicsWorldComponent : WorldComponent
    {
        public Vector4 Gravity { get; set; }
        public Vector3 BroadPhaseMin { get; set; }
        public Vector3 BroadPhaseMax { get; set; }

        public PhysicsWorldComponent()
        {
        }

        public override void Read(DataReader reader)
        {
            Gravity = reader.ReadVector4();
            BroadPhaseMin = reader.ReadVector3();
            BroadPhaseMax = reader.ReadVector3();
        }
    }

    public class PermanentIconManagerComponent : WorldComponent
    {
        public int NumIcons { get; set; }
        public PermanentIconData[] Icons { get; set; }
        public int NumUILocations { get; set; }
        public UILocationBase[] UILocations { get; set; }

        public PermanentIconManagerComponent()
        {
        }

        public override void Read(DataReader reader)
        {
            NumIcons = reader.ReadInt32();
            Icons = new PermanentIconData[NumIcons];

            for (int i = 0; i < Icons.Length; i++)
            {
                Icons[i] = new PermanentIconData();
                Icons[i].Read(reader);
            }

            NumUILocations = reader.ReadInt32();
            UILocations = new UILocationBase[NumUILocations];

            for (int i = 0; i < UILocations.Length; i++)
            {
                UILocations[i] = new UILocationBase();
                UILocations[i].Read(reader);
            }
        }
    }

    public class SpaceWorldComponent : WorldComponent
    {
        public SpaceSection GlobalSection { get; set; }
        public SpaceSection GlobalSectionHandle { get; set; }
        public SoundOcclusionPortalExteriorLinksTable SoundOcclusionPortalExteriorLinksTable { get; set; }

        public SpaceWorldComponent()
        {
        }

        public override void Read(DataReader reader)
        {
            GlobalSection = new SpaceSection();
            GlobalSection.Read(reader, false);

            GlobalSectionHandle = new SpaceSection();
            GlobalSectionHandle.Read(reader, true);

            SoundOcclusionPortalExteriorLinksTable = new SoundOcclusionPortalExteriorLinksTable();
            SoundOcclusionPortalExteriorLinksTable.Read(reader);
        }
    }

    public class TransformPredictionComponent : WorldComponent
    {
        public float MaxDistance { get; set; }
        public bool EnableGuidancePrediction { get; set; }

        public TransformPredictionComponent()
        {
        }

        public override void Read(DataReader reader)
        {
            MaxDistance = reader.ReadSingle();
            EnableGuidancePrediction = reader.ReadBoolean();
        }
    }

    public class WorldBookmarkComponent : WorldComponent
    {
        public int NumTeleportBookmarks { get; set; }
        public TeleportBookmark[] TeleportBookmarks { get; set; }

        public WorldBookmarkComponent()
        {
        }

        public override void Read(DataReader reader)
        {
            NumTeleportBookmarks = reader.ReadInt32();
            TeleportBookmarks = new TeleportBookmark[NumTeleportBookmarks];

            for (int i = 0; i < TeleportBookmarks.Length; i++)
            {
                TeleportBookmarks[i] = new TeleportBookmark();
                TeleportBookmarks[i].Read(reader);
            }
        }
    }

    public class AIGameNetworkInterfaceComponent : WorldComponent
    {
        public ForgeFileReference WorldPathManager { get; set; }

        public AIGameNetworkInterfaceComponent()
        {
        }

        public override void Read(DataReader reader)
        {
            WorldPathManager = new ForgeFileReference();
            WorldPathManager.Read(reader);
        }
    }

    public class MetaAIComponent : WorldComponent
    {
        public int NumExceptions { get; set; }
        public MetaAIObjective[] DebugNotInstanceObjectiveExceptions { get; set; }
        public MetaAISystemicEventTrackingManager SystemicEventTracking { get; set; }
        public ForgeObjectPtr Unknown1 { get; set; }
        public ForgeFileReference MetaAIComponentSettings { get; set; }
        public bool DebugActivatePreRoll { get; set; }
        public bool EnableMetaAI { get; set; }
        public bool EnableSchedule { get; set; }
        public bool EnableAgenda { get; set; }

        public MetaAIComponent()
        {
        }

        public override void Read(DataReader reader)
        {
            NumExceptions = reader.ReadInt32();
            DebugNotInstanceObjectiveExceptions = new MetaAIObjective[NumExceptions];

            for (int i = 0; i < DebugNotInstanceObjectiveExceptions.Length; i++)
            {
                DebugNotInstanceObjectiveExceptions[i] = new MetaAIObjective();
                DebugNotInstanceObjectiveExceptions[i].Read(reader);
            }

            SystemicEventTracking = new MetaAISystemicEventTrackingManager();
            SystemicEventTracking.Read(reader);

            Unknown1 = new ForgeObjectPtr();
            Unknown1.Read(reader);

            MetaAIComponentSettings = new ForgeFileReference();
            MetaAIComponentSettings.Read(reader);

            DebugActivatePreRoll = reader.ReadBoolean();
            EnableMetaAI = reader.ReadBoolean();
            EnableSchedule = reader.ReadBoolean();
            EnableAgenda = reader.ReadBoolean();
        }
    }

    public class RideableSummoningManager : WorldComponent
    {
        public float Unknown_0h { get; set; }

        public RideableSummoningManager()
        {
        }

        public override void Read(DataReader reader)
        {
            Unknown_0h = reader.ReadSingle();
        }
    }

    public class PersonalRideableManagerComponent : WorldComponent
    {
        public int NumHandlers { get; private set; }
        public ForgeFileReference[] RideableHandlers { get; private set; }

        public PersonalRideableManagerComponent()
        {
        }

        public override void Read(DataReader reader)
        {
            NumHandlers = reader.ReadInt32();
            RideableHandlers = new ForgeFileReference[NumHandlers];

            for (int i = 0; i < RideableHandlers.Length; i++)
            {
                RideableHandlers[i] = new ForgeFileReference();
                RideableHandlers[i].Read(reader);
            }
        }
    }

    public class SoundRfxManagerComponent : WorldComponent
    {
        public ForgeFileReference SoundRfxData { get; private set; }
        public float PositionEspilon { get; private set; }

        public SoundRfxManagerComponent()
        {
        }

        public override void Read(DataReader reader)
        {
            SoundRfxData = new ForgeFileReference();
            SoundRfxData.Read(reader);

            PositionEspilon = reader.ReadSingle();
        }
    }

    public class PlayerRestrictionManagerComponent : WorldComponent
    {
        public ForgeFileReference SoundRfxData { get; private set; }
        public float PositionEspilon { get; private set; }

        public PlayerRestrictionManagerComponent()
        {
        }

        public override void Read(DataReader reader) //Unknown data
        {
        }
    }

    public class GameWordAreaFogManagerComponent : WorldComponent
    {
        public float FogThresholdForLOS { get; set; }
        public float SetPlayerVisibilityRatio { get; set; }
        public ForgeFileReference Unknown_8h { get; set; }
        public ForgeFileReference Unknown_Ch { get; set; }
        public uint DensityAdressMapAdressMode { get; set; }
        public float DensityMapStartX { get; set; }
        public float DensityMapStartY { get; set; }
        public float DensityMapRangeX { get; set; }
        public float DensityMapRangeY { get; set; }
        public float HeightMapMax { get; set; }
        public float HeightMapMin { get; set; }
        public bool EnableCPUSampling { get; set; }
        public float SamplingUnit { get; set; }
        public bool EnableFogAmbienceSound { get; set; }
        public float FogThresholdForSound { get; set; }
        public float MinVisibilityForSound { get; set; }
        public float EaseForRTPC { get; set; }

        public GameWordAreaFogManagerComponent()
        {
        }

        public override void Read(DataReader reader)
        {
            FogThresholdForLOS = reader.ReadSingle();
            SetPlayerVisibilityRatio = reader.ReadSingle();

            Unknown_8h = new ForgeFileReference();
            Unknown_8h.Read(reader);

            Unknown_Ch = new ForgeFileReference();
            Unknown_Ch.Read(reader);

            DensityAdressMapAdressMode = reader.ReadUInt32();
            DensityMapStartX = reader.ReadSingle();
            DensityMapStartY = reader.ReadSingle();
            DensityMapRangeX = reader.ReadSingle();
            DensityMapRangeY = reader.ReadSingle();
            HeightMapMax = reader.ReadSingle();
            HeightMapMin = reader.ReadSingle();
            EnableCPUSampling = reader.ReadBoolean();
            SamplingUnit = reader.ReadSingle();
            EnableFogAmbienceSound = reader.ReadBoolean();
            FogThresholdForSound = reader.ReadSingle();
            MinVisibilityForSound = reader.ReadSingle();
            EaseForRTPC = reader.ReadSingle();
        }
    }

    public class RefCountedEventManagerComponent : WorldComponent
    {
        public RefCountedEventManagerComponent()
        {
        }

        public override void Read(DataReader reader) //Unknown data
        {
        }
    }

    public class SoundBankComponent : WorldComponent
    {
        public ForgeFileReference SoundBanksRef { get; set; }

        public SoundBankComponent()
        {
        }

        public override void Read(DataReader reader)
        {
            SoundBanksRef = new ForgeFileReference(); //Reference<SoundBanksLoadOnDemand>
            SoundBanksRef.Read(reader);
        }
    }

    public class SoundRiverManagerComponent : WorldComponent
    {
        public ForgeFileReference SoundSet { get; set; }
        public float MaxSearchDistance { get; set; }

        public SoundRiverManagerComponent()
        {
        }

        public override void Read(DataReader reader)
        {
            SoundSet = new ForgeFileReference(); //Reference<RiversSoundSet>
            SoundSet.Read(reader);

            MaxSearchDistance = reader.ReadSingle();
        }
    }

    public class InteriorManagerComponent : WorldComponent
    {
        public int NumInteriorSettings { get; set; }
        public InteriorSettings[] InteriorSettings { get; set; }

        public InteriorManagerComponent()
        {
        }

        public override void Read(DataReader reader)
        {
            NumInteriorSettings = reader.ReadInt32();
            InteriorSettings = new InteriorSettings[NumInteriorSettings];

            for (int i = 0; i < InteriorSettings.Length; i++)
            {
                InteriorSettings[i] = new InteriorSettings();
                InteriorSettings[i].Read(reader);
            }
        }
    }

    public class DebugSettingsComponent : WorldComponent
    {
        public int NumDebugNPCInfo { get; set; }
        public SpawnNPCInfo[] DebugSpawnNPCInfo { get; set; }

        public int NumDebugAnimalInfo { get; set; }
        public SpawnNPCInfo[] DebugSpawnAnimalInfo { get; set; }

        public int NumDebugRideableAnimalInfo { get; set; }
        public DebugRideableAnimalInfo[] DebugRideableAnimalInfo { get; set; }

        public int NumDebugSpawnObjectInfo { get; set; }
        public SpawnObjectInfo[] DebugSpawnObjectInfo { get; set; }

        public int NumDebugSpawnChariotInfo { get; set; }
        public SpawnChariotInfo[] DebugSpawnChariotInfo { get; set; }

        public int NumDebugSpawnWaterVehicleInfo { get; set; }
        public SpawnWaterVehicleInfo[] DebugSpawnWaterVehicleInfo { get; set; }

        public int NumDebugSpawnNPCInfoGroup { get; set; }
        public SpawnNPCInfoGroup[] DebugSpawnNPCInfoGroup { get; set; }
        public byte Unknown1 { get; set; }
        public ForgeFileReference DebugSpawnNPCFleeReaction { get; set; }
        public ForgeFileReference DebugSpawnFollowObjectiveSchedule { get; set; }
        public ForgeFileReference Unknown2 { get; set; }

        public int NumDebugSpawnStationNPCContextBehaviours { get; set; }
        public ForgeFileReference[] DebugSpawnStationNPCContextBehaviours { get; set; }

        public int NumDebugSpawnMetaAIAnimalBehaviours { get; set; }
        public ForgeFileReference[] DebugSpawnMetaAIAnimalBehaviours { get; set; }

        public int NumDebugFightAvailableVehicles { get; set; }
        public ForgeFileReference[] DebugFightAvailableVehicles { get; set; }
        public BuildTag DebugTag { get; set; }
        public FactionSelector HostileFaction { get; set; }
        public ForgeObjectPtr UniqueNPCWorldFilter { get; set; }
        public ForgeFileReference Unknown3 { get; set; }
        public BuildTags DebugTag2 { get; set; }
        public bool Unknown4 { get; set; }
        public bool PauseSplineMovers { get; set; }
        public float SplineMoversTimeStep { get; set; }

        public DebugSettingsComponent()
        {
        }

        public override void Read(DataReader reader)
        {
            //NPCs
            NumDebugNPCInfo = reader.ReadInt32();
            DebugSpawnNPCInfo = new SpawnNPCInfo[NumDebugNPCInfo];

            for (int i = 0; i < DebugSpawnNPCInfo.Length; i++)
            {
                DebugSpawnNPCInfo[i] = new SpawnNPCInfo();
                DebugSpawnNPCInfo[i].Read(reader);
            }

            //Animals
            NumDebugAnimalInfo = reader.ReadInt32();
            DebugSpawnAnimalInfo = new SpawnNPCInfo[NumDebugAnimalInfo];

            for (int i = 0; i < DebugSpawnAnimalInfo.Length; i++)
            {
                DebugSpawnAnimalInfo[i] = new SpawnNPCInfo();
                DebugSpawnAnimalInfo[i].Read(reader);
            }

            //Rideable animals
            NumDebugRideableAnimalInfo = reader.ReadInt32();
            DebugRideableAnimalInfo = new DebugRideableAnimalInfo[NumDebugRideableAnimalInfo];

            for (int i = 0; i < DebugRideableAnimalInfo.Length; i++)
            {
                DebugRideableAnimalInfo[i] = new DebugRideableAnimalInfo();
                DebugRideableAnimalInfo[i].Read(reader);
            }

            //Objects
            NumDebugSpawnObjectInfo = reader.ReadInt32();
            DebugSpawnObjectInfo = new SpawnObjectInfo[NumDebugSpawnObjectInfo];

            for (int i = 0; i < DebugSpawnObjectInfo.Length; i++)
            {
                DebugSpawnObjectInfo[i] = new SpawnObjectInfo();
                DebugSpawnObjectInfo[i].Read(reader);
            }

            //Chariots
            NumDebugSpawnChariotInfo = reader.ReadInt32();
            DebugSpawnChariotInfo = new SpawnChariotInfo[NumDebugSpawnChariotInfo];

            for (int i = 0; i < DebugSpawnChariotInfo.Length; i++)
            {
                DebugSpawnChariotInfo[i] = new SpawnChariotInfo();
                DebugSpawnChariotInfo[i].Read(reader);
            }

            //Water vehicles
            NumDebugSpawnWaterVehicleInfo = reader.ReadInt32();
            DebugSpawnWaterVehicleInfo = new SpawnWaterVehicleInfo[NumDebugSpawnWaterVehicleInfo];

            for (int i = 0; i < DebugSpawnWaterVehicleInfo.Length; i++)
            {
                DebugSpawnWaterVehicleInfo[i] = new SpawnWaterVehicleInfo();
                DebugSpawnWaterVehicleInfo[i].Read(reader);
            }

            //NPC groups
            NumDebugSpawnNPCInfoGroup = reader.ReadInt32();
            DebugSpawnNPCInfoGroup = new SpawnNPCInfoGroup[NumDebugSpawnNPCInfoGroup];

            for (int i = 0; i < DebugSpawnNPCInfoGroup.Length; i++)
            {
                DebugSpawnNPCInfoGroup[i] = new SpawnNPCInfoGroup();
                DebugSpawnNPCInfoGroup[i].Read(reader);
            }

            Unknown1 = reader.ReadByte();

            DebugSpawnNPCFleeReaction = new ForgeFileReference(); //Reference<MetaAIBehaviourFleeToLocation>
            DebugSpawnNPCFleeReaction.Read(reader);

            DebugSpawnFollowObjectiveSchedule = new ForgeFileReference(); //Reference<MetaAISchedule>
            DebugSpawnFollowObjectiveSchedule.Read(reader);

            Unknown2 = new ForgeFileReference(); //Reference<MetaAISchedule>
            Unknown2.Read(reader);

            NumDebugSpawnStationNPCContextBehaviours = reader.ReadInt32();
            DebugSpawnStationNPCContextBehaviours = new ForgeFileReference[NumDebugSpawnStationNPCContextBehaviours]; //Reference<MetaAIBehaviourGoto>[]

            for (int i = 0; i < DebugSpawnStationNPCContextBehaviours.Length; i++)
            {
                DebugSpawnStationNPCContextBehaviours[i] = new ForgeFileReference();
                DebugSpawnStationNPCContextBehaviours[i].Read(reader);
            }

            NumDebugSpawnMetaAIAnimalBehaviours = reader.ReadInt32();
            DebugSpawnMetaAIAnimalBehaviours = new ForgeFileReference[NumDebugSpawnMetaAIAnimalBehaviours]; //Reference<MetaAIBehaviour>[]

            for (int i = 0; i < DebugSpawnMetaAIAnimalBehaviours.Length; i++)
            {
                DebugSpawnMetaAIAnimalBehaviours[i] = new ForgeFileReference();
                DebugSpawnMetaAIAnimalBehaviours[i].Read(reader);
            }

            NumDebugFightAvailableVehicles = reader.ReadInt32();
            DebugFightAvailableVehicles = new ForgeFileReference[NumDebugFightAvailableVehicles]; //Reference<MetaAIObjectiveRideable>[]

            for (int i = 0; i < DebugFightAvailableVehicles.Length; i++)
            {
                DebugFightAvailableVehicles[i] = new ForgeFileReference();
                DebugFightAvailableVehicles[i].Read(reader);
            }

            DebugTag = new BuildTag();
            DebugTag.Read(reader);

            HostileFaction = new FactionSelector();
            HostileFaction.Read(reader);

            UniqueNPCWorldFilter = new ForgeObjectPtr();
            UniqueNPCWorldFilter.Read(reader);

            Unknown3 = new ForgeFileReference(); //Reference<MetaAIObjectiveDynamic>
            Unknown3.Read(reader);

            DebugTag2 = new BuildTags();
            DebugTag2.Read(reader);

            Unknown4 = reader.ReadBoolean();
            PauseSplineMovers = reader.ReadBoolean();
            SplineMoversTimeStep = reader.ReadSingle();
        }
    }

    public class UIWorldComponent : WorldComponent
    {
        public int NumModes { get; set; }
        public uint[] WorldAdditionHUDModes { get; set; }

        public UIWorldComponent()
        {
        }

        public override void Read(DataReader reader)
        {
            NumModes = reader.ReadInt32();
            WorldAdditionHUDModes = new uint[NumModes];

            for (int i = 0; i < WorldAdditionHUDModes.Length; i++)
            {
                WorldAdditionHUDModes[i] = reader.ReadUInt32(); //Enum
            }
        }
    }

    public class BlobSettingsComponent : WorldComponent
    {
        public bool Enabled { get; private set; }
        public float MinimumSpawnDistance { get; private set; }
        public float MaximumSpawnDistance { get; private set; }
        public float Unknown1 { get; private set; }
        public bool Unknown2 { get; private set; }
        public float Unknown3 { get; private set; }
        public float MinimumUnspawnDistance { get; private set; }
        public float MaximumUnspawnDistance { get; private set; }
        public float UnspawnFOVDistance { get; private set; }
        public uint RecycleMaxLifeInSec { get; private set; }
        public float Unknown4 { get; private set; }
        public uint BlobSpreadingSpeed { get; private set; }
        public float BlobSpreadingSearchDistance { get; private set; }
        public float BlobSize { get; private set; }
        public float CorpseMaxUnspawnRange { get; private set; }
        public float CorpseMinUnspawnRange { get; private set; }
        public uint CorpseCountStartReduceUnspawnRange { get; private set; }
        public uint CorpseCountStopReduceUnspawnRange { get; private set; }
        public uint NumCorpseUnspawnedPerRangeInterval { get; private set; }

        public BlobSettingsComponent()
        {
        }

        public override void Read(DataReader reader)
        {
            Enabled = reader.ReadBoolean();
            MinimumSpawnDistance = reader.ReadSingle();
            MaximumSpawnDistance = reader.ReadSingle();
            Unknown1 = reader.ReadSingle();
            Unknown2 = reader.ReadBoolean();
            Unknown3 = reader.ReadSingle();
            MinimumUnspawnDistance = reader.ReadSingle();
            MaximumUnspawnDistance = reader.ReadSingle();
            UnspawnFOVDistance = reader.ReadSingle();
            RecycleMaxLifeInSec = reader.ReadUInt32();
            Unknown4 = reader.ReadSingle();
            BlobSpreadingSpeed = reader.ReadUInt32();
            BlobSpreadingSearchDistance = reader.ReadSingle();
            BlobSize = reader.ReadSingle();
            CorpseMaxUnspawnRange = reader.ReadSingle();
            CorpseMinUnspawnRange = reader.ReadSingle();
            CorpseCountStartReduceUnspawnRange = reader.ReadUInt32();
            CorpseCountStopReduceUnspawnRange = reader.ReadUInt32();
            NumCorpseUnspawnedPerRangeInterval = reader.ReadUInt32();
        }
    }

    public class SpawnSettingsComponent : WorldComponent
    {
        public uint MaxNPCsSpawnedPerFrame { get; set; }
        public uint MaxNPCsUnpawnedPerFrame { get; set; }
        public uint MaxNPCs { get; set; }
        public float MaxNPCsMultiplier_XB1 { get; set; }
        public float MaxNPCsMultiplier_XB1Scorpio { get; set; }
        public float MaxNPCsMultiplier_PS4 { get; set; }
        public float MaxNPCsMultiplier_PS4Pro { get; set; }
        public float MaxNPCsMultiplier_PC_0_VeryLow { get; set; }
        public float MaxNPCsMultiplier_PC_1_Low { get; set; }
        public float MaxNPCsMultiplier_PC_2_Medium { get; set; }
        public float MaxNPCsMultiplier_PC_3_High { get; set; }
        public float MaxNPCsMultiplier_PC_4_VeryHigh { get; set; }
        public float MaxNPCsMultiplier_PC_5_UltraHigh { get; set; }
        public float SleepDuration { get; set; }
        public float MinimumCharacterLODSwitchDistanceMultiplier { get; set; }
        public float MaximumCharacterLODSwitchDistanceMultiplier { get; set; }
        public float DynamicLODTargetCost { get; set; }
        public float DynamicLODVeryNearCost { get; set; }
        public float DynamicLODNearCost { get; set; }
        public float DynamicLODMidCost { get; set; }
        public float DynamicLODMinDistanceMultiplier { get; set; }
        public float DynamicLODMaxDistanceMultiplier { get; set; }

        public SpawnSettingsComponent()
        {
        }

        public override void Read(DataReader reader)
        {
            MaxNPCsSpawnedPerFrame = reader.ReadUInt32();
            MaxNPCsUnpawnedPerFrame = reader.ReadUInt32();
            MaxNPCs = reader.ReadUInt32();
            MaxNPCsMultiplier_XB1 = reader.ReadSingle();
            MaxNPCsMultiplier_XB1Scorpio = reader.ReadSingle();
            MaxNPCsMultiplier_PS4 = reader.ReadSingle();
            MaxNPCsMultiplier_PS4Pro = reader.ReadSingle();
            MaxNPCsMultiplier_PC_0_VeryLow = reader.ReadSingle();
            MaxNPCsMultiplier_PC_1_Low = reader.ReadSingle();
            MaxNPCsMultiplier_PC_2_Medium = reader.ReadSingle();
            MaxNPCsMultiplier_PC_3_High = reader.ReadSingle();
            MaxNPCsMultiplier_PC_4_VeryHigh = reader.ReadSingle();
            MaxNPCsMultiplier_PC_5_UltraHigh = reader.ReadSingle();
            SleepDuration = reader.ReadSingle();
            MinimumCharacterLODSwitchDistanceMultiplier = reader.ReadSingle();
            MaximumCharacterLODSwitchDistanceMultiplier = reader.ReadSingle();
            DynamicLODTargetCost = reader.ReadSingle();
            DynamicLODVeryNearCost = reader.ReadSingle();
            DynamicLODNearCost = reader.ReadSingle();
            DynamicLODMidCost = reader.ReadSingle();
            DynamicLODMinDistanceMultiplier = reader.ReadSingle();
            DynamicLODMaxDistanceMultiplier = reader.ReadSingle();
        }
    }

    public class ACSoundAmbienceManagerComponent : WorldComponent
    {
        public int NumGlobalAmbiences { get; set; }
        public SoundInstance[] GlobalAmbiences { get; set; }
        public int NumAmbienceSoundStatesIndexes { get; set; }
        public byte[] AmbienceSoundStatesInWorldIndexes { get; set; }
        public int NumAmbienceSoundStates { get; set; }
        public SoundState[] AmbienceSoundStatesInWorld { get; set; }
        public int AmbienceWidth { get; set; }
        public int AmbienceHeight { get; set; }
        public Vector2 OriginOffset { get; set; }
        public float AmbienceSoundStatesInWorldPrecision { get; set; }

        public ACSoundAmbienceManagerComponent()
        {
        }

        public override void Read(DataReader reader)
        {
            NumGlobalAmbiences = reader.ReadInt32();
            GlobalAmbiences = new SoundInstance[NumGlobalAmbiences];

            for (int i = 0; i < GlobalAmbiences.Length; i++)
            {
                GlobalAmbiences[i] = new SoundInstance();
                GlobalAmbiences[i].Read(reader);
            }

            NumAmbienceSoundStatesIndexes = reader.ReadInt32();
            AmbienceSoundStatesInWorldIndexes = new byte[NumAmbienceSoundStatesIndexes];

            for (int i = 0; i < AmbienceSoundStatesInWorldIndexes.Length; i++)
            {
                AmbienceSoundStatesInWorldIndexes[i] = reader.ReadByte();
            }

            NumAmbienceSoundStates = reader.ReadInt32();
            AmbienceSoundStatesInWorld = new SoundState[NumAmbienceSoundStates];

            for (int i = 0; i < AmbienceSoundStatesInWorld.Length; i++)
            {
                AmbienceSoundStatesInWorld[i] = new SoundState();
                AmbienceSoundStatesInWorld[i].Read(reader);
            }

            AmbienceWidth = reader.ReadInt32();
            AmbienceHeight = reader.ReadInt32();
            OriginOffset = reader.ReadVector2();
            AmbienceSoundStatesInWorldPrecision = reader.ReadSingle();
        }
    }

    public class DesyncManagerComponent : WorldComponent
    {
        public DesynchronizationSettings DesynchronizationSettings { get; set; }

        public DesyncManagerComponent()
        {
        }

        public override void Read(DataReader reader)
        {
            DesynchronizationSettings = new DesynchronizationSettings();
            DesynchronizationSettings.Read(reader);
        }
    }

    public class CrewAssistanceManagerComponent : WorldComponent
    {
        public SpecialAbilityDataAbstract SpecialAbilityData { get; set; }

        public CrewAssistanceManagerComponent()
        {
        }

        public override void Read(DataReader reader)
        {
            SpecialAbilityData = new SpecialAbilityDataAbstract();
            SpecialAbilityData.Read(reader);
        }
    }

    public class WorldTagRulesComponent : WorldComponent
    {
        public int NumRules { get; set; }
        public TagRules[] Rules { get; set; }

        public WorldTagRulesComponent()
        {
        }

        public override void Read(DataReader reader)
        {
            NumRules = reader.ReadInt32();
            Rules = new TagRules[NumRules];

            for (int i = 0; i < Rules.Length; i++)
            {
                Rules[i] = new TagRules();
                Rules[i].Read(reader);
            }
        }
    }

    public enum ComponentType : uint
    {
        //Default components
        CLASSIC = 0xEC658D29,
        GUIDANCE_SYSTEM = 0x55AF1C3E,
        RIGID_BODY = 0xC177D702,
        SOUND = 0xE8134060,
        FIRE = 0x1B614D31,

        //World components
        NAVIGATION_MANAGER = 0x9B9C3F3,
        SYSTEMIC_MANAGER = 0x2FB78D2E,
        GAMEPLAY_COORDINATOR = 0xEC7E0424,
        DEFERRED_CAST_MANAGER = 0x4F8DA0EB,
        MISSION_INTRO_ACTIVATOR_REPOSITORY = 0xB0FFD20C,
        CROWD_HERDER = 0x1697D188,
        VISUAL_AMBIANCE_MANAGER = 0xDBB9593E,
        GAME_AI_WORLD = 0xC443D6DE,
        ABSTRACT_GAMEPLAY_COORDINATOR = 0xD4451318,
        BULK_WORLD = 0x4601ADF1,
        PHYSICS_WORLD = 0xD0B67579,
        NAVMESH_LINKING_MANAGER = 0xE71B6847, //Unknown data
        SPAWNING_SOURCE_WORLD = 0x82CAFF23, //Unknown data
        PERMANENT_ICON_MANAGER = 0xB8D2E7F0,
        FIGHT_DIRECTOR_WORLD = 0x247A289B, //Unknown data
        NAVMESH_PATCHING_MANAGER = 0x81FCDBED, //Unknown data
        SPACE_WORLD = 0x77FFB40E,
        AI_CONTEXT_MANAGER = 0xB7BDF1C2, //Unknown data
        CONFLICT_LOOP_MANAGER = 0xB707DC33, //Unknown data
        TRANSFORM_PREDICTION_MANAGER = 0x3ACE509C,
        LOCATE_TARGET_WORLD = 0xC3A82AB7, //Unknown data
        WORLD_BOOKMARK = 0x50BA2D04,
        AI_NETWORK_GAME_INTERFACE = 0xC2437385,
        META_AI = 0x788D00B8,
        RIDEABLE_SUMMONING_MANAGER = 0x53AEA36A,
        SOUND_REGION_CELL_MANAGER = 0x09EB880D, //Unknown data
        PERSONAL_RIDEABLE_MANAGER = 0xDB398804,
        SOUND_RFX_MANAGER = 0x5113C46D,
        PLAYER_ABILITY_RESTRICTION_MANAGER = 0x34F1EDF8, //Unknown data
        GAME_WORLD_AREA_FOG_MANAGER = 0x3D25788F,
        REF_COUNTED_EVENT_MANAGER = 0x011DB368, //Unknown data
        SOUND_BANK_WORLD = 0x4DA30368,
        SOUND_RIVER_MANAGER = 0x8BE4BAE7,
        INTERIOR_MANAGER = 0xA846671D,
        DEBUG_SETTINGS = 0x9B7A3615,
        PARKOUR_DEBUGGING_MANAGER = 0xE8A40587, //Unknown data
        UI_WORLD = 0xEEC2D4C7,
        EAGLE_VISION_MANAGER = 0xBECAA0EC, //Unknown data
        BLOB_SETTINGS = 0xD62B00DA,
        SPAWN_SETTINGS = 0xF7010C1C,
        VANISHING_MANAGER = 0x30DD45CD, //Unknown data
        GAMEPLAY_CONTROL_MANAGER = 0x3A98F27D, //Unknown data
        ATOM_WORLD_MANAGER = 0x2013DBD0, //Unknown data
        AC_SOUND_AMBIENCE_MANAGER = 0x72D950FE,
        DESYNC_MANAGER = 0xCD1D0232,
        ROLE_BASED_GROUP_MANAGER = 0x72A40EA6, //Unknown data
        SOUND_WALL_MANAGER = 0xC79FBD00, //Unknown data
        SOUND_FALL_MANAGER = 0x50221C93, //Unknown data
        DIALOGUE_WORLD = 0xBC54960E, //Unknown data
        CREW_ASSISTANCE_MANAGER = 0x8445C078,
        COMMUNITY_WORLD = 0x10114CEE, //Unknown data
        WORLD_TAG_RULES = 0x2A749F1C
    }
}