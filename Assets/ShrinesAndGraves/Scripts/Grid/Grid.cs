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

        public Tile[] surface;

        public Grid(int w, int h)
        {
            width = w;
            height = h;
            tiles = new Tile[w, h];
            surface = new Tile[w];
        }

        public static Grid PerlinNoise(int width, int height, TileData[] types) {
            var grid = new Grid(width, height);
            var noise = Math.FrequencyNoise1D(x => x, x => x * x, x => (1 - x) * (1 - x), 0.1f, 0.25f, 4);
            for (int x = 0; x < width; x++)
            {
                var g = noise(x);
                for (int y = 0; y < height; y++)
                {
                    TileData data = null;
                    if (y < 25 + g * 50)
                    {
                        data = types[0];
                    }
                    else
                    {
                        data = types[1];
                    }
                    var tile = new Tile(x,y,data);
                    grid.SetTile(x, y, tile);
                }
            }
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    grid.UpdateSurfaces(new Recti(0,0,width, height));
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

        public void SetTileData(int x, int y, TileData tileData)
        {
            var tile = GetTile(x, y);
            if(tile!=null) {
                tile.SetData(tileData);
            }
        }

        public void SetTileData(Recti r, TileData tileData)
        {
            int x = r.position.x,
                y = r.position.y;
            int X = x + r.size.x,
                Y = y + r.size.y;
            for (x = r.position.x; x < X; x++)
            {
                for (y = r.position.y; y < Y; y++)
                {
                    var tile = GetTile(x, y);
                    tile.SetData(tileData);
                }
            }
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

        public void UpdateSurfaces(Recti rect)
        {
            for (int x = rect.xMin; x < rect.xMax; x++)
            {
                for (int y = rect.yMin; y < rect.yMax; y++)
                {
                    var tile = GetTile(x, y);
                    byte neighboursBits = 0;
                    var t = GetTile(x + 1, y);
                    neighboursBits += (byte)(t.collides ? 1 : 0);
                    neighboursBits <<= 1;

                    t = GetTile(x + 1, y - 1);
                    neighboursBits += (byte)(t.collides ? 1 : 0);
                    neighboursBits <<= 1;

                    t = GetTile(x, y - 1);
                    neighboursBits += (byte)(t.collides ? 1 : 0);
                    neighboursBits <<= 1;

                    t = GetTile(x - 1, y - 1);
                    neighboursBits += (byte)(t.collides ? 1 : 0);
                    neighboursBits <<= 1;

                    t = GetTile(x - 1, y);
                    neighboursBits += (byte)(t.collides ? 1 : 0);
                    neighboursBits <<= 1;

                    t = GetTile(x - 1, y + 1);
                    neighboursBits += (byte)(t.collides ? 1 : 0);
                    neighboursBits <<= 1;

                    t = GetTile(x, y + 1);
                    neighboursBits += (byte)(t.collides ? 1 : 0);
                    neighboursBits <<= 1;

                    t = GetTile(x + 1, y + 1);
                    neighboursBits += (byte)(t.collides ? 1 : 0);

                    tile.SetSurface(neighboursBits);
                }
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