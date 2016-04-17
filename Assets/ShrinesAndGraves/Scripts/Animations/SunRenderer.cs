using UnityEngine;
using System.Collections;

namespace Shrines
{
    public class SunRenderer : MonoBehaviour
    {

        [System.Serializable]
        public struct Sunrays
        {
            public SpriteRenderer sprite;
            public Gradient gradient;

            public void SetTime(float t)
            {
                sprite.color = gradient.Evaluate(t);
            }
        }

        public SpriteRenderer core;
        public Sunrays rays1;
        public Sunrays rays2;
        public Sunrays rays3;

        Material rays1Mat, rays2Mat, rays3Mat;

        void Awake()
        {
            rays1Mat = rays1.sprite.material;
            rays2Mat = rays2.sprite.material;
            rays3Mat = rays3.sprite.material;
            rays1Mat.SetFloat("_Phase", Random.value * 2 - 1);
            rays1Mat.SetFloat("_Resolution", 48);
            rays2Mat.SetFloat("_Phase", Random.value * 2 - 1);
            rays2Mat.SetFloat("_Resolution", 64);
            rays3Mat.SetFloat("_Phase", Random.value * 2 - 1);
            rays3Mat.SetFloat("_Resolution", 96);

            rays1.SetTime(0);
            rays2.SetTime(0);
            rays3.SetTime(0);
        }

        public void UpdateTime(float time)
        {
            rays1.SetTime(time);
            rays2.SetTime(time);
            rays3.SetTime(time);
        }
    }
}