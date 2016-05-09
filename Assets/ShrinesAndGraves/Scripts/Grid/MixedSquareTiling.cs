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
        public void TileRect(Grid g, Recti rect, TileGraphicData.SpriteShape[] shapes)
        {
            if (_setTiles == null || _setTiles.Length < rect.width * rect.height*2)
            {
                _setTiles = new BitArray(rect.width * rect.height*2);
            }

            _setTiles.SetAll(false);

            int choices = shapes.Length;

            /// fill rect left to right, bottom to top
            for (int y = 0; y < rect.height; y++)
            {
                for (int x = 0; x < rect.width; x++)
                {
                    if (_setTiles[y * rect.width + x]) continue;
                    int i;
                    var pos = new Vector2i(x, y);
                    for (i = Random.Range(0, choices); i >= 0;)
                    {
                        if (shapes[i].CanPosition(pos))
                        {
                            for (int tx = x; tx < shapes[i].dimensions.x + x; tx++)
                            {
                                for (int ty = y; ty < shapes[i].dimensions.y + y; ty++)
                                {
                                    if (_setTiles[ty * rect.width + tx]) goto __checkNextChoice;
                                }
                            }

                            break;
                        }
                        __checkNextChoice:
                        i--;
                    }
                    if (i >= 0)
                    {
                        var gt = g.GetTile(rect.position.x+x,rect.position.y+y);
                        for (int tx = x; tx < shapes[i].dimensions.x + x; tx++)
                        {
                            for (int ty = y; ty < shapes[i].dimensions.y + y; ty++)
                            {
                                _setTiles[ty * rect.width + tx] = true;

                                var t = g.GetTile(rect.position.x+tx, rect.position.y+ty);
                                if (t != null)
                                {
                                    var gmd = t.graphicMetadata.graphicCache[Grid.Layer.Background];
                                    gmd.dontDraw = true;
                                    gmd.tileGraphicSize = shapes[i].dimensions;
                                    gmd.tileWithgraphic = gt;
                                }
                            }
                        }
                        var gtmd = gt.graphicMetadata.graphicCache[Grid.Layer.Background];
                        gtmd.dontDraw = false;
                        gtmd.graphicOverride = shapes[i].sprites.Random();
                    }
                }
            }
        }
    }
}