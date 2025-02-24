using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PathPlacement : MonoBehaviour
{
    public GameObject pointPrefab;
    public GameObject controlPointPrefab;
    public GameObject previewPointPrefab;
    public float movementSmoothness = 5f;
    public float curveAmount = 0.5f;
    public int interpolationSteps = 10;
    public bool placementMode = true;
    public bool useCurves = true;
    public bool enableSnap = true;
    public float snapDistance = 1f;
    public float pathWidth = 1f;
    public Material pathMaterial;
    public BuildingDatabase buildingDatabase;
    public CityPathManager pathManager;
    public float noiseScale = 0.1f;

    private List<GameObject> currentPath = new List<GameObject>();
    private List<MeshFilter> pathMeshes = new List<MeshFilter>();
    private MeshFilter meshFilter;
    private GameObject pathPreview;
    private GameObject previewPoint;
    private Dictionary<GameObject, Vector3> controlPoints = new Dictionary<GameObject, Vector3>();
    private Dictionary<GameObject, GameObject> controlPointObjects = new Dictionary<GameObject, GameObject>();
    private HashSet<Vector3> existingPoints = new HashSet<Vector3>();
    private GameObject selectedControlPoint = null;
    private GameObject adjustingCurveFor = null;

    void Start()
    {
        CreatePathPreview();
        CreatePreviewPoint();
    }

    void Update()
    {
        if (placementMode) HandleInput();
        if (selectedControlPoint != null) MoveControlPoint();
        if (adjustingCurveFor != null) AdjustCurve();
        UpdatePreviewPoint();
    }

    void HandleInput()
    {
        if (Input.GetMouseButtonDown(0)) PlaceOrSelectPoint();
        if (Input.GetMouseButton(0)) adjustingCurveFor = currentPath.LastOrDefault();
        if (Input.GetMouseButtonUp(0)) { selectedControlPoint = null; adjustingCurveFor = null; }
        if (Input.GetKeyDown(KeyCode.Return)) FinishCurrentPath();
        if (Input.GetKeyDown(KeyCode.Backspace)) RemoveLastPoint();
        if (Input.GetKeyDown(KeyCode.C)) useCurves = !useCurves;
        if (Input.GetKeyDown(KeyCode.S)) enableSnap = !enableSnap;
    }

    void CreatePathPreview()
    {
        pathPreview = new GameObject("PathPreview");
        meshFilter = pathPreview.AddComponent<MeshFilter>();
        var renderer = pathPreview.AddComponent<MeshRenderer>();
        renderer.material = pathMaterial ?? new Material(Shader.Find("Standard")) { color = Color.yellow };
    }

    void CreatePreviewPoint()
    {
        previewPoint = Instantiate(previewPointPrefab);
        previewPoint.SetActive(false);
    }

    Vector3 GetMouseWorldPosition()
    {
        Plane plane = new Plane(Vector3.up, Vector3.zero);
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        return plane.Raycast(ray, out float distance) ? ray.GetPoint(distance) : Vector3.zero;
    }

    void PlaceOrSelectPoint()
    {
        Vector3 position = GetSnappedPosition(GetMouseWorldPosition());

        foreach (var controlPoint in controlPointObjects.Values)
        {
            if (Vector3.Distance(position, controlPoint.transform.position) < snapDistance)
            {
                selectedControlPoint = controlPoint;
                return;
            }
        }

        if (existingPoints.Contains(position) && !adjustingCurveFor) return;

        GameObject newPoint = Instantiate(pointPrefab, position, Quaternion.identity);
        currentPath.Add(newPoint);
        existingPoints.Add(position);

        if (currentPath.Count > 1 && useCurves)
        {
            GameObject previousPoint = currentPath[currentPath.Count - 2];
            Vector3 controlPointPosition = (previousPoint.transform.position + newPoint.transform.position) / 2;
            GameObject controlPoint = Instantiate(controlPointPrefab, controlPointPosition, Quaternion.identity);
            controlPoints[newPoint] = controlPointPosition;
            controlPointObjects[newPoint] = controlPoint;
        }

        UpdateMesh(meshFilter, currentPath);
    }

    void AdjustCurve()
    {
        if (!useCurves || adjustingCurveFor == null) return;

        Vector3 position = GetMouseWorldPosition();
        if (controlPoints.ContainsKey(adjustingCurveFor))
        {
            controlPoints[adjustingCurveFor] = position;
            controlPointObjects[adjustingCurveFor].transform.position = position;
            UpdateMesh(meshFilter, currentPath);
        }
    }

    void MoveControlPoint()
    {
        if (selectedControlPoint == null) return;

        Vector3 position = GetMouseWorldPosition();
        selectedControlPoint.transform.position = position;

        foreach (var kvp in controlPointObjects)
        {
            if (kvp.Value == selectedControlPoint)
            {
                controlPoints[kvp.Key] = position;
                break;
            }
        }

        UpdateMesh(meshFilter, currentPath);
    }

    void RemoveLastPoint()
    {
        if (currentPath.Count == 0) return;

        GameObject lastPoint = currentPath.Last();
        currentPath.RemoveAt(currentPath.Count - 1);
        existingPoints.Remove(lastPoint.transform.position);
        if (controlPointObjects.ContainsKey(lastPoint))
        {
            Destroy(controlPointObjects[lastPoint]);
            controlPointObjects.Remove(lastPoint);
        }
        controlPoints.Remove(lastPoint);
        Destroy(lastPoint);

        UpdateMesh(meshFilter, currentPath);
    }

    void FinishCurrentPath()
    {
        if (currentPath.Count == 0) return;

        pathManager.SavePath(currentPath, controlPoints, useCurves);

        GameObject finishedPath = new GameObject("FinishedPath");
        MeshFilter finishedMeshFilter = finishedPath.AddComponent<MeshFilter>();
        MeshRenderer finishedMeshRenderer = finishedPath.AddComponent<MeshRenderer>();
        finishedMeshRenderer.material = pathMaterial ?? new Material(Shader.Find("Standard")) { color = Color.yellow };

        pathMeshes.Add(finishedMeshFilter);
        UpdateMesh(finishedMeshFilter, currentPath);

        currentPath.Clear();
        controlPoints.Clear();
        foreach (var controlPoint in controlPointObjects.Values)
        {
            Destroy(controlPoint);
        }
        controlPointObjects.Clear();
    }

    void UpdateMesh(MeshFilter meshFilter, List<GameObject> path)
    {
        if (meshFilter == null || path.Count < 2) return;

        Mesh mesh = new Mesh();
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();

        for (int i = 0; i < path.Count - 1; i++)
        {
            Vector3 p0 = path[i].transform.position;
            Vector3 p1 = controlPoints.ContainsKey(path[i + 1]) && useCurves ? controlPoints[path[i + 1]] : (p0 + path[i + 1].transform.position) / 2;
            Vector3 p2 = path[i + 1].transform.position;

            for (int j = 0; j <= interpolationSteps; j++)
            {
                float t = j / (float)interpolationSteps;
                Vector3 point = useCurves ? Bezier(p0, p1, p2, t) : Vector3.Lerp(p0, p2, t);
                Vector3 direction = ((useCurves ? Bezier(p0, p1, p2, t + 0.01f) : Vector3.Lerp(p0, p2, t + 0.01f)) - point).normalized;
                Vector3 normal = Vector3.Cross(direction, Vector3.up).normalized;
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

    void UpdatePreviewPoint()
    {
        if (!placementMode || previewPoint == null) return;

        Vector3 position = GetSnappedPosition(GetMouseWorldPosition());
        previewPoint.transform.position = position;
        previewPoint.SetActive(true);
    }

    Vector3 Bezier(Vector3 p0, Vector3 p1, Vector3 p2, float t)
    {
        float u = 1 - t;
        return u * u * p0 + 2 * u * t * p1 + t * t * p2;
    }

    Vector3 GetSnappedPosition(Vector3 originalPosition)
    {
        if (!enableSnap) return originalPosition;

        Vector3 closestPoint = originalPosition;
        float closestDistance = snapDistance;

        foreach (var point in existingPoints)
        {
            if (currentPath.Any(p => p.transform.position == point)) continue; // Excluir puntos del mismo camino
            if (controlPoints.Values.Contains(point)) continue; // Excluir puntos de control
            float distance = Vector3.Distance(originalPosition, point);
            if (distance < closestDistance)
            {
                closestPoint = point;
                closestDistance = distance;
            }
        }

        foreach (var pathMesh in pathMeshes)
        {
            foreach (var vertex in pathMesh.mesh.vertices)
            {
                float distance = Vector3.Distance(originalPosition, vertex);
                if (distance < closestDistance)
                {
                    closestPoint = vertex;
                    closestDistance = distance;
                }
            }
        }

        if (buildingDatabase != null)
        {
            foreach (var doorPosition in buildingDatabase.GetAllDoorPositions())
            {
                float distance = Vector3.Distance(originalPosition, doorPosition);
                if (distance < closestDistance)
                {
                    closestPoint = doorPosition;
                    closestDistance = distance;
                }
            }
        }

        return closestPoint;
    }
}
