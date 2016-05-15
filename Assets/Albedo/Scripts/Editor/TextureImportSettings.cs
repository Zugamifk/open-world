using UnityEngine;
using UnityEditor;
using System.Collections;
using Albedo;

namespace Albedo.Editor {
	public class TextureImportSettings : AssetPostprocessor
	{
		void OnPostprocessTexture(Texture2D texture)
		{
            if (assetPath.Contains("Albedo"))
            {
                TextureImporter importer = assetImporter as TextureImporter;
                importer.anisoLevel = 0;
                importer.filterMode = FilterMode.Point;
                importer.mipmapEnabled = false;
                importer.spritePixelsPerUnit = Constants.SpritePPU;
                importer.textureFormat = TextureImporterFormat.RGBA32;
            }

        }
	}
}
