using CodeX.Core.Engine;
using CodeX.Core.Shaders;
using CodeX.Core.Utilities;
using CodeX.Games.ACOdyssey.FORGE;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using EXP = System.ComponentModel.ExpandableObjectConverter;
using TC = System.ComponentModel.TypeConverterAttribute;

namespace CodeX.Games.ACOdyssey.Resources
{
    [TC(typeof(EXP))]
    public class ForgeModel : Piece
    {
        public ForgeDataHeader Header { get; set; }
        public ForgeBaseObjectPtr BaseObjectPtrModel { get; set; }
        public byte DescriptorMask { get; set; }
        public bool Generated { get; set; }
        public bool HasHighOverdraw { get; set; }
        public float MaxCullingDistance { get; set; }
        public int EstimatedMemoryUsage { get; set; }
        public int NumSubMeshes { get; set; }
        public SubMesh[] SubMeshes { get; set; }
        public int NumBones { get; set; }
        public MeshBone[] Bones { get; set; }
        public int NumBlendShapes { get; set; }
        public SubMeshesBlendShapeID[] BlendShapesIDs { get; set; }
        public SkinWrapProxyMesh SkinWrapProxyMesh { get; set; }
        public SkinWrapLayerMesh SkinWrapLayerMesh { get; set; }
        public Vector4 LocalExtentMin { get; set; }
        public Vector4 LocalExtentMax { get; set; }
        public ForgeCompiledMesh CompiledMesh { get; set; }
        public Mesh[] Meshes { get; set; }
        public ForgeFileReference TracePreciseMeshShape { get; set; }
        public int NumMaterials { get; set; }
        public ForgeFileReference[] Materials { get; set; }
        public bool Dynamic { get; set; }
        public bool PrecomputedSkinning { get; set; }
        public bool RecomputeTBN { get; set; }
        public bool VertexDisplacementEnabled { get; set; }
        public bool KeepSubMeshesFastLoad { get; set; }
        public bool KeepNonClusteredMeshData { get; set; }
        public bool UseEntityBoundsForCulling { get; set; }
        public bool IgnoreAlphaTestForShadowPass { get; set; }
        public bool IgnorePositionModificationDepthOnlyPass { get; set; }
        public bool ZPrePassDisabled { get; set; }
        public bool InoreAlphaTestForAllPass { get; set; }
        public bool IgnoreTwoSided { get; set; }
        public bool DisableSolidSkinning { get; set; }
        public bool UseSkinWrap { get; set; }
        public bool UseBlendShape { get; set; }
        public bool MorphEnabled { get; set; }
        public ForgeFileReference MorphDisplacementTexture { get; set; }
        public int NumMappings { get; set; }
        public MorphMapping[] MorphMappings { get; set; }
        public float MorphMinRange { get; set; }
        public float MorphMaxRange { get; set; }
        public float MorphMultiplier { get; set; }
        public int MorphVerticalTextureOffset { get; set; }
        public byte Layer { get; set; }
        public bool LayerGroup1 { get; set; }
        public bool LayerGroup2 { get; set; }
        public bool LayerGroup3 { get; set; }
        public bool LayerGroup4 { get; set; }
        public bool LayerGroup5 { get; set; }
        public bool LayerGroup6 { get; set; }
        public bool LayerGroup7 { get; set; }
        public bool LayerGroup8 { get; set; }
        public bool WrinklesEnabled { get; set; }
        public float OpaqueSurfaceArea { get; set; }
        public int DynamicMeshVertexCount { get; set; }
        public int DynamicMeshIndexCount { get; set; }
        public byte DynamicMeshUVChannelCount { get; set; }
        public int UserCategory { get; set; }

        public ForgeModel()
        {
        }

