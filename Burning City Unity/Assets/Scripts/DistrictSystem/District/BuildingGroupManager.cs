using System.Collections.Generic;
using UnityEngine;

public class BuildingGroupManager : MonoBehaviour
{
    public BuildingDatabase buildingDatabase;
    public float groupingDistance = 10.0f;

    public Dictionary<BuildingObject.DistrictZone, List<List<Vector3>>> districtGroups = new Dictionary<BuildingObject.DistrictZone, List<List<Vector3>>>();
    public Dictionary<BuildingObject.CityRaces, List<List<Vector3>>> raceGroups = new Dictionary<BuildingObject.CityRaces, List<List<Vector3>>>();
    public List<List<Vector3>> zonasMixtas = new List<List<Vector3>>();
    public List<List<Vector3>> zonasMulticulturales = new List<List<Vector3>>();

    private List<BuildingData> buildingDataList = new List<BuildingData>();

    void Start()
    {
        if (buildingDatabase == null)
        {
            Debug.LogError("BuildingDatabase is not assigned.");
            return;
        }

        GroupBuildings();
    }

    void GroupBuildings()
    {
        foreach (BuildingData buildingData in buildingDatabase.buildingDataObjects)
        {
            if (buildingData.buildingPrefab != null)
            {
                buildingDataList.Add(buildingData);

                Vector3 position = buildingData.buildingPosition;
                position.y = 0; // Asegurarse de que la posición esté en el plano XZ

                // Agrupar por DistrictZone
                if (!districtGroups.ContainsKey(buildingData.districtZone))
                {
                    districtGroups[buildingData.districtZone] = new List<List<Vector3>>();
                }
                AddToGroup(districtGroups[buildingData.districtZone], position);

                // Agrupar por CityRaces
                if (!raceGroups.ContainsKey(buildingData.cityRaces))
                {
                    raceGroups[buildingData.cityRaces] = new List<List<Vector3>>();
                }
                AddToGroup(raceGroups[buildingData.cityRaces], position);
            }
        }

        // Identificar zonas mixtas y multiculturales
        IdentifyMixedAndMulticulturalZones();
    }

    void AddToGroup(List<List<Vector3>> groups, Vector3 position)
    {
        foreach (var group in groups)
        {
            if (IsWithinDistance(group, position))
            {
                group.Add(position);
                return;
            }
        }

        // Crear un nuevo grupo si no se encontró un grupo cercano
        groups.Add(new List<Vector3> { position });
    }

    void IdentifyMixedAndMulticulturalZones()
    {
        foreach (var buildingData in buildingDataList)
        {
            Vector3 position = buildingData.buildingPosition;
            position.y = 0; // Asegurarse de que la posición esté en el plano XZ

            // Verificar y agregar a zonas mixtas
            bool isMixedZone = false;
            foreach (var otherBuildingData in buildingDataList)
            {
                if (otherBuildingData != buildingData && Vector3.Distance(position, otherBuildingData.buildingPosition) <= groupingDistance)
                {
                    if (otherBuildingData.districtZone != buildingData.districtZone)
                    {
                        isMixedZone = true;
                        break;
                    }
                }
            }
            if (isMixedZone)
            {
                AddToGroup(zonasMixtas, position);
            }

            // Verificar y agregar a zonas multiculturales
            bool isMulticulturalZone = false;
            foreach (var otherBuildingData in buildingDataList)
            {
                if (otherBuildingData != buildingData && Vector3.Distance(position, otherBuildingData.buildingPosition) <= groupingDistance)
                {
                    if (otherBuildingData.cityRaces != buildingData.cityRaces)
                    {
                        isMulticulturalZone = true;
                        break;
                    }
                }
            }
            if (isMulticulturalZone)
            {
                AddToGroup(zonasMulticulturales, position);
            }
        }
    }

    bool IsWithinDistance(List<Vector3> group, Vector3 position)
    {
        foreach (var pos in group)
        {
            if (Vector3.Distance(pos, position) <= groupingDistance)
            {
                return true;
            }
        }
        return false;
    }
}
