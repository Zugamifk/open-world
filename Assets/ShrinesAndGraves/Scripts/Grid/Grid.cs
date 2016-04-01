using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Extensions;

namespace Shrines
{
    [System.Serializable]
    public class Grid
    {
        Tile[,] tiles;

        public static Tile[] TileArray = new Tile[1024];

        static bool collideOnExceededWorldBounds = true;

        public int width;
        public int height;

        public Grid(int w, int h)
        {
            width = w;
            height = h;
            tiles = new Tile[w, h];
        }

        public static Grid PerlinNoise(int width, int height, TileData[] types) {
            var grid = new Grid(width, height);
            var noise = Math.FrequencyNoise1D(x => x, x => x * x, x => (1 - x) * (1 - x), 0.1f, 0.25f, 4);
            for (int x = 0; x < width; x++)
            {
                var g = noise(x);
                for (int y = 0; y < height; y++)
                {
                    var tile = new Tile();

                    if (y < 25 + g * 50)
                    {
                        tile.tileData = types[0];
                    }
                    else
                    {
                        tile.tileData = types[1];
                    }
                    tile.gridPosition = new Vector2i(x, y);
                    tile.position = tile.gridPosition;
                    grid.SetTile(x, y, tile);
                }
            }
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    var tile = grid.GetTile(x, y);
                    byte neighboursBits = 0;
                    var t = grid.GetTile(x+1, y);
                    neighboursBits += (byte)(t.collides ? 1 : 0);
                    neighboursBits <<= 1;
                    
                    t = grid.GetTile(x + 1, y-1);
                    neighboursBits += (byte)(t.collides ? 1 : 0);
                    neighboursBits <<= 1;

                    t = grid.GetTile(x, y-1);
                    neighboursBits += (byte)(t.collides ? 1 : 0);
                    neighboursBits <<= 1;

                    t = grid.GetTile(x - 1, y-1);
                    neighboursBits += (byte)(t.collides ? 1 : 0);
                    neighboursBits <<= 1;

                    t = grid.GetTile(x - 1, y);
                    neighboursBits += (byte)(t.collides ? 1 : 0);
                    neighboursBits <<= 1;

                    t = grid.GetTile(x - 1, y+1);
                    neighboursBits += (byte)(t.collides ? 1 : 0);
                    neighboursBits <<= 1;

                    t = grid.GetTile(x, y+1);
                    neighboursBits += (byte)(t.collides ? 1 : 0);
                    neighboursBits <<= 1;

                    t = grid.GetTile(x + 1, y+1);
                    neighboursBits += (byte)(t.collides ? 1 : 0);

                    tile.SetSurface(neighboursBits);
                }
            }
            return grid;
        }

        public Tile GetTile(int x, int y)
        {
            if (!InBounds(x, y)) return Tile.Null;
            Tile t = tiles[x, y];
            if (t == null)
            {
                Debug.LogWarning("No tile at position (" + x + ", " + y + ")!");
            }
            return t;
        }
        public Tile GetTile(float x, float y)
        {
            return GetTile((int)x,(int)y);
        }


        public Tile GetTile(Vector2 position)
        {
            return GetTile((int)position.x, (int)position.y);
        }

        public Tile GetTile(Vector2i position)
        {
            return GetTile(position.x, position.y);
        }

        public void SetTile(int x, int y, Tile tile)
        {
            tiles[x, y] = tile;
        }

        public bool InBounds(int x, int y)
        {
            return !(
                x < 0 || x >= width ||
                y < 0 || y >= height
                );
        }

        public bool Collides(Tile tile)
        {
            return (tile != null && tile.collides) ||
                    (tile == null && collideOnExceededWorldBounds);
        }

        public int GetTiles(Rect rect, Tile[] tiles)
        {
            int x = Mathf.FloorToInt(rect.xMin);
            int X = Mathf.CeilToInt(rect.xMax);
            int y = Mathf.FloorToInt(rect.yMin);
            int Y = Mathf.CeilToInt(rect.yMax);
            int i = 0;
            for (; x < X; x++)
            {
                for (; y < Y; y++)
                {
                    tiles[i] = GetTile(x, y);
                    i++;
                }
            }
            return i;
        }

        public void AddEntity(Entity e)
        {
            var g = GetTiles(e.rect, TileArray);
            for (int i = 0; i < g; i++)
            {
                TileArray[i].contained.Add(e);
            }
        }

        //public bool Raycast(Vector2 start, Vector2 end, out RaycastData info)
        //{
        //    info = new RaycastData();
        //    if (Math.Approximately(start, end)) // not moving, return end point
        //    {
        //        info.point = end;
        //        info.distance = 0;
        //        return false;
        //    }

        //    var line = end - start; // vector for line
        //    var normal = Vector2.zero; // normal for collision
        //    float xm = -1, ym = -1; // multiplier for normal directions
        //    bool hit = false; // was there a collision?
        //    var sqo = new Vector2(start.x - (float)(int)start.x, start.y - (float)(int)start.y);
        //    foreach (var position in Math2D.SuperCover(start, end)) // march through grind squares
        //    {
        //        //Debug.Log(position.y);
        //        var tile = GetTile(position);
        //        if (Collides(tile)) // collide on tile or leaving map bounds
        //        {
        //            var to = position - start + sqo; // vector to collision point

        //            if (to.x < 0)
        //            {
        //                to.x += 1;
        //                xm = 1;
        //            }
        //            if (to.y < 0)
        //            {
        //                to.y += 1;
        //                ym = 1;
        //            }
        //            var dx = Mathf.Clamp01(to.x / line.x);
        //            var dy = Mathf.Clamp01(to.y / line.y);

        //            if (Math.Approximately(to, 0))
        //            {
        //                info.overlapping = true;
        //                //line *= 0;
        //            } else if (Mathf.Approximately(line.y, 0))
        //            {
        //                line *= dx;
        //                normal = Vector2.right * xm;
        //            }
        //            else if (Mathf.Approximately(line.x, 0))
        //            {
        //                line *= dy;
        //                normal = Vector2.up * ym;
        //            }
        //            else
        //            {
        //                if (dx > dy)
        //                {
        //                    line *= dx;
        //                    normal = Vector2.right * xm;
        //                }
        //                else
        //                {
        //                    line *= dy;
        //                    normal = Vector2.up * ym;
        //                }
        //            }
        //            hit = true;
        //            break;

        //        }
        //    }
        //    info.point = start + line;
        //    info.distance = line.magnitude;
        //    info.normal = normal;
        //    info.collided = hit;
        //    return hit;
        //}
    }
}