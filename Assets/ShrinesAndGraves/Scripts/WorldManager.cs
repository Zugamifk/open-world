using UnityEngine;
using System.Collections;

namespace Shrines
{
    public class WorldManager : MonoBehaviour
    {
        public World world;
        public GridView view;

        static WorldManager s_instance;

        void Awake()
        {
            if (!this.SetInstanceOrKill(ref s_instance))
            {
                return;
            }
            view.grid = world.grid;
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}