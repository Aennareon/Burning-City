using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class BuildingOutline : MonoBehaviour
{
    public Color lineColor = Color.red;
    public float lineWidth = 0.1f;
    public float distanceFromBuildings = 1.0f;
    public bool showLine = true; // Variable para controlar la visibilidad de la línea

    private LineRenderer lineRenderer;
    private List<Vector3> buildingPositions = new List<Vector3>();

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.useWorldSpace = true;
        lineRenderer.loop = true;

        // Activar el LineRenderer al inicio del modo de juego
        lineRenderer.enabled = showLine;
    }

    void Update()
    {
        // Actualizar color y grosor de la línea en tiempo real
        lineRenderer.startColor = lineColor;
        lineRenderer.endColor = lineColor;
        lineRenderer.startWidth = lineWidth;
        lineRenderer.endWidth = lineWidth;

        // Controlar la visibilidad de la línea
        lineRenderer.enabled = showLine;

        if (showLine)
        {
            DrawOutline();
        }
    }

    public void SetPositions(List<Vector3> positions)
    {
        buildingPositions = positions;
    }

    void DrawOutline()
    {
        if (buildingPositions.Count < 3)
        {
            lineRenderer.positionCount = 0;
            return;
        }

        List<Vector3> hull = CalculateConvexHull(buildingPositions);
        Vector3 centroid = CalculateCentroid(hull);
        Vector3[] outlinePoints = new Vector3[hull.Count];

        for (int i = 0; i < hull.Count; i++)
        {
            Vector3 direction = (hull[i] - centroid).normalized;
            outlinePoints[i] = hull[i] + direction * distanceFromBuildings;
            outlinePoints[i].y = 0; // Asegurarse de que la posición esté en el plano XZ
        }

        lineRenderer.positionCount = outlinePoints.Length;
        lineRenderer.SetPositions(outlinePoints);
    }

    List<Vector3> CalculateConvexHull(List<Vector3> points)
    {
        // Implementación del algoritmo de Andrew para calcular el casco convexo
        points.Sort((a, b) => a.x.CompareTo(b.x));
        List<Vector3> hull = new List<Vector3>();

        // Construir la mitad inferior del casco convexo
        foreach (Vector3 point in points)
        {
            while (hull.Count >= 2 && Cross(hull[hull.Count - 2], hull[hull.Count - 1], point) <= 0)
            {
                hull.RemoveAt(hull.Count - 1);
            }
            hull.Add(point);
        }

        // Construir la mitad superior del casco convexo
        int t = hull.Count + 1;
        for (int i = points.Count - 1; i >= 0; i--)
        {
            Vector3 point = points[i];
            while (hull.Count >= t && Cross(hull[hull.Count - 2], hull[hull.Count - 1], point) <= 0)
            {
                hull.RemoveAt(hull.Count - 1);
            }
            hull.Add(point);
        }

        hull.RemoveAt(hull.Count - 1);
        return hull;
    }

    Vector3 CalculateCentroid(List<Vector3> points)
    {
        Vector3 centroid = Vector3.zero;
        foreach (Vector3 point in points)
        {
            centroid += point;
        }
        centroid /= points.Count;
        return centroid;
    }

    float Cross(Vector3 o, Vector3 a, Vector3 b)
    {
        return (a.x - o.x) * (b.z - o.z) - (a.z - o.z) * (b.x - o.x);
    }
}
