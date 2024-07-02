using CodeX.Core.Utilities;
using SharpDX;
using System.Numerics;

namespace CodeX.Games.ACOdyssey.FORGE
{
    public class ForgeGameplayCoordinator
    {
        public ForgeBaseObjectPtr BaseObjectPtrGameplayCoordinator { get; set; }
        public GameplayAbstract GameplayLogic { get; set; }
        public GameplayCoordinatorSettings Settings { get; set; }
        public GameplayCoordinatorOutput DoneOutput { get; private set; }
        public GameplayCoordinatorOutput CompletedOutput { get; private set; }
        public GameplayCoordinatorOutput AbortedOutput { get; private set; }
        public GameplayCoordinatorOutput AbortedInternallyOutput { get; private set; }
        public GameplayCoordinatorOutput AbortedExternallyOutput { get; private set; }
        public GameplayCoordinatorOutput SuccessOutput { get; private set; }
        public GameplayCoordinatorOutput FailureOutput { get; private set; }
        public GameplayCoordinatorOutput StartedOutput { get; private set; }
        public GameplayCoordinatorOutput PausedOutput { get; private set; }
        public GameplayCoordinatorOutput UnpausedOutput { get; private set; }
        public GameplayCoordinatorOutput HardPausedOutput { get; private set; }
        public GameplayCoordinatorOutput HardUnpausedOutput { get; private set; }

        public ForgeGameplayCoordinator()
        {
        }

        public void Read(DataReader reader, bool abstractGameplay)
        {
            BaseObjectPtrGameplayCoordinator = new ForgeBaseObjectPtr();
            BaseObjectPtrGameplayCoordinator.Read(reader);

            if (abstractGameplay)
                GameplayLogic = new GameplayComplexCrowdLife();
            else
                GameplayLogic = new GameplayPlayer();
            GameplayLogic.Read(reader);

            Settings = new GameplayCoordinatorSettings();
            Settings.Read(reader);

            DoneOutput = new GameplayCoordinatorOutput();
            DoneOutput.Read(reader);

            CompletedOutput = new GameplayCoordinatorOutput();
            CompletedOutput.Read(reader);

            AbortedOutput = new GameplayCoordinatorOutput();
            AbortedOutput.Read(reader);

            AbortedInternallyOutput = new GameplayCoordinatorOutput();
            AbortedInternallyOutput.Read(reader);

            AbortedExternallyOutput = new GameplayCoordinatorOutput();
            AbortedExternallyOutput.Read(reader);

            SuccessOutput = new GameplayCoordinatorOutput();
            SuccessOutput.Read(reader);

            FailureOutput = new GameplayCoordinatorOutput();
            FailureOutput.Read(reader);

            StartedOutput = new GameplayCoordinatorOutput();
            StartedOutput.Read(reader);

            PausedOutput = new GameplayCoordinatorOutput();
            PausedOutput.Read(reader);

            UnpausedOutput = new GameplayCoordinatorOutput();
            UnpausedOutput.Read(reader);

            HardPausedOutput = new GameplayCoordinatorOutput();
            HardPausedOutput.Read(reader);

            HardUnpausedOutput = new GameplayCoordinatorOutput();
            HardUnpausedOutput.Read(reader);
        }
    }

    public abstract class GameplayAbstract
    {
        public abstract void Read(DataReader reader);
    }

    public class GameplayPlayer : GameplayAbstract
    {
        public ForgeObject ObjectGameplayLogic { get; set; }
        public SpawningSpecification PlayerSpecification { get; set; }
        public SpawningSpecification EagleSpecification { get; set; }
        public SpawningSpecification FirstPersonPlayerSpecification { get; set; }

        public GameplayPlayer()
        {
        }

