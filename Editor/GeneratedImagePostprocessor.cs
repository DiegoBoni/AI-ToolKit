using UnityEditor;
using UnityEngine;
using System.IO;

public class GeneratedImagePostprocessor : AssetPostprocessor
{
    private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
    {
        foreach (string assetPath in importedAssets)
        {
            // Check if the asset is an image and is in the "GeneratedImages" folder
            if (assetPath.Contains("GeneratedImages") && (assetPath.EndsWith(".png") || assetPath.EndsWith(".jpg") || assetPath.EndsWith(".jpeg")))
            {
                TextureImporter textureImporter = AssetImporter.GetAtPath(assetPath) as TextureImporter;
                if (textureImporter != null)
                {
                    if (textureImporter.textureType != TextureImporterType.Sprite || textureImporter.spriteImportMode != SpriteImportMode.Single)
                    {
                        textureImporter.textureType = TextureImporterType.Sprite;
                        textureImporter.spriteImportMode = SpriteImportMode.Single;
                        textureImporter.spritePixelsPerUnit = 100; // Esta es una propiedad directa

                        Debug.Log($"[GeneratedImagePostprocessor] Setting texture type to Sprite 2D UI for: {assetPath}");
                        textureImporter.SaveAndReimport();

                        Debug.Log($"[GeneratedImagePostprocessor] Setting texture type to Sprite 2D UI for: {assetPath}");
                        textureImporter.SaveAndReimport();
                    }
                }
            }
        }
    }
}
