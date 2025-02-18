using UnityEngine;
using System.IO;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(fileName = "BuildingDatabase", menuName = "City Data/Building Database")]
public class BuildingDatabase : ScriptableObject
{
    public BuildingData[] buildingDataObjects;
    public string directoryPath;

    public void UpdateDatabase()
    {
        UpdateDatabaseFromDirectory(directoryPath);
    }

    public void UpdateDatabaseFromDirectory(string directoryPath)
    {
        if (!Directory.Exists(directoryPath))
        {
            Debug.LogError($"Directory not found: {directoryPath}");
            return;
        }

        string[] assetPaths = AssetDatabase.FindAssets("t:BuildingData", new[] { directoryPath });
        buildingDataObjects = assetPaths.Select(assetPath =>
        {
            string path = AssetDatabase.GUIDToAssetPath(assetPath);
            return AssetDatabase.LoadAssetAtPath<BuildingData>(path);
        }).Where(buildingData => buildingData != null).ToArray();
    }
}