        public ForgeModel(ForgeModel model)
        {
            if (model != null)
            {
                AllModels = model.AllModels;
                Name = model.Name;
                Header = model.Header != null ? new ForgeDataHeader(model.Header) : null;
                BaseObjectPtrModel = model.BaseObjectPtrModel != null ? new ForgeBaseObjectPtr(model.BaseObjectPtrModel) : null;
                DescriptorMask = model.DescriptorMask;
                Generated = model.Generated;
                HasHighOverdraw = model.HasHighOverdraw;
                MaxCullingDistance = model.MaxCullingDistance;
                EstimatedMemoryUsage = model.EstimatedMemoryUsage;
                NumSubMeshes = model.NumSubMeshes;
                SubMeshes = model.SubMeshes?.Select(subMesh => new SubMesh(subMesh)).ToArray();
                NumBones = model.NumBones;
                Bones = model.Bones?.Select(bone => new MeshBone(bone)).ToArray();
                NumBlendShapes = model.NumBlendShapes;
                BlendShapesIDs = model.BlendShapesIDs?.Select(id => new SubMeshesBlendShapeID(id)).ToArray();
                SkinWrapProxyMesh = model.SkinWrapProxyMesh != null ? new SkinWrapProxyMesh(model.SkinWrapProxyMesh) : null;
                SkinWrapLayerMesh = model.SkinWrapLayerMesh != null ? new SkinWrapLayerMesh(model.SkinWrapLayerMesh) : null;
                LocalExtentMin = model.LocalExtentMin;
                LocalExtentMax = model.LocalExtentMax;
                CompiledMesh = model.CompiledMesh != null ? new ForgeCompiledMesh(model.CompiledMesh) : null;
                Meshes = model.Meshes?.Select(mesh => mesh.Clone()).ToArray();
                TracePreciseMeshShape = model.TracePreciseMeshShape != null ? new ForgeFileReference(model.TracePreciseMeshShape) : null;
                NumMaterials = model.NumMaterials;
                Materials = model.Materials?.Select(material => new ForgeFileReference(material)).ToArray();
                Dynamic = model.Dynamic;
                PrecomputedSkinning = model.PrecomputedSkinning;
                RecomputeTBN = model.RecomputeTBN;
                VertexDisplacementEnabled = model.VertexDisplacementEnabled;
                KeepSubMeshesFastLoad = model.KeepSubMeshesFastLoad;
                KeepNonClusteredMeshData = model.KeepNonClusteredMeshData;
                UseEntityBoundsForCulling = model.UseEntityBoundsForCulling;
                IgnoreAlphaTestForShadowPass = model.IgnoreAlphaTestForShadowPass;
                IgnorePositionModificationDepthOnlyPass = model.IgnorePositionModificationDepthOnlyPass;
                ZPrePassDisabled = model.ZPrePassDisabled;
                InoreAlphaTestForAllPass = model.InoreAlphaTestForAllPass;
                IgnoreTwoSided = model.IgnoreTwoSided;
                DisableSolidSkinning = model.DisableSolidSkinning;
                UseSkinWrap = model.UseSkinWrap;
                UseBlendShape = model.UseBlendShape;
                MorphEnabled = model.MorphEnabled;
                MorphDisplacementTexture = model.MorphDisplacementTexture != null ? new ForgeFileReference(model.MorphDisplacementTexture) : null;
                NumMappings = model.NumMappings;
                MorphMappings = model.MorphMappings?.Select(mapping => new MorphMapping(mapping)).ToArray();
                MorphMinRange = model.MorphMinRange;
                MorphMaxRange = model.MorphMaxRange;
                MorphMultiplier = model.MorphMultiplier;
                MorphVerticalTextureOffset = model.MorphVerticalTextureOffset;
                Layer = model.Layer;
                LayerGroup1 = model.LayerGroup1;
                LayerGroup2 = model.LayerGroup2;
                LayerGroup3 = model.LayerGroup3;
                LayerGroup4 = model.LayerGroup4;
                LayerGroup5 = model.LayerGroup5;
                LayerGroup6 = model.LayerGroup6;
                LayerGroup7 = model.LayerGroup7;
                LayerGroup8 = model.LayerGroup8;
                WrinklesEnabled = model.WrinklesEnabled;
                OpaqueSurfaceArea = model.OpaqueSurfaceArea;
                DynamicMeshVertexCount = model.DynamicMeshVertexCount;
                DynamicMeshIndexCount = model.DynamicMeshIndexCount;
                DynamicMeshUVChannelCount = model.DynamicMeshUVChannelCount;
                UserCategory = model.UserCategory;
            }
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

            BaseObjectPtrModel = new ForgeBaseObjectPtr();
            BaseObjectPtrModel.Read(reader);

            DescriptorMask = reader.ReadByte();
            Generated = reader.ReadBoolean();
            HasHighOverdraw = reader.ReadBoolean();
            MaxCullingDistance = reader.ReadSingle();
            EstimatedMemoryUsage = reader.ReadInt32();

            //Submeshes
            NumSubMeshes = reader.ReadInt32();
            SubMeshes = new SubMesh[NumSubMeshes];

            for (int i = 0; i < SubMeshes.Length; i++)
            {
                SubMeshes[i] = new SubMesh();
                SubMeshes[i].Read(reader);
            }

            //Bones
            NumBones = reader.ReadInt32();
            Bones = new MeshBone[NumBones];

            for (int i = 0; i < Bones.Length; i++)
            {
                Bones[i] = new MeshBone();
                Bones[i].Read(reader);
            }

            //Blendshapes
            NumBlendShapes = reader.ReadInt32();
            BlendShapesIDs = new SubMeshesBlendShapeID[NumBlendShapes];

            for (int i = 0; i < BlendShapesIDs.Length; i++)
            {
                BlendShapesIDs[i] = new SubMeshesBlendShapeID();
                BlendShapesIDs[i].Read(reader);
            }

            SkinWrapProxyMesh = new SkinWrapProxyMesh();
            SkinWrapProxyMesh.Read(reader);

            SkinWrapLayerMesh = new SkinWrapLayerMesh();
            SkinWrapLayerMesh.Read(reader);

            LocalExtentMin = reader.ReadVector4();
            LocalExtentMax = reader.ReadVector4();

            CompiledMesh = new ForgeCompiledMesh();
            CompiledMesh.Read(reader);

            CreateMeshes();

            TracePreciseMeshShape = new ForgeFileReference();
            TracePreciseMeshShape.Read(reader);

            //Materials
            NumMaterials = reader.ReadInt32();
            Materials = new ForgeFileReference[NumMaterials];

            for (int i = 0; i < Materials.Length; i++)
            {
                Materials[i] = new ForgeFileReference();
                Materials[i].Read(reader);
            }

            Dynamic = reader.ReadBoolean();
            PrecomputedSkinning = reader.ReadBoolean();
            RecomputeTBN = reader.ReadBoolean();
            VertexDisplacementEnabled = reader.ReadBoolean();
            KeepSubMeshesFastLoad = reader.ReadBoolean();
            KeepNonClusteredMeshData = reader.ReadBoolean();
            UseEntityBoundsForCulling = reader.ReadBoolean();
            IgnoreAlphaTestForShadowPass = reader.ReadBoolean();
            IgnorePositionModificationDepthOnlyPass = reader.ReadBoolean();
            ZPrePassDisabled = reader.ReadBoolean();
            InoreAlphaTestForAllPass = reader.ReadBoolean();
            IgnoreTwoSided = reader.ReadBoolean();
            DisableSolidSkinning = reader.ReadBoolean();
            UseSkinWrap = reader.ReadBoolean();
            UseBlendShape = reader.ReadBoolean();
            MorphEnabled = reader.ReadBoolean();

            MorphDisplacementTexture = new ForgeFileReference();
            MorphDisplacementTexture.Read(reader);

            NumMappings = reader.ReadInt32();
            MorphMappings = new MorphMapping[NumMappings];

            for (int i = 0; i < MorphMappings.Length; i++)
            {
                MorphMappings[i] = new MorphMapping();
                MorphMappings[i].Read(reader);
            }

            MorphMinRange = reader.ReadSingle();
            MorphMaxRange = reader.ReadSingle();
            MorphMultiplier = reader.ReadSingle();
            MorphVerticalTextureOffset = reader.ReadInt32();
            Layer = reader.ReadByte();
            LayerGroup1 = reader.ReadBoolean();
            LayerGroup2 = reader.ReadBoolean();
            LayerGroup3 = reader.ReadBoolean();
            LayerGroup4 = reader.ReadBoolean();
            LayerGroup5 = reader.ReadBoolean();
            LayerGroup6 = reader.ReadBoolean();
            LayerGroup7 = reader.ReadBoolean();
            LayerGroup8 = reader.ReadBoolean();
            WrinklesEnabled = reader.ReadBoolean();
            OpaqueSurfaceArea = reader.ReadSingle();
            DynamicMeshVertexCount = reader.ReadInt32();
            DynamicMeshIndexCount = reader.ReadInt32();
            DynamicMeshUVChannelCount = reader.ReadByte();
            UserCategory = reader.ReadInt32();
        }

        public void CreateMeshes()
        {
            var id = CompiledMesh?.InstancingData;
            var cm = CompiledMesh?.ClusteredMeshData;
            var md = CompiledMesh?.MeshData;
            long vertexOffset = 0;

            if (cm == null || md == null || id == null || cm.DrawPrimitives.Length <= 0 || cm.VertexBuffer0.Buffer == null || cm.IndexBuffer.Buffer == null || md.StandardPrimitives == null)
            {
                throw new Exception("Failed to read model. Invalid data");
            }

            Meshes = new Mesh[cm.NumPrimitives];
            for (int i = 0; i < Meshes.Length; i++)
            {
                //Get data
                var data = ReadVertexData(out char[] semanticCode, vertexOffset, i);
                ushort[] indices = ReadIndexData(i);
                var normals = CalculateNormals(data.Item1.ToList(), indices);
                byte[] vertexBuffer = CreateVertexBufferForCodeX(data.Item1, normals.ToArray(), data.Item2);

                //Create new vertex layout
                var elements = new VertexElement[semanticCode.Length];
                int actualStride = 0;
                vertexOffset = data.Item3;

                for (int c = 0; c < semanticCode.Length; c++)
                {
                    switch (semanticCode[c])
                    {
                        case 'P':
                            elements[c] = new VertexElement(VertexElementFormat.Float3, "POSITION", 0, 0, 0);
                            actualStride += 12;
                            break;
                        case 'N':
                            elements[c] = new VertexElement(VertexElementFormat.Float3, "NORMAL", 0, 12, 0);
                            actualStride += 12;
                            break;
                        case 'C':
                            elements[c] = new VertexElement(VertexElementFormat.Colour, "COLOR", 0, 24, 0);
                            actualStride += 4;
                            break;
                        case 'T':
                            elements[c] = new VertexElement(VertexElementFormat.Float2, "TEXCOORD", 0, 28, 0);
                            actualStride += 8;
                            break;
                    }
                }

                Meshes[i] = new Mesh
                {
                    VertexCount = id[i].NumVertices,
                    VertexStride = actualStride,
                    VertexLayout = new VertexLayout(elements),
                    VertexData = vertexBuffer,
                    Indices = indices,
                    Name = Header?.FileName ?? string.Empty
                };
                Meshes[i].UpdateBounds();
                Meshes[i].SetDefaultShader();

                var df = Meshes[i].Shader as DefaultShader;
                Meshes[i].ShaderInputs = df?.MeshVars.GetDataBag();
                Meshes[i].ShaderInputs.SetFloat4(0x7CB163F5, new Vector4(0.2f)); //BumpScale
                Meshes[i].ShaderInputs.SetFloat(0x83DDF493, 0.1f); //"EmissiveMult"
            }
        }

