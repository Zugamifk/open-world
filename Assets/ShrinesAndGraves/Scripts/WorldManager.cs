using UnityEngine;
using System.Collections;
using Extensions.Managers;
using Animations;

namespace Shrines
{
    public class WorldManager : MonoBehaviour
    {

        public WorldData worldData;
        public GridView[] views;
        public PlayerObject player;
        public LoadingBar loadingBar;
        public CameraController cameraController;
        public float restartDelay;

        bool initialized;
        World world;
        static WorldManager s_instance;

        public static void Restart()
        {
            s_instance._Restart();
        }

        void _Restart() {
            player.gameObject.SetActive(false);
            foreach (var view in views)
            {
                view.gameObject.SetActive(false);
            }

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
            };
        }

        public static void DelayedRestart()
        {
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