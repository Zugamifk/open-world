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
                child.SetParent(transform.parent, true);
                child.GetComponent<Rigidbody2D>().AddForce(Quaternion.AngleAxis(Random.Range(-45, 45), Vector3.forward) * Randomx.OnXYCircle * Random.value * power);
            }
        }

        void LateUpdate()
        {
            Destroy(gameObject);
        }
    }
}