        public (Vector3[], Vector2[], long) ReadVertexData(out char[] semanticCode, long lastVertexOffset, int index)
        {
            var id = CompiledMesh?.InstancingData;
            var cm = CompiledMesh?.ClusteredMeshData;
            var sd = CompiledMesh?.MeshData;

            if (sd == null || cm == null || id == null || sd.StandardPrimitives == null)
            {
                throw new Exception("Invalid vertex data");
            }

            long offset = 0;
            byte[] vertexBuffer = GetCorrectVertexBuffer(sd.StandardPrimitives[index].NumVertices, cm);
            var vertices = new List<Vector3>();
            var normals = new List<Vector3>();
            var texcoords = new List<Vector2>();
            var boneIndices = new Vector4();
            var boneIndices2 = new Vector4();
            var boneWeights = new Vector4();
            var boneWeights2 = new Vector4();

            using (var dr = new ForgeDataReader(new MemoryStream(vertexBuffer)))
            {
                if (cm.VertexStride == 12)
                {
                    texcoords = GetTexcoordsFromBuffer0(id[index].NumVertices, cm);
                }

                dr.BaseStream.Position += lastVertexOffset;
                for (int i = 0; i < id[index].NumVertices; i++)
                {
                    short scaleFactor = 0;
                    vertices.Add(dr.ReadVector3AsInt16());

                    switch (cm.VertexStride)
                    {
                        case 12:
                            normals.Add(dr.ReadVector3AsInt16());
                            break;
                        case 16:
                            scaleFactor = dr.ReadInt16();
                            _ = dr.ReadInt32();
                            texcoords.Add(dr.ReadVector2AsInt16());
                            break;
                        case 20:
                            scaleFactor = dr.ReadInt16();
                            normals.Add(dr.ReadVector3AsInt16());
                            _ = dr.ReadInt16();
                            texcoords.Add(dr.ReadVector2AsInt16());
                            break;
                        case 24:
                            _ = dr.ReadInt16();
                            normals.Add(dr.ReadVector3AsInt16());
                            _ = dr.ReadInt16();
                            _ = dr.ReadInt16();
                            _ = dr.ReadInt16();
                            texcoords.Add(dr.ReadVector2AsInt16());
                            break;
                        case 28:
                            scaleFactor = dr.ReadInt16();
                            normals.Add(dr.ReadVector3AsInt16());
                            _ = dr.ReadInt16();
                            _ = dr.ReadInt16();
                            _ = dr.ReadInt16();
                            texcoords.Add(dr.ReadVector2AsInt16());
                            _ = dr.ReadInt16();
                            _ = dr.ReadInt16();
                            break;
                        case 32:
                            _ = dr.ReadInt16();
                            normals.Add(dr.ReadVector3AsInt16());
                            _ = dr.ReadInt16();
                            _ = dr.ReadInt16();
                            _ = dr.ReadInt16();
                            texcoords.Add(dr.ReadVector2AsInt16());
                            boneIndices = dr.ReadVector4AsByte();
                            boneWeights = dr.ReadVector4AsByte();
                            break;
                        case 36:
                            _ = dr.ReadInt16();
                            normals.Add(dr.ReadVector3AsInt16());
                            _ = dr.ReadInt16();
                            _ = dr.ReadInt16();
                            _ = dr.ReadInt16();
                            texcoords.Add(dr.ReadVector2AsInt16());
                            _ = dr.ReadInt16();
                            _ = dr.ReadInt16();
                            boneIndices = dr.ReadVector4AsByte();
                            boneWeights = dr.ReadVector4AsByte();
                            break;
                        case 40:
                            _ = dr.ReadInt16();
                            normals.Add(dr.ReadVector3AsInt16());
                            _ = dr.ReadInt16();
                            _ = dr.ReadInt16();
                            _ = dr.ReadInt16();
                            texcoords.Add(dr.ReadVector2AsInt16());
                            boneIndices = dr.ReadVector4AsByte();
                            boneIndices2 = dr.ReadVector4AsByte();
                            boneWeights = dr.ReadVector4AsByte();
                            boneWeights2 = dr.ReadVector4AsByte();
                            break;
                        case 44:
                            dr.BaseStream.Seek(14, SeekOrigin.Current);
                            texcoords.Add(dr.ReadVector2AsInt16());
                            boneIndices = dr.ReadVector4AsByte();
                            boneIndices2 = dr.ReadVector4AsByte();
                            boneWeights = dr.ReadVector4AsByte();
                            boneWeights2 = dr.ReadVector4AsByte();
                            _ = dr.ReadInt16();
                            _ = dr.ReadInt16();
                            break;
                        case 48:
                            _ = dr.ReadInt16();
                            normals.Add(dr.ReadVector3AsInt16());
                            _ = dr.ReadInt16();
                            _ = dr.ReadInt16();
                            _ = dr.ReadInt16();
                            texcoords.Add(dr.ReadVector2AsInt16());
                            boneIndices = dr.ReadVector4AsByte();
                            boneIndices2 = dr.ReadVector4AsByte();
                            boneWeights = dr.ReadVector4AsByte();
                            boneWeights2 = dr.ReadVector4AsByte();
                            _ = dr.ReadInt16();
                            _ = dr.ReadInt16();
                            break;
                        default:
                            dr.BaseStream.Position += cm.VertexStride - 0x6;
                            break;
                    }

                    vertices[i] = new Vector3(vertices[i].Y, vertices[i].X, vertices[i].Z);
                    vertices[i] *= scaleFactor * 0.0001f;

                    if (normals.Count > 0)
                    {
                        normals[i] = Vector3.Normalize(new Vector3(normals[i].X, normals[i].Y, normals[i].Z));
                        normals[i] *= scaleFactor * 0.0001f;
                    }
                }
                semanticCode = GenerateSemantics(normals, texcoords);
                offset = dr.BaseStream.Position;
            }
            return (vertices.ToArray(), texcoords.ToArray(), offset);
        }

        public ushort[] ReadIndexData(int meshIndex)
        {
            var cm = CompiledMesh?.ClusteredMeshData;
            var sd = CompiledMesh?.MeshData;

            if (sd == null || cm == null)
            {
                throw new Exception("Invalid index data");
            }

            var reader = new DataReader(new MemoryStream(cm.IndexBuffer.Buffer));
            var count = sd.StandardPrimitives[meshIndex].PrimitiveCount;
            ushort[] faceData = new ushort[count * 3];

            reader.BaseStream.Position += cm.DrawPrimitives[meshIndex].IndexStart * 2;
            for (int i = 0; i < faceData.Length; i++)
            {
                faceData[i] = reader.ReadUInt16();
            }

            for (int i = 0; i < faceData.Length; i += 3)
            {
                var f1 = faceData[i];
                var f2 = faceData[i + 1];
                faceData[i] = f2;
                faceData[i + 1] = f1;
            }
            return faceData;
        }

        public byte[] GetCorrectVertexBuffer(uint numVertices, ForgeClusteredMeshData cm)
        {
            uint bufferLength = numVertices * cm.VertexStride;
            byte[] buffer = cm.VertexBuffer0.Buffer;

            if (cm.VertexBuffer1 != null && cm.VertexBuffer1.Buffer.Length == 0)
            {
                return buffer;
            }

            if (bufferLength != buffer.Length)
            {
                if (cm.VertexBuffer1 != null && (cm.VertexBuffer1.Buffer.Length == bufferLength))
                    buffer = cm.VertexBuffer1.Buffer;
                else if (cm.VertexBuffer2 != null && (cm.VertexBuffer2.Buffer.Length == bufferLength))
                    buffer = cm.VertexBuffer2.Buffer;
                else if (cm.VertexBuffer3 != null && (cm.VertexBuffer3.Buffer.Length == bufferLength))
                    buffer = cm.VertexBuffer3.Buffer;
                else
                    throw new Exception("Invalid vertex data buffer");
            }
            return buffer;
        }

