using UnityEngine;
using System.Collections;

namespace Albedo.Graphics {
	using World = Albedo.World;

	[RequireComponent(typeof(SpriteRenderer))]
	public class TileObjectAnimator : MonoBehaviour {

        new public SpriteRenderer renderer;
		public World.TileObject tileObject;

        private string nullName;

        void Awake() {
            renderer = GetComponent<SpriteRenderer>();
            renderer.sortingOrder = Constants.GroundSurfaceSortingOrder;
            nullName = name;
        }

		void Start() {
            renderer.material = ResourceManager.Defaults.spriteMaterial;
        }

		public void SetMaterialFloat(string name, float value) {
			renderer.material.SetFloat(name, value);
		}

		public void SetObject(World.TileObject obj) {
			if(obj==null) {
				renderer.sprite = null;
                name = nullName;
				tileObject = null;
			} else
            {
                tileObject = obj;
                renderer.sprite = obj.graphic != null ? obj.graphic : Albedo.ResourceManager.Defaults.tileSprite;
                name = obj.name;
            }
        }

		public void Update() {
			if(tileObject!=null) {
				Debugx.DrawRect(new Rect(transform.position.x, transform.position.y, 1, 1), Color.green );
			}
		}
	}
}
