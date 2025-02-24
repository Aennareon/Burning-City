using UnityEngine;
using System.IO;
using System.Linq;
using System.Collections.Generic; // Asegúrate de incluir esta directiva

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
        if (string.IsNullOrEmpty(directoryPath))
        {
            Debug.LogError("El directorio de la base de datos de edificios no está configurado.");
            return;
        }

        // Lógica para actualizar la base de datos desde el directorio
        UpdateDatabaseFromDirectory(directoryPath);
#endif
    }

    public void UpdateDatabaseFromDirectory(string directoryPath)
    {
#if UNITY_EDITOR
        if (string.IsNullOrEmpty(directoryPath))
        {
            Debug.LogError("El directorio de la base de datos de edificios no está configurado.");
            return;
        }

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

    public List<Vector3> GetAllDoorPositions()
    {
        List<Vector3> doorPositions = new List<Vector3>();
        BuildingObject[] buildingObjects = Object.FindObjectsByType<BuildingObject>(FindObjectsSortMode.None);

        foreach (var building in buildingObjects)
        {
            Vector3 rotatedDoorPosition = Quaternion.Euler(0, building.transform.eulerAngles.y, 0) * building.doorPosition;
            doorPositions.Add(building.transform.position + rotatedDoorPosition);
        }

        return doorPositions;
    }
}
