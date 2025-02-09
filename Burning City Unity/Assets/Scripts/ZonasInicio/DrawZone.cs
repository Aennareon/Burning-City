using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Collections;

[System.Serializable]
public class SubZone
{
    public List<Vector3> limits;

    public SubZone(List<Vector3> limits)
    {
        this.limits = new List<Vector3>(limits); // Crear una copia de la lista
    }
}

public class DrawZone : MonoBehaviour
{
    [Header("DATA")]
    public ZoneData zoneData;

    [Header("Zones Limits")]
    public List<Vector3> zoneLimits;
    public List<SubZone> subZones = new List<SubZone>();  // Inicializar la lista de subzonas
    public List<Vector3> spawnPoints = new List<Vector3>();  // Inicializar la lista de puntos de spawn

    [Header("Zone Settings")]
    [Range(0, 10)]
    public int numberOfDivisions;
    [Range(1, 5)]
    public int zoneDefinition;

    [Header("Gizmo Settings")]
    public bool drawZone;
    public bool drawSubZones;
    public bool drawCenters;

    private void OnValidate()
    {
        //Zone Managment
        spawnPoints.Clear();
        if (numberOfDivisions >= 2)
        {
            StartCoroutine(DrawSubZonesCoroutine());  // Coroutine para subdividir zonas
        }
        else
        {
            subZones.Clear();
        }
        spawnPoints.Add(CalculateCentroid(zoneLimits));
        foreach (SubZone zone in subZones)
        {
            spawnPoints.Add(CalculateCentroid(zone.limits));
        }

        //Data managment
        UpdateZoneData();
    }

    void OnDrawGizmos()
    {
        if (drawZone)
        {
            Gizmos.color = Color.green;
            List<Vector3> subdividedPolygon = SubdividePolygon(zoneLimits, zoneDefinition);  // Dibujar la zona subdividida
            DrawPolygonGizmo(subdividedPolygon);
        }

        if (drawSubZones)
        {
            foreach (SubZone zone in subZones)
            {
                Gizmos.color = Color.red;
                DrawPolygonGizmo(zone.limits);  // Dibujar las subzonas
            }
        }

        if (drawCenters)
        {
            Gizmos.color = Color.blue;
            Vector3 originalCentroid = CalculateCentroid(zoneLimits);
            Gizmos.DrawSphere(originalCentroid, 0.3f);  // Dibujar el centroide de la zona principal
            foreach (SubZone zone in subZones)
            {
                Vector3 subZoneCentroid = CalculateCentroid(zone.limits);
                Gizmos.DrawSphere(subZoneCentroid, 0.3f);  // Dibujar los centroides de las subzonas
            }
        }
    }

    private void DrawPolygonGizmo(List<Vector3> points)
    {
        if (points == null || points.Count < 3) return;

        for (int i = 0; i < points.Count; i++)
        {
            Gizmos.DrawLine(points[i], points[(i + 1) % points.Count]);
            Gizmos.DrawSphere(points[i], 0.2f);
        }
    }

    private IEnumerator DrawSubZonesCoroutine()
    {
        // Limpiar subzones para evitar acumulación de datos
        subZones.Clear();
        Debug.Log("Starting Subzone Creation");

        List<Vector3> subdividedPolygon = SubdividePolygon(zoneLimits, zoneDefinition);  // Subdividir la zona principal
        Vector3 originalCentroid = CalculateCentroid(zoneLimits);  // Calcular el centroide del polígono original
        Vector3 firstPoint = subdividedPolygon[0];  // Guardar el primer punto del polígono original

        int numberOfPoints = subdividedPolygon.Count;
        int pointsPerSubZone = Mathf.Max(1, numberOfPoints / numberOfDivisions);

        // Subdividir en partes según el número de divisiones
        for (int i = 0; i < numberOfDivisions; i++)
        {
            int startIdx = i * pointsPerSubZone;
            int endIdx = (i + 1) * pointsPerSubZone;

            // Si es la última subzona, aseguramos que no quede fuera de rango
            if (i == numberOfDivisions - 1)
            {
                endIdx = numberOfPoints;
            }

            List<Vector3> subZoneLimits = new List<Vector3>();

            // Agregar los puntos de la subzona desde el polígono subdividido
            for (int j = startIdx; j < endIdx; j++)
            {
                subZoneLimits.Add(subdividedPolygon[j % numberOfPoints]);
            }

            // Asegurarse de que el último punto de una subzona sea el primero de la siguiente
            if (i < numberOfDivisions - 1)
            {
                subZoneLimits.Add(subdividedPolygon[(endIdx) % numberOfPoints]);  // Añadir el primer punto de la siguiente subzona
            }

            // Añadir el centroide del polígono original como el último punto de la subzona
            subZoneLimits.Add(originalCentroid);

            // Si es la última subzona, cambiar el penúltimo valor para que sea el primer punto del polígono original
            if (i == numberOfDivisions - 1)
            {
                subZoneLimits[subZoneLimits.Count - 2] = firstPoint;  // Penúltimo valor será el primer punto original
            }

            // Crear la subzona con los puntos calculados
            SubZone newSubZone = new SubZone(subZoneLimits);
            subZones.Add(newSubZone);

            Debug.Log($"Created SubZone {i + 1} with {subZoneLimits.Count} points.");
            yield return null;  // Yield para permitir actualizaciones en el editor
        }

        Debug.Log("Subzone Creation Completed");
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
                newPoints.Add((p1 + p2) * 0.5f);  // Añadir el punto medio entre puntos consecutivos
            }
            subdivided = newPoints;
        }
        return subdivided;
    }

    private void UpdateZoneData()
    {
        zoneData.zoneLimits = zoneLimits;
        zoneData.subZones = subZones;
        zoneData.spawnPoints = spawnPoints;
    }
}
