using UnityEngine;
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

        public Grid grid;

        Dictionary<Vector3i, TileObject> currentTiles;

        int width, height;

        Vector2 offset;
        Vector2 position;
        Vector3i leastTile;

        public delegate Vector2 GetPositionCallback();
        GetPositionCallback positionUpdate;

        public void SetPositionCallback(GetPositionCallback cb)
        {
            positionUpdate = cb;
        }

        // Use this for initialization
        void Start()
        {
            height = Mathf.FloorToInt(viewCamera.orthographicSize*2) + 1;
            width = (int)(viewCamera.orthographicSize * viewCamera.aspect * 2) + 1;

            position = Vector2.zero;
            leastTile = Vector3i.zero;

            currentTiles = new Dictionary<Vector3i, TileObject>();

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    var tile = new GameObject("Tile");
                    var to = tile.AddComponent<TileObject>();
                    to.tile = grid[new Vector3i(leastTile.x + x, leastTile.y+y, 0)];
                    tile.transform.SetParent(root, false);
                    var pos = new Vector3i(x, y, 0);
                    tile.transform.localPosition = pos;
                    to.InitializeGameobject();
                    to.SetTile(to.tile);
                    currentTiles[pos] = to;
                }
            }
            Debug.Log(width+" : "+height);
        }

        // Update is called once per frame
        bool fff = false;
        void Update()
        {
            if (positionUpdate != null)
            {
                var newPos = BoundPosition(positionUpdate.Invoke());
                Debug.Log("Tiles: "+currentTiles.Count);

                int xs = Math.RoundUpMagnitude(newPos.x) - leastTile.x;
                int ys = Math.RoundUpMagnitude(newPos.y) - leastTile.y;
                var newLeast = new Vector3i(leastTile.x+xs, leastTile.y+ys,0);

                int ox = 0, oy = 0;
                int nx = 0, ny = 0;
                int ystep = 0, xstep = 0;
                if (!fff && ys * xs != 0)
                {
                    Debug.Log("fff");
                    fff = true;
                }
                if (ys > 0)
                {
                    ystep = Mathf.Min(ys, height);
                    ny = height-ystep;
                }
                else if (ys < 0)
                {
                    ystep = -Mathf.Max(ys, -height);
                    oy = height - ystep;
                }

                if (xs > 0)
                {
                    xstep = Mathf.Min(xs, width);
                    nx = width - xstep;
                }
                else if (xs < 0)
                {
                    xstep = -Mathf.Max(xs, -width);
                    ox = width - xstep;
                }


                for (int x = 0; x < width; x++)
                {
                    for (int y = 0; y < ystep; y++)
                    {
                        var op = new Vector3i(leastTile.x + x, leastTile.y + oy + y, 0);
                        TileObject o;
                        if (!currentTiles.TryGetValue(op, out o))
                        {
                            Debug.Log(op + " : " + leastTile + " : " + xs + " : " + ys);
                            return;
                        }
                        var np = new Vector3i(newLeast.x + x, newLeast.y + ny + y, 0);
                        UpdateTile(currentTiles[op], np);
                    }
                }

                for (int x = 0; x < xstep; x++)
                {
                    for (int y = ystep; y < height; y++)
                    {
                        var op = new Vector3i(leastTile.x + ox + x, leastTile.y + y, 0);
                        var np = new Vector3i(newLeast.x + nx + x, leastTile.y + y, 0);
                        TileObject o;
                        if (!currentTiles.TryGetValue(op, out o))
                        {
                            Debug.Log(op + " : " + leastTile + " : " + xs + " : " + ys);
                            return;
                        }
                        UpdateTile(currentTiles[op], np);
                    }
                }

                position = newPos;
                offset = position - (Vector2)newLeast;
                root.localPosition = -offset;
                leastTile = newLeast;

                RefreshTiles();

            }
        }

        void RefreshTiles()
        {
            foreach (var tile in currentTiles.Values)
            {
                tile.transform.localPosition = tile.tile.position - leastTile;
            }
        }

        void UpdateTile(TileObject tile, Vector3i position)
        {
            if (!currentTiles.Remove(tile.tile.position))
            {
                Debug.Log("Missed " + position);
            }
            tile.SetTile(grid[position]);
            if (currentTiles.ContainsKey(position))
            {
                Debug.Log("Writing over " + position);
            }
            currentTiles[position] = tile;
        }

        Vector2 BoundPosition(Vector2 pos)
        {
            return new Vector2(
                Mathf.Clamp(pos.x, 0, grid.width-width),
                Mathf.Clamp(pos.y, 0, grid.height-height)
            );
        }
    }
}