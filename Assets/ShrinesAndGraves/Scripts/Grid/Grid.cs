using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Extensions;

namespace Shrines
{
    public class Grid
    {
        Tile[,] tiles;

        static bool collideOnExceededWorldBounds = true;

        public int width;
        public int height;

        public Grid(int w, int h)
        {
            width = w;
            height = h;
            tiles = new Tile[w, h];
        }

        public Tile GetTile(int x, int y)
        {
            if (!InBounds(x, y)) return null;
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

        public Tile GetTile(Vector3i position)
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

        public bool Raycast(Vector2 start, Vector2 end, out RaycastData info)
        {
            info = new RaycastData();
            if (Math.Approximately(start, end)) // not moving, return end point
            {
                info.point = end;
                info.distance = 0;
                return false;
            }

            var line = end - start; // vector for line
            var normal = Vector2.zero; // normal for collision
            float xm = -1, ym = -1; // multiplier for normal directions
            bool hit = false; // was there a collision?
            foreach (var position in Math2D.SuperCover(start, end)) // march through grind squares
            {
                //Debug.Log(position.y);
                var tile = GetTile(position);
                if ((tile != null && tile.collides) ||
                    (tile == null && collideOnExceededWorldBounds)) // collide on tile or leaving map bounds
                {
                    var to = position - start; // vector to collision point
                    //Debug.Log(to+" : "+line.y);
                    //line = to;
                    var bl = -to; // opposite vector to simplify algorithm
                    if (to.x < 0)
                    {
                        to.x += 1;
                        xm = 1;
                    }
                    if (to.y < 0)
                    {
                        to.y += 1;
                        ym = 1;
                    }
                    var dx = Physics.Bound(Mathf.Abs(to.x / line.x));
                    var dy = Physics.Bound(Mathf.Abs(to.y / line.y));

                    if (Mathf.Approximately(line.y, 0))
                    {
                        //Debug.Log(to.x + " : " + line.x+":"+dx);
                        line *= dx;
                        normal = Vector3.right * xm;
                    }
                    else if (Mathf.Approximately(line.x, 0))
                    {
                        line *= dy;
                        normal = Vector3.up * ym;
                    }
                    else
                    {
                        if (dx > dy)
                        {
                            line *= dx;
                            normal = Vector3.right * xm;
                        }
                        else
                        {
                            line *= dy;
                            normal = Vector3.up * ym;
                        }
                    }
                    hit = true;
                    break;

                }
            }
            info.point = start + line;
            info.distance = line.magnitude;
            info.normal = normal;
            info.collided = hit;
            return hit;
        }
    }
}