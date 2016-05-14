using UnityEngine;
using System.Collections;

namespace Shrines
{
    public class Gibs : MonoBehaviour
    {
        public float power;

        [System.NonSerialized]
        public Vector2 normal;

        void Start()
        {
            int I = transform.childCount;
            for (int i = I; i > 0; i--)
            {
                var child = transform.GetChild(i-1);
                var rb = child.GetComponent<Rigidbody2D>();
                var p = Random.value * power;
                rb.AddForce(Randomx.OnXYCircle * p);
                rb.AddTorque(p);
            }
        }
    }
}