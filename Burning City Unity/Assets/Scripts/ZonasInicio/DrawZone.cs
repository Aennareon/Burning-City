using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[System.Serializable]
public class SubZone
{
    public List<Vector3> limits;

    public SubZone(List<Vector3> limits)
    {
        this.limits = new List<Vector3>(limits); // Hacemos una copia de la lista.
    }
}

public class DrawZone : MonoBehaviour
{
    [Header("Zones Limits")]

    public Vector3 zonePositionOffset;
    public List<Vector3> zoneLimits;
    public List<SubZone> subZones = new List<SubZone>();  // Inicializar la lista de subzonas

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
        if(numberOfDivisions >= 2)
        {
            DrawSubZones();  // Ahora se encarga de crear las subzonas
        }
        else
        {
            subZones.Clear();
        }
    }
    void OnDrawGizmos()
    {
        if (drawZone)
        {
            Gizmos.color = Color.green;
            DrawPolygonGizmo(SubdividePolygon(zoneLimits, zoneDefinition));  // Mostramos el pol�gono subdividido
        }
        if (drawSubZones)
        {
            foreach (SubZone zone in subZones)
            {
                Gizmos.color = Color.red;
                DrawPolygonGizmo(zone.limits);  // Dibuja las subzonas con subdivisi�n
            }
        }
        if (drawCenters)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(CalculateCentroid(SubdividePolygon(zoneLimits, zoneDefinition)), 0.3f);
            foreach (SubZone zone in subZones)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawSphere(CalculateCentroid(zone.limits), 0.3f);
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
    private void DrawSubZones()
    {
        // Limpiar las subzonas existentes para volver a crear nuevas
        subZones.Clear();

        // Subdividir el pol�gono original (ajustar el n�mero de subdivisiones)
        List<Vector3> subdividedPolygon = SubdividePolygon(zoneLimits, zoneDefinition); // Aqu� podemos ajustar el n�mero de subdivisiones

        // Calcular el centroide del pol�gono original
        Vector3 originalCentroid = CalculateCentroid(zoneLimits); // Este es el centroide del pol�gono original

        // Dividir el pol�gono subdividido en partes iguales seg�n el n�mero de divisiones
        int numberOfPoints = subdividedPolygon.Count;
        int pointsPerSubZone = numberOfPoints / numberOfDivisions;

        for (int i = 0; i < numberOfDivisions; i++)
        {
            int startIdx = i * pointsPerSubZone;
            int endIdx = (i + 1) * pointsPerSubZone;

            // Si es la �ltima subzona, asegurarse de incluir todos los puntos restantes
            if (i == numberOfDivisions - 1)
            {
                endIdx = numberOfPoints;  // Incluir todos los puntos restantes en la �ltima subzona
            }

            // Obtener la subzona
            List<Vector3> subZoneLimits = new List<Vector3>();

            for (int j = startIdx; j < endIdx; j++)
            {
                subZoneLimits.Add(subdividedPolygon[j % numberOfPoints]); // Usamos el operador % para manejar el caso del �ltimo punto
            }

            // Reemplazar el �ltimo punto de la subzona por el centroide del pol�gono original
            subZoneLimits[subZoneLimits.Count - 1] = originalCentroid;

            // Crear una nueva subzona y agregarla
            SubZone newSubZone = new SubZone(subZoneLimits);
            subZones.Add(newSubZone);
        }
    }
    private Vector3 CalculateCentroid(List<Vector3> points)
    {
        return points.Aggregate(Vector3.zero, (acc, p) => acc + p) / points.Count;
    }
    private List<Vector3> SubdividePolygon(List<Vector3> polygon, int subdivisions)
    {
        if (polygon == null || polygon.Count < 3 || subdivisions < 1) return polygon;

        List<Vector3> subdivided = new List<Vector3>(polygon);
        for (int s = 0; s < subdivisions; s++)
        {
            List<Vector3> newPoints = new List<Vector3>();
            for (int i = 0; i < subdivided.Count; i++)
            {
                Vector3 p1 = subdivided[i];
                Vector3 p2 = subdivided[(i + 1) % subdivided.Count];
                newPoints.Add(p1);
                newPoints.Add((p1 + p2) * 0.5f); // Punto medio
            }
            subdivided = newPoints;
        }
        return subdivided;
    }
}
