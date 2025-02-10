using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DataObjects", menuName = "CityData/Zone Data Object")]
public class ZoneData : ScriptableObject
{
    [Header("Zones Limits")]
    public List<Vector3> zoneLimits;
    public List<SubZone> subZones = new List<SubZone>();  // Inicializar la lista de subzonas
    public List<Vector3> spawnPoints = new List<Vector3>();  // Inicializar la lista de puntos de spawn
}
