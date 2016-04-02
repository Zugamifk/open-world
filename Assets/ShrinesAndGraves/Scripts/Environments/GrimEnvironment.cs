using UnityEngine;
using System.Collections;

namespace Shrines
{
    public class GrimEnvironment : Environment
    {
        public override void FillGrid(Grid grid)
        {
            for (int i = 0; i < 100; i++)
            {
                var cloud = new Entity(
                    GetWorldObjetData("cloud"), 
                    new Vector2f16(Random.value * grid.width, Random.RandomRange(50, 75))
                );
                grid.AddEntity(cloud);
            }
        }
    }
}