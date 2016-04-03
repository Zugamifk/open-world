using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Shrines
{
    public class World
    {
        public WorldData data;
        public Grid grid;

        List<Entity> entities = new List<Entity>();

        public World(WorldData data)
        {
            this.data = data;

            for (int i = 0; i < 1000; i++)
            {
                var cloud = new Entity();
                //grid.AddEntity()
            }
        }
    }
}