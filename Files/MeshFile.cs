using CodeX.Core.Engine;
using CodeX.Core.Numerics;
using CodeX.Core.Utilities;
using CodeX.Games.ACOdyssey.FORGE;
using CodeX.Games.ACOdyssey.Resources;
using System.Collections.Generic;
using System.IO;

namespace CodeX.Games.ACOdyssey.Files
{
    public class MeshFile : PiecePack
    {
        public ForgeModel? Model;
        public JenkHash Hash;
        public string Name;

        public MeshFile(ForgeEntry? file) : base(file)
        {
            Name = file?.NameLower ?? string.Empty;
            Hash = JenkHash.GenHash(file?.NameLower ?? string.Empty);
        }

        public MeshFile(string name)
        {
            Name = name.ToLower();
            Hash = JenkHash.GenHash(name.ToLower());
        }

        public MeshFile(MeshFile piece)
        {
            Model = piece.Model;
            FileInfo = new ForgeEntry();
            Name = piece.Name;
            Hash = JenkHash.GenHash(piece.Name.ToLower());

            var pieces = new Dictionary<JenkHash, Piece>();
            foreach (var p in piece.Pieces)
            {
                var newPiece = new ForgeModel((ForgeModel)p.Value);
                pieces.Add(Hash, newPiece);
            }
            Piece = piece.Piece;
            Pieces = pieces;
        }

        public override void Load(byte[] data)
        {
            var r = new DataReader(new MemoryStream(data));

            Model = new ForgeModel();
            Model.Read(r);

            Pieces = new Dictionary<JenkHash, Piece>();
            Piece = Model;

            ConstructPiecePack(Model);
        }

        public void ConstructPiecePack(ForgeModel model)
        {
            Pieces = new Dictionary<JenkHash, Piece>();
            Piece = model;

            if (model?.Meshes != null)
            {
                model.Lods = new PieceLod[]
                {
                    new PieceLod()
                    {
                        Models = new Model[]
                        {
                            new Model(model.Meshes)
                        },
                        LodDist = 10000f
                    }
                };

                model.UpdateAllModels();
                model.UpdateBounds();
                model.Name = Name;
                model.BoundingSphere = new BoundingSphere(model.BoundingBox.Center, model.BoundingBox.Size.Length() * 0.5f);

                Pieces.Add(JenkHash.GenHash(Name), model);
            }
        }
    }
}
