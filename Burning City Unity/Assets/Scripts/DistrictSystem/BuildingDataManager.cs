using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class BuildingDataManager : MonoBehaviour
{
    public static BuildingData CreateBuildingData(BuildingObject building)
    {
#if UNITY_EDITOR
        BuildingData buildingData = ScriptableObject.CreateInstance<BuildingData>();
        UpdateBuildingData(building, buildingData);

        AssetDatabase.CreateAsset(buildingData, $"Assets/Data/CityData/Buildings/{building.gameObject.name}_BuildingData.asset");
        AssetDatabase.SaveAssets();

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
            buildingData.doorPosition = building.doorPosition;

            EditorUtility.SetDirty(buildingData);
            AssetDatabase.SaveAssets();
        }
#endif
    }

    public static void DeleteBuildingData(BuildingData buildingData)
    {
#if UNITY_EDITOR
        if (buildingData != null)
        {
            AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(buildingData));
        }
#endif
    }
}
