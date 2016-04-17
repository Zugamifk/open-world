using UnityEngine;
using System.Collections;

namespace Shrines
{
    public class BackgroundParallax : MonoBehaviour
    {

        [System.Serializable]
        public struct Layer
        {
            public Transform root;
            public float MinX;
            public float MaxX;
            public float distance;
            public void SetX(float x)
            {
                var pos = root.localPosition;
                pos.x = Mathf.LerpUnclamped(MinX, MaxX, x);
                root.localPosition = (Vector2f16)pos;
            }
        }

        [SerializeField]
        public Layer[] layers;
        public Transform perspectiveRoot;
        public float minX;
        public float maxX;

        public void Update()
        {
            var x = perspectiveRoot.localPosition.x;
            var t = Mathf.InverseLerp(minX, maxX, x);
            for (int i = 0; i < layers.Length; i++)
            {
                layers[i].SetX(t);
            }
        }
    }
}