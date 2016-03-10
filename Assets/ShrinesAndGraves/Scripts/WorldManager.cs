using UnityEngine;
using System.Collections;

namespace Shrines
{
    public class WorldManager : MonoBehaviour
    {
        public World world;
        public GridView view;
        public PlayerObject player;

        static WorldManager s_instance;

        void Awake()
        {
            if (!this.SetInstanceOrKill(ref s_instance))
            {
                return;
            }
            player.InitializeGameobject(new Player());
            
            view.grid = world.grid;
            Physics.grid = world.grid;

            view.SetPositionCallback(() => player.position);

        }

        void Start()
        {
            player.SetPosition(world.spawnPosition);

            Tile t = world.grid.GetTile(player.position);
            while (t.collides)
            {
                t = world.grid.GetTile(t.gridPosition + Vector3i.up);
            }
            for (int i = 0; i < 5; i++)
            {
                t = world.grid.GetTile(t.gridPosition + Vector3i.up);
            }
            player.SetPosition(t.gridPosition);
        }

        void FixedUpdate()
        {
            Physics.Update();
        }
    }
}