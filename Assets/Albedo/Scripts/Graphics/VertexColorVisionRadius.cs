using UnityEngine;
using System.Collections;
using System.Linq;
using MeshGenerator;
using Extensions;

namespace Albedo.Graphics {
	[RequireComponent(typeof(MeshRenderer))]
	[RequireComponent(typeof(MeshFilter))]
	public class VertexColorVisionRadius : MonoBehaviour {

        [SerializeField]
        protected int radius;
        [SerializeField]
        protected float fadeSpeed;

        new MeshRenderer renderer;
        MeshFilter filter;
        int width, height;
        Vector2 rootPosition;

        private static VertexColorVisionRadius instance;

        void Awake() {
			renderer = GetComponent<MeshRenderer>();
            renderer.sortingOrder = Constants.VisionSortingOrder;
            filter = GetComponent<MeshFilter>();
			this.SetInstanceOrKill(ref instance);
        }

        public static void Initialize(int width, int height) {
            var gen = new MeshGenerator.Plane();
			gen.gridWidth = width;
            gen.gridHeight = height;
			instance.width = width+1;
			instance.height = height+1;
            instance.filter.mesh = gen.Generate();
            instance.filter.mesh.colors = instance.filter.mesh.vertices.Select(v => Color.black).ToArray();
            instance.rootPosition = new Vector3(-width/2, -height/2, -1);
            instance.transform.localScale = new Vector3(width, 1, height);
        }

		// Update is called once per frame
		void Update () {
            transform.position = rootPosition;// - PlayerControl.TileOffset;
            var colors = filter.mesh.colors;
            var mid = (Vector3i)PlayerControl.Position;
            colors = colors.ToEach(c => Mathf.Clamp01(c.a+fadeSpeed*Time.deltaTime*0.5f)*Color.black);
            foreach(var end in Math2D.CirclePoints(Vector3i.zero, radius)) {
                foreach(var pt in Math2D.SuperCover(Vector3i.zero, end)) {
                    var an = MapView.GetTileAnimator(mid+pt);
                    if(an!=null && an.Tile.Collides) {
						break;
					}

					colors[pt.x+width/2 + (pt.y+height/2)*width] = Mathf.Clamp01(colors[pt.x+width/2 + (pt.y+height/2)*width].a-fadeSpeed*Time.deltaTime)*Color.black;
				}
            }
			filter.mesh.colors = colors;
		}
	}
}