        public override void Read(DataReader reader)
        {
            ObjectGameplayLogic = new ForgeObject();
            ObjectGameplayLogic.Read(reader);

            PlayerSpecification = new SpawningSpecification(); //Object<SpawningSpecification>
            PlayerSpecification.Read(reader);

            EagleSpecification = new SpawningSpecification(); //ObjectPtr<SpawningSpecification>
            EagleSpecification.Read(reader, objectPtr: true);

            FirstPersonPlayerSpecification = new SpawningSpecification(); //Object<SpawningSpecification>
            FirstPersonPlayerSpecification.Read(reader, defaultStrategy: false, defaultPosition: false);
        }
    }

    public class GameplayComplexCrowdLife : GameplayAbstract
    {
        public ForgeObject ObjectGameplayLogic { get; set; }
        public CrowdLifeConfiguration Configuration { get; set; }
        public float GoToReachDistance { get; set; }
        public bool PersistOnSpawnSucceeded { get; set; }
        public bool PersistOnVirtualPatrol { get; set; }
        public bool AllowAcrobatics { get; set; }
        public bool IsManager { get; set; }
        public EntityControlOptions ReferencingOptions { get; set; }

        public GameplayComplexCrowdLife()
        {
        }

        public override void Read(DataReader reader)
        {
            ObjectGameplayLogic = new ForgeObject();
            ObjectGameplayLogic.Read(reader);

            Configuration = new CrowdLifeConfiguration();
            Configuration.Read(reader);

            GoToReachDistance = reader.ReadSingle();
            PersistOnSpawnSucceeded = reader.ReadBoolean();
            PersistOnVirtualPatrol = reader.ReadBoolean();
            AllowAcrobatics = reader.ReadBoolean();
            IsManager = reader.ReadBoolean();

            ReferencingOptions = new EntityControlOptions();
            ReferencingOptions.Read(reader);
        }
    }

    public class EntityControlOptions
    {
        public ForgeBaseObject BaseObjectEntityControlOptions { get; set; }
        public bool AllowInterruptions { get; set; }
        public bool DisableForceUnspawn { get; set; }
        public bool QueueRequest { get; set; }
        public bool ClearExclusiveReactions { get; set; }

        public EntityControlOptions()
        {
        }

        public void Read(DataReader reader)
        {
            BaseObjectEntityControlOptions = new ForgeBaseObject();
            BaseObjectEntityControlOptions.Read(reader);

            AllowInterruptions = reader.ReadBoolean();
            DisableForceUnspawn = reader.ReadBoolean();
            QueueRequest = reader.ReadBoolean();
            ClearExclusiveReactions = reader.ReadBoolean();
        }
    }

    public class CrowdLifeConfiguration
    {
        public ForgeObject ObjectGameplayLogic { get; set; }
        public uint UnspawnMode { get; set; }
        public int NumStationBlock { get; set; }

        public CrowdLifeConfiguration()
        {
        }

        public void Read(DataReader reader)
        {
            ObjectGameplayLogic = new ForgeObject();
            ObjectGameplayLogic.Read(reader);

            UnspawnMode = reader.ReadUInt32();
            NumStationBlock = reader.ReadInt32();
        }
    }

    public class SpawningSpecification
    {
        public object ObjectSpawningSpecification { get; set; }
        public SpawnStrategyParams StrategyParams { get; set; }
        public SpawnPositionParams PositionParams { get; set; }
        public int NumEntityParams { get; set; }
        public SpawnPlayerParams[] EntityParams { get; set; }
        public bool StayReferencable { get; set; }
        public ForgeObjectPtr GroupParams { get; set; }
        public ForgeObjectPtr AICondition { get; set; }

        public SpawningSpecification()
        {
        }

