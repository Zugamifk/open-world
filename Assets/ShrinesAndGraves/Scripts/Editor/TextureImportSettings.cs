using UnityEngine;
using UnityEditor;
using System.Collections;

namespace Shrines
{
    public class TextureImportSettings : AssetPostprocessor
    {
        void OnPostprocessTexture(Texture2D texture)
        {
            if (assetPath.Contains("Shrines"))
            {
                
                TextureImporter importer = assetImporter as TextureImporter;
                importer.anisoLevel = 0;
                importer.filterMode = FilterMode.Point;
                importer.mipmapEnabled = false;
                importer.spritePixelsPerUnit = 16;
                importer.textureFormat = TextureImporterFormat.RGBA32;

                if (assetPath.Contains("UI"))
                {
                    importer.spritePixelsPerUnit = 64;
                }
            }

        }
    }
}