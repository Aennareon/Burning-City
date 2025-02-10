using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[CreateAssetMenu(fileName = "ZoneData", menuName = "CityData/Zone Data Object")]
public class ZoneData : ScriptableObject
{
    [Header("Zones Limits")]
    public List<Vector3> zoneLimits;
    public List<SubZone> subZones = new List<SubZone>();  // Inicializar la lista de subzonas
    public List<Vector3> spawnPoints = new List<Vector3>();  // Inicializar la lista de puntos de spawn

    [Header("Zone Settings")]
    public int zoneSubdivisions;

    public void CalculateZones(int numberOfDivisions, int zoneDefinition)
    {
        subZones.Clear();
        spawnPoints.Clear();

        if (zoneLimits != null && zoneLimits.Count >= 3 && numberOfDivisions > 1)
        {
            // Subdividimos la zona principal
            List<Vector3> subdividedPolygon = SubdividePolygon(zoneLimits, zoneDefinition);
            Vector3 originalCentroid = CalculateCentroid(zoneLimits);
            spawnPoints.Add(originalCentroid);

            int numberOfPoints = subdividedPolygon.Count;
            int pointsPerSubZone = Mathf.Max(1, numberOfPoints / numberOfDivisions);

            // Calculamos las subzonas
            for (int i = 0; i < numberOfDivisions; i++)
            {
                int startIdx = i * pointsPerSubZone;
                int endIdx = (i + 1) * pointsPerSubZone;

                if (i == numberOfDivisions - 1)
                {
                    endIdx = numberOfPoints;
                }

                List<Vector3> subZoneLimitsList = new List<Vector3>();
                for (int j = startIdx; j < endIdx; j++)
                {
                    subZoneLimitsList.Add(subdividedPolygon[j % numberOfPoints]);
                }

                if (i < numberOfDivisions - 1)
                {
                    subZoneLimitsList.Add(subdividedPolygon[(endIdx) % numberOfPoints]);
                }

                subZoneLimitsList.Add(originalCentroid);

                if (i == numberOfDivisions - 1)
                {
                    subZoneLimitsList[subZoneLimitsList.Count - 2] = subdividedPolygon[0];
                }

                SubZone newSubZone = new SubZone(subZoneLimitsList);
                subZones.Add(newSubZone);
            }
        }
    }

    private Vector3 CalculateCentroid(List<Vector3> points)
    {
        return points.Aggregate(Vector3.zero, (acc, p) => acc + p) / points.Count;
    }

    private List<Vector3> SubdividePolygon(List<Vector3> polygon, int subdivisions)
    {
        if (polygon == null || polygon.Count < 3 || subdivisions < 1)
        {
            Debug.LogWarning("Invalid polygon or subdivision count.");
            return polygon;
        }

        List<Vector3> subdivided = new List<Vector3>(polygon);
        for (int s = 0; s < subdivisions; s++)
        {
            List<Vector3> newPoints = new List<Vector3>();
            for (int i = 0; i < subdivided.Count; i++)
            {
                Vector3 p1 = subdivided[i];
                Vector3 p2 = subdivided[(i + 1) % subdivided.Count];
                newPoints.Add(p1);
                newPoints.Add((p1 + p2) * 0.5f); // Añadir el punto medio entre puntos consecutivos
            }
            subdivided = newPoints;
        }
        return subdivided;
    }
}

[System.Serializable]
public class SubZone
{
    public List<Vector3> limits;

    // Constructor que recibe una lista de puntos como límites para la subzona
    public SubZone(List<Vector3> limits)
    {
        this.limits = new List<Vector3>(limits);  // Se asegura de crear una copia de la lista
    }
}