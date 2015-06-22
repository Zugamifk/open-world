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

            yield break;
        }
    }
}
