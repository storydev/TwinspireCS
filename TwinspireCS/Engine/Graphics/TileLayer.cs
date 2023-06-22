using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace TwinspireCS.Engine.Graphics
{
    public class TileLayer
    {

        public Tile[] Tiles;
        public float Opacity;
        public Vector2 Offset;
        public Matrix4x4 Transform;
        public Matrix4x4[][] TileTransforms;

        public TileLayer()
        {
            Tiles = Array.Empty<Tile>();
            Opacity = 1.0f;
            Offset = Vector2.Zero;
            Transform = Matrix4x4.Identity;
            TileTransforms = Array.Empty<Matrix4x4[]>();
        }

    }
}
