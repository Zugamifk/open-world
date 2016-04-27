using UnityEngine;
using System.Collections;

namespace Shrines
{
    public class WorldManager : MonoBehaviour
    {

        public WorldData worldData;
        public GridView[] views;
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
                var er = worldData.emptyRegion;
                world.grid = new Grid(er.rect.width, er.rect.height);
                er.Fill(world.grid);
                for (int i = 0; i < worldData.regions.Length; i++)
                {
                    worldData.regions[i].Fill(world.grid);
                }
            }
            else
            {
                world.grid = SaveData.file.grid;
            }

            world.grid.UpdateSurfaces(worldData.emptyRegion.rect);

            UnityEngine.Physics.gravity = Vector3.up * Units.MetresToUnits(-9.81f);

            player.InitializeGameobject(new Player());

            Physics.grid = world.grid;

            foreach (var view in views)
            {
                view.grid = world.grid;
                view.SetPositionCallback(() => player.position);
            }
        }

        void Start()
        {
            player.SetPosition(worldData.spawnPosition);

            Tile t = world.grid.surface[(int)player.position.x];
            
            player.SetPosition(t.gridPosition);
        }

        void OnApplicationQuit()
        {
            SaveData.Save();
        }

    }
}