        public byte[] CreateVertexBufferForCodeX(Vector3[] vertices, Vector3[] normals, Vector2[] texcoords)
        {
            var buffer = new List<byte>();
            for (int i = 0; i < vertices.Length; i++)
            {
                //Add vertices
                byte[] vBytes = ForgeCrypto.GetBytes(vertices[i]);
                buffer.AddRange(vBytes);

                //Add vertex normals
                if (normals != null && normals.Length > 0)
                {
                    byte[] nBytes = ForgeCrypto.GetBytes(normals[i]);
                    buffer.AddRange(nBytes);
                }

                //Add vertex color
                buffer.AddRange(BitConverter.GetBytes(uint.MaxValue));

                //Add texture coordinates
                if (texcoords != null && texcoords.Length > 0)
                {
                    byte[] nBytes = ForgeCrypto.GetBytes(texcoords[i] / 2048.0f);
                    buffer.AddRange(nBytes);
                }
            }
            return buffer.ToArray();
        }

        public char[] GenerateSemantics(List<Vector3> normals, List<Vector2> texcoords)
        {
            var code = new List<char> { 'P' };
            if (normals != null && normals.Count() > 0)
            {
                code.Add('N');
            }

            code.Add('C');
            if (texcoords != null && texcoords.Count() > 0)
            {
                code.Add('T');
            }
            return code.ToArray();
        }

        public List<Vector3> CalculateNormals(List<Vector3> vertices, ushort[] indices)
        {
            Vector3[] normals = new Vector3[vertices.Count];
            for (int i = 0; i < indices.Length; i += 3)
            {
                if (indices[i] < vertices.Count && indices[i + 1] < vertices.Count && indices[i + 2] < vertices.Count)
                {
                    Vector3 v1 = vertices[indices[i]];
                    Vector3 v2 = vertices[indices[i + 1]];
                    Vector3 v3 = vertices[indices[i + 2]];

                    //The normal is the cross-product of two sides of the triangle
                    normals[indices[i]] += Vector3.Cross(v2 - v1, v3 - v1);
                    normals[indices[i + 1]] += Vector3.Cross(v2 - v1, v3 - v1);
                    normals[indices[i + 2]] += Vector3.Cross(v2 - v1, v3 - v1);
                }
            }

            for (int i = 0; i < vertices.Count; i++)
            {
                normals[i] = Vector3.Normalize(normals[i]);
            }
            return normals.ToList();
        }

        public List<Vector2> GetTexcoordsFromBuffer0(uint numVertices, ForgeClusteredMeshData cm)
        {
            var texcoords = new List<Vector2>();
            using (var dr = new DataReader(new MemoryStream(cm.VertexBuffer0.Buffer)))
            {
                for (int i = 0; i < numVertices; i++)
                {
                    uint vertexColor = dr.ReadUInt32();
                    texcoords.Add(new Vector2(dr.ReadInt16() * 2, dr.ReadInt16()));
                }
            }
            return texcoords;
        }
    }

    public class ForgeCompiledMesh
    {
        public ForgeObject ObjectCompiledMesh;
        public int NumDataBytes;
        public byte[] Data;
        public ForgeClusteredMeshData ClusteredMeshData;
        public ForgeMeshData MeshData;
        public int NumInstancingData;
        public ForgeMeshInstancingData[] InstancingData;
        public uint PlatformVersion;
        public uint SDKVersion;
        public float QuantizationFactor;

        public ForgeCompiledMesh()
        {
            InstancingData = Array.Empty<ForgeMeshInstancingData>();
        }

        public ForgeCompiledMesh(ForgeCompiledMesh mesh)
        {
            ObjectCompiledMesh = mesh.ObjectCompiledMesh;
            NumDataBytes = mesh.NumDataBytes;
            Data = mesh.Data;
            ClusteredMeshData = mesh.ClusteredMeshData;
            MeshData = mesh.MeshData;
            NumInstancingData = mesh.NumInstancingData;
            InstancingData = mesh.InstancingData;
            PlatformVersion = mesh.PlatformVersion;
            SDKVersion = mesh.SDKVersion;
            QuantizationFactor = mesh.QuantizationFactor;
        }

        public void Read(DataReader reader)
        {
            ObjectCompiledMesh = new ForgeObject();
            ObjectCompiledMesh.Read(reader);

            NumDataBytes = reader.ReadInt32();
            Data = new byte[NumDataBytes];
            for (int i = 0; i < Data.Length; i++)
            {
                Data[i] = reader.ReadByte();
            }

            ClusteredMeshData = new ForgeClusteredMeshData();
            ClusteredMeshData.Read(reader);

            MeshData = new ForgeMeshData();
            MeshData.Read(reader);

            NumInstancingData = reader.ReadInt32();
            InstancingData = new ForgeMeshInstancingData[NumInstancingData];
            for (int i = 0; i < InstancingData.Length; i++)
            {
                InstancingData[i] = new ForgeMeshInstancingData();
                InstancingData[i].Read(reader);
            }

            PlatformVersion = reader.ReadUInt32();
            SDKVersion = reader.ReadUInt32();
            QuantizationFactor = reader.ReadSingle();
        }
    }

    public class ForgeMesh
    {
        public short ID { get; set; }
        public int VertexCount { get; set; }
        public int FaceCount { get; set; }
        public int IndexCount { get; set; }

        public ForgeMesh()
        {
        }
    }

    public class ForgeBone
    {
        public long ID { get; set; }
        public int Type { get; set; }
        public int Name { get; set; }
        public Matrix4x4 TransformMatrix { get; set; }

        public ForgeBone()
        {
        }
    }

    public class ForgeClusteredMeshData
    {
        public ForgeObject ObjectForgeClusteredMeshData { get; set; }
        public uint Version { get; set; }
        public byte VertexFormat { get; set; }
        public uint VertexStride { get; set; }
        public uint VertexStrideDynamic { get; set; }
        public uint VertexStrideIndexWeight { get; set; }
        public uint VertexStridePrecompute { get; set; }
        public uint ClusterCount { get; set; }
        public Vector3 Center { get; set; }
        public Vector3 HalfExtend { get; set; }
        public bool IsFixedClusterSize { get; set; }
        public int NumPrimitives { get; set; }
        public ClusteredPrimitiveInfo[] DrawPrimitives { get; set; }
        public ForgeDataBuffer VertexBuffer0 { get; set; }
        public ForgeDataBuffer VertexBuffer1 { get; set; }
        public ForgeDataBuffer VertexBuffer2 { get; set; }
        public ForgeDataBuffer VertexBuffer3 { get; set; }
        public ForgeDataBuffer IndexBuffer { get; set; }
        public ForgeDataBuffer ClusteredDescData { get; set; }
        public ForgeDataBuffer ClusterBitMask { get; set; }
        public ForgeDataBuffer PrimitiveDescData { get; set; }

        public ForgeClusteredMeshData()
        {
            DrawPrimitives = Array.Empty<ClusteredPrimitiveInfo>();
            VertexBuffer0 = new ForgeDataBuffer();
            IndexBuffer = new ForgeDataBuffer();
        }

