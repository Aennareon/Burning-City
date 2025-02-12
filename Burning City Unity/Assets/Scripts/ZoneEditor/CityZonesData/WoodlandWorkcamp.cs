using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WoodWorkcampZoneData", menuName = "CityData/Zone Data/Wood Workcamp Zone")]
public class WoodlandWorkcamp : ZoneData
{
    [System.Serializable]
    public enum WoodsProductionTypes { WoodCutter, Forager, Hunter, Herbolist };

    [Header("Farm Field Settings")]
    public List<WoodsProductionTypes> listOfProductions = new List<WoodsProductionTypes>();
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
