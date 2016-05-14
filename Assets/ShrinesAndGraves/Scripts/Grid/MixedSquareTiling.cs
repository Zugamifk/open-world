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
        public void TileRect(Grid g, Recti rect, List<List<TileGraphicData.SpriteShape>> shapes)
        {
            if (_setTiles == null || _setTiles.Length < rect.width * rect.height*2)
            {
                _setTiles = new BitArray(rect.width * rect.height*2);
            }

            _setTiles.SetAll(false);

            int choices = shapes.Count;
            if (choices == 0)
            {
                Debug.LogError("Can't fill rect with empty shape list!");
                return;
            }

            /// fill rect left to right, bottom to top
            for (int y = 0; y < rect.height; y++)
            {
                for (int x = 0; x < rect.width; x++)
                {
                    if (_setTiles[y * rect.width + x]) continue;


                    var pos = new Vector2i(rect.position.x+x, rect.position.y+y);
                    var tile = g.GetTile(pos);

                    if (tile!=null && tile.graphicMetadata.graphicCache[Grid.Layer.Background].hasOverride)
                    {
                        _setTiles[y * rect.width + x] = true;
                        continue;
                    }

                    TileGraphicData.SpriteShape choice = null;
                    
                    int i;
                    for (i = choices - 1-(int)Mathf.Sqrt(Random.Range(0, choices*choices)); i >= 0;)
                    {
                        if (choice != null) break;
                        for (int s = 0; s < shapes[i].Count; s++)
                        {
                            var shape = shapes[i][s];
                            if (shape.CanPosition(pos))
                            {
                                for (int tx = x; tx < shape.dimensions.x + x; tx++)
                                {
                                    for (int ty = y; ty < shape.dimensions.y + y; ty++)
                                    {
                                        if (_setTiles[ty * rect.width + tx]) goto __checkNextChoice;
                                    }
                                }
                                choice = shape;
                                break;
                            }
                        }
                        __checkNextChoice:
                        i--;
                    }
                    if (choice!=null)
                    {
                        var gt = g.GetTile(pos);
                        for (int tx = x; tx < choice.dimensions.x + x; tx++)
                        {
                            for (int ty = y; ty < choice.dimensions.y + y; ty++)
                            {
                                _setTiles[ty * rect.width + tx] = true;

                                var t = g.GetTile(rect.position.x+tx, rect.position.y+ty);
                                if (t != null)
                                {
                                    var gmd = t.graphicMetadata.graphicCache[Grid.Layer.Background];
                                    gmd.dontDraw = true;
                                    gmd.tileGraphicSize = choice.dimensions;
                                    gmd.tileWithgraphic = gt;
                                }
                            }
                        }
                        var gtmd = gt.graphicMetadata.graphicCache[Grid.Layer.Background];
                        gtmd.dontDraw = false;
                        gtmd.graphicOverride = choice.sprites.Random();
                        x += choice.dimensions.x-1;
                    }
                    if (!_setTiles[y * rect.width + x])
                    {
                        Debug.Log("Tile not set at "+pos+"!");
                        Debug.Break();
                    }
                }
            }

            if (_setTiles.Any())
            {
                Debug.Log("Some tiles not set!");
                Debug.Break();
            }
        }
    }
}