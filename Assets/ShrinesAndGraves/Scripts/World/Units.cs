using UnityEngine;
using System.Collections;

namespace Shrines
{
    public static class Units
    {
        public const float UnitScale = 0.6f;

        public static float UnitsToMetres(float units)
        {
            return units * UnitScale;
        }

        public static float MetresToUnits(float metres)
        {
            return metres / UnitScale;
        }

    }
}