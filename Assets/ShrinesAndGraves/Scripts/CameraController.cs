using UnityEngine;
using System.Collections;

namespace Shrines
{
    public class CameraController : MonoBehaviour
    {
        public Transform followRoot;
        public float maxDistance;
        public AnimationCurve distanceSpeedGain;
        public float minFollowSpeed;
        public float maxFollowSpeed;
        public Camera camera;
        public float cameraScale;

        // Update is called once per frame
        void Update()
        {
            camera.orthographicSize = cameraScale * 0.5f * Screen.height / 16f;

            if (!followRoot.gameObject.activeInHierarchy) return;
            var to = followRoot.position - transform.position;
            var dir = to.normalized;
            var distance = to.magnitude;
            var speed = Mathf.Lerp(minFollowSpeed, maxFollowSpeed, distanceSpeedGain.Evaluate(distance/maxDistance));
            transform.Translate(dir * Time.deltaTime * speed);
        }
    }
}