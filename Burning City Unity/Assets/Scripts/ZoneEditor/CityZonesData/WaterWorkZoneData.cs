using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WaterWorkcampZoneData", menuName = "CityData/Zone Data/Water Workcamp Zone")]
public class WaterWorkZoneData : ZoneData
{
    [System.Serializable]

    public enum WaterProductionTypes { Fish, WaterMill};

    [Header("Farm Field Settings")]
    public List<WaterProductionTypes> listOfProductions = new List<WaterProductionTypes>();
    private void OnValidate()
    {
        zoneSubdivisions = listOfProductions.Count;
        Debug.Log("zoneSubdivisions adjusted");

        // Llamamos explícitamente a la actualización de las zonas
        UpdateZoneData();
    }

    // Método para actualizar las zonas
    public void UpdateZoneData()
    {
        // Aquí recalculamos las zonas usando los valores actuales de `FarmFildData`
        CalculateZones(zoneSubdivisions, 1);
    }
}
