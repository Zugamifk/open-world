using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

namespace MeshGenerator
{
    public partial class Plane : IMeshGenerator
    {

        public bool smooth = true;
        public bool tile = false;
        public int gridWidth = 1;
        public int gridHeight = 1;

        public string Name
        {
            get { return "Plane"; }
        }

        public Mesh Generate()
        {
            // Extreme points of plane
            var pts = Utils.GetRingPoints(
				4,
				Vector3.up,
				new Vector3(-0.5f, 0, -0.5f)
			).ToArray().ToEach(
				v => v += new Vector3(0.5f, 0, 0.5f)
			);

            // width of a tile in the plane
            var xs = 1f / (float)gridWidth;
            var ys = 1f / (float)gridHeight;

            // Start making verts
            var verts = new Vector3[(gridWidth + 1) * (gridHeight + 1)];
            // Add first vert
            verts[0] = pts[0];

            // Fill first row
            verts.Fill(
                verts.SubArray(0, 1),
                (v, i) =>
                    v + (float)(i + 1) * xs * Vector3.right,
                1,
                gridWidth
            );

            // Fill columns
            verts.Fill(
                verts.SubArray(0, gridWidth + 1),
                (v, i) =>
                    v + (float)(i / (gridWidth + 1) + 1) * ys * Vector3.forward,
                gridWidth + 1,
                (gridWidth + 1) * gridHeight
            );

            var tris = new int[gridHeight * gridWidth * 6];
            var uv = new Vector2[verts.Length];

            if (smooth)
            {
                // fill grid tiles
                var ti = 0;
                for (int y = 0; y < gridHeight; y++)
                {
                    for (int x = 0; x < gridWidth; x++)
                    {
                        var xi0 = y * (gridWidth + 1) + x;
                        var xi1 = (y + 1) * (gridWidth + 1) + x;
                        Utils.AddFace(tris, ti * 6, xi0 + 1, xi0, xi1, xi1 + 1);
                        ti++;
                    }
                }

                uv.Fill(verts, v => new Vector2(v.x, v.z));
            }
            else
            {
                var grid = new Vector3[(gridWidth + 1) * (gridHeight + 1)];
                verts.CopyTo(grid, 0);

                verts = new Vector3[gridWidth * gridHeight * 4];
                uv = new Vector2[verts.Length];

                // fill grid tiles
                var ti = 0;
                for (int y = 0; y < gridHeight; y++)
                {
                    for (int x = 0; x < gridWidth; x++)
                    {
                        var xi0 = y * (gridWidth + 1) + x;
                        var xi1 = (y + 1) * (gridWidth + 1) + x;
                        Utils.DuplicateVerts(grid, verts, ti * 4, xi0 + 1, xi0, xi1, xi1 + 1);
                        Utils.AddFace(tris, ti * 6, ti * 4, 4);
                        ti++;
                    }
                }

                if (tile)
                {
                    var tileUV = new Vector2[]
                    {
                    Vector2.right,
                    Vector2.zero,
                    Vector2.up,
                    new Vector2(1,1)
                    };
                    uv.Fill(tileUV, c => c);
                }
                else
                {
                    uv.Fill(verts, v => new Vector2(v.x, v.z));
                }

            }

            // fill new mesh object
            var mesh = new Mesh();

            mesh.vertices = verts;
            mesh.triangles = tris;
            mesh.uv = uv;

            return mesh;
        }

    }
}
