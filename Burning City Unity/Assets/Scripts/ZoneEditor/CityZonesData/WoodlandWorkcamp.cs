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
