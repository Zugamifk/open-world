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

        public delegate void Observer(float t);
        public Observer ScaleObserver;
        public Observer AngleObserver;

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
            StartCoroutine(appears);

            Quaternion rot = transform.localRotation;
            Quaternion to = Random.rotation;
            var spinI = Interpolation.Smooth(14);
            var bounceI = new Interpolation(t => (1 - t) * (1 - t) * t * 27 / 4);
            //DebugDrawer.DrawCalls += spinI.Draw;
            while(true) {
                var mag = Random.value*3;
                var bounce = bounceI.AnimateValue(
                    1,
                    t =>
                    {
                        transform.localScale = Vector3.one * (1 + t * mag);
                        if (ScaleObserver != null) ScaleObserver(t * mag/3);
                    }
                );
                StartCoroutine(bounce);
                var spin = spinI.AnimateValue(
                    1,
                    t =>
                    {
                        transform.localRotation = Quaternion.Slerp(rot, to, t);
                        if(AngleObserver!=null) AngleObserver(Quaternion.Angle(transform.localRotation, Quaternion.identity));
                    }
                );
                yield return StartCoroutine(spin);
                rot = to;
                to = Random.rotation;
            }
        }

    }
}
