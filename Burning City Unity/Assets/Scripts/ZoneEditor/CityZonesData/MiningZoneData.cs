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

        // Llamamos expl�citamente a la actualizaci�n de las zonas
        UpdateZoneData();
    }

    // M�todo para actualizar las zonas
    public void UpdateZoneData()
    {
        // Aqu� recalculamos las zonas usando los valores actuales de `FarmFildData`
        CalculateZones(zoneSubdivisions, 1);
    }
}
