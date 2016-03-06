/*
Constants to be used in the game
*/
using UnityEngine;
using System.Collections;

namespace Albedo {
    public static class Constants {

        /** COUNTS */
        public const int MaxTileObjects = 8;

        /** UNIT MEASURES */

        /** Tile width in pixel units */
        public const int TileWidthPixels = 16;

		/** Tile height in pixel units */
		public const int TileHeightPixels = 16;

        /** Sprite pixels-per-unit */
        public const int SpritePPU = 16;

        /** size of a pixel in world units */
        public const float PixelSize = 1f / 16f;

        /** Sprite sorting orders */

        public const int GroundSortingOrder = 0;
        public const int GroundSurfaceSortingOrder = 1;
        public const int PlayerSortingOrder = 2;
        public const int OverPlayerSortingOrder = 3;
        public const int HighSortingOrder = 4;
        public const int VisionSortingOrder = 5;

        /** STRINGS */

        /** game conditions */
        public const string PlayerControlInitialized = "Player Control Initialized";
    }
}
