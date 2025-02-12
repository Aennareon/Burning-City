using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PathDraw : MonoBehaviour
{
    [System.Serializable]
    public enum PathPointsTypes { Normal, Connector, StEn }

    [System.Serializable]
    public class PathPoint
    {
        public Vector3 position;
        public PathPointsTypes pathPointType;
    }

    [System.Serializable]
    public class CrossPathPoint
    {
        public Vector3 position;
        public List<int> connectedPathsIndexes = new List<int>();
    }

    [System.Serializable]
    public class PathFlow
    {
        public List<PathPoint> pathRoute = new List<PathPoint>();
        public float pathWidth = 1;
    }

    public CityZonesManager zonesManager;
    public List<PathFlow> listOfPaths = new List<PathFlow>();
    public List<CrossPathPoint> crossPathPoints = new List<CrossPathPoint>();

    [Header("Path Settings")]
    public float maxClusterDistance = 5f; // Distancia máxima para agrupar puntos
    public int intermediatePointsPerSegment = 5; // Puntos intermedios por segmento
    public float noiseScale = 0.1f; // Escala del ruido de Perlin
    public float noiseIntensity = 0.5f; // Intensidad del ruido de Perlin

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (zonesManager == null || zonesManager.AllSpawnPoints == null) return;

        // Dibujar los puntos de spawn originales
        Gizmos.color = Color.yellow;
        foreach (var point in zonesManager.AllSpawnPoints)
        {
            Gizmos.DrawSphere(point, 0.2f);
        }

        // Dibujar los caminos
        if (listOfPaths != null)
        {
            foreach (var path in listOfPaths)
            {
                for (int i = 0; i < path.pathRoute.Count - 1; i++)
                {
                    Gizmos.color = Color.green;
                    Gizmos.DrawLine(path.pathRoute[i].position, path.pathRoute[i + 1].position);
                }
            }
        }

        // Dibujar los crossPathPoints (puntos de cruce)
        Gizmos.color = Color.blue;
        foreach (var cross in crossPathPoints)
        {
            Gizmos.DrawSphere(cross.position, 0.3f);
        }
    }
