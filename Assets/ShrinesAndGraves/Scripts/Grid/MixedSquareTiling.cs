using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Extensions;
using System.Linq;

namespace Shrines
{
    public class MixedSquareTiling
    {
        /// <summary>
        /// get the size of a tile to be placed at a position, if there is one
        /// will always return false unless the position corresponds to the bottom left corner of the tile
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="bounds"></param>
        /// <returns></returns>
        static BitArray _setTiles;
        public void TileRect(Recti rect, Vector2i[] sizes, List<Vector3i> tiles)
        {
            if (_setTiles == null || _setTiles.Length < rect.width * rect.height)
            {
                _setTiles = new BitArray(rect.width * rect.height);
            }

            _setTiles.SetAll(false);

            int choices = sizes.Length;

            /// fill rect left to right, bottom to top
            for (int y = 0; y < rect.height; y++)
            {
                for (int x = 0; x < rect.width; x++)
                {
                    if (_setTiles[y * rect.width + x]) continue;
                    int i;
                    for (i = Random.Range(0, choices); i >= 0;)
                    {
                        for (int tx = x; tx < sizes[i].x + x; tx++)
                        {
                            for (int ty = y; ty < sizes[i].y + y; ty++)
                            {
                                if (_setTiles[ty * rect.width + tx]) goto __checkNextChoice;
                            }
                        }

                        break;
                        __checkNextChoice:
                        i--;
                    }
                    if (i < 0)
                    {
                        Debug.LogError("Couldn't find a tile to fit out of these choices: " + string.Join(", ", sizes.Select(s => s.ToString()).ToArray()));
                        return;
                    }
                    else
                    {
                        for (int tx = x; tx < sizes[i].x + x; tx++)
                        {
                            for (int ty = y; ty < sizes[i].y + y; ty++)
                            {
                                _setTiles[ty * rect.width + tx] = true;
                            }
                        }
                        tiles.Add(new Vector3i(x,y,i));
                    }
                }
            }
        }
    }
}