        public void Read(DataReader reader)
        {
            ObjectForgeClusteredMeshData = new ForgeObject();
            ObjectForgeClusteredMeshData.Read(reader);

            Version = reader.ReadUInt32();
            VertexFormat = reader.ReadByte();
            VertexStride = reader.ReadUInt32();
            VertexStrideDynamic = reader.ReadUInt32();
            VertexStrideIndexWeight = reader.ReadUInt32();
            VertexStridePrecompute = reader.ReadUInt32();
            ClusterCount = reader.ReadUInt32();
            Center = reader.ReadVector3();
            HalfExtend = reader.ReadVector3();
            IsFixedClusterSize = reader.ReadBoolean();

            NumPrimitives = reader.ReadInt32();
            DrawPrimitives = new ClusteredPrimitiveInfo[NumPrimitives];
            for (int i = 0; i < DrawPrimitives.Length; i++)
            {
                DrawPrimitives[i] = new ClusteredPrimitiveInfo();
                DrawPrimitives[i].Read(reader);
            }

            VertexBuffer0 = new ForgeDataBuffer();
            VertexBuffer0.Read(reader);

            VertexBuffer1 = new ForgeDataBuffer();
            VertexBuffer1.Read(reader);

            VertexBuffer2 = new ForgeDataBuffer();
            VertexBuffer2.Read(reader);

            VertexBuffer3 = new ForgeDataBuffer();
            VertexBuffer3.Read(reader);

            IndexBuffer = new ForgeDataBuffer();
            IndexBuffer.Read(reader);

            ClusteredDescData = new ForgeDataBuffer();
            ClusteredDescData.Read(reader);

            ClusterBitMask = new ForgeDataBuffer();
            ClusterBitMask.Read(reader);

            PrimitiveDescData = new ForgeDataBuffer();
            PrimitiveDescData.Read(reader);

            VertexStride = (VertexStride != 8 && VertexStride != 16) ? VertexStride : VertexStrideDynamic;
        }
    }

    public class ForgeMeshData
    {
        public ForgeBaseObject BaseObjectForgeMeshData { get; set; }
        public int MeshCount { get; set; }
        public bool IsIndexBuffer32bit { get; set; }
        public byte VertexFormat { get; set; }
        public byte VertexStrideStatic { get; set; }
        public byte VertexStrideDynamic { get; set; }
        public byte VertexStrideIndexWeight { get; set; }
        public byte VertexStridePrecompute { get; set; }
        public byte NbBonesPerVertex { get; set; }
        public byte NbBlendShapes { get; set; }
        public byte NbSkinWrapWeights { get; set; }
        public byte NbFaceIndices { get; set; }
        public byte FaceIndicesStartOffset { get; set; }
        public int NumStandardPrimitives { get; set; }
        public MeshPrimitive[] StandardPrimitives { get; set; }
        public ForgeDataBuffer VertexBuffer0 { get; set; }
        public ForgeDataBuffer VertexBuffer1 { get; set; }
        public ForgeDataBuffer VertexBuffer2 { get; set; }
        public ForgeDataBuffer VertexBuffer3 { get; set; }
        public ForgeDataBuffer IndexBuffer { get; set; }

        public ForgeMeshData()
        {
        }

        public void Read(DataReader reader)
        {
            BaseObjectForgeMeshData = new ForgeBaseObject();
            BaseObjectForgeMeshData.Read(reader);

            IsIndexBuffer32bit = reader.ReadBoolean();
            VertexFormat = reader.ReadByte();
            VertexStrideStatic = reader.ReadByte();
            VertexStrideDynamic = reader.ReadByte();
            VertexStrideIndexWeight = reader.ReadByte();
            VertexStridePrecompute = reader.ReadByte();
            NbBonesPerVertex = reader.ReadByte();
            NbBlendShapes = reader.ReadByte();
            NbSkinWrapWeights = reader.ReadByte();
            NbFaceIndices = reader.ReadByte();
            FaceIndicesStartOffset = reader.ReadByte();

            NumStandardPrimitives = reader.ReadInt32();
            StandardPrimitives = new MeshPrimitive[NumStandardPrimitives];
            for (int i = 0; i < StandardPrimitives.Length; i++)
            {
                StandardPrimitives[i] = new MeshPrimitive();
                StandardPrimitives[i].Read(reader);
            }

            VertexBuffer0 = new ForgeDataBuffer();
            VertexBuffer0.Read(reader);

            VertexBuffer1 = new ForgeDataBuffer();
            VertexBuffer1.Read(reader);

            VertexBuffer2 = new ForgeDataBuffer();
            VertexBuffer2.Read(reader);

            VertexBuffer3 = new ForgeDataBuffer();
            VertexBuffer3.Read(reader);

            IndexBuffer = new ForgeDataBuffer();
            IndexBuffer.Read(reader);
        }
    }

    public class ForgeMeshInstancingData
    {
        public ForgeBaseObject BaseObjectForgeMeshInstancingData { get; set; }
        public bool ShadowCaster { get; set; }
        public ushort SubMeshIndex { get; set; }
        public ushort MaterialType { get; set; }
        public ushort NumVertices { get; set; }
        public ushort RenderingMask { get; set; }
        public ForgeObjectPtr Material { get; set; }

        public ForgeMeshInstancingData()
        {
        }

        public void Read(DataReader reader)
        {
            BaseObjectForgeMeshInstancingData = new ForgeBaseObject();
            BaseObjectForgeMeshInstancingData.Read(reader);

            ShadowCaster = reader.ReadBoolean();
            SubMeshIndex = reader.ReadUInt16();
            MaterialType = reader.ReadUInt16();
            NumVertices = reader.ReadUInt16();
            RenderingMask = reader.ReadUInt16();

            Material = new ForgeObjectPtr();
            Material.Read(reader);
        }
    }

    public class MeshPrimitive
    {
        public ForgeBaseObject BaseObjectMeshPrimitive { get; set; }
        public uint MinIndex { get; set; }
        public uint IsUsingDepthOnlyBuffers { get; set; }
        public uint NumVertices { get; set; }
        public uint StartIndex { get; set; }
        public uint PrimitiveCount { get; set; }
        public PrimitiveType Type { get; set; }

        public MeshPrimitive()
        {
        }

        public void Read(DataReader reader)
        {
            BaseObjectMeshPrimitive = new ForgeBaseObject();
            BaseObjectMeshPrimitive.Read(reader);

            MinIndex = reader.ReadUInt32();
            IsUsingDepthOnlyBuffers = reader.ReadUInt32();
            NumVertices = reader.ReadUInt32();
            StartIndex = reader.ReadUInt32();
            PrimitiveCount = reader.ReadUInt32();
            Type = (PrimitiveType)reader.ReadUInt32();
        }
    }

    public class ClusteredPrimitiveInfo
    {
        public ForgeBaseObject BaseObjectClusteredPrimitiveInfo { get; set; }
        public int ClusterCount { get; set; }
        public int IndexStart { get; set; }
        public int VertexStart { get; set; }

        public ClusteredPrimitiveInfo()
        {
        }

        public void Read(DataReader reader)
        {
            BaseObjectClusteredPrimitiveInfo = new ForgeBaseObject();
            BaseObjectClusteredPrimitiveInfo.Read(reader);

            ClusterCount = reader.ReadInt32();
            IndexStart = reader.ReadInt32();
            VertexStart = reader.ReadInt32();
        }
    }

    public class ForgeDataBuffer
    {
        public int Size { get; set; }
        public long Offset { get; set; }
        public byte[] Buffer { get; set; }

        public ForgeDataBuffer()
        {
            Buffer = new byte[Size];
        }

        public void Read(DataReader reader)
        {
            Size = reader.ReadInt32();
            Offset = reader.BaseStream.Position;
            Buffer = reader.ReadBytes(Size < 0 ? 0 : Size);
        }
    }

