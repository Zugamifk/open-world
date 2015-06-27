using UnityEngine;
using System.Collections;
using Demos.Cube;
using MeshGenerator;

namespace Demos.Cube
{
    public class Cube : MonoBehaviour
    {
		IMeshGenerator Generator;
        Mesh mesh;

        void Start() {
            Generator = new MeshGenerator.Cube();
            var mf = gameObject.AddComponent<MeshFilter>();
            var mr = gameObject.AddComponent<MeshRenderer>();
            mesh = Generator.Generate();
			Utils.PostGenerateMesh(mesh);
			mf.mesh = mesh;
            mr.material = new Material(Shader.Find("Demo/Cube/Megashader"));
        }
		public IEnumerator AnimateAppear() {
            var popI = Interpolation.Power(2);
            var appears = popI.AnimateValue(
                0.5f,
				t => transform.localScale = Vector3.one * t
                );
            yield return StartCoroutine(appears);
        }

    }
}
