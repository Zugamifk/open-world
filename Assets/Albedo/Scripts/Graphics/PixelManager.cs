﻿using UnityEngine;
using System.Collections;

namespace Albedo.Graphics {
	public class PixelManager : MonoBehaviour {

        public int pixelsPerUnit;

        private static float PPUfloat;

        private static PixelManager instance;
        void Awake() {
            if(this.SetInstanceOrKill(ref instance)) {
                PPUfloat = (float)pixelsPerUnit;
            }
        }

		public static Vector2 SnapVector(Vector2 input) {
            return new Vector2(
                Mathf.Round(input.x * PPUfloat) / PPUfloat,
                Mathf.Round(input.y * PPUfloat) / PPUfloat
            );
        }
	}
}
