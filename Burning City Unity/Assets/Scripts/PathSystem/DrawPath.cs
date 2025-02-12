using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PathDraw : MonoBehaviour
{
    [System.Serializable]
    public enum pathPointsTypes { normal, connector, StEn }

    [System.Serializable]
    public class PathPoint
    {
        public Vector3 position;
        public pathPointsTypes pathPointType;
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

    public List<Vector3> pointsToConnect = new List<Vector3>();
    public List<PathFlow> listOfPaths = new List<PathFlow>();
    public List<CrossPathPoint> crossPathPoints = new List<CrossPathPoint>();

    private void OnValidate()
    {
        if (listOfPaths == null)
            listOfPaths = new List<PathFlow>();

        if (pointsToConnect == null || pointsToConnect.Count == 0) return;

        var orderedPoints = OrderPoints(pointsToConnect);
        if (listOfPaths.Count == 0)
            listOfPaths.Add(CreatePath(orderedPoints));
        else
            listOfPaths[0] = CreatePath(orderedPoints);
    }

    private List<Vector3> OrderPoints(List<Vector3> points)
    {
        List<Vector3> ordered = new List<Vector3>();
        HashSet<Vector3> visited = new HashSet<Vector3>();

        Vector3 current = points.OrderBy(p => p.x).ThenBy(p => p.z).First();
        ordered.Add(current);
        visited.Add(current);

        while (ordered.Count < points.Count)
        {
            Vector3 next = points.Where(p => !visited.Contains(p))
                                 .OrderBy(p => Vector3.Distance(current, p))
                                 .FirstOrDefault();
            if (next != Vector3.zero)
            {
                ordered.Add(next);
                visited.Add(next);
                current = next;
            }
            else
                break;
        }

        return ordered;
    }

    private PathFlow CreatePath(List<Vector3> points)
    {
        if (points == null || points.Count == 0)
            return new PathFlow();

        PathFlow path = new PathFlow();
        foreach (var point in points)
        {
            path.pathRoute.Add(new PathPoint
            {
                position = point,
                pathPointType = (point == points[0] || point == points[^1]) ? pathPointsTypes.StEn : pathPointsTypes.normal
            });
        }
        return path;
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        foreach (Vector3 point in pointsToConnect)
        {
            Gizmos.DrawSphere(point, 0.2f);
        }

        if (listOfPaths != null)
        {
            foreach (var path in listOfPaths)
            {
                for (int i = 0; i < path.pathRoute.Count; i++)
                {
                    Gizmos.color = path.pathRoute[i].pathPointType switch
                    {
                        pathPointsTypes.StEn => Color.red,
                        pathPointsTypes.connector => Color.blue,
                        _ => Color.green,
                    };
                    Gizmos.DrawSphere(path.pathRoute[i].position, 0.2f);
                }
                for (int i = 0; i < path.pathRoute.Count - 1; i++)
                {
                    Gizmos.color = Color.green;
                    Gizmos.DrawLine(path.pathRoute[i].position, path.pathRoute[i + 1].position);
                }
            }
        }

        Gizmos.color = Color.blue;
        if (crossPathPoints != null)
        {
            foreach (CrossPathPoint point in crossPathPoints)
            {
                Gizmos.DrawSphere(point.position, 0.2f);
            }
        }
    }
#endif
}
