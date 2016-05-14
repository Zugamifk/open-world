using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Extensions.Managers;
using Animations;

namespace Shrines
{
    public class WorldManager : MonoBehaviour
    {
        public class SelfDestroy : MonoBehaviour {
            void OnBecameInvisible()
            {
                Destroy(gameObject);
            }
        }

        public WorldData worldData;
        public GridView[] views;
        public PlayerObject player;
        public LoadingBar loadingBar;
        public CameraController cameraController;
        public float restartDelay;

        bool restarting;
        bool initialized;
        World world;
        static WorldManager s_instance;

        List<GameObject> spawns;

        public static void Restart()
        {
            s_instance._Restart();
        }

        public static GameObject SpawnObject(GameObject go, Vector3 position, Quaternion rotation)
        {
            var result = (GameObject)Instantiate(go, position, rotation);
            s_instance.spawns.Add(result);
            return result;
        }

        public static GameObject SpawnObject(GameObject go)
        {
            return SpawnObject(go, Vector3.zero, Quaternion.identity);
        }

        void _Restart() {
            restarting = true;
            player.gameObject.SetActive(false);
            foreach (var view in views)
            {
                view.gameObject.SetActive(false);
            }

            foreach (var go in spawns)
            {
                if (go != null)
                {
                    Destroy(go);
                }
            }
            spawns.Clear();

            CrunchManager.AddRoutine(InitializeWorld);
            loadingBar.gameObject.SetActive(true);
            LoadingBar.AddJob("Filling Grid...", 1, () => (initialized ? 1 : 0));
            LoadingBar.OnPostLoad += () =>
            {

                player.gameObject.SetActive(true);
                player.InitializeGameobject(new Player());
                player.SetPosition(worldData.spawnPosition);

                Tile t = world.grid.surface[(int)player.position.x];

                player.SetPosition(t.gridPosition + Vector2i.up * 10);
                cameraController.transform.position = player.position;
                restarting = false;
            };
        }

        public static void DelayedRestart()
        {
            s_instance.restarting = true;
            s_instance.StartCoroutine(s_instance._DelayedRestart());
        }

        IEnumerator _DelayedRestart()
        {
            yield return new WaitForSeconds(restartDelay);
            _Restart();
        }

        void InitializeWorld()
        {
            initialized = false;
            world = new World(worldData);

            var er = worldData.emptyRegion;
            world.grid = new Grid(er.rect.width, er.rect.height);
            er.Fill(world.grid);
            for (int i = 0; i < worldData.regions.Length; i++)
            {
                worldData.regions[i].Fill(world.grid);
            }

            world.grid.UpdateSurfaces(worldData.emptyRegion.rect);

            UnityEngine.Physics.gravity = Vector3.up * Units.MetresToUnits(-9.81f);

            Physics.grid = world.grid;

            foreach (var view in views)
            {
                view.grid = world.grid;
                view.SetPositionCallback(() => player.position);
                view.gameObject.SetActive(true);
            }
            initialized = true;
        }

        void Awake()
        {
            if (!this.SetInstanceOrKill(ref s_instance))
            {
                return;
            }

            spawns = new List<GameObject>();
        }

        void Start()
        {
            _Restart();
        }

        void OnApplicationQuit()
        {
            //SaveData.Save();
        }

    }
}