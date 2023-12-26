using CodeX.Core.Utilities;
using CodeX.Games.ACOdyssey.FORGE;

namespace CodeX.Games.ACOdyssey.Resources
{
    public class ForgeTextureSet
    {
        public ForgeBaseObjectPtr? BaseObjectPtrTextureSet { get; set; }
        public ForgeFileReference? DiffuseMap { get; set; } //1
        public ForgeFileReference? NormalMap { get; set; } //2
        public ForgeFileReference? SpecularMap { get; set; } //3
        public ForgeFileReference? SpecularPowerMap { get; set; } //4
        public ForgeFileReference? OffetBumpMap { get; set; } //5
        public ForgeFileReference? EmissiveMap { get; set; } //6
        public ForgeFileReference? TransmissionMap { get; set; } //7
        public ForgeFileReference? Mask1Map { get; set; } //8
        public ForgeFileReference? Mask2Map { get; set; } //9
        public ForgeFileReference? OcclusionMap { get; set; } //10
        public ForgeFileReference? RoughnessMap { get; set; } //11
        public ForgeFileReference? GenericMap { get; set; } //12
        public ForgeFileReference? UnknownMap1 { get; set; } //13
        public ForgeFileReference? UnknownMap2 { get; set; } //14
        public int UserCategory { get; set; }

        public ForgeTextureSet()
        {
        }

        public void Read(DataReader reader, bool skipHeader = false)
        {
            if (!skipHeader)
            {
                var header = new ForgeDataHeader()
                {
                    ResourceIdentifier = (ForgeResourceType)reader.ReadUInt32(),
                    FileSize = reader.ReadInt32(),
                    FileNameSize = reader.ReadInt32()
                };
                header.FileName = new(reader.ReadChars(header.FileNameSize));
            }

            BaseObjectPtrTextureSet = new ForgeBaseObjectPtr();
            BaseObjectPtrTextureSet.Read(reader);

            DiffuseMap = new ForgeFileReference();
            DiffuseMap.Read(reader);

            NormalMap = new ForgeFileReference();
            NormalMap.Read(reader);

            SpecularMap = new ForgeFileReference();
            SpecularMap.Read(reader);

            SpecularPowerMap = new ForgeFileReference();
            SpecularPowerMap.Read(reader);

            OffetBumpMap = new ForgeFileReference();
            OffetBumpMap.Read(reader);

            EmissiveMap = new ForgeFileReference();
            EmissiveMap.Read(reader);

            TransmissionMap = new ForgeFileReference();
            TransmissionMap.Read(reader);

            Mask1Map = new ForgeFileReference();
            Mask1Map.Read(reader);

            Mask2Map = new ForgeFileReference();
            Mask2Map.Read(reader);

            OcclusionMap = new ForgeFileReference();
            OcclusionMap.Read(reader);

            RoughnessMap = new ForgeFileReference();
            RoughnessMap.Read(reader);

            GenericMap = new ForgeFileReference();
            GenericMap.Read(reader);

            UnknownMap1 = new ForgeFileReference();
            UnknownMap1.Read(reader);

            UnknownMap2 = new ForgeFileReference();
            UnknownMap2.Read(reader);

            UserCategory = reader.ReadInt32();
        }
    }
}