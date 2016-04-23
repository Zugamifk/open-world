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

            var bc = gameObject.GetOrAddComponent<BoxCollider2D>();
            bc.offset = Vector2.one*0.5f;
            collider = bc;

            SetTile(e as Tile);
        }

        public void SetTile(Tile tile)
        {
            this.tile = tile;

            if (tile != null && tile.tileData!=null)
            {
                renderer.sprite = tile.tileData.GetSprite(tile);
            }
            else ResetGameobject();
            transform.position = tile.position;
            collider.enabled = tile.collides && tile.surface != Tile.Surface.None;
        }

        public override void ResetGameobject()
        {
            base.ResetGameobject();
            tile = null;
        }
    }
}