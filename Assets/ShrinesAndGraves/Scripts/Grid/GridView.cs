using UnityEngine;
using UnityEngine.Assertions;
using System.Collections;
using System.Collections.Generic;
using Extensions;

namespace Shrines
{
    public class GridView : MonoBehaviour
    {

        [SerializeField]
        Camera viewCamera;
        [SerializeField]
        Transform root;
        [SerializeField]
        int objectPoolSize;

        GameObject[] objectPool;

        Dictionary<string, WorldObject> activeObjects;

        public Grid grid;

        TileObject[,] currentTiles, buffer;

        int width, height;

        Vector2f16 offset;
        Vector2f16 position;
        Vector2i bottomLeftTile;

        public delegate Vector2 GetPositionCallback();
        GetPositionCallback positionUpdate;

        public void SetPositionCallback(GetPositionCallback cb)
        {
            positionUpdate = cb;
        }

        void Awake()
        {
            objectPool = new GameObject[objectPoolSize];
            activeObjects = new Dictionary<string, WorldObject>();
            for (int i = 0; i < objectPoolSize; i++)
            {
                var o =  new GameObject("object");
                o.transform.SetParent(root, false);
                objectPool[i] = o;
            }
        }

        // Use this for initialization
        void Start()
        {
            height = Mathf.FloorToInt(viewCamera.orthographicSize*2) + 2;
            width = (int)(viewCamera.orthographicSize * viewCamera.aspect * 2) + 2;

            offset = new Vector2(viewCamera.orthographicSize * viewCamera.aspect, viewCamera.orthographicSize);

            position = Vector2.zero;
            bottomLeftTile = position - offset;

            currentTiles = new TileObject[width, height];
            buffer = new TileObject[width, height];

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    var tile = new GameObject("Tile");
                    var to = tile.AddComponent<TileObject>();
                    var te = grid.GetTile(x,  y);
                    to.InitializeGameobject(te);
                    tile.transform.SetParent(root, false);
                    var pos = new Vector3i(x, y, 0);
                    tile.transform.localPosition = pos;
                    currentTiles[x,y] = to;
                }
            }
            //Debug.Log(width+" : "+height);
        }

        // Update is called once per frame
        void Update()
        {
            if (positionUpdate != null)
            {
                // get new position
                var newpos = BoundPosition(positionUpdate.Invoke());
                var step = GetStep(position, newpos);
                offset = new Vector2f16(viewCamera.orthographicSize * viewCamera.aspect, viewCamera.orthographicSize);
                position = newpos;
                bottomLeftTile += step;

                if (step.x+step.y != 0) // shift tiles
                {
                    for (int x = 0; x < width; x++)
                    {
                        for (int y = 0; y < height; y++)
                        {
                            var tile = currentTiles[x, y];

                            tile.SetTile(grid.GetTile(bottomLeftTile.x + x, bottomLeftTile.y + y));
                            
                        }
                    }
                }

                root.localPosition = (Vector2f16)bottomLeftTile - position;
            }
            foreach (var b in Physics.registeredBodies)
            {
                b.DebugDraw(position);
            }
        }


        TileObject GetTile(int x, int y)
        {
            return currentTiles[x, y];
        }

        /// <summary>
        /// Get the step direction to take between tiles, given two positions
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns>Direction of step to take</returns>
        Vector2i GetStep(Vector2 start, Vector2 end)
        {
            return new Vector2i(
                (int)end.x - (int)start.x,
                (int)end.y - (int)start.y
            );
        }

        /// <summary>
        /// Bound a position to within grid bounds
        /// </summary>
        /// <param name="pos">position to bound</param>
        /// <returns>bounded position</returns>
        Vector2 BoundPosition(Vector2 pos)
        {
            return new Vector2(
                Mathf.Clamp(pos.x, 0, grid.width),
                Mathf.Clamp(pos.y, 0, grid.height)
            );
        }
    }
}