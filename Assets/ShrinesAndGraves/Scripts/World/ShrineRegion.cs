using UnityEngine;
using System.Collections;

namespace Shrines
{
    public class ShrineRegion : Region
    {
        public AnimationCurve heights;
        public float heightScalar;

        public override void Fill(Grid g)
        {

            int level = g.surface[rect.xMin].gridPosition.y;
            for (int x = rect.xMin; x < rect.xMax; x++)
            {
                var surface = g.surface[x];
                var surfHeight = level + (int)(heightScalar * heights.Evaluate((float)(x-rect.xMin) / (float)rect.width));
                Tile tile;
                for (int y = level; y < level + heightScalar; y++)
                {
                    tile = g.GetTile(x, y);
                    if (y <= surfHeight)
                    {
                        tile.tileData = environment.GetTileData("ground");
                    }
                    else
                    {
                        tile.tileData = environment.GetTileData("empty");
                    }
                    tile.altitude = y - surfHeight;
                }
                g.surface[x] = g.GetTile(x, surfHeight);

                for(int y=level-1;y > 0;y--) {
                    tile = g.GetTile(x, y);
                    if (tile == null || tile.collides) break;
                    tile.tileData = environment.GetTileData("ground");
                }
            }

            ObjectSpawn seer;
            if (objects.TryGetValue("seer", out seer))
            {
                var pos = (Vector2f16)(rect.position + seer.spawnPosition);
                if(pos.y < g.surface[(int)pos.x].gridPosition.y) {
                    pos.y = g.surface[(int)pos.x].gridPosition.y + 1;
                }
                g.AddEntity(new Character((CharacterData)seer.data, pos));
            }
        }
    }
}