    public class SubMesh
    {
        public SubMesh()
        {
        }

        public SubMesh(SubMesh submesh)
        {
        }

        public void Read(DataReader reader)
        {
            throw new Exception("Unknown data: SubMesh");
        }
    }

    public class MeshBone
    {
        public ForgeBaseObject BaseObjectMeshBone { get; set; }
        public BoneID BoneID { get; private set; }
        public Matrix4x4 InitialMatrixInverse { get; private set; }
        public bool IsUsedBySubMeshes { get; private set; }

        public MeshBone()
        {
        }

        public MeshBone(MeshBone bone)
        {
            BaseObjectMeshBone = bone.BaseObjectMeshBone;
            BoneID = bone.BoneID;
            InitialMatrixInverse = bone.InitialMatrixInverse;
            IsUsedBySubMeshes = bone.IsUsedBySubMeshes;
        }

        public void Read(DataReader reader)
        {
            BaseObjectMeshBone = new ForgeBaseObject();
            BaseObjectMeshBone.Read(reader);

            BoneID = (BoneID)reader.ReadUInt32();
            InitialMatrixInverse = reader.ReadMatrix4x4();
            IsUsedBySubMeshes = reader.ReadBoolean();
        }

        public override string ToString()
        {
            return BoneID.ToString();
        }
    }

    public class SkinWrapProxyMesh
    {
        public ForgeObject ObjectPtrSkinWrapLayerMesh { get; set; }
        public int NumVertices { get; set; }
        public Vector3[] Vertices { get; set; }
        public int NumNormals { get; set; }
        public Vector3[] Normals { get; set; }
        public int NumIndices { get; set; }
        public ushort[] Indices { get; set; }
        public int NumProjectedNormals { get; set; }
        public float[] ProjectedNormals { get; set; }
        public int NumTriangles { get; set; }
        public SkinWrapProxyMeshTriangle[] Triangles { get; set; }
        public int NumTargets { get; set; }
        public SkinWrapProxyMeshTarget[] Targets { get; set; }

        public SkinWrapProxyMesh()
        {
        }

        public SkinWrapProxyMesh(SkinWrapProxyMesh proxyMesh)
        {

        }

        public void Read(DataReader reader)
        {
            ObjectPtrSkinWrapLayerMesh = new ForgeObject();
            ObjectPtrSkinWrapLayerMesh.Read(reader, true);

            if (ObjectPtrSkinWrapLayerMesh.Num == 3)
            {
                return;
            }

            NumVertices = reader.ReadInt32();
            Vertices = new Vector3[NumVertices];
            for (int i = 0; i < Vertices.Length; i++)
            {
                Vertices[i] = reader.ReadVector3();
            }

            NumNormals = reader.ReadInt32();
            Normals = new Vector3[NumNormals];
            for (int i = 0; i < Normals.Length; i++)
            {
                Normals[i] = reader.ReadVector3();
            }

            NumIndices = reader.ReadInt32();
            Indices = new ushort[NumIndices];
            for (int i = 0; i < Indices.Length; i++)
            {
                Indices[i] = reader.ReadUInt16();
            }

            NumProjectedNormals = reader.ReadInt32();
            ProjectedNormals = new float[NumProjectedNormals];
            for (int i = 0; i < ProjectedNormals.Length; i++)
            {
                ProjectedNormals[i] = reader.ReadSingle();
            }

            NumTriangles = reader.ReadInt32();
            Triangles = new SkinWrapProxyMeshTriangle[NumTriangles];
            for (int i = 0; i < Triangles.Length; i++)
            {
                Triangles[i] = new SkinWrapProxyMeshTriangle();
                Triangles[i].Read(reader);
            }

            NumTargets = reader.ReadInt32();
            Targets = new SkinWrapProxyMeshTarget[NumTargets];
            for (int i = 0; i < Targets.Length; i++)
            {
                Targets[i] = new SkinWrapProxyMeshTarget();
                Targets[i].Read(reader);
            }
        }
    }

    public class SkinWrapLayerMesh
    {
        public ForgeObject ObjectPtrSkinWrapLayerMesh { get; set; }
        public int NumX { get; set; }
        public float[] FloatsX { get; set; }
        public int NumY { get; set; }
        public float[] FloatsY { get; set; }
        public int NumZ { get; set; }
        public float[] FloatsZ { get; set; }
        public int NumSkinLayerMeshIndex { get; set; }
        public byte[] SkinWrapLayerMeshIndexData { get; set; }
        public int NumWeightIndices { get; set; }
        public SkinWrapLayerMeshPackedWeightIndicesArray[] WeightIndices { get; set; }
        public int NumWeights { get; set; }
        public SkinWrapLayerMeshPackedWeightsArray[] Weights { get; set; }

        public SkinWrapLayerMesh()
        {
        }

        public SkinWrapLayerMesh(SkinWrapLayerMesh layerMesh)
        {
        }

        public void Read(DataReader reader)
        {
            ObjectPtrSkinWrapLayerMesh = new ForgeObject();
            ObjectPtrSkinWrapLayerMesh.Read(reader, true);

            if (ObjectPtrSkinWrapLayerMesh.Num == 3)
            {
                return;
            }

            NumX = reader.ReadInt32();
            FloatsX = new float[NumX];
            for (int i = 0; i < FloatsX.Length; i++)
            {
                FloatsX[i] = reader.ReadSingle();
            }

            NumY = reader.ReadInt32();
            FloatsY = new float[NumY];
            for (int i = 0; i < FloatsY.Length; i++)
            {
                FloatsY[i] = reader.ReadSingle();
            }

            NumZ = reader.ReadInt32();
            FloatsZ = new float[NumZ];
            for (int i = 0; i < FloatsZ.Length; i++)
            {
                FloatsZ[i] = reader.ReadSingle();
            }

            NumSkinLayerMeshIndex = reader.ReadInt32();
            SkinWrapLayerMeshIndexData = reader.ReadBytes(NumSkinLayerMeshIndex);

            NumWeightIndices = reader.ReadInt32();
            WeightIndices = new SkinWrapLayerMeshPackedWeightIndicesArray[NumWeightIndices];
            for (int i = 0; i < WeightIndices.Length; i++)
            {
                WeightIndices[i] = new SkinWrapLayerMeshPackedWeightIndicesArray();
                WeightIndices[i].Read(reader);
            }

            NumWeights = reader.ReadInt32();
            Weights = new SkinWrapLayerMeshPackedWeightsArray[NumWeights];
            for (int i = 0; i < WeightIndices.Length; i++)
            {
                Weights[i] = new SkinWrapLayerMeshPackedWeightsArray();
                Weights[i].Read(reader);
            }
        }
    }

    public class SkinWrapProxyMeshTriangle
    {
        public ForgeBaseObject BaseObjectSkinWrapProxyMeshTriangle { get; set; }
        public float PlaneNormalX { get; set; }
        public float PlaneNormalY { get; set; }
        public float PlaneNormalZ { get; set; }
        public float PlaneConstant { get; set; }
        public float Edge0PerpX { get; set; }
        public float Edge0PerpY { get; set; }
        public float Edge0PerpZ { get; set; }
        public float Edge1PerpX { get; set; }
        public float Edge1PerpY { get; set; }
        public float Edge1PerpZ { get; set; }
        public float Edge2PerpX { get; set; }
        public float Edge2PerpY { get; set; }
        public float Edge2PerpZ { get; set; }

        public SkinWrapProxyMeshTriangle()
        {
        }

