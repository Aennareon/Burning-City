using UnityEngine;

public class BuildingInstantiator : MonoBehaviour
{
    public BuildingDatabase buildingDatabase;

    private void Start()
    {
        InstantiateBuildings();
    }

    public void InstantiateBuildings()
    {
        if (buildingDatabase == null)
        {
            Debug.LogError("BuildingDatabase is not assigned.");
            return;
        }

        foreach (BuildingData buildingData in buildingDatabase.buildingDataObjects)
        {
            if (buildingData.buildingPrefab != null)
            {
                GameObject buildingInstance = Instantiate(buildingData.buildingPrefab, buildingData.buildingPosition, Quaternion.Euler(buildingData.buildingRotation));
                BuildingObject buildingObject = buildingInstance.GetComponent<BuildingObject>();

                if (buildingObject != null)
                {
                    buildingObject.SetBuildingData(buildingData);
                }
            }
            else
            {
                Debug.LogWarning($"Building prefab is missing for {buildingData.name}");
            }
        }
    }
}
