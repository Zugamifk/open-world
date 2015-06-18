using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEditor;
using System.Reflection;

namespace MeshGenerator
{
    public class Plane : IMeshGenerator
    {

        public bool smooth = true;
        public bool tile = false;
        public int gridWidth = 1;
        public int gridHeight = 1;
        public List<PlaneDeformer> deformationSteps = new List<PlaneDeformer>();

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

            // Apply deformations
            verts = verts.ToEach(v => v -= new Vector3(0.5f, 0, 0.5f)); // recentre
            verts = verts.Select(v => deformationSteps.Aggregate(v, (dv, d) => d.Deform(dv))).ToArray();

            // fill new mesh object
            var mesh = new Mesh();

            mesh.vertices = verts;
            mesh.triangles = tris;
            mesh.uv = uv;

            return mesh;
        }

    }

    // A class for drawing and abstracting a plane deformation step
    public class PlaneDeformer : NullEditorDrawer {

        // current function selected
        DeformerFunction current;

        // Field drawer for a reorderable list
        public override EditorGUIx.FieldDrawer GetFieldDrawer(string label, IEditorDrawer value, Action<IEditorDrawer> setCallback) {
            // Cast type
            var deformer = value as PlaneDeformer;

            // Get a collection of draw functions for the fields
            List<EditorGUIx.FieldDrawer> drawers;

            // Local function for changing the deformer
            Action<Type> newFunction = type =>
            {
                // Instantiate a new object and get its draw functions
                var constructor = type.GetConstructor(Type.EmptyTypes);
                current = constructor.Invoke(null) as DeformerFunction;
                drawers = current.GetFieldDrawers().ToList();
            };

            // Instantiate an empty deformer
            if(current == null) newFunction(typeof(NoiseDeformer));
            drawers = current.GetFieldDrawers().ToList();

            // Get drawer for choosing a new deformer
            var deformChooser = EditorGUIx.GetFieldDrawer(
                typeof(DeformerFunction),
                newFunction);

            // return drawer
            return (Rect position) => {
                // Get a rect for the label
                var labelRect = position;
                labelRect.width = 25;
                GUI.Label(labelRect, label);

                // Get a rect for the fields
                var fieldsRect = position;
                fieldsRect.x = 25;
                fieldsRect.width = position.width-100;
                var fieldsubrect = fieldsRect;
                fieldsubrect.width /= drawers.Count();
                var fieldssubrectstep = fieldsubrect.width;
                drawers.ForEach(d => {
                    d(fieldsubrect);
                    fieldsubrect.x += fieldssubrectstep;
                });

                var chooserRect = position;
                chooserRect.x = position.width - 75;
                chooserRect.width = 75;
                deformChooser(chooserRect);

                if (current.dirty)
                {
                    setCallback(deformer);
                    current.dirty = false;
                }
            };
        }

        public Vector3 Deform(Vector3 point) {
            return current.Deform(point);
        }

    }

    // An object representing a plane deformation step
    public class DeformerFunction {
        public bool dirty = false;
        public virtual IEnumerable<EditorGUIx.FieldDrawer> GetFieldDrawers()
        {
            var bindingflags = BindingFlags.FlattenHierarchy | BindingFlags.Instance |
                BindingFlags.Public | BindingFlags.NonPublic;
            return this.GetType().GetFields(bindingflags)
                .Where( info =>
                    info.GetCustomAttributes(typeof(CustomEditorField), true).Length > 0
                )
                .Select( field =>
                    EditorGUIx.GetFieldDrawer(
                        field.FieldType,
                        field.Name.Uncamel() + ":",
                        () => field.GetValue(this),
                        v =>{
                            field.SetValue(this, v);
                            dirty = true;
                        }
                    )
                );
        }

        public virtual Vector3 Deform(Vector3 point) {
            return point;
        }
    }

    // Random noise
    public class NoiseDeformer : DeformerFunction {
        [CustomEditorField] private float magnitude = 1;

        public override Vector3 Deform(Vector3 point) {
            point.y += UnityEngine.Random.value * magnitude;
            return point;
        }
    }

    // Perlin noise
    public class PerlinDeformer : DeformerFunction {
        [CustomEditorField] private float magnitude = 1;
        [CustomEditorField] private float scale = 1;

        public override Vector3 Deform(Vector3 point) {
            point.y += Mathf.PerlinNoise(point.x*scale, point.z*scale)*magnitude;
            return point;
        }
    }

}
