using UnityEngine;
using System.Collections;

namespace Shrines
{
    public class TileObject : WorldObject
    {
        public class LedgeTrigger : EntityTrigger
        {
            public TileObject tileObject;
        }

        public Tile tile;
        public BoxCollider2D leftLedge, rightLedge;

        public override void InitializeRenderers(Entity e)
        {
            base.InitializeRenderers(e);
            SetTile(e as Tile);
        }

        public override void InitializeGameobject(Entity e)
        {
            base.InitializeGameobject(e);

            var go = new GameObject("Left Ledge");
            go.transform.SetParent(transform);
            go.transform.localPosition = Vector2.up;
            leftLedge = go.AddComponent<BoxCollider2D>();
            leftLedge.size = Vector2.one * 0.5f;
            leftLedge.isTrigger = true;
            leftLedge.enabled = false;
            var lt = go.AddComponent<LedgeTrigger>();
            lt.tileObject = this;

            go = new GameObject("Right Ledge");
            go.transform.SetParent(transform);
            go.transform.localPosition = Vector2.one;
            rightLedge = go.AddComponent<BoxCollider2D>();
            rightLedge.isTrigger = true;
            rightLedge.enabled = false;
            rightLedge.size = Vector2.one * 0.5f;
            lt = go.AddComponent<LedgeTrigger>();
            lt.tileObject = this;

            SetTile(e as Tile);
        }

        public void SetTile(Tile tile)
        {
            this.tile = tile;

            if (tile != null && tile.tileData!=null)
            {
                m_renderer.sprite = tile.tileData.GetSprite(tile, m_renderer.sortingLayerID);
                if (m_renderer.sprite != null)
                {
                    m_rendererTransform.localPosition = m_renderer.sprite.pivot/m_renderer.sprite.pixelsPerUnit;
                }
            }
            else ResetGameobject();
            transform.position = tile.position;

            if (collider == null) return;
            if (tile.collides)
            {
                EnableLedges();
                collider.enabled = tile.surface != Tile.Surface.All;
            }
            else
            {
                collider.enabled = false;
                rightLedge.enabled = false;
                leftLedge.enabled = false;
            }
        }

        void EnableLedges()
        {
            if (tile.Is3NodesFree(Tile.Surface.TopLeft))
            {
                leftLedge.enabled = true;
            }
            else
            {
                leftLedge.enabled = false;
            }

            if (tile.Is3NodesFree(Tile.Surface.TopRight))
            {
                rightLedge.enabled = true;
            }
            else
            {
                rightLedge.enabled = false;
            }
        }

        public override void ResetGameobject()
        {
            base.ResetGameobject();
            tile = null;
        }
    }
}