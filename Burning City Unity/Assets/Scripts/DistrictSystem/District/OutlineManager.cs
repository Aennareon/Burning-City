using System.Collections.Generic;
using UnityEngine;

public class OutlineManager : MonoBehaviour
{
    public GroupDatabase groupDatabase;
    public OutlineDatabase outlineDatabase;

    public bool viewByDistrictZone = true;
    public bool viewByCityRaces = false;
    public bool viewByMixedZones = false;
    public bool viewByMulticulturalZones = false;

    private List<BuildingOutline> outlines = new List<BuildingOutline>();

    void Start()
    {
        if (groupDatabase == null)
        {
            Debug.LogError("GroupDatabase is not assigned.");
            return;
        }

        if (outlineDatabase == null)
        {
            Debug.LogError("OutlineDatabase is not assigned.");
            return;
        }

        Debug.Log("Starting CreateOutlines process...");
        CreateOutlines();
        Debug.Log("Finished CreateOutlines process.");
        UpdateView();
    }

    void Update()
    {
        UpdateView();
    }

    void CreateOutlines()
    {
        // Crear outlines para DistrictZone
        foreach (var group in groupDatabase.districtGroups)
        {
            var outlinePrefab = GetOutlinePrefabForDistrict(group.districtZone);
            if (outlinePrefab != null)
            {
                foreach (var subGroup in group.groups)
                {
                    CreateOutline(subGroup.positions, group.districtZone, outlinePrefab);
                }
            }
        }

        // Crear outlines para CityRaces
        foreach (var group in groupDatabase.raceGroups)
        {
            var outlinePrefab = GetOutlinePrefabForRace(group.cityRace);
            if (outlinePrefab != null)
            {
                foreach (var subGroup in group.groups)
                {
                    CreateOutline(subGroup.positions, group.cityRace, outlinePrefab);
                }
            }
        }

        // Crear outlines para zonas mixtas
        foreach (var group in groupDatabase.zonasMixtas)
        {
            CreateOutline(group.positions, "MixedZone", outlineDatabase.mixZoneOutline);
        }

        // Crear outlines para zonas multiculturales
        foreach (var group in groupDatabase.zonasMulticulturales)
        {
            CreateOutline(group.positions, "MulticulturalZone", outlineDatabase.mixRaceOutline);
        }
    }

    GameObject GetOutlinePrefabForDistrict(BuildingObject.DistrictZone districtZone)
    {
        foreach (var outline in outlineDatabase.districtOutlines)
        {
            if (outline.districtZone == districtZone)
            {
                return outline.outlinePrefab;
            }
        }
        return null;
    }

    GameObject GetOutlinePrefabForRace(BuildingObject.CityRaces cityRace)
    {
        foreach (var outline in outlineDatabase.raceOutlines)
        {
            if (outline.cityRace == cityRace)
            {
                return outline.outlinePrefab;
            }
        }
        return null;
    }

    void CreateOutline(List<Vector3> positions, object groupKey, GameObject outlinePrefab)
    {
        if (outlinePrefab == null)
        {
            Debug.LogError($"Outline prefab for {groupKey} is not assigned.");
            return;
        }

        if (positions == null || positions.Count < 3)
        {
            Debug.LogWarning($"Not enough positions to create outline for {groupKey}.");
            return;
        }

        GameObject outlineObject = Instantiate(outlinePrefab, Vector3.zero, Quaternion.identity, transform);
        outlineObject.name = "Outline_" + groupKey.ToString();
        BuildingOutline buildingOutline = outlineObject.GetComponent<BuildingOutline>();
        buildingOutline.SetPositions(positions);
        outlines.Add(buildingOutline);
        Debug.Log($"Created outline for {groupKey} with {positions.Count} positions.");
    }

    public void SetViewByDistrictZone()
    {
        viewByDistrictZone = true;
        viewByCityRaces = false;
        viewByMixedZones = false;
        viewByMulticulturalZones = false;
    }

    public void SetViewByCityRaces()
    {
        viewByDistrictZone = false;
        viewByCityRaces = true;
        viewByMixedZones = false;
        viewByMulticulturalZones = false;
    }

    public void SetViewByMixedZones()
    {
        viewByDistrictZone = false;
        viewByCityRaces = false;
        viewByMixedZones = true;
        viewByMulticulturalZones = false;
    }

    public void SetViewByMulticulturalZones()
    {
        viewByDistrictZone = false;
        viewByCityRaces = false;
        viewByMixedZones = false;
        viewByMulticulturalZones = true;
    }

    public void ClearView()
    {
        viewByDistrictZone = false;
        viewByCityRaces = false;
        viewByMixedZones = false;
        viewByMulticulturalZones = false;
    }

    void UpdateView()
    {
        foreach (var outline in outlines)
        {
            BuildingObject.DistrictZone districtZone;
            BuildingObject.CityRaces cityRaces;

            if (viewByDistrictZone && System.Enum.TryParse(outline.name.Replace("Outline_", ""), out districtZone))
            {
                outline.gameObject.SetActive(true);
            }
            else if (viewByCityRaces && System.Enum.TryParse(outline.name.Replace("Outline_", ""), out cityRaces))
            {
                outline.gameObject.SetActive(true);
            }
            else if (viewByMixedZones && outline.name.Contains("MixedZone"))
            {
                outline.gameObject.SetActive(true);
            }
            else if (viewByMulticulturalZones && outline.name.Contains("MulticulturalZone"))
            {
                outline.gameObject.SetActive(true);
            }
            else
            {
                outline.gameObject.SetActive(false);
            }
        }
    }
}
