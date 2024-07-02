using CodeX.Core.Utilities;
using CodeX.Games.ACOdyssey.Files;
using CodeX.Games.ACOdyssey.FORGE;

namespace CodeX.Games.ACOdyssey.Resources
{
    public class ForgeLODSelector
    {
        public ForgeDataHeader Header { get; set; }
        public ForgeBaseObjectPtr BaseObjectPtrLODSelector { get; set; }
        public byte DescriptorMask { get; set; }
        public bool Generated { get; set; }
        public bool HasHighOverdraw { get; set; }
        public float MaxCullingDistance { get; set; }
        public uint EstimatedMemoryUsage { get; set; }
        public LODDescriptor[] LODDescs { get; set; }
        public bool TransitionToEmptyLOD { get; set; }
        public bool UseNearNullObjects { get; set; }
        public bool ForceNoBlend { get; set; }
        public bool ForceNoComputeBlendFactor { get; set; }
        public bool ForceBlendOnEmptyLODTransition { get; set; }
        public bool UseBlendFactorWithDefaultTransition { get; set; }
        public bool IsStreamed { get; set; }
        public bool IsStreamAutoFake { get; set; }
        public bool IsStreamWeapon { get; set; }
        public bool ExcludeFromGridLODDistance { get; set; }
        public byte RenderOrderPriorityHint { get; set; }

        public ForgeLODSelector()
        {
        }

        public void Read(DataReader reader, bool skipHeader = false)
        {
            if (!skipHeader)
            {
                Header = new ForgeDataHeader()
                {
                    ResourceIdentifier = (ForgeResourceType)reader.ReadUInt32(),
                    FileSize = reader.ReadInt32(),
                    FileNameSize = reader.ReadInt32()
                };
                Header.FileName = new(reader.ReadChars(Header.FileNameSize));
            }

            BaseObjectPtrLODSelector = new ForgeBaseObjectPtr();
            BaseObjectPtrLODSelector.Read(reader);

            DescriptorMask = reader.ReadByte();
            Generated = reader.ReadBoolean();
            HasHighOverdraw = reader.ReadBoolean();
            MaxCullingDistance = reader.ReadSingle();
            EstimatedMemoryUsage = reader.ReadUInt32();

            LODDescs = new LODDescriptor[5];
            for (int i = 0; i < LODDescs.Length; i++)
            {
                LODDescs[i] = new LODDescriptor();
                LODDescs[i].Read(reader);
            }

            TransitionToEmptyLOD = reader.ReadBoolean();
            UseNearNullObjects = reader.ReadBoolean();
            ForceNoBlend = reader.ReadBoolean();
            ForceNoComputeBlendFactor = reader.ReadBoolean();
            ForceBlendOnEmptyLODTransition = reader.ReadBoolean();
            UseBlendFactorWithDefaultTransition = reader.ReadBoolean();
            IsStreamed = reader.ReadBoolean();
            IsStreamAutoFake = reader.ReadBoolean();
            IsStreamWeapon = reader.ReadBoolean();
            ExcludeFromGridLODDistance = reader.ReadBoolean();
            RenderOrderPriorityHint = reader.ReadByte();
        }
    }

    public class LODDescriptor
    {
        public ForgeBaseObject BaseObjectLODDescriptor { get; set; }
        public ForgeFileReference Object { get; set; }
        public float SwitchDistance { get; set; }
        public float TransitionZoneSide { get; set; }
        public float FadeTimeMultiplier { get; set; }
        public int FakeType { get; set; }
        public bool Optional { get; set; }
        public bool ForceIncludeInFake { get; set; }
        public int ShaderLOD { get; set; }
        public ForgeObjectPtr StreamObjectHandle { get; set; }
        public int NumMaterials { get; set; }
        public ForgeFileReference[] StreamObjectMaterials { get; set; }
        public int NumBones { get; set; }
        public uint[] BoneIDs { get; set; }

        public LODDescriptor()
        {
        }

        public void Read(DataReader reader)
        {
            BaseObjectLODDescriptor = new ForgeBaseObject();
            BaseObjectLODDescriptor.Read(reader);

            Object = new ForgeFileReference(); //Reference to a mesh file
            Object.Read(reader);

            SwitchDistance = reader.ReadSingle();
            TransitionZoneSide = reader.ReadSingle();
            FadeTimeMultiplier = reader.ReadSingle();
            FakeType = reader.ReadInt32();
            Optional = reader.ReadBoolean();
            ForceIncludeInFake = reader.ReadBoolean();
            ShaderLOD = reader.ReadInt32();

            StreamObjectHandle = new ForgeObjectPtr(); //Reference to a mesh file
            StreamObjectHandle.Read(reader);

            NumMaterials = reader.ReadInt32();
            StreamObjectMaterials = new ForgeFileReference[NumMaterials]; //References to mesh materials

            for (int i = 0; i < StreamObjectMaterials.Length; i++)
            {
                StreamObjectMaterials[i] = new ForgeFileReference();
                StreamObjectMaterials[i].Read(reader);
            }

            NumBones = reader.ReadInt32();
            BoneIDs = new uint[NumBones];

            for (int i = 0; i < BoneIDs.Length; i++)
            {
                BoneIDs[i] = reader.ReadUInt32();
            }
        }
    }
}