        public void Read(DataReader reader, bool defaultStrategy = true, bool defaultPosition = true, bool objectPtr = false)
        {
            if (objectPtr)
            {
                ObjectSpawningSpecification = new ForgeBaseObjectPtr();
                ((ForgeBaseObjectPtr)ObjectSpawningSpecification).Read(reader);
            }
            else
            {
                ObjectSpawningSpecification = new ForgeObject();
                ((ForgeObject)ObjectSpawningSpecification).Read(reader);
            }

            if (defaultStrategy)
            {
                StrategyParams = new SpawnStrategyParams();
                StrategyParams.Read(reader);
            }
            else
            {
                StrategyParams = new BlobStrategyParams();
                ((BlobStrategyParams)StrategyParams).Read(reader);
            }

            if (defaultPosition)
            {
                PositionParams = new SpawnPositionParams();
                PositionParams.Read(reader);
            }
            else
            {
                PositionParams = new SpawnPositionStaticParams();
                ((SpawnPositionStaticParams)PositionParams).Read(reader);
            }

            NumEntityParams = reader.ReadInt32();
            EntityParams = new SpawnPlayerParams[NumEntityParams];

            for (int i = 0; i < NumEntityParams; i++)
            {
                EntityParams[i] = new SpawnPlayerParams();
                EntityParams[i].Read(reader);
            }

            StayReferencable = reader.ReadBoolean();

            GroupParams = new ForgeObjectPtr();
            GroupParams.Read(reader, true);

            AICondition = new ForgeObjectPtr();
            AICondition.Read(reader, true);
        }
    }

    public class SpawnStrategyParams
    {
        public ForgeObject ObjectBlobStrategyParams { get; set; }

        public SpawnStrategyParams()
        {
        }

        public void Read(DataReader reader)
        {
            ObjectBlobStrategyParams = new ForgeObject();
            ObjectBlobStrategyParams.Read(reader);
        }
    }

    public class BlobStrategyParams : SpawnStrategyParams
    {
        public uint UnspawnMode { get; set; }
        public uint Caste { get; set; }

        public BlobStrategyParams()
        {
        }

        public new void Read(DataReader reader)
        {
            base.Read(reader);
            UnspawnMode = reader.ReadUInt32();
            Caste = reader.ReadUInt32();
        }
    }

    public class SpawnPositionParams
    {
        public ForgeObject ObjectSpawnPositionBlobParams { get; set; }
        public bool AllowSpawningOffNavmesh { get; set; }
        public bool Unknown_1h { get; set; }
        public bool Unknown_2h { get; set; }
        public bool CanSpawnAtDistance { get; set; }
        public bool ValidatePhysicsSpace { get; set; }
        public uint PositionType { get; set; }
        public Vector4 LocalSpawnPosition { get; set; }
        public Vector4 LocalSpawnOrientation { get; set; }
        public ForgeObjectPtr SpawnReferencePosition { get; set; }

        public SpawnPositionParams()
        {
        }

        public void Read(DataReader reader)
        {
            ObjectSpawnPositionBlobParams = new ForgeObject();
            ObjectSpawnPositionBlobParams.Read(reader);

            AllowSpawningOffNavmesh = reader.ReadBoolean();
            Unknown_1h = reader.ReadBoolean();
            Unknown_2h = reader.ReadBoolean();
            CanSpawnAtDistance = reader.ReadBoolean();
            ValidatePhysicsSpace = reader.ReadBoolean();
            PositionType = reader.ReadUInt32();
            LocalSpawnPosition = reader.ReadVector4();
            LocalSpawnOrientation = reader.ReadVector4();

            SpawnReferencePosition = new ForgeObjectPtr();
            SpawnReferencePosition.Read(reader, true);
        }
    }

    public class SpawnPositionStaticParams : SpawnPositionParams
    {
        public float MinSpawningDistance { get; set; }
        public float MaxSpawningDistance { get; set; }
        public bool ForceSpawnInFOV { get; set; }
        public bool ExcludeAwarenessZone { get; set; }
        public bool ExcludeNoCrowdRegions { get; set; }

        public SpawnPositionStaticParams()
        {
        }

        public new void Read(DataReader reader)
        {
            base.Read(reader);
            MinSpawningDistance = reader.ReadSingle();
            MaxSpawningDistance = reader.ReadSingle();
            ForceSpawnInFOV = reader.ReadBoolean();
            ExcludeAwarenessZone = reader.ReadBoolean();
            ExcludeNoCrowdRegions = reader.ReadBoolean();
        }
    }

