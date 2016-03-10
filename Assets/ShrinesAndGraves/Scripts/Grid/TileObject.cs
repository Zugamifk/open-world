using UnityEngine;
using System.Collections;

namespace Shrines
{
    public class TileObject : WorldObject
    {

        public Tile tile;

        public override void InitializeGameobject(Entity e)
        {
            base.InitializeGameobject(e);
            SetTile(e as Tile);
        }

        public void SetTile(Tile tile)
        {
            this.tile = tile;

            if (tile != null)
            {
                renderer.sprite = tile.data.sprite;
                var spr = renderer.sprite;
                if (spr != null)
                {
                    var os = spr.Size();
                    os.Scale(spr.pivot);
                    renderer.transform.localPosition = -os;
                }
            }
            else ResetGameobject();
        }

        public override void ResetGameobject()
        {
            base.ResetGameobject();
            tile = null;
        }
    }
}