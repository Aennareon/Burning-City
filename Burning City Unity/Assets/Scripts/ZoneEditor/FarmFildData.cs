using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "FarmFildData", menuName = "CityData/Farm Field Zone Data Object")]
public class FarmFildData : ZoneData
{
    [System.Serializable]
    public enum FieldProductionTypes { AnimalField, GrainField, GrassField };

    [Header("Farm Field Settings")]
    public List<FieldProductionTypes> listOfProductions = new List<FieldProductionTypes>();

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
