using CodeX.Core.Utilities;
using CodeX.Games.ACOdyssey.FORGE;
using System;
using System.Numerics;

namespace CodeX.Games.ACOdyssey.Resources
{
    public class ForgeEntityGroup : ForgeEntity
    {
        public int NumEntity { get; set; }
        public ForgeEntity[] Entities { get; set; }
        public int NumUIEntities { get; set; }
        public ForgeObjectPtr[] UIEntitiesDisplayOrder { get; set; }

        public ForgeEntityGroup()
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

            base.Read(reader, false);
            NumEntity = reader.ReadInt32();
            Entities = new ForgeEntity[NumEntity];
            
            for (int i = 0; i < NumEntity; i++)
            {
                Entities[i] = new ForgeEntity();
                Entities[i].Read(reader, true);
            }

            NumUIEntities = reader.ReadInt32();
            UIEntitiesDisplayOrder = new ForgeObjectPtr[NumEntity];

            for (int i = 0; i < NumUIEntities; i++)
            {
                UIEntitiesDisplayOrder[i] = new ForgeObjectPtr();
                UIEntitiesDisplayOrder[i].Read(reader, true);
            }
        }
    }

    public class ForgeEntity
    {
        public ForgeFileReference ReferenceEntity { get; set; }
        public ForgeBaseObjectPtr Hierarchy { get; set; }
        public byte CheckByte { get; set; } //3
        public Matrix4x4? GlobalMatrix { get; set; }
        public int ComponentCount { get; set; }
        public Component[] Components { get; set; }
        public bool SkipGroupMatrixUpdate { get; set; }
        public bool IsGraphicsUnitTestCameraReferencePosition { get; set; }
        public bool IsPhantom { get; set; }
        public bool IsHidden { get; set; }
        public bool IsSmallObject { get; set; }
        public bool IsMediumObject { get; set; }
        public byte RenderLast { get; set; }
        public bool ForceIncludedInReflection { get; set; }
        public bool ForceIncludedFromReflection { get; set; }
        public bool IsCharacter { get; set; }
        public bool ShadowCaster { get; set; }
        public bool UsesVisualOrderingHint { get; set; }
        public bool UseVisibilityQueries { get; set; }
        public bool VisibilityOccluder { get; set; }
        public bool IgnoreOcclusionForLODLevel { get; set; }
        public int EntityCategory { get; set; }
        public bool NeverDynamicInStencil { get; set; }
        public bool PropagateEventToHighestParent { get; set; }
        public bool ComponentLODGroupDisabled { get; set; }
        public bool CastStaticShadows { get; set; }
        public bool IsExcludedFromParentBV { get; set; }
        public int UICategory { get; set; }
        public bool UIFlag { get; set; }
        public bool UsePlanarDistanceForGraphicObject { get; set; }
        public bool PerInstanceLODTransition { get; set; }
        public bool CameraTAADitherEnabled { get; set; }
        public bool WetnessDisabled { get; set; }
        public float WetnessBias { get; set; }
        public int LightMask { get; set; }
        public Mask DeconstructionGroupMask { get; set; }
        public bool OptimizeForHardwareInstancing { get; set; }
        public bool FakeCellIndexValid { get; set; }
        public int FakeCellIndex { get; set; }
        public int GridTypeForFakeMasking { get; set; }
        public float Scale { get; set; }
        public ForgeBaseObject BaseObjectBV { get; set; }
        public Vector3 BoundingVolumeMin { get; set; }
        public Vector3 BoundingVolumeMax { get; set; }
        public BoundingVolumeType BoundingVolumeType { get; set; }
        public EntityDescriptor EntityDescriptor { get; set; }
        public DataLayerFilter DataLayerFilter { get; set; }
        public int NumUIVisuals { get; set; }
        public Visual[] UIVisualsDisplayOrder { get; set; }
        public byte[] PerViewDirCullingDistances { get; set; }
        public GameStateData ResetData { get; set; }
        public StreamableTexture[] StreamableTextures { get; set; }
        public int EffectiveGridType { get; set; }
        public ushort MinLodIndex { get; set; }
        public ushort MaxLodIndex { get; set; }

        public ForgeEntity()
        {
        }

        public void Read(DataReader reader, bool singleEntity)
        {
            if (singleEntity)
            {
                ReferenceEntity = new ForgeFileReference();
                ReferenceEntity.Read(reader);

                if (ReferenceEntity.Num != 0)
                {
                    return;
                }
            }

            Hierarchy = new ForgeBaseObjectPtr();
            Hierarchy.Read(reader, true, 0x3);
            CheckByte = reader.ReadByte();
            GlobalMatrix = reader.ReadMatrix4x4();
            ComponentCount = reader.ReadInt32();
            Components = new Component[ComponentCount];

            for (int i = 0; i < ComponentCount; i++)
            {
                var componentType = new ForgeBaseObjectPtr();
                componentType.Read(reader);

                switch ((ComponentType)componentType.FileType)
                {
                    case ComponentType.CLASSIC:
                        Components[i] = new Component();
                        Components[i].Read(reader);
                        break;
                    case ComponentType.GUIDANCE_SYSTEM:
                        Components[i] = new GuidanceSystemComponent();
                        ((GuidanceSystemComponent)Components[i]).Read(reader);
                        break;
                    case ComponentType.RIGID_BODY:
                        Components[i] = new RigidBodyComponent();
                        ((RigidBodyComponent)Components[i]).Read(reader);
                        break;
                    case ComponentType.SOUND:
                        Components[i] = new AudioComponent();
                        ((AudioComponent)Components[i]).Read(reader);
                        break;
                    case ComponentType.FIRE:
                        Components[i] = new FireComponent();
                        ((FireComponent)Components[i]).Read(reader);
                        break;
                    default:
                        throw new Exception("Unknown component type");
                }

                if (Components[i] != null)
                {
                    Components[i].ComponentType = (ComponentType)componentType.FileType;
                }
            }

            SkipGroupMatrixUpdate = reader.ReadBoolean();
            IsGraphicsUnitTestCameraReferencePosition = reader.ReadBoolean();
            IsPhantom = reader.ReadBoolean();
            IsHidden = reader.ReadBoolean();
            IsSmallObject = reader.ReadBoolean();
            IsMediumObject = reader.ReadBoolean();
            RenderLast = reader.ReadByte();
            ForceIncludedInReflection = reader.ReadBoolean();
            ForceIncludedFromReflection = reader.ReadBoolean();
            IsCharacter = reader.ReadBoolean();
            ShadowCaster = reader.ReadBoolean();
            UsesVisualOrderingHint = reader.ReadBoolean();
            UseVisibilityQueries = reader.ReadBoolean();
            VisibilityOccluder = reader.ReadBoolean();
            IgnoreOcclusionForLODLevel = reader.ReadBoolean();
            EntityCategory = reader.ReadInt32();
            NeverDynamicInStencil = reader.ReadBoolean();
            PropagateEventToHighestParent = reader.ReadBoolean();
            ComponentLODGroupDisabled = reader.ReadBoolean();
            CastStaticShadows = reader.ReadBoolean();
            IsExcludedFromParentBV = reader.ReadBoolean();
            UICategory = reader.ReadInt32();
            UIFlag = reader.ReadBoolean();
            UsePlanarDistanceForGraphicObject = reader.ReadBoolean();
            PerInstanceLODTransition = reader.ReadBoolean();
            CameraTAADitherEnabled = reader.ReadBoolean();
            WetnessDisabled = reader.ReadBoolean();
            WetnessBias = reader.ReadSingle();
            LightMask = reader.ReadInt32();

            DeconstructionGroupMask = new Mask();
            DeconstructionGroupMask.Read(reader);

            OptimizeForHardwareInstancing = reader.ReadBoolean();
            FakeCellIndexValid = reader.ReadBoolean();
            FakeCellIndex = reader.ReadInt32();
            GridTypeForFakeMasking = reader.ReadInt32();
            Scale = reader.ReadSingle();

            BaseObjectBV = new ForgeBaseObject();
            BaseObjectBV.Read(reader);

            BoundingVolumeMin = reader.ReadVector3();
            BoundingVolumeMax = reader.ReadVector3();
            BoundingVolumeType = (BoundingVolumeType)reader.ReadInt32();

            EntityDescriptor = new EntityDescriptor();
            EntityDescriptor.Read(reader);

            DataLayerFilter = new DataLayerFilter();
            DataLayerFilter.Read(reader);

            NumUIVisuals = reader.ReadInt32();
            UIVisualsDisplayOrder = new Visual[NumUIVisuals];
            for (int i = 0; i < NumUIVisuals; i++)
            {
                UIVisualsDisplayOrder[i] = new Visual();
                UIVisualsDisplayOrder[i].Read(reader);
            }

            PerViewDirCullingDistances = reader.ReadBytes(6);

            ResetData = new GameStateData();
            ResetData.Read(reader);

            StreamableTextures = new StreamableTexture[4];
            for (int i = 0; i < StreamableTextures.Length; i++)
            {
                StreamableTextures[i] = new StreamableTexture();
                StreamableTextures[i].Read(reader);
            }

            EffectiveGridType = reader.ReadInt32();
            MinLodIndex = reader.ReadUInt16();
            MaxLodIndex = reader.ReadUInt16();
        }
    }

    public class FireParticle
    {
        public ForgeBaseObjectPtr ObjectPtrFireParticle { get; set; }
        public ForgeFileReference CustomFX { get; set; }
        public Vector4 FXPositionOffset { get; set; }
        public float FXRadiusModifier { get; set; }
        public int ParticleShape { get; set; }
        public int BipedBone { get; set; }
        public float Radius { get; set; }
        public Vector4 HalfExtentsInternal { get; set; }
        public Vector3 LocalPosition { get; set; }
        public Vector4 LocalRotation { get; set; }
        public ForgeObjectPtr Tag { get; set; }
        public bool SelfPropagationOnly { get; set; }
        public bool UseEntityPositionForWaterLevel { get; set; }

        public FireParticle()
        {
        }

        public void Read(DataReader reader)
        {
            ObjectPtrFireParticle = new ForgeBaseObjectPtr();
            ObjectPtrFireParticle.Read(reader);

            CustomFX = new ForgeFileReference();
            CustomFX.Read(reader);

            FXPositionOffset = reader.ReadVector4();
            FXRadiusModifier = reader.ReadSingle();
            ParticleShape = reader.ReadInt32();
            BipedBone = reader.ReadInt32();
            Radius = reader.ReadSingle();
            HalfExtentsInternal = reader.ReadVector4();
            LocalPosition = reader.ReadVector3();
            LocalRotation = reader.ReadVector4();

            Tag = new ForgeObjectPtr();
            Tag.Read(reader);

            SelfPropagationOnly = reader.ReadBoolean();
            UseEntityPositionForWaterLevel = reader.ReadBoolean();
        }
    }

    public class SimpleSoundSubComponent
    {
        public ForgeBaseObjectPtr BaseObjectPtrSubEmitters { get; set; }
        public SoundEmitter SoundEmitter { get; set; }
        public Vector4 PersistentLocalPosition { get; set; }
        public int SoundEmitterTag1 { get; set; }
        public int SoundEmitterTag2 { get; set; }
        public bool ActivateHWInstancing { get; set; }

        public SimpleSoundSubComponent()
        {
        }

        public void Read(DataReader reader)
        {
            BaseObjectPtrSubEmitters = new ForgeBaseObjectPtr();
            BaseObjectPtrSubEmitters.Read(reader);

            SoundEmitter = new SoundEmitter();
            SoundEmitter.Read(reader);

            PersistentLocalPosition = reader.ReadVector4();
            SoundEmitterTag1 = reader.ReadInt32();
            SoundEmitterTag2 = reader.ReadInt32();
            ActivateHWInstancing = reader.ReadBoolean();
        }
    }

    public class SoundEmitter
    {
        public ForgeBaseObject BaseObjectSoundEmitter { get; set; }
        public bool ForceCenterSpeaker { get; set; }
        public bool IsEnvironmental { get; set; }
        public bool EnableControllerRumble { get; set; }
        public bool StopSoundsUponUnregister { get; set; }
        public bool NeverForceBypassIgnorableEvents { get; set; }
        public bool CheckOwnerRotation { get; set; }
        public bool CheckOwnerRotationTEMP { get; set; }
        public bool DisableMicrophoneConeAttenuation { get; set; }
        public ForgeFileReference OcclusionAdditionalConeAttenuation { get; set; }
        public float OrientationAngleRadian { get; set; }
        public ForgeBaseObject BaseObjectSwitchDependencies { get; set; }
        public int SwitchPresetIndex { get; set; }
        public int NumSwitchValues { get; set; }
        public int NumRTPC { get; set; }
        public int NumSoundRegisterInstance { get; set; }
        public int NumSoundUnregisterInstance { get; set; }

        public SoundEmitter()
        {
        }

        public void Read(DataReader reader)
        {
            BaseObjectSoundEmitter = new ForgeBaseObject();
            BaseObjectSoundEmitter.Read(reader);

            ForceCenterSpeaker = reader.ReadBoolean();
            IsEnvironmental = reader.ReadBoolean();
            EnableControllerRumble = reader.ReadBoolean();
            StopSoundsUponUnregister = reader.ReadBoolean();
            NeverForceBypassIgnorableEvents = reader.ReadBoolean();
            CheckOwnerRotation = reader.ReadBoolean();
            CheckOwnerRotationTEMP = reader.ReadBoolean();
            DisableMicrophoneConeAttenuation = reader.ReadBoolean();

            OcclusionAdditionalConeAttenuation = new ForgeFileReference();
            OcclusionAdditionalConeAttenuation.Read(reader);

            OrientationAngleRadian = reader.ReadSingle();

            BaseObjectSwitchDependencies = new ForgeBaseObject();
            BaseObjectSwitchDependencies.Read(reader);
            SwitchPresetIndex = reader.ReadInt32();

            NumSwitchValues = reader.ReadInt32(); //ObjectPtr<SoundSwitch>[]
            NumRTPC = reader.ReadInt32(); //ObjectPtr<SoundRTPC>[]
            NumSoundRegisterInstance = reader.ReadInt32(); //BaseObject<SoundInstance>[]
            NumSoundUnregisterInstance = reader.ReadInt32(); //BaseObject<SoundInstance>[]
        }
    }

    public class SoundRTPC
    {
        public ForgeBaseObject BaseObjectSoundRTPC { get; set; }
        public SoundID ID { get; set; }
        public float InitValue { get; set; }
        public float Precision { get; set; }

        public SoundRTPC()
        {
        }

        public void Read(DataReader reader)
        {
            BaseObjectSoundRTPC = new ForgeBaseObject();
            BaseObjectSoundRTPC.Read(reader);

            ID = new SoundID();
            ID.Read(reader);

            InitValue = reader.ReadSingle();
            Precision = reader.ReadSingle();
        }
    }

    public class SoundID
    {
        public ForgeBaseObject BaseObjectSoundID { get; set; }
        public uint ShortID { get; set; }

        public SoundID()
        {
        }

        public void Read(DataReader reader)
        {
            BaseObjectSoundID = new ForgeBaseObject();
            BaseObjectSoundID.Read(reader);

            ShortID = reader.ReadUInt32();
        }
    }

    public class TimeOfDayTransition
    {
        public ForgeBaseObject BaseObjectTimeOfDayTransition { get; set; }
        public ForgeObjectPtr AssociatedLayer { get; set; }

        public TimeOfDayTransition()
        {
        }

        public void Read(DataReader reader)
        {
            BaseObjectTimeOfDayTransition = new ForgeBaseObject();
            BaseObjectTimeOfDayTransition.Read(reader);

            AssociatedLayer = new ForgeObjectPtr();
            AssociatedLayer.Read(reader);
        }
    }

    public class EntityDescriptor
    {
        public ForgeBaseObject BaseObjectEntityDescriptor { get; set; }
        public int DescriptorType { get; set; }
        public uint SubDescriptorType { get; set; }
        public uint ExplicitProperty { get; set; }
        public bool IsPickableByPlayer { get; set; }
        public bool IsPickableByNPC { get; set; }
        public bool IsASmartObject { get; set; }
        public uint ADBEntryID { get; set; }
        public bool Unknown1 { get; set; }

        public EntityDescriptor()
        {
        }

        public void Read(DataReader reader)
        {
            BaseObjectEntityDescriptor = new ForgeBaseObject();
            BaseObjectEntityDescriptor.Read(reader);
            DescriptorType = reader.ReadInt32();
            SubDescriptorType = reader.ReadUInt32();
            ExplicitProperty = reader.ReadUInt32();
            IsPickableByPlayer = reader.ReadBoolean();
            IsPickableByNPC = reader.ReadBoolean();
            IsASmartObject = reader.ReadBoolean();
            ADBEntryID = reader.ReadUInt32();
            Unknown1 = reader.ReadBoolean();
        }
    }

    public class DataLayerFilter
    {
        public ForgeBaseObject BaseObjectDataLayerFilter { get; set; }
        public DataLayerAction[] LayerActions { get; set; }

        public DataLayerFilter()
        {
        }

        public void Read(DataReader reader)
        {
            BaseObjectDataLayerFilter = new ForgeBaseObject();
            BaseObjectDataLayerFilter.Read(reader);
            LayerActions = new DataLayerAction[BaseObjectDataLayerFilter.Unknown_4h];

            for (int i = 0; i < LayerActions.Length; i++)
            {
                LayerActions[i] = new DataLayerAction();
                LayerActions[i].Read(reader);
            }
        }
    }

    public class Visual
    {
        public Visual()
        {
        }

        public void Read(DataReader reader) //Unknown data block
        {
        }
    }

    public class GameStateData
    {
        public int Unknown1 { get; set; }
        public ForgeObject ObjectGameStateData { get; set; }
        public int PropertyCount { get; set; }
        public int NumDescBuffer { get; set; }
        public byte[] DescBuffer { get; set; }
        public int NumDataBuffer { get; set; }
        public byte[] DataBuffer { get; set; }

        public GameStateData()
        {
        }

        public void Read(DataReader reader) //Unknown data block
        {
            Unknown1 = reader.ReadInt32();

            ObjectGameStateData = new ForgeObject();
            ObjectGameStateData.Read(reader, true);

            PropertyCount = reader.ReadInt32();

            NumDescBuffer = reader.ReadInt32();
            DescBuffer = reader.ReadBytes(NumDescBuffer);

            NumDataBuffer = reader.ReadInt32();
            DataBuffer = reader.ReadBytes(NumDataBuffer);
        }
    }

    public class StreamableTexture
    {
        public ForgeBaseObject BaseObjectStreamableTexture { get; set; }
        public int Unknown1 { get; set; }
        public int NumTextures { get; private set; }
        public ForgeObjectPtr[] Textures { get; private set; }

        public StreamableTexture()
        {
        }

        public void Read(DataReader reader)
        {
            BaseObjectStreamableTexture = new ForgeBaseObject();
            BaseObjectStreamableTexture.Read(reader);

            NumTextures = reader.ReadInt32();
            Textures = new ForgeObjectPtr[NumTextures];

            for (int i = 0; i < Textures.Length; i++)
            {
                Textures[i] = new ForgeObjectPtr();
                Textures[i].Read(reader);
            }
        }
    }

    public class DataLayerAction
    {
        public DataLayerAction()
        {
        }

        public void Read(DataReader reader) //Unknown data block
        {
        }
    }

    public class ListShapeOverrideData
    {
        public ForgeBaseObject BaseObjectShapeData { get; set; }
        public bool IsGround { get; set; }
        public CollisionFilterInfo FilterInfo { get; set; }

        public ListShapeOverrideData()
        {
        }

        public void Read(DataReader reader)
        {
            BaseObjectShapeData = new ForgeBaseObject();
            BaseObjectShapeData.Read(reader);

            IsGround = reader.ReadBoolean();

            FilterInfo = new CollisionFilterInfo();
            FilterInfo.Read(reader);
        }
    }

    public class RigidBody
    {
        public ForgeBaseObject BaseObjectRigid { get; set; }
        public CollisionFilterInfo FilterInfo { get; set; }
        public ForgeFileReference Material { get; set; }
        public int Quality { get; set; }
        public ForgeFileReference Shape { get; set; }
        public ForgeFileReference OwnedShape { get; set; }
        public float Mass { get; set; }
        public float LinearDamping { get; set; }
        public float AngularDamping { get; set; }
        public float GravityFactor { get; set; }
        public float DrivingMode { get; set; }
        public bool IsCollidable { get; set; }
        public bool Unknown1 { get; set; }
        public bool LockAxisX { get; set; }
        public bool LockAxisY { get; set; }
        public bool LockAxisZ { get; set; }

        public RigidBody()
        {
        }

        public void Read(DataReader reader)
        {
            BaseObjectRigid = new ForgeBaseObject();
            BaseObjectRigid.Read(reader);

            FilterInfo = new CollisionFilterInfo();
            FilterInfo.Read(reader);

            Material = new ForgeFileReference();
            Material.Read(reader);

            Quality = reader.ReadInt32();

            Shape = new ForgeFileReference();
            Shape.Read(reader);

            OwnedShape = new ForgeFileReference();
            OwnedShape.Read(reader);

            Mass = reader.ReadSingle();
            LinearDamping = reader.ReadSingle();
            AngularDamping = reader.ReadSingle();
            GravityFactor = reader.ReadSingle();
            DrivingMode = reader.ReadSingle();
            IsCollidable = reader.ReadBoolean();
            Unknown1 = reader.ReadBoolean();
            LockAxisX = reader.ReadBoolean();
            LockAxisY = reader.ReadBoolean();
            LockAxisZ = reader.ReadBoolean();
        }
    }

    public class CollisionFilterInfo
    {
        public ForgeBaseObject BaseObjectFilterInfo { get; set; }
        public int Layer { get; set; }
        public int Part { get; set; }
        public bool NoCameraCollision { get; set; }
        public bool NoCameraOcclusion { get; set; }
        public bool NoLineOfSightOcclusion { get; set; }
        public bool NoLineOfSightDynamicOcclusion { get; set; }
        public bool NoWeatherExposed { get; set; }
        public bool ForceNoCover { get; set; }
        public bool RockClimbing { get; set; }
        public bool ScrambleSlideBehavior { get; set; }
        public bool Unknown1 { get; set; }

        public CollisionFilterInfo()
        {
        }

        public void Read(DataReader reader)
        {
            BaseObjectFilterInfo = new ForgeBaseObject();
            BaseObjectFilterInfo.Read(reader);

            Layer = reader.ReadInt32();
            Part = reader.ReadInt32();
            NoCameraCollision = reader.ReadBoolean();
            NoCameraOcclusion = reader.ReadBoolean();
            NoLineOfSightOcclusion = reader.ReadBoolean();
            NoLineOfSightDynamicOcclusion = reader.ReadBoolean();
            NoWeatherExposed = reader.ReadBoolean();
            ForceNoCover = reader.ReadBoolean();
            RockClimbing = reader.ReadBoolean();
            ScrambleSlideBehavior = reader.ReadBoolean();
            Unknown1 = reader.ReadBoolean();
        }
    }

    public class GuidanceSystemData
    {
        public int NumGuidanceObjects { get; set; }
        public GuidanceObject[] GuidanceObjects { get; set; }
        public int NumPoints { get; set; }
        public Vector3[] Points { get; set; }

        public GuidanceSystemData()
        {
        }

        public void Read(DataReader reader)
        {
            reader.BaseStream.Position += 0xD;
            NumGuidanceObjects = reader.ReadInt32();
            GuidanceObjects = new GuidanceObject[NumGuidanceObjects];

            for (int i = 0; i < GuidanceObjects.Length; i++)
            {
                GuidanceObjects[i] = new GuidanceObject();
                GuidanceObjects[i].Read(reader);
            }

            NumPoints = reader.ReadInt32();
            Points = new Vector3[NumPoints];

            for (int i = 0; i < Points.Length; i++)
            {
                Points[i] = reader.ReadVector3();
            }
        }
    }

    public class EdgeFilter
    {
        public bool EdgeWallWall { get; set; }
        public bool EdgeWallGround { get; set; }
        public bool EdgeWallCeiling { get; set; }
        public bool EdgeWallVoid { get; set; }
        public bool EdgeGroundCeiling { get; set; }
        public bool EdgeGroundVoid { get; set; }
        public bool EdgeCeilingVoid { get; set; }
        public bool UseSlopeAngleForWallAgainstGroundOrCeiling { get; set; }
        public float SlopeCosAngle { get; set; }
        public float WallAngle { get; set; }
        public float MinZDiffAngle { get; set; }
        public float SlopeAngleForWallAgainstGroundOrCeiling { get; set; }

        public EdgeFilter()
        {
        }

        public void Read(DataReader reader)
        {
            reader.BaseStream.Position += 0xC;
            EdgeWallWall = reader.ReadBoolean();
            EdgeWallGround = reader.ReadBoolean();
            EdgeWallCeiling = reader.ReadBoolean();
            EdgeWallVoid = reader.ReadBoolean();
            EdgeGroundCeiling = reader.ReadBoolean();
            EdgeGroundVoid = reader.ReadBoolean();
            EdgeCeilingVoid = reader.ReadBoolean();
            UseSlopeAngleForWallAgainstGroundOrCeiling = reader.ReadBoolean();
            SlopeCosAngle = reader.ReadSingle();
            WallAngle = reader.ReadSingle();
            MinZDiffAngle = reader.ReadSingle();
            SlopeAngleForWallAgainstGroundOrCeiling = reader.ReadSingle();
        }
    }

    public class Partitioner
    {
        public ushort RootIndex { get; set; }
        public uint NumLeafs { get; set; }
        public ushort[] LeafsIndex { get; set; }
        public Vector3 Min { get; set; }
        public Vector3 Max { get; set; }
        public uint NumNodes { get; set; }
        public PartNode[] ListNodes { get; set; }

        public Partitioner()
        {
        }

        public void Read(DataReader reader)
        {
            reader.BaseStream.Position += 0xD;
            RootIndex = reader.ReadUInt16();
            NumLeafs = reader.ReadUInt32();
            LeafsIndex = new ushort[NumLeafs];

            for (int i = 0; i < NumLeafs; i++)
            {
                LeafsIndex[i] = reader.ReadUInt16();
            }

            Min = reader.ReadVector3();
            Max = reader.ReadVector3();
            NumNodes = reader.ReadUInt32();
            ListNodes = new PartNode[NumNodes];

            for (int i = 0; i < NumNodes; i++)
            {
                ListNodes[i] = new PartNode();
                ListNodes[i].Read(reader);
            }
        }
    }

    public class PartNode
    {
        public ForgeBaseObject BaseObjectPartNode { get; set; }
        public int NodeType { get; set; }
        public ushort Index0 { get; set; }
        public ushort Index1 { get; set; }
        public float Middle { get; set; }

        public PartNode()
        {
        }

        public void Read(DataReader reader)
        {
            BaseObjectPartNode = new ForgeBaseObject();
            BaseObjectPartNode.Read(reader);

            NodeType = reader.ReadInt32();
            Index0 = reader.ReadUInt16();
            Index1 = reader.ReadUInt16();
            Middle = reader.ReadSingle();
        }
    }

    public class GuidanceObject
    {
        public ForgeBaseObject BaseObjectGuidanceObject { get; set; }
        public bool Valid { get; set; }
        public int Index0 { get; set; }
        public int Index1 { get; set; }
        public GuidanceObjectTypingInfo TypingInfo { get; set; }
        public uint Dec4NNormal_0 { get; set; }
        public uint Dec4NNormal_1 { get; set; }

        public GuidanceObject()
        {
        }

        public void Read(DataReader reader)
        {
            BaseObjectGuidanceObject = new ForgeBaseObject();
            BaseObjectGuidanceObject.Read(reader);

            Valid = reader.ReadBoolean();
            Index0 = reader.ReadInt32();
            Index1 = reader.ReadInt32();

            TypingInfo = new GuidanceObjectTypingInfo();
            TypingInfo.Read(reader);

            Dec4NNormal_0 = reader.ReadUInt32();
            Dec4NNormal_1 = reader.ReadUInt32();
        }
    }

    public class GuidanceObjectTypingInfo
    {
        public ForgeBaseObject BaseObjectTypingInfo { get; set; }
        public int SubType { get; set; }
        public bool IsPassOver { get; set; }
        public bool IsNoHand { get; set; }
        public bool IsNoHandsOnly { get; set; }
        public bool IsNoPassOver { get; set; }
        public bool IsAvoid { get; set; }
        public bool IsSwing { get; set; }

        public GuidanceObjectTypingInfo()
        {
        }

        public void Read(DataReader reader)
        {
            BaseObjectTypingInfo = new ForgeBaseObject();
            BaseObjectTypingInfo.Read(reader);

            SubType = reader.ReadInt32();
            IsPassOver = reader.ReadBoolean();
            IsNoHand = reader.ReadBoolean();
            IsNoHandsOnly = reader.ReadBoolean();
            IsNoPassOver = reader.ReadBoolean();
            IsAvoid = reader.ReadBoolean();
            IsSwing = reader.ReadBoolean();
        }
    }

    public class LODSelectorInstance
    {
        public int InstanceMatricesCount { get; set; }
        public Matrix4x4[] InstanceMatrices { get; set; }
        public VisualShaderConstantsContainer InstanceShaderConstants { get; set; }
        public VisualStaticPermutationsContainer InstanceStaticPermutations { get; set; }
        public BoundingVolume InstanceOriginalBV { get; set; }
        public InstanceLODConsts InstanceLODConsts { get; set; }
        public LODSelectorRef LODSelector { get; set; }
        public GraphicObjectInstanceData[] LODInstanceData { get; set; }

        public LODSelectorInstance()
        {
        }

        public void Read(DataReader reader)
        {
            InstanceMatricesCount = reader.ReadInt32();
            InstanceMatrices = new Matrix4x4[InstanceMatricesCount];

            for (int i = 0; i < InstanceMatricesCount; i++)
            {
                InstanceMatrices[i] = reader.ReadMatrix4x4();
            }

            InstanceShaderConstants = new VisualShaderConstantsContainer();
            InstanceShaderConstants.Read(reader);

            InstanceStaticPermutations = new VisualStaticPermutationsContainer();
            InstanceStaticPermutations.Read(reader);

            InstanceOriginalBV = new BoundingVolume();
            InstanceOriginalBV.Read(reader);

            InstanceLODConsts = new InstanceLODConsts();
            InstanceLODConsts.Read(reader);

            LODSelector = new LODSelectorRef();
            LODSelector.Read(reader);

            LODInstanceData = new GraphicObjectInstanceData[5];
            for (int i = 0; i < LODInstanceData.Length; i++)
            {
                LODInstanceData[i] = new GraphicObjectInstanceData();
                LODInstanceData[i].Read(reader);
            }
        }
    }

    public class VisualShaderConstantsContainer
    {
        public byte CheckByte { get; set; }
        public int NumShaderConstants { get; set; }
        public VisualShaderConstants[] UniqueShaderConstants { get; set; }
        public int NumConstants { get; set; }
        public int[] ShaderConstantIndirections { get; set; }

        public VisualShaderConstantsContainer()
        {
            ShaderConstantIndirections = Array.Empty<int>();
        }

        public void Read(DataReader reader) //437 vs 1576
        {
            CheckByte = reader.ReadByte();
            if (CheckByte != 0)
            {
                return;
            }

            reader.BaseStream.Position += 0xC;
            NumShaderConstants = reader.ReadInt32();
            if (NumShaderConstants <= 0)
            {
                reader.BaseStream.Position += 0x1;
                return;
            }

            UniqueShaderConstants = new VisualShaderConstants[NumShaderConstants];

            for (int i = 0; i < NumShaderConstants; i++)
            {
                UniqueShaderConstants[i] = new VisualShaderConstants();
                UniqueShaderConstants[i].Read(reader);
            }

            NumConstants = reader.ReadInt32();
            ShaderConstantIndirections = new int[NumConstants];

            for (int i = 0; i < NumConstants; i++)
            {
                ShaderConstantIndirections[i] = reader.ReadInt32();
            }
        }
    }

    public class VisualShaderConstants
    {
        public ForgeBaseObject BaseObjectDataLayout { get; set; }
        public uint DataLayout { get; set; }
        public ForgeBaseObject DefaultLayout { get; set; }
        public ForgeBaseObject CollectionLayout { get; set; }
        public ForgeBaseObject UILayout { get; set; }
        public ForgeBaseObject CharacterLayout { get; set; }
        public ForgeBaseObject SkinLayout { get; set; }
        public ForgeBaseObject ClothLayout { get; set; }
        public ForgeBaseObject MetalLayout { get; set; }
        public byte[] Data { get; set; }

        public VisualShaderConstants()
        {
            Data = Array.Empty<byte>();
        }

        public void Read(DataReader reader)
        {
            BaseObjectDataLayout = new ForgeBaseObject();
            BaseObjectDataLayout.Read(reader);
            DataLayout = reader.ReadUInt32(); //Enum

            DefaultLayout = new ForgeBaseObject();
            DefaultLayout.Read(reader);

            CollectionLayout = new ForgeBaseObject();
            CollectionLayout.Read(reader);

            UILayout = new ForgeBaseObject();
            UILayout.Read(reader);

            CharacterLayout = new ForgeBaseObject();
            CharacterLayout.Read(reader);

            SkinLayout = new ForgeBaseObject();
            SkinLayout.Read(reader);

            ClothLayout = new ForgeBaseObject();
            ClothLayout.Read(reader);

            MetalLayout = new ForgeBaseObject();
            MetalLayout.Read(reader);

            Data = reader.ReadBytes(16);
        }
    }

    public class VisualStaticPermutationsContainer
    {
        public ForgeObject ObjectPermutations { get; set; }
        public int NumPermutations { get; set; }
        public VisualStaticPermutationsArray[] UniqueVisualPermutations { get; set; }
        public uint UniqueVisualPermutationsHash { get; set; }
        public int NumInstances { get; set; }
        public ushort[] Instances { get; set; }

        public VisualStaticPermutationsContainer()
        {
        }

        public void Read(DataReader reader)
        {
            ObjectPermutations = new ForgeObject();
            ObjectPermutations.Read(reader, true);

            if (ObjectPermutations.Num == 3)
            {
                return;
            }

            NumPermutations = reader.ReadInt32();
            UniqueVisualPermutations = new VisualStaticPermutationsArray[NumPermutations];

            for (int i = 0; i < UniqueVisualPermutations.Length; i++)
            {
                UniqueVisualPermutations[i] = new VisualStaticPermutationsArray();
                UniqueVisualPermutations[i].Read(reader);
            }

            UniqueVisualPermutationsHash = reader.ReadUInt32();

            NumInstances = reader.ReadInt32();
            Instances = new ushort[NumInstances];

            for (int i = 0; i < Instances.Length; i++)
            {
                Instances[i] = reader.ReadUInt16();
            }
        }
    }

    public class VisualStaticPermutationsArray
    {
        public ForgeBaseObject BaseObjectVisualStaticPermutationsArray { get; set; }
        public int NumStaticPermutations { get; set; }
        public ForgeFileReference[] StaticPermutations { get; set; }

        public VisualStaticPermutationsArray()
        {
        }

        public void Read(DataReader reader)
        {
            BaseObjectVisualStaticPermutationsArray = new ForgeBaseObject();
            BaseObjectVisualStaticPermutationsArray.Read(reader);

            NumStaticPermutations = reader.ReadInt32();
            StaticPermutations = new ForgeFileReference[NumStaticPermutations];

            for (int i = 0; i < StaticPermutations.Length; i++)
            {
                StaticPermutations[i] = new ForgeFileReference();
                StaticPermutations[i].Read(reader);
            }
        }
    }

    public class InstanceLODConsts
    {
        public ForgeBaseObject BaseObjectInstanceLOD { get; set; }
        public float DistanceScale { get; set; }
        public float SwitchInDistance { get; set; }
        public float SwitchOutDistance { get; set; }
        public float SwitchInFadeRange { get; set; }
        public float SwitchOutFadeRange { get; set; }
        public int LODLevel { get; set; }

        public InstanceLODConsts()
        {
        }

        public void Read(DataReader reader)
        {
            BaseObjectInstanceLOD = new ForgeBaseObject();
            BaseObjectInstanceLOD.Read(reader);
            DistanceScale = reader.ReadSingle();
            SwitchInDistance = reader.ReadSingle();
            SwitchOutDistance = reader.ReadSingle();
            SwitchInFadeRange = reader.ReadSingle();
            SwitchOutFadeRange = reader.ReadSingle();
            LODLevel = reader.ReadInt32();
        }
    }

    public class LODSelectorRef
    {
        public ForgeObjectPtr Selector { get; set; }

        public LODSelectorRef()
        {
        }

        public void Read(DataReader reader)
        {
            Selector = new ForgeObjectPtr();
            Selector.Read(reader);
        }
    }

    public class GraphicObjectInstanceData
    {
        public VisualShaderConstantsContainer InstanceShaderConstants { get; set; }
        public VisualStaticPermutationsContainer InstanceStaticPermutations { get; set; }
        public BoundingVolume InstanceOriginalBV { get; set; }
        public InstanceLODConsts InstanceLODConsts { get; set; }
        public ForgeFileReference Mesh { get; set; }
        public CompiledMeshInstance CompiledMeshInstance { get; set; }
        public int NumMaterialInfos { get; set; }
        public MeshInstanceMaterialInfo[] MaterialInfos { get; set; }

        public GraphicObjectInstanceData()
        {
        }

        public void Read(DataReader reader)
        {
            InstanceShaderConstants = new VisualShaderConstantsContainer();
            InstanceShaderConstants.Read(reader);

            if (InstanceShaderConstants.CheckByte != 0) //Invalid object
            {
                return;
            }

            InstanceStaticPermutations = new VisualStaticPermutationsContainer();
            InstanceStaticPermutations.Read(reader);

            InstanceOriginalBV = new BoundingVolume();
            InstanceOriginalBV.Read(reader);

            InstanceLODConsts = new InstanceLODConsts();
            InstanceLODConsts.Read(reader);

            Mesh = new ForgeFileReference();
            Mesh.Read(reader);

            CompiledMeshInstance = new CompiledMeshInstance();
            CompiledMeshInstance.Read(reader);

            NumMaterialInfos = reader.ReadInt32();
            MaterialInfos = new MeshInstanceMaterialInfo[NumMaterialInfos];
            for (int i = 0; i < NumMaterialInfos; i++)
            {
                MaterialInfos[i] = new MeshInstanceMaterialInfo();
                MaterialInfos[i].Read(reader);
            }
        }
    }

    public class CompiledMeshInstance
    {
        public byte VertexFormat { get; set; }
        public int NumStreams { get; set; }
        public CompiledMeshInstanceStream[] Streams { get; set; }
        public int PlatformVersion { get; set; }
        public int SDKVersion { get; set; }
        public ulong MeshHash { get; set; }

        public CompiledMeshInstance()
        {
        }

        public void Read(DataReader reader)
        {
            reader.BaseStream.Position += 0xD;
            VertexFormat = reader.ReadByte();
            NumStreams = reader.ReadInt32();
            Streams = new CompiledMeshInstanceStream[NumStreams];

            for (int i = 0; i < NumStreams; i++)
            {
                Streams[i] = new CompiledMeshInstanceStream();
                Streams[i].Read(reader);
            }

            PlatformVersion = reader.ReadInt32();
            SDKVersion = reader.ReadInt32();
            MeshHash = reader.ReadUInt64();
        }
    }

    public class CompiledMeshInstanceStream
    {
        public CompiledMeshInstanceStream()
        {
        }

        public void Read(DataReader reader)
        {
            //Not sure yet
        }
    }

    public class MeshInstanceMaterialInfo
    {
        public ForgeObjectPtr GraphicObjectInstance { get; set; }
        public ForgeObjectPtr MeshMaterial { get; set; }
        public ForgeFileReference InstanceMaterial { get; set; }

        public MeshInstanceMaterialInfo()
        {
        }

        public void Read(DataReader reader)
        {
            reader.BaseStream.Position += 0xD;

            GraphicObjectInstance = new ForgeObjectPtr();
            GraphicObjectInstance.Read(reader);

            MeshMaterial = new ForgeObjectPtr();
            MeshMaterial.Read(reader);

            InstanceMaterial = new ForgeFileReference();
            InstanceMaterial.Read(reader);
        }
    }

    public class VisualAutoResizer
    {
        public byte CheckByte { get; set; }

        public VisualAutoResizer()
        {
        }

        public void Read(DataReader reader) //Unknown data, needs more testing
        {
            CheckByte = reader.ReadByte();
            if (CheckByte != 0)
            {
                return;
            }
            throw new Exception("Unknown data - Im Foxxyyy needs to work on that part...");
        }
    }

    public class StaticPermutation
    {
        public StaticPermutation()
        {
        }

        public void Read(DataReader reader) //Unknown data, needs more testing
        {
        }
    }

    public class FCurveFloat
    {
        public ForgeObject ObjectPtrFCurveFloat { get; set; }
        public int NumFCurves { get; set; }
        public FCurve[] FCurves { get; set; }
        public float PlaybackMode { get; set; }

        public FCurveFloat()
        {
        }

        public void Read(DataReader reader)
        {
            ObjectPtrFCurveFloat = new ForgeObject();
            ObjectPtrFCurveFloat.Read(reader);

            NumFCurves = reader.ReadInt32();
            FCurves = new FCurve[NumFCurves];

            for (int i = 0; i < FCurves.Length; i++)
            {
                FCurves[i] = new FCurve();
                FCurves[i].Read(reader);
            }

            PlaybackMode = reader.ReadSingle();
        }
    }

    public class FCurve
    {
        public ForgeObject ObjectPtrFCurve { get; set; }
        public int NumKeys { get; set; }
        public FCurveKey[] Keys { get; set; }
        public float DefaultValue { get; set; }

        public FCurve()
        {
        }

        public void Read(DataReader reader)
        {
            ObjectPtrFCurve = new ForgeObject();
            ObjectPtrFCurve.Read(reader);

            NumKeys = reader.ReadInt32();
            Keys = new FCurveKey[NumKeys];

            for (int i = 0; i < Keys.Length; i++)
            {
                Keys[i] = new FCurveKey();
                Keys[i].Read(reader);
            }

            DefaultValue = reader.ReadSingle();
        }
    }

    public class FCurveKey
    {
        public ForgeObject ObjectPtrFCurveKey { get; set; }
        public Vector2 Point { get; set; }
        public Vector2 PreviousTangentPoint { get; set; }
        public Vector2 NextTangentPoint { get; set; }
        public float Range { get; set; }
        public uint InterpolatorType { get; set; }

        public FCurveKey()
        {
        }

        public void Read(DataReader reader)
        {
            ObjectPtrFCurveKey = new ForgeObject();
            ObjectPtrFCurveKey.Read(reader);

            Point = reader.ReadVector2();
            PreviousTangentPoint = reader.ReadVector2();
            NextTangentPoint = reader.ReadVector2();
            Range = reader.ReadSingle();
            InterpolatorType = reader.ReadUInt32();
        }
    }

    public class AmbientEventManager
    {
        public ForgeBaseObject BaseObjectAmbientEventManager { get; set; }
        public ForgeFileReference AmbientSpawnDirector { get; set; }

        public AmbientEventManager()
        {
        }

        public void Read(DataReader reader)
        {
            BaseObjectAmbientEventManager = new ForgeBaseObject();
            BaseObjectAmbientEventManager.Read(reader);

            AmbientSpawnDirector = new ForgeFileReference();
            AmbientSpawnDirector.Read(reader);
        }
    }

    public class ZoneSpawnerManager
    {
        public ForgeBaseObject BaseObjectZoneSpawnerManager { get; set; }
        public ZoneSpawnerLoadingAdvisor LoadingAdvisor { get; set; }

        public ZoneSpawnerManager()
        {
        }

        public void Read(DataReader reader)
        {
            BaseObjectZoneSpawnerManager = new ForgeBaseObject();
            BaseObjectZoneSpawnerManager.Read(reader);

            LoadingAdvisor = new ZoneSpawnerLoadingAdvisor();
            LoadingAdvisor.Read(reader);
        }
    }

    public class ZoneSpawnerLoadingAdvisor
    {
        public ForgeBaseObject BaseObjectZoneSpawnerLoadingAdvisor { get; set; }
        public ZoneSpawnerLoadingSettings Settings { get; set; }
        public int NumEntitySummaries { get; set; }
        public ZoneSpawnerEntitySummary[] ZoneSpawnerEntitySummaries { get; set; }

        public ZoneSpawnerLoadingAdvisor()
        {
        }

        public void Read(DataReader reader)
        {
            BaseObjectZoneSpawnerLoadingAdvisor = new ForgeBaseObject();
            BaseObjectZoneSpawnerLoadingAdvisor.Read(reader);

            Settings = new ZoneSpawnerLoadingSettings();
            Settings.Read(reader);

            NumEntitySummaries = reader.ReadInt32();
            ZoneSpawnerEntitySummaries = new ZoneSpawnerEntitySummary[NumEntitySummaries];

            for (int i = 0; i < ZoneSpawnerEntitySummaries.Length; i++)
            {
                ZoneSpawnerEntitySummaries[i] = new ZoneSpawnerEntitySummary();
                ZoneSpawnerEntitySummaries[i].Read(reader);
            }
        }
    }

    public class ZoneSpawnerLoadingSettings
    {
        public ForgeBaseObject BaseObjectZoneSpawnerLoadingAdvisor { get; set; }
        public float UnloadBufferDistance { get; set; }

        public ZoneSpawnerLoadingSettings()
        {
        }

        public void Read(DataReader reader)
        {
            BaseObjectZoneSpawnerLoadingAdvisor = new ForgeBaseObject();
            BaseObjectZoneSpawnerLoadingAdvisor.Read(reader);

            UnloadBufferDistance = reader.ReadSingle();
        }
    }

    public class ZoneSpawnerEntitySummary
    {
        public ForgeObjectPtr ObjectPtrZoneSpawnerEntitySummary { get; set; }

        public ZoneSpawnerEntitySummary()
        {
        }

        public void Read(DataReader reader) //Unknown data
        {
            ObjectPtrZoneSpawnerEntitySummary = new ForgeObjectPtr();
            ObjectPtrZoneSpawnerEntitySummary.Read(reader);
        }
    }

    public class PermanentIconData
    {
        public PermanentIconData()
        {
        }

        public void Read(DataReader reader)
        {
            throw new Exception("Unknown data: PermanentIconData");
        }
    }

    public class UILocationBase
    {
        public UILocationBase()
        {
        }

        public void Read(DataReader reader)
        {
            throw new Exception("Unknown data: UILocationBase");
        }
    }

    public class SpaceSection
    {
        public object PtrSpaceSection { get; set; }
        public int NumPortals { get; set; }
        public SpacePortal[] Portals { get; set; }
        public int NumLinkedSections { get; set; }
        public SpaceSection[] LinkedSections { get; set; }
        public short ExteriorLinkedPortalsIndex { get; set; }
        public int NumObjectShapes { get; set; }
        public ObjectShape[] ObjectShapes { get; set; }
        public int Priority { get; set; }
        public bool IsInterior { get; set; }
        public int InteriorReactionType { get; set; }
        public bool Unknown1 { get; set; }
        public bool NoWater { get; set; }
        public bool IsWallaInterior { get; set; }
        public bool LinksOnlyToExterior { get; set; }
        public ForgeFileReference InteriorAbilitySet { get; set; }
        public ushort FindShortestPathsearchPointsLimit { get; set; }
        public bool EnableSoundOcclusion { get; set; }
        public bool IsClosedInterior { get; set; }

        public SpaceSection()
        {
        }

        public void Read(DataReader reader, bool handle)
        {
            if (handle)
            {
                PtrSpaceSection = new ForgeObjectPtr();
                ((ForgeObjectPtr)PtrSpaceSection).Read(reader);
                return;
            }
            else
            {
                PtrSpaceSection = new ForgeBaseObjectPtr();
                ((ForgeBaseObjectPtr)PtrSpaceSection).Read(reader);
            }

            NumPortals = reader.ReadInt32();
            Portals = new SpacePortal[NumPortals];

            for (int i = 0; i < Portals.Length; i++)
            {
                Portals[i] = new SpacePortal();
                Portals[i].Read(reader);
            }

            NumLinkedSections = reader.ReadInt32();
            LinkedSections = new SpaceSection[NumLinkedSections];

            for (int i = 0; i < LinkedSections.Length; i++)
            {
                LinkedSections[i] = new SpaceSection();
                LinkedSections[i].Read(reader, true);
            }

            ExteriorLinkedPortalsIndex = reader.ReadInt16();

            NumObjectShapes = reader.ReadInt32();
            ObjectShapes = new ObjectShape[NumObjectShapes];

            for (int i = 0; i < ObjectShapes.Length; i++)
            {
                ObjectShapes[i] = new ObjectShape();
                ObjectShapes[i].Read(reader);
            }

            Priority = reader.ReadInt32();
            IsInterior = reader.ReadBoolean();
            InteriorReactionType = reader.ReadInt32();
            Unknown1 = reader.ReadBoolean();
            NoWater = reader.ReadBoolean();
            IsWallaInterior = reader.ReadBoolean();
            LinksOnlyToExterior = reader.ReadBoolean();
            InteriorAbilitySet = new ForgeFileReference();
            InteriorAbilitySet.Read(reader);
            FindShortestPathsearchPointsLimit = reader.ReadUInt16();
            EnableSoundOcclusion = reader.ReadBoolean();
            IsClosedInterior = reader.ReadBoolean();
        }
    }

    public class SpacePortal
    {
        public SpacePortal()
        {
        }

        public void Read(DataReader reader)
        {
            throw new Exception("Unknown data: SpacePortal");
        }
    }

    public class ObjectShape
    {
        public ObjectShape()
        {
        }

        public void Read(DataReader reader)
        {
            throw new Exception("Unknown data: ObjectShape");
        }
    }

    public class SoundOcclusionPortalExteriorLinksTable
    {
        public ForgeBaseObjectPtr BaseObjectPtrSoundOcclusionPortal { get; set; }
        public int NumPortalListTable { get; set; }
        public SpacePortal[] PortalListTable { get; set; }
        public int NumLowerListTable { get; private set; }
        public ushort[] PortalListLowerBoundIndexTable { get; private set; }

        public SoundOcclusionPortalExteriorLinksTable()
        {
        }

        public void Read(DataReader reader)
        {
            BaseObjectPtrSoundOcclusionPortal = new ForgeBaseObjectPtr();
            BaseObjectPtrSoundOcclusionPortal.Read(reader);

            NumPortalListTable = reader.ReadInt32();
            PortalListTable = new SpacePortal[NumPortalListTable];

            for (int i = 0; i < PortalListTable.Length; i++)
            {
                PortalListTable[i] = new SpacePortal();
                PortalListTable[i].Read(reader);
            }

            NumLowerListTable = reader.ReadInt32();
            PortalListLowerBoundIndexTable = new ushort[NumLowerListTable];

            for (int i = 0; i < PortalListLowerBoundIndexTable.Length; i++)
            {
                PortalListLowerBoundIndexTable[i] = reader.ReadUInt16();
            }
        }
    }

    public class TeleportBookmark
    {
        public TeleportBookmark()
        {
        }

        public void Read(DataReader reader)
        {
            throw new Exception("Unknown data: TeleportBookmark");
        }
    }

    public class MetaAIObjective
    {
        public MetaAIObjective()
        {
        }

        public void Read(DataReader reader)
        {
            throw new Exception("Unknown data: MetaAIObjective");
        }
    }

    public class MetaAISystemicEventTrackingManager
    {
        public ForgeBaseObject BaseObjectMetaAISystemicEventTrackingManager { get; set; }
        public float Unknown_0h { get; set; }
        public float Unknown_4h { get; set; }
        public float Unknown_8h { get; set; }
        public ForgeFileReference Unknown_Ch { get; set; }
        public ForgeFileReference Unknown_10h { get; set; }
        public ForgeFileReference Unknown_14h { get; set; }
        public ForgeFileReference Unknown_18h { get; set; }
        public ForgeFileReference Unknown_1Ch { get; set; }
        public ForgeFileReference MilitaryLabel { get; set; }
        public ForgeFileReference CivilianLabel { get; set; }
        public ForgeFileReference Unknown_52h { get; set; }

        public MetaAISystemicEventTrackingManager()
        {
        }

        public void Read(DataReader reader)
        {
            BaseObjectMetaAISystemicEventTrackingManager = new ForgeBaseObject();
            BaseObjectMetaAISystemicEventTrackingManager.Read(reader);

            Unknown_0h = reader.ReadSingle();
            Unknown_4h = reader.ReadSingle();
            Unknown_8h = reader.ReadSingle();

            Unknown_Ch = new ForgeFileReference();
            Unknown_Ch.Read(reader);

            Unknown_10h = new ForgeFileReference();
            Unknown_10h.Read(reader);

            Unknown_14h = new ForgeFileReference();
            Unknown_14h.Read(reader);

            Unknown_18h = new ForgeFileReference();
            Unknown_18h.Read(reader);

            Unknown_1Ch = new ForgeFileReference();
            Unknown_1Ch.Read(reader);

            MilitaryLabel = new ForgeFileReference();
            MilitaryLabel.Read(reader);

            CivilianLabel = new ForgeFileReference();
            CivilianLabel.Read(reader);

            Unknown_52h = new ForgeFileReference();
            Unknown_52h.Read(reader);
        }
    }

    public class InteriorSettings
    {
        public InteriorSettings()
        {
        }

        public void Read(DataReader reader)
        {
            throw new Exception("Unknown data: InteriorSettings");
        }
    }

    public class SpawnNPCInfo
    {
        public ForgeObject ObjectSpawnNPCInfo { get; set; }
        public int DisplayNameLength { get; set; }
        public string DisplayName { get; set; } = string.Empty;
        public byte NullTerminator { get; set; }
        public ForgeFileReference Objective { get; set; }

        public SpawnNPCInfo()
        {
        }

        public void Read(DataReader reader)
        {
            ObjectSpawnNPCInfo = new ForgeObject();
            ObjectSpawnNPCInfo.Read(reader);

            DisplayNameLength = reader.ReadInt32();
            DisplayName = new(reader.ReadChars(DisplayNameLength));

            if (DisplayNameLength > 0)
            {
                NullTerminator = reader.ReadByte();
            }

            Objective = new ForgeFileReference(); //Reference<MetaAIObjectiveDynamic>
            Objective.Read(reader);
        }
    }

    public class SpawnObjectInfo
    {
        public ForgeObject ObjectSpawnObjectInfo { get; set; }
        public int DisplayNameLength { get; set; }
        public string DisplayName { get; set; } = string.Empty;
        public byte NullTerminator { get; set; }
        public SpawnObjectParams SpawnParams { get; set; }

        public SpawnObjectInfo()
        {
        }

        public void Read(DataReader reader)
        {
            ObjectSpawnObjectInfo = new ForgeObject();
            ObjectSpawnObjectInfo.Read(reader);

            DisplayNameLength = reader.ReadInt32();
            DisplayName = new(reader.ReadChars(DisplayNameLength));

            if (DisplayNameLength > 0)
            {
                NullTerminator = reader.ReadByte();
            }

            SpawnParams = new SpawnObjectParams();
            SpawnParams.Read(reader);
        }
    }

    public class SpawnChariotInfo
    {
        public SpawnChariotInfo()
        {
        }

        public void Read(DataReader reader)
        {
            throw new Exception("Unknown data: SpawnChariotInfo");
        }
    }

    public class SpawnWaterVehicleInfo
    {
        public ForgeObject ObjectSpawnWaterVehicleInfo { get; set; }
        public int DisplayNameLength { get; set; }
        public string DisplayName { get; set; } = string.Empty;
        public byte NullTerminator { get; set; }
        public ForgeFileReference Objective { get; set; }

        public SpawnWaterVehicleInfo()
        {
        }

        public void Read(DataReader reader)
        {
            ObjectSpawnWaterVehicleInfo = new ForgeObject();
            ObjectSpawnWaterVehicleInfo.Read(reader);

            DisplayNameLength = reader.ReadInt32();
            DisplayName = new(reader.ReadChars(DisplayNameLength));

            if (DisplayNameLength > 0)
            {
                NullTerminator = reader.ReadByte();
            }

            Objective = new ForgeFileReference(); //Reference<MetaAIObjectiveWaterVehicle>
            Objective.Read(reader);
        }
    }

    public class SpawnNPCInfoGroup
    {
        public SpawnNPCInfoGroup()
        {
        }

        public void Read(DataReader reader)
        {
            throw new Exception("Unknown data: SpawnNPCInfoGroup");
        }
    }

    public class SpawnObjectParams
    {
        public ForgeBaseObject BaseObjectSpawnObjectParams { get; set; }
        public BuildTags Tags { get; set; }
        public int NumWeaponTags { get; set; }
        public BuildTags[] WeaponTags { get; set; }
        public int SeedToUseForGeneration { get; set; }
        public DynamicReference TagBuilderDynamicRef { get; set; }
        public bool Exclusive { get; set; }
        public int ExclusiveNPCType { get; set; }
        public int ExplicitNPC { get; set; }
        public ForgeBaseObjectPtr SelectionFilter { get; set; }
        public ForgeFileReference EntityPack { get; set; }
        public ForgeObjectPtr EntityPackHandle { get; set; }
        public ForgeObjectPtr TemplateEntity { get; set; }

        public SpawnObjectParams()
        {
        }

        public void Read(DataReader reader)
        {
            BaseObjectSpawnObjectParams = new ForgeBaseObject();
            BaseObjectSpawnObjectParams.Read(reader);

            Tags = new BuildTags();
            Tags.Read(reader);

            NumWeaponTags = reader.ReadInt32();
            WeaponTags = new BuildTags[NumWeaponTags];

            for (int i = 0; i < WeaponTags.Length; i++)
            {
                WeaponTags[i] = new BuildTags();
                WeaponTags[i].Read(reader);
            }

            SeedToUseForGeneration = reader.ReadInt32();

            TagBuilderDynamicRef = new DynamicReference();
            TagBuilderDynamicRef.Read(reader);

            Exclusive = reader.ReadBoolean();
            ExclusiveNPCType = reader.ReadInt32();
            ExplicitNPC = reader.ReadInt32();

            SelectionFilter = new ForgeBaseObjectPtr(); //BaseObjectPtr<ObjectPack>
            SelectionFilter.Read(reader, true);

            EntityPack = new ForgeFileReference(); //Reference<ObjectPack>
            EntityPack.Read(reader);

            EntityPackHandle = new ForgeObjectPtr(); //Handle<ObjectPack>
            EntityPackHandle.Read(reader);

            TemplateEntity = new ForgeObjectPtr(); //Handle<Entity>
            TemplateEntity.Read(reader);
        }
    }

    public class DebugRideableAnimalInfo
    {
        public ForgeObject ObjectSpawnObjectInfo { get; set; }
        public int DisplayNameLength { get; set; }
        public string DisplayName { get; set; } = string.Empty;
        public int NullTerminator { get; set; }
        public ForgeFileReference Objective { get; set; }

        public DebugRideableAnimalInfo()
        {
        }

        public void Read(DataReader reader)
        {
            ObjectSpawnObjectInfo = new ForgeObject();
            ObjectSpawnObjectInfo.Read(reader);

            DisplayNameLength = reader.ReadInt32();
            DisplayName = new(reader.ReadChars(DisplayNameLength));

            if (DisplayNameLength > 0)
            {
                NullTerminator = reader.ReadByte();
            }

            Objective = new ForgeFileReference(); //Reference<MetaAIObjectiveRideableAnimal>
            Objective.Read(reader);
        }
    }

    public class FactionSelector
    {
        public ForgeBaseObject BaseObjectFactionSelector { get; set; }
        public ForgeObjectPtr Faction { get; set; }

        public FactionSelector()
        {
        }

        public void Read(DataReader reader)
        {
            BaseObjectFactionSelector = new ForgeBaseObject();
            BaseObjectFactionSelector.Read(reader);

            Faction = new ForgeObjectPtr();
            Faction.Read(reader);
        }
    }

    public class SoundInstance
    {
        public ForgeBaseObject BaseObjectSoundInstance { get; set; }

        public SoundInstance()
        {
        }

        public void Read(DataReader reader)
        {
            BaseObjectSoundInstance = new ForgeBaseObject();
            BaseObjectSoundInstance.Read(reader);

            throw new Exception("Unknown data SoundInstance");
        }
    }

    public class SoundState
    {
        public ForgeObjectPtr ObjectPtrSoundInstance { get; set; }

        public SoundState()
        {
        }

        public void Read(DataReader reader)
        {
            ObjectPtrSoundInstance = new ForgeObjectPtr();
            ObjectPtrSoundInstance.Read(reader);

            throw new Exception("Unknown data SoundState");
        }
    }

    public class DesynchronizationSettings
    {
        public ForgeBaseObject ObjectDesynchronizationSettings { get; set; }
        public UIString OutOfBoundsFailure { get; private set; }
        public UIString PlayerDeathFailure { get; private set; }
        public UIString Unknown1 { get; private set; }
        public int NumLockMovementList { get; private set; }
        public uint[] LockMovementList { get; private set; }

        public DesynchronizationSettings()
        {
        }

        public void Read(DataReader reader)
        {
            ObjectDesynchronizationSettings = new ForgeBaseObject();
            ObjectDesynchronizationSettings.Read(reader);

            OutOfBoundsFailure = new UIString();
            OutOfBoundsFailure.Read(reader);

            PlayerDeathFailure = new UIString();
            PlayerDeathFailure.Read(reader);

            Unknown1 = new UIString();
            Unknown1.Read(reader);

            NumLockMovementList = reader.ReadInt32();
            LockMovementList = new uint[NumLockMovementList];

            for (int i = 0; i < LockMovementList.Length; i++)
            {
                LockMovementList[i] = reader.ReadUInt32();
            }
        }
    }

    public class UIString
    {
        public ForgeBaseObject BaseObjectUIString { get; set; }
        public int OasisLineID { get; private set; }
        public int TempStringLength { get; private set; }
        public string TempString { get; private set; } = string.Empty;

        public UIString()
        {
        }

        public void Read(DataReader reader)
        {
            BaseObjectUIString = new ForgeBaseObject();
            BaseObjectUIString.Read(reader);

            OasisLineID = reader.ReadInt32();
            TempStringLength = reader.ReadInt32();
            TempString = new string(reader.ReadChars(TempStringLength * 2 + 2)).Replace("\0", string.Empty);
        }
    }

    public class SpecialAbilityDataAbstract
    {
        public ForgeObjectPtr ObjectPtrSpecialAbilityDataAbstract { get; set; }

        public SpecialAbilityDataAbstract()
        {
        }

        public void Read(DataReader reader)
        {
            ObjectPtrSpecialAbilityDataAbstract = new ForgeObjectPtr();
            ObjectPtrSpecialAbilityDataAbstract.Read(reader, true);
        }
    }

    public class TagRules
    {
        public ForgeFileReference Rule { get; set; }

        public TagRules()
        {
        }

        public void Read(DataReader reader)
        {
            Rule = new ForgeFileReference();
            Rule.Read(reader);
        }
    }

    public class WorldMapParameters
    {
        public ForgeBaseObject BaseObjectWorldMapParameters { get; set; }
        public float XMin { get; private set; }
        public float XMax { get; private set; }
        public float YMin { get; private set; }
        public float YMax { get; private set; }

        public WorldMapParameters()
        {
        }

        public void Read(DataReader reader)
        {
            BaseObjectWorldMapParameters = new ForgeBaseObject();
            BaseObjectWorldMapParameters.Read(reader);

            XMin = reader.ReadSingle();
            XMax = reader.ReadSingle();
            YMin = reader.ReadSingle();
            YMax = reader.ReadSingle();
        }
    }

    public class BoundingVolume
    {
        public ForgeBaseObject BaseObjectBoundingVolume { get; set; }
        public Vector3 InstanceOriginalBVMin { get; set; }
        public Vector3 InstanceOriginalBVMax { get; set; }
        public BoundingVolumeType? BoundingVolumeType { get; set; }

        public BoundingVolume()
        {
        }

        public void Read(DataReader reader)
        {
            BaseObjectBoundingVolume = new ForgeBaseObject();
            BaseObjectBoundingVolume.Read(reader);

            InstanceOriginalBVMin = reader.ReadVector3();
            InstanceOriginalBVMax = reader.ReadVector3();
            BoundingVolumeType = (BoundingVolumeType)reader.ReadInt32();
        }
    }

    public enum BoundingVolumeType
    {
        AABB,
        SPHERE,
        OOBB,
    }
}