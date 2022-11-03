using UnityEditor;
using UnityEngine;

namespace CoreDev.CustomEditors
{
    public static class AssetDatabaseHelper
    {
        public static T LoadAsset<T>(string filter, string[] searchFolders = null) where T : UnityEngine.Object
        {
            filter = $"t:{typeof(T).Name} {filter}";
            string[] guids = AssetDatabase.FindAssets(filter, searchFolders);
            T asset = null;

            if (guids.Length > 0)
            {
                if (guids.Length > 1)
                {
                    Debug.LogWarning("GetAssetPath: More than one asset found, returned path may not be the one intended");
                }
                string assetPath = AssetDatabase.GUIDToAssetPath(guids[0]);
                asset = AssetDatabase.LoadAssetAtPath<T>(assetPath);
            }
            else
            {
                Debug.LogError($"GetAssetPath: No asset found using filter \"{filter}\"");
            }
            return asset;
        }
    }
}