    public class SpawnPlayerParams
    {
        public ForgeObject ObjectSpawnPlayerParams { get; set; }
        public BuildTags Tags { get; set; }
        public int NumWeaponTags { get; set; }
        public BuildTags[] WeaponsTags { get; set; }
        public int SeedToUseForGeneration { get; set; }
        public DynamicReference TagBuilderDynamicRef { get; set; }
        public bool Exclusive { get; set; }
        public int ExclusiveNPCType { get; set; }
        public uint ExplicitNPC { get; set; }
        public ForgeBaseObjectPtr SelectionFilter { get; set; }
        public ForgeFileReference OverrideInitialAction { get; set; }
        public ProgressionCharacterSelector ProgressionCharacterSelector { get; set; }

        public SpawnPlayerParams()
        {
        }

        public void Read(DataReader reader)
        {
            ObjectSpawnPlayerParams = new ForgeObject();
            ObjectSpawnPlayerParams.Read(reader);

            Tags = new BuildTags();
            Tags.Read(reader);

            NumWeaponTags = reader.ReadInt32();
            WeaponsTags = new BuildTags[NumWeaponTags];

            for (int i = 0; i < NumWeaponTags; i++)
            {
                WeaponsTags[i] = new BuildTags();
                WeaponsTags[i].Read(reader);
            }

            SeedToUseForGeneration = reader.ReadInt32();

            TagBuilderDynamicRef = new DynamicReference();
            TagBuilderDynamicRef.Read(reader);

            Exclusive = reader.ReadBoolean();
            ExclusiveNPCType = reader.ReadInt32();
            ExplicitNPC = reader.ReadUInt32();

            SelectionFilter = new ForgeBaseObjectPtr(); //BaseObjectPtr<DataLayerFilter>
            SelectionFilter.Read(reader, true);

            OverrideInitialAction = new ForgeFileReference(); //Reference<Animation>
            OverrideInitialAction.Read(reader);

            ProgressionCharacterSelector = new ProgressionCharacterSelector();
            ProgressionCharacterSelector.Read(reader);
        }
    }

    public class BuildTags
    {
        public ForgeBaseObject BaseObjectBuildNumTags { get; set; }
        public int NumTags { get; set; }
        public BuildTag[] Tags { get; set; }

        public BuildTags()
        {
        }

        public void Read(DataReader reader)
        {
            BaseObjectBuildNumTags = new ForgeObject();
            BaseObjectBuildNumTags.Read(reader);

            NumTags = reader.ReadInt32();
            Tags = new BuildTag[NumTags];

            for (int i = 0; i < NumTags; i++)
            {
                Tags[i] = new BuildTag();
                Tags[i].Read(reader);
            }
        }
    }

    public class BuildTag
    {
        public ForgeBaseObject BaseObjectBuildTag { get; set; }
        public uint EngineTagSave { get; set; }

        public BuildTag()
        {
        }

        public void Read(DataReader reader)
        {
            BaseObjectBuildTag = new ForgeBaseObject();
            BaseObjectBuildTag.Read(reader);

            EngineTagSave = reader.ReadUInt32();
        }
    }

    public class DynamicReference
    {
        public ForgeBaseObject BaseObjectDynamicReference { get; set; }
        public ForgeFileReference ObjectReference { get; set; }
        public ForgeObjectPtr ObjectHandle { get; set; }

        public DynamicReference()
        {
        }

        public void Read(DataReader reader)
        {
            BaseObjectDynamicReference = new ForgeBaseObject();
            BaseObjectDynamicReference.Read(reader);

            ObjectReference = new ForgeFileReference();
            ObjectReference.Read(reader);

            ObjectHandle = new ForgeObjectPtr();
            ObjectHandle.Read(reader);
        }
    }

    public class ProgressionCharacterSelector
    {
        public ForgeBaseObject BaseObjectProgressionCharacterSelector { get; set; }
        public ForgeObjectPtr ProgressionCharacter { get; set; }
        public bool UseCurrent { get; set; }

        public ProgressionCharacterSelector()
        {
        }

