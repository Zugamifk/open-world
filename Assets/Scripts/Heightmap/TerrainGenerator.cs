using UnityEngine;
using System.Collections;
using System.Linq;

namespace Landscape {
	public delegate float TerrainHeightFunction(float x, float y, float z);
	public class TerrainGenerator : MonoBehaviour, IHeightMap {

		public TerrainHeightFunction HeightGenerator;
		private Vector2 offset;
		public void Init() {
			HeightGenerator += PerlinNoise;
			offset = new Vector2(Random.value*1000, Random.value*1000);
		}

		public float GetHeight(float x, float y) {
			float height = 0;
			foreach(TerrainHeightFunction generator in HeightGenerator.GetInvocationList()) {
				height = generator(x,y,height);
			}
			return height;
		}

		public float GetHeight(Vector2 position) { return GetHeight(position.x, position.y); }

		public void GetHeight(ref Vector3 position) {
			position.z =  GetHeight(position.x, position.y);
		}

		// genrators
		float pscale = 1;
		private float PerlinNoise(float x, float y, float z) {
			return Mathf.PerlinNoise(offset.x+x*pscale, offset.y+y*pscale);
		}
	}
}
