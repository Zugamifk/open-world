using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Extensions;

namespace Albedo.Graphics {
	using World = Albedo.World;
	public class LightingManager : MonoBehaviour {

        static LightingManager instance;

        List<TileAnimator> Animators = new List<TileAnimator>();
        List<World.Tile> visibleTiles = new List<World.Tile>();

        public static void RegisterTileAnimator(TileAnimator ta) {
            instance.Animators.Add(ta);
        }

        void Awake() {
            this.SetInstanceOrKill(ref instance);
        }

		void Update() {
			foreach(var tile in Animators) {
				foreach(var obj in tile.Objects) {
					float saturation = 1;
                    float contrast = 1;
                    float burn=0;
                    float overlay = 1;

					if(!tile.Tile.Visible) {
                        saturation = 0.25f;
                        burn = 1;
                    }

					obj.SetMaterialFloat("_DayCycle", saturation);
					obj.SetMaterialFloat("_Contrast", contrast);
					obj.SetMaterialFloat("_Burn", burn);
                    obj.SetMaterialFloat("_Overlay", overlay);
				}
			}
        }

		void LateUpdate() {
			UpdateVisibility();
		}

		void UpdateVisibility() {
			visibleTiles.ForEach(t=>t.Visible=false);
			visibleTiles.Clear();
			var mid = (Vector3i)PlayerControl.Position;
			foreach(var end in Math2D.CirclePoints(Vector3i.zero, (int)MapView.Main.ViewRect.width)) {
				foreach(var pt in Math2D.SuperCover(Vector3i.zero, end)) {
					var an = MapView.GetTileAnimator(mid+pt);
					if(an!=null) {
						if(an.Tile.Collides) {
							break;
						} else {
							an.Tile.Visible = true;
							visibleTiles.Add(an.Tile);
						}
					} else break;
				}
			}
		}
	}
}
