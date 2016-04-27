using UnityEngine;
using System.Collections;

namespace Shrines
{
    public class Hazard : Entity
    {
        HazardData hazardData;

        public Hazard(HazardData d, Vector2f16 pos) : base(d, pos, true)
        {
            hazardData = d;
        }

        public override void OnTriggerEnter(WorldObject wo)
        {
            Debug.Log("HAZARD!");
        }

    }
}