using UnityEngine;
using System.Collections;

namespace Shrines
{
    public class WorldManager : MonoBehaviour
    {

        public WorldData worldData;
        public GridView view;
        public PlayerObject player;

        World world;
        static WorldManager s_instance;

        void Awake()
        {
            if (!this.SetInstanceOrKill(ref s_instance))
            {
                return;
            }

            world = new World(worldData);

            SaveData.Load("game");
            if (!SaveData.IsFileLoaded)
            {
                SaveData.Initialize("game");
                world.grid = Grid.PerlinNoise(worldData.width, worldData.height, worldData.types);
            }
            else
            {
                world.grid = SaveData.file.grid;
            }

            if (worldData != null)
            {
                worldData.environment.FillGrid(world.grid);
            }

            player.InitializeGameobject(new Player());

            view.grid = world.grid;
            Physics.grid = world.grid;

            view.SetPositionCallback(() => player.position);

        }

        void Start()
        {
            player.SetPosition(worldData.spawnPosition);

            Tile t = world.grid.GetTile(player.position);
            while (t.collides)
            {
                t = world.grid.GetTile(t.gridPosition + Vector2i.up);
            }
            for (int i = 0; i < 5; i++)
            {
                t = world.grid.GetTile(t.gridPosition + Vector2i.up);
            }
            player.SetPosition(t.gridPosition);
        }

        void OnApplicationQuit()
        {
            SaveData.Save();
        }

    }
}