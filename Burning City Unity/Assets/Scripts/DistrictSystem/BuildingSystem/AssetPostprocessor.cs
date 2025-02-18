#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

public class BuildingDatabasePostprocessor : AssetPostprocessor
{
    private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
    {
        EditorApplication.delayCall += UpdateBuildingDatabase;
    }

    private static void UpdateBuildingDatabase()
    {
        BuildingDatabase buildingDatabase = AssetDatabase.LoadAssetAtPath<BuildingDatabase>("Assets/Data/CityData/BuildingDatabase.asset");
        if (buildingDatabase != null)
        {
            buildingDatabase.UpdateDatabase();
        }
    }
}
#endif
