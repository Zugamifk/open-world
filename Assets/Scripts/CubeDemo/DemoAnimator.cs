using UnityEngine;
using System.Collections;

namespace Demos.Cube
{
    public class DemoAnimator : MonoBehaviour
    {

        // Use this for initialization
        void Start()
        {
            StartCoroutine(Animation());
        }


        private IEnumerator Animation() {
            yield return new WaitForSeconds(1);

            var curve = Interpolation.Power(2);
            var startCol = CameraSingleton.Camera.backgroundColor;
            yield return StartCoroutine(curve.AnimateValue(3,
                t => CameraSingleton.Camera.backgroundColor = Color.Lerp(startCol, Colors.greybeige, t)
            ));

            StartCoroutine(Light());

            var cubeGO = new GameObject("The Cube");
            cubeGO.transform.OrientTo(transform);
            var cube = cubeGO.AddComponent<Cube>();
            yield return StartCoroutine(cube.AnimateAppear());

            yield break;
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
                yield return 1;
            }
        }
    }
}
