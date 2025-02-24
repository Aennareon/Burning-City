using System;
using System.IO; // Importar para Path
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class BuildingDataManager : MonoBehaviour
{
    public BuildingDatabase buildingDatabase;

    public static event Action<BuildingData> OnBuildingCreated;
    public static event Action<BuildingData> OnBuildingDeleted;

    public BuildingData CreateBuildingData(BuildingObject building)
    {
#if UNITY_EDITOR
        BuildingData buildingData = ScriptableObject.CreateInstance<BuildingData>();
        UpdateBuildingData(building, buildingData);

        // Obtener el DataPath de la ciudad activa
        CityDataObject activeCity = GetActiveCityDataObject();
        if (activeCity == null)
        {
            Debug.LogError("No se encontr� ninguna ciudad activa.");
            return null;
        }

        string uniqueID = System.Guid.NewGuid().ToString();
        string path = Path.Combine(activeCity.DataPath, "Buildings", $"{building.gameObject.name}_{uniqueID}_BuildingData.asset");
        AssetDatabase.CreateAsset(buildingData, path);
        AssetDatabase.SaveAssets();

        // Actualizar la base de datos despu�s de crear el archivo de datos
        if (buildingDatabase != null)
        {
            buildingDatabase.UpdateDatabase();
        }

        OnBuildingCreated?.Invoke(buildingData);
        return buildingData;
#else
        return null;
#endif
    }

    public static void UpdateBuildingData(BuildingObject building, BuildingData buildingData)
    {
#if UNITY_EDITOR
        if (buildingData != null)
        {
            buildingData.buildingType = building.buildingType;
            buildingData.districtZone = building.districtZone;
            buildingData.cityRaces = building.cityRaces;
            buildingData.buildingPosition = building.transform.position;
            buildingData.buildingRotation = building.transform.rotation.eulerAngles;
            buildingData.doorPosition = building.doorPosition;
            buildingData.buildingPrefab = building.buildingPrefab;

            EditorUtility.SetDirty(buildingData);
            AssetDatabase.SaveAssets();
        }
#endif
    }

    public void DeleteBuildingData(BuildingData buildingData)
    {
#if UNITY_EDITOR
        if (buildingData != null && Application.isPlaying)
        {
            string assetPath = AssetDatabase.GetAssetPath(buildingData);
            AssetDatabase.DeleteAsset(assetPath);
            AssetDatabase.SaveAssets();

            // Actualizar la base de datos despu�s de eliminar el archivo de datos
            if (buildingDatabase != null)
            {
                buildingDatabase.UpdateDatabase();
            }

            OnBuildingDeleted?.Invoke(buildingData);
        }
#endif
    }

    private CityDataObject GetActiveCityDataObject()
    {
        CityDataObject[] cityDataObjects = Resources.FindObjectsOfTypeAll<CityDataObject>();
        foreach (CityDataObject cityDataObject in cityDataObjects)
        {
            if (cityDataObject.ActiveCity)
            {
                return cityDataObject;
            }
        }
        return null;
    }
}