#endif

    private void OnValidate()
    {
        if (zonesManager == null || zonesManager.AllSpawnPoints == null || zonesManager.AllSpawnPoints.Count == 0) return;

        listOfPaths.Clear();
        crossPathPoints.Clear();

        // Crear caminos y puntos de cruce
        List<List<Vector3>> clusters = ClusterPoints(zonesManager.AllSpawnPoints, maxClusterDistance);
        foreach (var cluster in clusters)
        {
            Vector3 clusterCenter = cluster.Aggregate(Vector3.zero, (acc, p) => acc + p) / cluster.Count;
            crossPathPoints.Add(new CrossPathPoint { position = clusterCenter });
            listOfPaths.Add(CreatePath(cluster, clusterCenter));
        }

        // Conectar los caminos entre sí, detectando cruces
        ConnectCrossPathPoints();

        // Añadir definición a los caminos con curvas y ruido
        AddPathVariations();

        // Añadir definición a las conexiones distantes con ruido
        AddDistantConnectionsVariations();
    }

    private List<List<Vector3>> ClusterPoints(List<Vector3> points, float maxDistance)
    {
        List<List<Vector3>> clusters = new List<List<Vector3>>();
        HashSet<Vector3> visited = new HashSet<Vector3>();

        foreach (var point in points)
        {
            if (visited.Contains(point)) continue;

            List<Vector3> cluster = new List<Vector3> { point };
            visited.Add(point);

            foreach (var other in points)
            {
                if (!visited.Contains(other) && Vector3.Distance(point, other) < maxDistance)
                {
                    cluster.Add(other);
                    visited.Add(other);
                }
            }
            clusters.Add(cluster);
        }
        return clusters;
    }

    private PathFlow CreatePath(List<Vector3> points, Vector3 crossPoint)
    {
        if (points == null || points.Count == 0) return null;

        PathFlow path = new PathFlow();
        foreach (var point in points)
        {
            path.pathRoute.Add(new PathPoint
            {
                position = point,
                pathPointType = (point == points[0] || point == points[^1]) ? PathPointsTypes.StEn : PathPointsTypes.Normal
            });
        }

        // Asegurarse de que el punto de cruce está en el camino
        if (!path.pathRoute.Any(p => p.position == crossPoint))
        {
            path.pathRoute.Add(new PathPoint { position = crossPoint, pathPointType = PathPointsTypes.Connector });
        }
        return path;
    }

    private void ConnectCrossPathPoints()
    {
        for (int i = 0; i < listOfPaths.Count; i++)
        {
            for (int j = i + 1; j < listOfPaths.Count; j++)
            {
                // Detectar si los caminos se cruzan o están lo suficientemente cerca
                for (int k = 0; k < listOfPaths[i].pathRoute.Count - 1; k++)
                {
                    for (int l = 0; l < listOfPaths[j].pathRoute.Count - 1; l++)
                    {
                        Vector3 intersection = GetLineIntersection(
                            listOfPaths[i].pathRoute[k].position, listOfPaths[i].pathRoute[k + 1].position,
                            listOfPaths[j].pathRoute[l].position, listOfPaths[j].pathRoute[l + 1].position);

                        if (intersection != Vector3.zero)
                        {
                            // Crear un punto de cruce si es necesario
                            crossPathPoints.Add(new CrossPathPoint { position = intersection });

                            // Conectar los caminos a este punto de cruce
                            listOfPaths[i].pathRoute.Add(new PathPoint { position = intersection, pathPointType = PathPointsTypes.Connector });
                            listOfPaths[j].pathRoute.Add(new PathPoint { position = intersection, pathPointType = PathPointsTypes.Connector });
                        }
                    }
                }
            }
        }
    }

    private Vector3 GetLineIntersection(Vector3 p1, Vector3 p2, Vector3 q1, Vector3 q2)
    {
        float s1_x = p2.x - p1.x;
        float s1_y = p2.z - p1.z;
        float s2_x = q2.x - q1.x;
        float s2_y = q2.z - q1.z;

        float s = (-s1_y * (p1.x - q1.x) + s1_x * (p1.z - q1.z)) / (-s2_x * s1_y + s1_x * s2_y);
        float t = (s2_x * (p1.z - q1.z) - s2_y * (p1.x - q1.x)) / (-s2_x * s1_y + s1_x * s2_y);

        if (s >= 0 && s <= 1 && t >= 0 && t <= 1)
        {
            return new Vector3(p1.x + (t * s1_x), 0, p1.z + (t * s1_y));
        }

        return Vector3.zero;
    }

    private void AddPathVariations()
    {
        foreach (var path in listOfPaths)
        {
            List<PathPoint> newPath = new List<PathPoint>();

            for (int i = 0; i < path.pathRoute.Count - 1; i++)
            {
                Vector3 p0 = path.pathRoute[i].position;
                Vector3 p1 = path.pathRoute[i + 1].position;

                newPath.Add(path.pathRoute[i]);

                for (int j = 1; j <= intermediatePointsPerSegment; j++)
                {
                    float t = (float)j / (intermediatePointsPerSegment + 1);
                    Vector3 intermediatePoint = Vector3.Lerp(p0, p1, t);

                    // Añadir ruido para variar ligeramente el punto
                    float noiseX = Mathf.PerlinNoise(intermediatePoint.x * noiseScale, intermediatePoint.z * noiseScale) * noiseIntensity;
                    float noiseY = Mathf.PerlinNoise(intermediatePoint.x * noiseScale, intermediatePoint.z * noiseScale) * noiseIntensity;

                    intermediatePoint = new Vector3(intermediatePoint.x + noiseX, intermediatePoint.y, intermediatePoint.z + noiseY);

                    newPath.Add(new PathPoint { position = intermediatePoint, pathPointType = PathPointsTypes.Normal });
                }
            }

            newPath.Add(path.pathRoute.Last());
            path.pathRoute = newPath;
        }
    }

    private void AddDistantConnectionsVariations()
    {
        for (int i = 0; i < crossPathPoints.Count; i++)
        {
            for (int j = i + 1; j < crossPathPoints.Count; j++)
            {
                if (Vector3.Distance(crossPathPoints[i].position, crossPathPoints[j].position) > 10f)
                {
                    Vector3 p0 = crossPathPoints[i].position;
                    Vector3 p1 = crossPathPoints[j].position;

                    List<Vector3> newConnection = new List<Vector3> { p0 };
                    for (int k = 1; k <= intermediatePointsPerSegment; k++)
                    {
                        float t = (float)k / (intermediatePointsPerSegment + 1);
                        Vector3 intermediatePoint = Vector3.Lerp(p0, p1, t);

                        float noiseX = Mathf.PerlinNoise(intermediatePoint.x * noiseScale, intermediatePoint.z * noiseScale) * noiseIntensity;
                        float noiseY = Mathf.PerlinNoise(intermediatePoint.x * noiseScale, intermediatePoint.z * noiseScale) * noiseIntensity;

                        intermediatePoint = new Vector3(intermediatePoint.x + noiseX, intermediatePoint.y, intermediatePoint.z + noiseY);

                        newConnection.Add(intermediatePoint);
                    }

                    newConnection.Add(p1);

                    // Añadir los puntos de la nueva conexión a la lista de caminos
                    listOfPaths.Add(new PathFlow
                    {
                        pathRoute = newConnection.Select(p => new PathPoint { position = p, pathPointType = PathPointsTypes.Connector }).ToList()
                    });
                }
            }
        }
    }

    private bool IsPointInPolygon(Vector3 point, List<Vector3> polygon)
    {
        bool isInside = false;
        for (int i = 0, j = polygon.Count - 1; i < polygon.Count; j = i++)
        {
            if (((polygon[i].z > point.z) != (polygon[j].z > point.z)) &&
                (point.x < (polygon[j].x - polygon[i].x) * (point.z - polygon[i].z) / (polygon[j].z - polygon[i].z) + polygon[i].x))
            {
                isInside = !isInside;
            }
        }
        return isInside;
    }

    private bool IsPathWithinZone(PathFlow path, List<Vector3> zoneLimits)
    {
        foreach (var point in path.pathRoute)
        {
            if (!IsPointInPolygon(point.position, zoneLimits))
            {
                return false;
            }
        }
        return true;
    }
}
