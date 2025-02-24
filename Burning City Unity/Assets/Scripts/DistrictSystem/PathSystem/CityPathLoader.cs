using System.Collections.Generic;
using UnityEngine;

public class CityPathLoader : MonoBehaviour
{
    public CityPathDatabase pathDatabase;
    public GameObject pointPrefab;
    public Material pathMaterial;
    public float pathWidth = 1f;
    public int interpolationSteps = 10;
    public float noiseScale = 0.1f;

    void Start()
    {
        // Asegurarse de que el pathDatabase esté configurado correctamente
        if (pathDatabase == null || string.IsNullOrEmpty(pathDatabase.directoryPath))
        {
            Debug.LogError("El pathDatabase o su directorio no está configurado correctamente.");
            return;
        }

        // Actualizar la base de datos antes de cargar los caminos
        pathDatabase.UpdateDatabase();

        LoadPaths();
    }

    void LoadPaths()
    {
        foreach (var pathData in pathDatabase.paths)
        {
            List<GameObject> pathPoints = new List<GameObject>();
            foreach (var point in pathData.points)
            {
                GameObject newPoint = Instantiate(pointPrefab, point, Quaternion.identity);
                pathPoints.Add(newPoint);
            }

            CreatePathMesh(pathPoints, pathData.controlPoints, pathData.useCurves);
        }
    }

    void CreatePathMesh(List<GameObject> pathPoints, List<Vector3> controlPoints, bool useCurves)
    {
        if (pathPoints.Count < 2) return;

        GameObject pathObject = new GameObject("LoadedPath");
        MeshFilter meshFilter = pathObject.AddComponent<MeshFilter>();
        MeshRenderer meshRenderer = pathObject.AddComponent<MeshRenderer>();
        meshRenderer.material = pathMaterial ?? new Material(Shader.Find("Standard")) { color = Color.yellow };

        Mesh mesh = new Mesh();
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();

        for (int i = 0; i < pathPoints.Count - 1; i++)
        {
            Vector3 p0 = pathPoints[i].transform.position;
            Vector3 p1 = useCurves && i < controlPoints.Count ? controlPoints[i] : (p0 + pathPoints[i + 1].transform.position) / 2;
            Vector3 p2 = pathPoints[i + 1].transform.position;

            for (int j = 0; j <= interpolationSteps; j++)
            {
                float t = j / (float)interpolationSteps;
                Vector3 point = useCurves ? Bezier(p0, p1, p2, t) : Vector3.Lerp(p0, p2, t);
                Vector3 direction = ((useCurves ? Bezier(p0, p1, p2, t + 0.01f) : Vector3.Lerp(p0, p2, t + 0.01f)) - point).normalized;
                Vector3 normal = Vector3.Cross(direction, Vector3.up).normalized;

                // Añadir ruido y variación
                float noise = Mathf.PerlinNoise(point.x * noiseScale, point.z * noiseScale) * 0.1f;
                point.y += noise;

                vertices.Add(point + normal * pathWidth * 0.5f);
                vertices.Add(point - normal * pathWidth * 0.5f);

                if (vertices.Count >= 4)
                {
                    int baseIndex = vertices.Count - 4;
                    triangles.Add(baseIndex);
                    triangles.Add(baseIndex + 2);
                    triangles.Add(baseIndex + 1);
                    triangles.Add(baseIndex + 1);
                    triangles.Add(baseIndex + 2);
                    triangles.Add(baseIndex + 3);
                }
            }
        }

        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateNormals();

        meshFilter.mesh = mesh;
    }

    Vector3 Bezier(Vector3 p0, Vector3 p1, Vector3 p2, float t)
    {
        float u = 1 - t;
        return u * u * p0 + 2 * u * t * p1 + t * t * p2;
    }
}
