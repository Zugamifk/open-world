using UnityEngine;
using UnityEngine.Events;
using System.Collections;

namespace Shrines
{
    public class TimeManager : MonoBehaviour
    {

        public float MinutesPer24Hours;

        [System.Serializable]
        public class TimeCallback : UnityEvent<float> { }
        public TimeCallback UpdateHour;

        float time;

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            time = Time.time/(60*MinutesPer24Hours);
            if (UpdateHour != null)
            {
                UpdateHour.Invoke(time%1);
            }
        }
    }
}