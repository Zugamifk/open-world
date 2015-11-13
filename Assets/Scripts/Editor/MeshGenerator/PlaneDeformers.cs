using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEditor;
using System.Reflection;

namespace MeshGenerator
{
    public partial class PlaneDeformers
    {
		public List<PlaneDeformer> deformationSteps = new List<PlaneDeformer>();

		public void Deform(ref Vector3[] verts) {
			verts = verts.Select(v => deformationSteps.Aggregate(v, (dv, d) => d.Deform(dv))).ToArray();
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

        public class SphericalScalar : DeformerFunction {
            [CustomEditorField] private AnimationCurve fallOff = AnimationCurve.Linear(0,0,1,0);
            [CustomEditorField] private Transform centre;

            public override Vector3 Deform(Vector3 point) {
                point.y *= fallOff.Evaluate((centre.position - point).magnitude);
                Debug.Log(point);

                return point;
            }
        }

    }
}