        public void Read(DataReader reader)
        {
            BaseObjectSkinWrapProxyMeshTriangle = new ForgeBaseObject();
            BaseObjectSkinWrapProxyMeshTriangle.Read(reader);

            PlaneNormalX = reader.ReadSingle();
            PlaneNormalY = reader.ReadSingle();
            PlaneNormalZ = reader.ReadSingle();
            PlaneConstant = reader.ReadSingle();
            Edge0PerpX = reader.ReadSingle();
            Edge0PerpY = reader.ReadSingle();
            Edge0PerpZ = reader.ReadSingle();
            Edge1PerpX = reader.ReadSingle();
            Edge1PerpY = reader.ReadSingle();
            Edge1PerpZ = reader.ReadSingle();
            Edge2PerpX = reader.ReadSingle();
            Edge2PerpY = reader.ReadSingle();
            Edge2PerpZ = reader.ReadSingle();
        }
    }

    public class SkinWrapProxyMeshTarget
    {
        public ForgeBaseObject BaseObjectSkinWrapProxyMeshTarget { get; set; }
        public int ID { get; set; }
        public int NumOffsets { get; set; }
        public ushort[] Offsets { get; set; }

        public SkinWrapProxyMeshTarget()
        {
        }

        public void Read(DataReader reader)
        {
            BaseObjectSkinWrapProxyMeshTarget = new ForgeBaseObject();
            BaseObjectSkinWrapProxyMeshTarget.Read(reader);

            ID = reader.ReadInt32();
            NumOffsets = reader.ReadInt32();

            Offsets = new ushort[NumOffsets];
            for (int i = 0; i < Offsets.Length; i++)
            {
                Offsets[i] = reader.ReadUInt16();
            }
        }
    }

    public class SkinWrapLayerMeshPackedWeightIndicesArray
    {
        public ForgeBaseObject BaseObjectClass { get; set; }
        public int NumFormats { get; set; }
        public byte[] Formats { get; set; }
        public int NumIndices { get; set; }
        public byte[] Indices { get; set; }

        public SkinWrapLayerMeshPackedWeightIndicesArray()
        {
        }

        public void Read(DataReader reader)
        {
            BaseObjectClass = new ForgeBaseObject();
            BaseObjectClass.Read(reader);

            NumFormats = reader.ReadInt32();
            Formats = reader.ReadBytes(NumFormats);

            NumIndices = reader.ReadInt32();
            Indices = reader.ReadBytes(NumIndices);
        }
    }

    public class SkinWrapLayerMeshPackedWeightsArray
    {
        public ForgeBaseObject BaseObjectClass { get; set; }
        public int NumWeights { get; set; }
        public byte[] Weights { get; set; }

        public SkinWrapLayerMeshPackedWeightsArray()
        {
        }

        public void Read(DataReader reader)
        {
            BaseObjectClass = new ForgeBaseObject();
            BaseObjectClass.Read(reader);

            NumWeights = reader.ReadInt32();
            Weights = reader.ReadBytes(NumWeights);
        }
    }

    public class SubMeshesBlendShapeID
    {
        public SubMeshesBlendShapeID()
        {
        }

        public SubMeshesBlendShapeID(SubMeshesBlendShapeID shape)
        {
        }

        public void Read(DataReader reader)
        {
            throw new Exception("Unknown data: SubMeshesBlendShapeID");
        }
    }

    public class MorphMapping
    {
        public MorphMapping()
        {
        }

        public MorphMapping(MorphMapping mapping)
        {
        }

        public void Read(DataReader reader)
        {
            throw new Exception("Unknown data: MorphMapping");
        }
    }

    public enum PrimitiveType
    {
        Strip,
        List
    }

    public enum VertexFormat
    {
        Null = -1, // 0xFFFFFFFF
        Skinning4Bones = 0,
        Skinning8Bones = 1,
        Skinning4BonesFat = 2,
        Skinning8BonesFat = 3,
        Static = 4,
        StaticQuantizedPosition = 5,
        StaticQuantizedPositionNoColor = 6,
        Position3f_Color4ub_Texcoord2f = 7,
        Position3f_Normal3f_Texcoord2f = 8,
        Position3f_Normal3f_Stream1_Texcoord2f_Color4ub = 9,
        OceanMeshFormat = 10, // 0x0000000A
        StaticMeshClutter = 11, // 0x0000000B
        Position3f_Texcoord2f = 12, // 0x0000000C
        BulkInstance = 13, // 0x0000000D
        BulkInstanceWithSkinBuffer = 14, // 0x0000000E
        FakeMesh = 15, // 0x0000000F
        SoftBodyMesh = 16, // 0x00000010
        SnowMesh = 17, // 0x00000011
        SnowMeshPatch = 18, // 0x00000012
        TerrainFormat = 19, // 0x00000013
        TerrainFormatOptimized = 20, // 0x00000014
        EdgeSkinning = 21, // 0x00000015
        RealTreeTrunk = 22, // 0x00000016
        RealTreeLeaves = 23, // 0x00000017
        RealTreeHybridLeaves = 24, // 0x00000018
        Position3f = 25, // 0x00000019
        SkinningUnk4BonesColor = 26, // 0x0000001A
        SkinningUnk8BonesColor = 27, // 0x0000001B
        Skinning4BonesColorUnk = 28, // 0x0000001C
        Skinning8BonesColorUnk = 29, // 0x0000001D
        Skinning4BonesColor = 30, // 0x0000001E
        Skinning8BonesColor = 31, // 0x0000001F
    }

