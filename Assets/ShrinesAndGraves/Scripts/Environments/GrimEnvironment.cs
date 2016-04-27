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
                var x = Random.value * grid.width;
                var y = grid.surface[(int)x].gridPosition.y;
                var cloud = new Entity(
                    GetWorldObjetData("cloud"), 
                    new Vector2f16(x, Random.RandomRange(x, Random.Range(y+25, y+75)))
                );
                grid.AddEntity(cloud);
            }
        }
    }
}