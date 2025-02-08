using static CityZoneData;
using static FarmFildsData;
using UnityEngine;
using System.Collections.Generic;

[ExecuteAlways]
public class DrawZone : MonoBehaviour
{
    public FarmFildsData FarmFildsData;

    [Header("Zone Data")]
    public Vector3[] zoneLimits;  // Los límites del polígono principal
    public int NumberOfDivisions;  // Número de divisiones para la cuadrícula

    [Header("Fild Data")]
    public fildType[] productionTipes;
    public subZonePoints[] subZoneLimits;

    [Header("Centroide y Offset")]
    public Vector3 centroidOffset = Vector3.zero; // Desplazamiento del centroide

    private void OnValidate()
    {
        if (productionTipes.Length != 0)
        {
            NumberOfDivisions = productionTipes.Length;
        }
        else
        {
            NumberOfDivisions = 0;
        }
    }

    private void OnDrawGizmos()
    {
        if (zoneLimits == null || zoneLimits.Length < 2)
        {
            Debug.LogWarning("Zone has 2 or less points");
            return;
        }

        Gizmos.color = Color.green;

        // Dibujar los límites del polígono
        for (int i = 0; i < zoneLimits.Length; i++)
        {
            Vector3 start = zoneLimits[i];
            Vector3 end = zoneLimits[(i + 1) % zoneLimits.Length];
            Gizmos.DrawLine(start, end);
            Gizmos.DrawSphere(start, 0.1f);
        }

        Gizmos.DrawSphere(FindPolygonCentroid(zoneLimits), 0.2f);
        DrawSubZones();
    }

    public Vector3 FindPolygonCentroid(Vector3[] points)
    {
        if (points == null || points.Length < 3)
        {
            Debug.LogWarning("El polígono debe tener al menos 3 puntos.");
            return Vector3.zero;
        }

        float area = 0;
        float centroidX = 0;
        float centroidZ = 0;

        int numPoints = points.Length;

        for (int i = 0; i < numPoints; i++)
        {
            Vector3 current = points[i];
            Vector3 next = points[(i + 1) % numPoints]; // Cierra el polígono

            float determinant = (current.x * next.z - next.x * current.z);
            area += determinant;

            centroidX += (current.x + next.x) * determinant;
            centroidZ += (current.z + next.z) * determinant;
        }

        area *= 0.5f;

        if (Mathf.Abs(area) < Mathf.Epsilon)
        {
            Debug.LogWarning("El área del polígono es cero, no se puede calcular el centroide.");
            return Vector3.zero;
        }

        centroidX /= (6 * area);
        centroidZ /= (6 * area);

        // Aplicar el offset al centroide
        return new Vector3(centroidX, 0, centroidZ) + centroidOffset;
    }

    private Vector3[] GenerateRefinedPolygon(Vector3[] originalPolygon, int subdivisions)
    {
        List<Vector3> refinedPoints = new List<Vector3>();

        for (int i = 0; i < originalPolygon.Length; i++)
        {
            Vector3 start = originalPolygon[i];
            Vector3 end = originalPolygon[(i + 1) % originalPolygon.Length]; // Cierra el polígono

            refinedPoints.Add(start);

            // Insertar puntos adicionales en la arista
            for (int j = 1; j <= subdivisions; j++)
            {
                float t = j / (float)(subdivisions + 1);
                Vector3 newPoint = Vector3.Lerp(start, end, t);
                refinedPoints.Add(newPoint);
            }
        }

        return refinedPoints.ToArray();
    }

    private Vector3 FindClosestPointInDirection(Vector3 centroid, Vector3 direction, Vector3[] polygonPoints)
    {
        float maxDistance = float.MaxValue;
        Vector3 closestPoint = centroid;

        foreach (Vector3 point in polygonPoints)
        {
            float projectedDistance = Vector3.Dot((point - centroid), direction);
            if (projectedDistance > 0 && projectedDistance < maxDistance)
            {
                maxDistance = projectedDistance;
                closestPoint = point;
            }
        }

        return closestPoint;
    }

    public void DrawSubZones()
    {
        if (NumberOfDivisions <= 0 || zoneLimits == null || zoneLimits.Length < 3) return;

        Gizmos.color = Color.red;

        // Generar un polígono más detallado sin modificar zoneLimits
        Vector3[] refinedPolygon = GenerateRefinedPolygon(zoneLimits, 2);

        // Encontrar el centroide del polígono refinado, ahora con el offset aplicado
        Vector3 centroid = FindPolygonCentroid(refinedPolygon);

        // Lista de puntos seleccionados para cortes en el nuevo orden
        List<Vector3> selectedPoints = new List<Vector3>();

        // Alternar entre ejes X y Z
        bool useX = true;

        for (int i = 0; i < NumberOfDivisions; i++)
        {
            Vector3 bestPoint = Vector3.zero;
            float bestScore = float.MaxValue;

            // Buscar el punto más alineado en el eje correspondiente
            foreach (var point in refinedPolygon)
            {
                float score = useX ? Mathf.Abs(point.x - centroid.x) : Mathf.Abs(point.z - centroid.z);
                if (score < bestScore)
                {
                    bestScore = score;
                    bestPoint = point;
                }
            }

            // Agregar el punto seleccionado
            selectedPoints.Add(bestPoint);

            // Alternar ejes para el siguiente corte
            useX = !useX;
        }

        // Dibujar los cortes desde el centroide a los puntos seleccionados
        for (int i = 0; i < selectedPoints.Count; i++)
        {
            Vector3 direction = (selectedPoints[i] - centroid).normalized;
            Vector3 closestPoint = FindClosestPointInDirection(centroid, direction, refinedPolygon);

            Gizmos.DrawLine(centroid, closestPoint);
        }
    }
}
