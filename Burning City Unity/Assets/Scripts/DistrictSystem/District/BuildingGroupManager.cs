using System.Collections.Generic;
using UnityEngine;

public class BuildingGroupManager : MonoBehaviour
{
    public BuildingDatabase buildingDatabase;
    public GroupDatabase groupDatabase;
    public float groupingDistance = 10.0f;

    private List<BuildingData> buildingDataList = new List<BuildingData>();

    void Start()
    {
        if (buildingDatabase == null)
        {
            Debug.LogError("BuildingDatabase is not assigned.");
            return;
        }

        if (groupDatabase == null)
        {
            Debug.LogError("GroupDatabase is not assigned.");
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
                var districtGroup = groupDatabase.districtGroups.Find(g => g.districtZone == buildingData.districtZone);
                if (districtGroup == null)
                {
                    districtGroup = new GroupDatabase.DistrictGroup { districtZone = buildingData.districtZone };
                    groupDatabase.districtGroups.Add(districtGroup);
                }
                AddToGroup(districtGroup.groups, position);

                // Agrupar por CityRaces
                var raceGroup = groupDatabase.raceGroups.Find(g => g.cityRace == buildingData.cityRaces);
                if (raceGroup == null)
                {
                    raceGroup = new GroupDatabase.RaceGroup { cityRace = buildingData.cityRaces };
                    groupDatabase.raceGroups.Add(raceGroup);
                }
                AddToGroup(raceGroup.groups, position);
            }
        }

        // Identificar zonas mixtas y multiculturales
        IdentifyMixedAndMulticulturalZones();
    }

    void AddToGroup(List<GroupDatabase.PositionGroup> groups, Vector3 position)
    {
        foreach (var group in groups)
        {
            if (IsWithinDistance(group.positions, position))
            {
                group.positions.Add(position);
                return;
            }
        }

        // Crear un nuevo grupo si no se encontró un grupo cercano
        var newGroup = new GroupDatabase.PositionGroup();
        newGroup.positions.Add(position);
        groups.Add(newGroup);
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
                AddToGroup(groupDatabase.zonasMixtas, position);
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
                AddToGroup(groupDatabase.zonasMulticulturales, position);
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
