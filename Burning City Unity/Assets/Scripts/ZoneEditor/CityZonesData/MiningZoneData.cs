using System.Collections.Generic;
using UnityEngine;
using static FarmFildData;

[CreateAssetMenu(fileName = "MiningZoneData", menuName = "CityData/Zone Data/Mining Area Zone Data Object")]
public class MiningZoneData : ZoneData
{
    [System.Serializable]
    public enum MineProductionTypes { Gold, Iron, ArcanaStones };

    [Header("Farm Field Settings")]
    public List<MineProductionTypes> listOfProductions = new List<MineProductionTypes>();
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