    public enum BoneID : uint
    {
        BONE_REFERENCE = 0xb6c65665, 
		BONE_ROOT = 0xb6c65665,
		BONE_ROOT_OFFSET = 0x22e53c03,
		BONE_HIPS = 0xded10611, //Hips
		BONE_SPINE = 0x530ec1cb, //Spine
		BONE_SPINE1 = 0x8f39fa4e, //Spine1
		BONE_SPINE2 = 0x1630abf4, //Spine2
		BONE_NECK = 0x8023796d, //Neck
		BONE_HEAD = 0x7c159a2, //Head
		BONE_RIGHTSHOULDER = 0xf60647e5, //RightShoulder
		BONE_RIGHTARM = 0x6bb3f727, //RightArm
		BONE_RIGHTFOREARM = 0x7257a1aa, //RightForeArm
		BONE_RIGHTHAND = 0x75f94d30, //RightHand
		BONE_RIGHTHANDTHUMB1 = 0x9829becf, //RightHandThumb1
		BONE_RIGHTHANDTHUMB2 = 0x120ef75, //RightHandThumb2
		BONE_RIGHTHANDTHUMB3 = 0x7627dfe3, //RightHandThumb3
		BONE_RIGHTHANDRING1 = 0xdbf635c5, //RightHandRing1
		BONE_RIGHTHANDRING2 = 0x42ff647f, //RightHandRing2
		BONE_RIGHTHANDRING3 = 0x35f854e9, //RightHandRing3
		BONE_RIGHTHANDMIDDLE1 = 0x45445772, //RightHandMiddle1
		BONE_RIGHTHANDMIDDLE2 = 0xdc4d06c8, //RightHandMiddle2
		BONE_RIGHTHANDMIDDLE3 = 0xab4a365e, //RightHandMiddle3
		BONE_RIGHTHANDINDEX1 = 0xfebac1b3, //RightHandIndex1
		BONE_RIGHTHANDINDEX2 = 0x67b39009, //RightHandIndex2
		BONE_RIGHTHANDINDEX3 = 0x10b4a09f, //RightHandIndex3
		BONE_RIGHTHANDCUP = 0x2a0d8811, //RightHandCup
		BONE_RIGHTHANDPINKY1 = 0x147bb2df, //RightHandPinky1
		BONE_RIGHTHANDPINKY2 = 0x8d72e365, //RightHandPinky2
		BONE_RIGHTHANDPINKY3 = 0xfa75d3f3, //RightHandPinky3
		BONE_R_RIGHTFOREARMROLL_U = 0x637d11c4,
		BONE_R_RIGHTFOREARMROLL_D = 0x9cd3136,
		BONE_C_RIGHTELBOW = 0xee3f1ccb, //RightElbow
		BONE_R_RIGHTARMROLL_U = 0xfee9f290,
		BONE_R_RIGHTARMROLL_D = 0x9459d262,
		BONE_FX_TEMPLE_R = 0xdfff7b8d,
		BONE_FX_TEMPLE_L = 0x25f046ee,
		BONE_FX_NOSE_R = 0x40498122,
		BONE_FX_NOSE_M = 0xcd418cd7,
		BONE_FX_NOSE_L = 0xba46bc41,
		BONE_FX_MOUTH_UR = 0x5e0f376e,
		BONE_FX_MOUTH_UL = 0xa4000a0d,
		BONE_FX_MOUTH_U = 0xdea4100c,
		BONE_FX_MOUTH_R = 0x40c085af,
		BONE_FX_MOUTH_L = 0xbacfb8cc,
		BONE_FX_JAW_R1 = 0xa363c4eb,
		BONE_FX_JAW_M = 0x5da6e280,
		BONE_FX_TONGUE = 0x942592d4,
		BONE_FX_MOUTH_DR = 0xdd6147e,
		BONE_FX_MOUTH_DL = 0xf7d9291d,
		BONE_FX_MOUTH_D = 0xb41430fe,
		BONE_FX_JAW_R2 = 0x3a6a9551,
		BONE_FX_JAW_L2 = 0xee2baa8e,
		BONE_FX_CHIN_R = 0x7d0616f6,
		BONE_FX_CHIN_M = 0xf00e1b03,
		BONE_FX_CHIN_L = 0x87092b95,
		BONE_FX_JAW_L1 = 0x7722fb34,
		BONE_FX_EYELID_RU = 0xfddd5cd4,
		BONE_FX_EYELID_RD = 0x976d7c26,
		BONE_FX_EYELID_LU = 0x299c630b,
		BONE_FX_EYELID_LD = 0x432c43f9,
		BONE_FX_EYEBROW_R2 = 0xf7774fcd,
		BONE_FX_EYEBROW_R1 = 0x6e7e1e77,
		BONE_FX_EYEBROW_M = 0x977aed75,
		BONE_FX_EYEBROW_L2 = 0x23367012,
		BONE_FX_EYEBROW_L1 = 0xba3f21a8,
		BONE_FX_EYE_R = 0xd8f170ca,
		BONE_FX_EYE_L = 0x22fe4da9,
		BONE_FX_EAR_R = 0x5435ba4f,
		BONE_FX_EAR_L = 0xae3a872c,
		BONE_FX_CHEEK_R4 = 0x29515a3f,
		BONE_FX_CHEEK_R3 = 0xb735cf9c,
		BONE_FX_CHEEK_R2 = 0xc032ff0a,
		BONE_FX_CHEEK_R1 = 0x593baeb0,
		BONE_FX_CHEEK_L4 = 0xfd1065e0,
		BONE_FX_CHEEK_L3 = 0x6374f043,
		BONE_FX_CHEEK_L2 = 0x1473c0d5,
		BONE_FX_CHEEK_L1 = 0x8d7a916f,
		BONE_FX_THROAT = 0x76c700fc,
		BONE_FX_TEETH_DOWN = 0x9e64f428,
		BONE_FX_CHIN_UP = 0x99e55949,
		BONE_LEFTSHOULDER = 0x2d4660a8, //LeftShoulder
		BONE_LEFTARM = 0xeb830ada, //LeftArm
		BONE_R_LEFTARMROLL_U = 0xd190180d,
		BONE_R_LEFTARMROLL_D = 0xbb2038ff,
		BONE_LEFTFOREARM = 0x89b93a80, //LeftForeArm
		BONE_R_LEFTFOREARMROLL_U = 0xdd0e822d,
		BONE_R_LEFTFOREARMROLL_D = 0xb7bea2df,
		BONE_LEFTHAND = 0xb675f36c, //LeftHand
		BONE_LEFTHANDTHUMB1 = 0x78f4e1ac, //LeftHandThumb1
		BONE_LEFTHANDTHUMB2 = 0xe1fdb016, //LeftHandThumb2
		BONE_LEFTHANDTHUMB3 = 0x96fa8080, //LeftHandThumb3
		BONE_LEFTHANDRING1 = 0xd34048cf, //LeftHandRing1
		BONE_LEFTHANDRING2 = 0x4a491975, //LeftHandRing2
		BONE_LEFTHANDRING3 = 0x3d4e29e3, //LeftHandRing3
		BONE_LEFTHANDMIDDLE1 = 0x911fbacf, //LeftHandMiddle1
		BONE_LEFTHANDMIDDLE2 = 0x816eb75, //LeftHandMiddle2
		BONE_LEFTHANDMIDDLE3 = 0x7f11dbe3, //LeftHandMiddle3
		BONE_LEFTHANDINDEX1 = 0x1e679ed0, //LeftHandIndex1
		BONE_LEFTHANDINDEX2 = 0x876ecf6a, //LeftHandIndex2
		BONE_LEFTHANDINDEX3 = 0xf069fffc, //LeftHandIndex3
		BONE_LEFTHANDCUP = 0xd1e3133b, //LeftHandCup
		BONE_LEFTHANDPINKY1 = 0xf4a6edbc, //LeftHandPinky1
		BONE_LEFTHANDPINKY2 = 0x6dafbc06, //LeftHandPinky2
		BONE_LEFTHANDPINKY3 = 0x1aa88c90, //LeftHandPinky3
		BONE_C_LEFTELBOW = 0x32fe6838, 
		BONE_RIGHTUPLEG = 0x757f1291, //RightUpLeg
		BONE_RIGHTLEG = 0x863d09fc, //RightLeg
		BONE_RIGHTFOOT = 0x9b14362c, //RightFoot
		BONE_RIGHTTOEBASE = 0x42be0fcb, //RightToeBase
		BONE_C_RIGHTKNEE = 0x72d84bd7, //RightKnee
		BONE_C_RIGHTASS = 0xd6697e1e,
		BONE_L_RIGHTTHIGH = 0x7cdc2c3,
		BONE_LEFTUPLEG = 0x176183f0, //LeftUpLeg
		BONE_LEFTLEG = 0x60df401, //LeftLeg
		BONE_LEFTFOOT = 0x58988870, //LeftFoot
		BONE_LEFTTOEBASE = 0xb95094e1, //LeftToeBase
		BONE_C_LEFTKNEE = 0x65a177f9,
		BONE_C_LEFTASS = 0x18eb1380,
		BONE_L_LEFTTHIGH = 0x3a60dd87,
		BONE_R65_BA_ROOT = 0x6aee6df1,
		BONE_R65_BA_L_HAND = 0x2897bdcb,
		BONE_R65_BA_R_HAND = 0x114bde20,
		BONE_R65_BA_R_UPPERARM = 0xc393ae8e,
		BONE_R65_BA_CAMERA = 0xc12857fb,
		BONE_CAMERANODE = 0xa9cefd4a, //CameraNode
		BONE_GROUNDNODE = 0xac67f5a7, //GroundNode
		BONE_SOCKET_ATT_GUN3RD = 0x2325860,
		BONE_SOCKET_ATT_SLEDGEHAMMER = 0x26322f6a,
		BONE_SOCKET_ATT_CANISTER = 0x37a967d3,
		BIPEDBONE_INVALID_ATTACHMENT_BONES_BEGIN = 0xffffffff
	};
}