        public void Read(DataReader reader)
        {
            BaseObjectProgressionCharacterSelector = new ForgeBaseObject();
            BaseObjectProgressionCharacterSelector.Read(reader);

            ProgressionCharacter = new ForgeObjectPtr();
            ProgressionCharacter.Read(reader);

            UseCurrent = reader.ReadBoolean();
        }
    }

    public class GameplayCoordinatorSettings
    {
        public ForgeBaseObject BaseObjectGameplayCoordinatorSettings { get; set; }
        public uint PlayMode { get; set; }
        public ForgeObjectPtr PlayModeParam { get; set; }
        public SpawningActionPack SpawningActionPack { get; set; }
        public ForgeObjectPtr CoordinatorActionPack { get; set; }
        public ForgeObjectPtr ReferencingActionPack { get; set; }
        public ForgeObjectPtr StartupCondition { get; set; }
        public int NumConstraints { get; set; }
        public ForgeObjectPtr[] Constraints { get; set; }

        public GameplayCoordinatorSettings()
        {
        }

        public void Read(DataReader reader)
        {
            BaseObjectGameplayCoordinatorSettings = new ForgeBaseObject();
            BaseObjectGameplayCoordinatorSettings.Read(reader);

            PlayMode = reader.ReadUInt32();

            PlayModeParam = new ForgeObjectPtr(); //GameplayCoordinatorPlayModeParam
            PlayModeParam.Read(reader, true);

            SpawningActionPack = new SpawningActionPack(); //SpawningActionPack
            SpawningActionPack.Read(reader);

            CoordinatorActionPack = new ForgeObjectPtr(); //CoordinatorActionPack
            CoordinatorActionPack.Read(reader, true);

            ReferencingActionPack = new ForgeObjectPtr(); //ReferencingActionPack
            ReferencingActionPack.Read(reader, true);

            StartupCondition = new ForgeObjectPtr(); //CoordinatorCondition
            StartupCondition.Read(reader, true);

            NumConstraints = reader.ReadInt32();
            Constraints = new ForgeObjectPtr[NumConstraints]; //CoordinatorConstraints

            for (int i = 0; i < Constraints.Length; i++)
            {
                Constraints[i] = new ForgeObjectPtr();
                Constraints[i].Read(reader);
            }
        }
    }

    public class SpawningActionPack
    {
        public ForgeObject ObjectSpawningActionPack { get; set; }
        public int NumActionsOnpawn { get; set; }
        public ForgeObjectPtr[] ActionsOnSpawn { get; set; }
        public int NumActionsOnAcquire { get; set; }
        public ForgeObjectPtr[] ActionsOnAcquire { get; set; }
        public int NumActionsOnPreUnspawn { get; set; }
        public ForgeObjectPtr[] ActionsOnPreUnspawn { get; set; }

        public SpawningActionPack()
        {
        }

        public void Read(DataReader reader)
        {
            ObjectSpawningActionPack = new ForgeObject();
            ObjectSpawningActionPack.Read(reader, true);

            if (ObjectSpawningActionPack.Num == 3)
            {
                return;
            }

            NumActionsOnpawn = reader.ReadInt32();
            ActionsOnSpawn = new ForgeObjectPtr[NumActionsOnpawn];

            NumActionsOnAcquire = reader.ReadInt32();
            ActionsOnAcquire = new ForgeObjectPtr[NumActionsOnAcquire];

            NumActionsOnPreUnspawn = reader.ReadInt32();
            ActionsOnPreUnspawn = new ForgeObjectPtr[NumActionsOnPreUnspawn];
        }
    }

    public class GameplayCoordinatorOutput
    {
        public ForgeObject ObjectGameplayCoordinatorOutput { get; set; }

        public GameplayCoordinatorOutput()
        {
        }

        public void Read(DataReader reader) //Unknown data
        {
            ObjectGameplayCoordinatorOutput = new ForgeObject();
            ObjectGameplayCoordinatorOutput.Read(reader);
        }
    }
}