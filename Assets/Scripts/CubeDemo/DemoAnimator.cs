using UnityEngine;
using System.Collections;
using System.Linq;

namespace Demos.Cube
{
    public class DemoAnimator : MonoBehaviour
    {

        // Use this for initialization
        void Start()
        {
            StartCoroutine(Animation());
            var sy = Group.Symmetric(4);
            foreach(int[] p in sy.Closure) {
                Debug.Log(p.Aggregate("", (str, i)=>str+i.ToString()) );
            }
        }

        private ColorHSV BG = (ColorHSV)Color.black;

        private IEnumerator Animation() {
            yield return new WaitForSeconds(1);

            var curve = Interpolation.Power(2);
            var startCol = CameraSingleton.Camera.backgroundColor;
            yield return StartCoroutine(curve.AnimateValue(3,
                t => CameraSingleton.Camera.backgroundColor = Color.Lerp(startCol, Colors.greybeige, t)
            ));

            StartCoroutine(ShiftBG());
            StartCoroutine(Light());

            var cubeGO = new GameObject("The Cube");
            cubeGO.transform.OrientTo(transform);
            var cube = cubeGO.AddComponent<Cube>();
            cube.ScaleObserver += t => BG.s = t;
            cube.AngleObserver += t => BG.h = (t % 180) / 180;
            yield return StartCoroutine(cube.AnimateAppear());

            yield break;
        }

        private IEnumerator ShiftBG() {
            var curve = Interpolation.Smooth(2);
            while (true)
            {
                var h0 = ((ColorHSV)(CameraSingleton.Camera.backgroundColor)).h;
                var h1 = BG.h;
                yield return StartCoroutine(curve.AnimateValue(3,
                    t => CameraSingleton.Camera.backgroundColor = new ColorHSV(Mathf.Lerp(h0, h1, t), BG.s, 1)
                ));
            }
        }

        private IEnumerator Light() {
            var lightGO = new GameObject("Spotlight");
            lightGO.transform.OrientTo(transform);
            lightGO.transform.localPosition = new Vector3(5, 5, 0);
            var light = lightGO.AddComponent<Light>();
            light.color = Colors.darkgold;
            light.type = LightType.Directional;

            while (true)
            {
                light.transform.LookAt(transform.position);
                light.transform.localPosition =
                    Quaternion.AngleAxis(180 * Time.deltaTime, Vector3.up) *
                    light.transform.localPosition;
                light.color = CameraSingleton.Camera.backgroundColor.Complement();
                yield return 1;
            }
        }
    }
}
