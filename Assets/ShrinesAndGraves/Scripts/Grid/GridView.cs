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
        GameObject target;
        [SerializeField]
        Camera viewCamera;
        [SerializeField]
        int bufferSize;
        [SerializeField]
        Transform root;
        [SerializeField]
        int objectPoolSize;
        [SerializeField, Layer]
        int layer;
        [SerializeField, Tooltip("Sorting oder in layer")]
        int sortingOrder;
        [SerializeField, Tooltip("should this view initialize colliders and other components?")]
        bool renderersOnly;

        Queue<WorldObject> objectPool;
        Queue<TileObject> tilePool;

        Dictionary<Entity, WorldObject> activeEntities;

        public Grid grid;

        TileObject[,] currentTiles, buffer;

        int width, height;

        Vector2f16 offset;
        Vector2f16 position;
        Vector2i bottomLeftTile;

        Transform poolRoot;

        public delegate Vector2 GetPositionCallback();
        GetPositionCallback positionUpdate;

        public void SetPositionCallback(GetPositionCallback cb)
        {
            positionUpdate = cb;
        }

        void Awake()
        {
            activeEntities = new Dictionary<Entity, WorldObject>();

            var pool = new GameObject("Object Pool");
            pool.transform.SetParent(transform, false);
            poolRoot = pool.transform;
            objectPool = new Queue<WorldObject>(objectPoolSize);
            for (int i = 0; i < objectPoolSize; i++)
            {
                var o =  new GameObject("object");
                o.transform.SetParent(poolRoot, false);
                var wo = o.AddComponent<WorldObject>();
                objectPool.Enqueue(wo);
            }

            tilePool = new Queue<TileObject>();

            transform.SetParent(target.transform, false);
        }

        // Use this for initialization
        void Start()
        {
            height = Mathf.FloorToInt(viewCamera.orthographicSize*2) + bufferSize*2;
            width = (int)(viewCamera.orthographicSize * viewCamera.aspect * 2) + bufferSize * 2;

            offset = new Vector2(viewCamera.orthographicSize * viewCamera.aspect+bufferSize, viewCamera.orthographicSize+bufferSize);

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
                    if (renderersOnly)
                    {
                        to.InitializeRenderers(te);
                    }
                    else
                    {
                        to.InitializeGameobject(te);
                    }
                    to.renderer.sortingLayerID = layer;
                    to.renderer.sortingOrder = sortingOrder;
                    tile.transform.SetParent(root, false);
                    var pos = new Vector3i(x, y, 0);
                    tile.transform.localPosition = pos;
                    currentTiles[x,y] = to;
                    tilePool.Enqueue(to);
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
                offset = new Vector2(viewCamera.orthographicSize * viewCamera.aspect + bufferSize, viewCamera.orthographicSize + bufferSize);
                position = newpos;

                UpdateTiles();
                
            }
        }

        void UpdateTiles()
        {
            var obl = bottomLeftTile;
            bottomLeftTile = position - offset;

            if (obl == bottomLeftTile) return;

            int n = 0;

            // corner rect of tiles
            for (int x = obl.x; x < obl.x+width; x++)
            {
                for (int y = obl.y; y < obl.y+height; y++)
                {
                    if (x < bottomLeftTile.x ||
                        x >= bottomLeftTile.x + width ||
                        y < bottomLeftTile.y ||
                        y >= bottomLeftTile.y + height)
                    {
                        n++;
                        var tile = grid.GetTile(x, y);
                        ResetTile(tile);
                    }
                }
            }

            int m = 0;
            for (int x = bottomLeftTile.x; x < bottomLeftTile.x + width; x++)
            {
                for (int y = bottomLeftTile.y; y < bottomLeftTile.y + height; y++)
                {
                    if (x < obl.x ||
                        x >= obl.x + width ||
                        y < obl.y ||
                        y >= obl.y + height)
                    {
                        m++;
                        var tile = grid.GetTile(x, y);
                        SetTile(tile);
                    }
                }
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

        void ResetTile(Tile tile)
        {
            WorldObject to;
            if (!activeEntities.TryGetValue(tile, out to))
            {
                //Debug.Log("No entity found for tile at " + tile.position);
                return;
            }
            activeEntities.Remove(tile);
            ReturnToPool(to);
            foreach (var e in tile.contained)
            {
                WorldObject wo;
                if (activeEntities.TryGetValue(e, out wo))
                {
                    ReturnToPool(wo);
                    wo.ResetGameobject();
                    activeEntities.Remove(e);
                    wo.transform.SetParent(poolRoot, false);
                }
            }
        }

        void SetTile(Tile tile)
        {
            var tileObject = tilePool.Dequeue();
            tileObject.SetTile(tile);

            activeEntities.Add(tile, tileObject);

            foreach (var e in tile.contained)
            {
                if (activeEntities.ContainsKey(e) || 
                    (e.data != null && e.data.sortinglayer != layer)
                ) continue;

                var wo = objectPool.Dequeue();
                wo.InitializeGameobject(e);
                wo.transform.SetParent(root, true);
                activeEntities.Add(e, wo);
            }
        }

        void ReturnToPool(WorldObject wo)
        {
            if (wo is TileObject)
            {
                tilePool.Enqueue(wo as TileObject);
            }
            else
            {
                objectPool.Enqueue(wo);
            }
        }
    }
}