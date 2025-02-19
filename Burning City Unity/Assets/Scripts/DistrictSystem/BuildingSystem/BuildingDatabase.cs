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
#if UNITY_EDITOR
        // Lógica para actualizar la base de datos desde el directorio
        UpdateDatabaseFromDirectory(directoryPath);
#endif
    }

    public void UpdateDatabaseFromDirectory(string directoryPath)
    {
#if UNITY_EDITOR
        // Lógica para cargar los datos desde el directorio
        string[] assetPaths = AssetDatabase.FindAssets("t:BuildingData", new[] { directoryPath });
        buildingDataObjects = new BuildingData[assetPaths.Length];

        for (int i = 0; i < assetPaths.Length; i++)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(assetPaths[i]);
            buildingDataObjects[i] = AssetDatabase.LoadAssetAtPath<BuildingData>(assetPath);
        }
#endif
    }
}
