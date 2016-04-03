﻿using UnityEngine;
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