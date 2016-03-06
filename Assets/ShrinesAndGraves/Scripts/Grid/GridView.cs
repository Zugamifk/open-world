using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    var tile = new GameObject("Tile");
                    var to = tile.AddComponent<TileObject>();
                    to.tile = grid[new Vector3i(leastTile.y + x, leastTile.y+y, 0)];
                    tile.transform.SetParent(root, false);
                    tile.transform.localPosition = new Vector3(x, y, 0);
                    to.InitializeGameobject();
                    to.SetTile(to.tile);
                }
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (positionUpdate != null)
            {
                var newPos = positionUpdate.Invoke();
                var dif = newPos - position;

                int xs = Mathf.FloorToInt(newPos.x) - leastTile.x;
                int ys = Mathf.FloorToInt(newPos.y) - leastTile.y;
                if (ys > 0)
                {
                    for (int x = 0; x < width; x++)
                    {
                        for (int y = 0; y < ys; y++)
                        {
                            var op = new Vector3i(leastTile.x + x, leastTile.y + y, 0);
                            UpdateTile(currentTiles[op], op + height * Vector3i.up);
                        }
                    }
                }
                else if (ys < 0)
                {
                    for (int x = 0; x < width; x++)
                    {
                        for (int y = 0; y > ys; y--)
                        {
                            var op = new Vector3i(leastTile.x + x, leastTile.y + y + height - 1, 0);
                            UpdateTile(currentTiles[op], op - height * Vector3i.up);
                        }
                    }
                }

                if (xs > 0)
                {
                    for (int x = 0; x < xs; x++)
                    {
                        for (int y = ys; y < height; y++)
                        {
                            var op = new Vector3i(leastTile.x + x, leastTile.y + y, 0);
                            UpdateTile(currentTiles[op], op + width * Vector3i.right);
                        }
                    }
                }
                else if (xs < 0)
                {
                    for (int x = 0; x > xs; x--)
                    {
                        for (int y = ys; y > height; y++)
                        {
                            var op = new Vector3i(leastTile.x + x + width - 1, leastTile.y + y, 0);
                            UpdateTile(currentTiles[op], op - width * Vector3i.right);
                        }
                    }
                }

                position = newPos;

                leastTile += new Vector3i(xs, ys, 0);
            }
        }

        void UpdateTile(TileObject tile, Vector3i position)
        {
            tile.transform.localPosition = new Vector3(position.x, position.y, 0);
            tile.SetTile(grid[new Vector3i(position.x, position.y, 0)]);
        }
    }
}