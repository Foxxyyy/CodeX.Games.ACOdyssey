using CodeX.Core.Engine;
using CodeX.Core.Numerics;
using CodeX.Core.Utilities;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace CodeX.Games.ACOdyssey.Files
{
    public class MeshGroupFile : PiecePack
    {
        public EntityGroupFile EntityGroup { get; set; }
        public List<PiecePack> Models { get; set; }
        public int ModelsCount { get; set; }

        public MeshGroupFile(List<PiecePack> pieces, EntityGroupFile group)
        {
            EntityGroup = group;
            Models = pieces;
            ModelsCount = pieces.Count;
        }

        public override void Load(byte[] data)
        {
            Pieces = new Dictionary<JenkHash, Piece>();
            if (Models != null && EntityGroup != null)
            {
                int index = 0;
                foreach (var model in Models)
                {
                    foreach (var piece in model?.Pieces ?? Enumerable.Empty<KeyValuePair<JenkHash, Piece>>())
                    {
                        if (piece.Value != null)
                        {
                            var gm = EntityGroup.EntityGroup?.GlobalMatrix;
                            var matrix = (gm == null) ? Matrix3x4.Identity : new Matrix3x4
                            {
                                Row1 = new Vector4(gm.Value.M11, gm.Value.M21, gm.Value.M31, gm.Value.M41),
                                Row2 = new Vector4(gm.Value.M12, gm.Value.M22, gm.Value.M32, gm.Value.M42),
                                Row3 = new Vector4(gm.Value.M13, gm.Value.M23, gm.Value.M33, gm.Value.M43),
                                Translation = new Vector3(gm.Value.M14, gm.Value.M24, gm.Value.M34),
                                //Orientation = new Quaternion(gm.Value.M11, gm.Value.M22, gm.Value.M33, gm.Value.M44)
                            };

                            /*for (int i = 0; i < piece.Value.AllModels.Length; i++)
                            {
                                for (int i1 = 0; i1 < piece.Value.AllModels[i].Meshes.Length; i1++)
                                {
                                    var geometry = piece.Value.AllModels[i].Meshes[i1];
                                    geometry.MeshTransform = matrix;
                                    geometry.MeshTransformMode = 1u;
                                    geometry.ShaderInputs.SetUInt32(0x14AB52EB, 1); //MeshTransformMode
                                    geometry.ShaderInputs.SetFloat3x4(0x467A3F64, matrix); //MeshTransform
                                    geometry.UpdateBounds();
                                }
                                piece.Value.UpdateAllModels();
                                piece.Value.UpdateBounds();
                                piece.Value.BoundingSphere = new BoundingSphere(piece.Value.BoundingBox.Center, piece.Value.BoundingBox.Size.Length() * 0.5f);
                            }*/
                            if (Pieces.ContainsKey(piece.Key))
                            {
                                string name = piece.Value.Name ?? string.Empty;
                                string[] parts = name.Split('.');

                                if (parts.Length > 1)
                                {
                                    string fileName = string.Join(".", parts.Take(parts.Length - 1));
                                    string extension = parts.Last();

                                    piece.Value.Name = $"{fileName}_{index}.{extension}";
                                    var key = JenkHash.GenHash($"{fileName}_{index}.{extension}");

                                    Pieces.Add(key, piece.Value);
                                }
                            }
                            else
                            {
                                Pieces.Add(piece.Key, piece.Value);
                            }
                            index++;
                        }
                    }
                }
            }
        }
    }
}