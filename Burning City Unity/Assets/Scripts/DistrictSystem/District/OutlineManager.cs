using System.Collections.Generic;
using UnityEngine;

public class OutlineManager : MonoBehaviour
{
    public BuildingGroupManager buildingGroupManager;
    public OutlineDatabase outlineDatabase;

    public bool viewByDistrictZone = true;
    public bool viewByCityRaces = false;
    public bool viewByMixedZones = false;
    public bool viewByMulticulturalZones = false;

    private List<BuildingOutline> outlines = new List<BuildingOutline>();

    void Start()
    {
        if (buildingGroupManager == null)
        {
            Debug.LogError("BuildingGroupManager is not assigned.");
            return;
        }

        if (outlineDatabase == null)
        {
            Debug.LogError("OutlineDatabase is not assigned.");
            return;
        }

        CreateOutlines();
        UpdateView();
    }

    void Update()
    {
        UpdateView();
    }

    void CreateOutlines()
    {
        // Crear outlines para DistrictZone
        foreach (var group in buildingGroupManager.districtGroups)
        {
            var outlinePrefab = GetOutlinePrefabForDistrict(group.Key);
            if (outlinePrefab != null)
            {
                foreach (var subGroup in group.Value)
                {
                    CreateOutline(subGroup, group.Key, outlinePrefab);
                }
            }
        }

        // Crear outlines para CityRaces
        foreach (var group in buildingGroupManager.raceGroups)
        {
            var outlinePrefab = GetOutlinePrefabForRace(group.Key);
            if (outlinePrefab != null)
            {
                foreach (var subGroup in group.Value)
                {
                    CreateOutline(subGroup, group.Key, outlinePrefab);
                }
            }
        }

        // Crear outlines para zonas mixtas
        foreach (var group in buildingGroupManager.zonasMixtas)
        {
            CreateOutline(group, "MixedZone", outlineDatabase.mixZoneOutline);
        }

        // Crear outlines para zonas multiculturales
        foreach (var group in buildingGroupManager.zonasMulticulturales)
        {
            CreateOutline(group, "MulticulturalZone", outlineDatabase.mixRaceOutline);
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
        GameObject outlineObject = Instantiate(outlinePrefab, Vector3.zero, Quaternion.identity, transform);
        outlineObject.name = "Outline_" + groupKey.ToString();
        BuildingOutline buildingOutline = outlineObject.GetComponent<BuildingOutline>();
        buildingOutline.SetPositions(positions);
        outlines.Add(buildingOutline);
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
