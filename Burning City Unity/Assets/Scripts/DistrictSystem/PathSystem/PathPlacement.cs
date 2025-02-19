using System.Collections.Generic;
using UnityEngine;

public class PathPlacement : MonoBehaviour
{
    public GameObject pointPrefab;
    // public OutlineDatabase outlineDatabase; // Asegúrate de definir e importar OutlineDatabase si es necesario
    public float movementSmoothness = 5f;
    public bool placementMode = true;
    public float curveAmount = 0.5f;
    public int interpolationSteps = 10;

    private List<List<GameObject>> paths = new List<List<GameObject>>();
    private List<GameObject> currentPath = new List<GameObject>();
    private List<GameObject> pathOutlines = new List<GameObject>();
    private LineRenderer lineRenderer;
    private GameObject pathPreview;
    private Vector3 previewOffset = Vector3.zero;
    private bool adjustingTangents = false;
    private Vector3 tangentStartPosition;
    private Vector3 controlPoint;
    private int lastPlacedPointIndex = -1;
    private Dictionary<GameObject, Vector3> controlPoints = new Dictionary<GameObject, Vector3>();

    void Start()
    {
        CreatePathPreview();
    }

    void Update()
    {
        if (placementMode)
        {
            HandleInput();
            UpdatePathPreview();
        }
        else
        {
            ClearCurrentPath();
        }
    }

    void HandleInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            PlacePointAtObtainedPosition();
            adjustingTangents = true;
        }
        if (Input.GetMouseButtonUp(0))
        {
            adjustingTangents = false;
            previewOffset = Vector3.zero;
        }
        if (adjustingTangents)
        {
            AdjustTangents();
        }
        if (Input.GetKeyDown(KeyCode.Return))
        {
            FinishCurrentPath();
        }
        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            RemoveLastPoint();
        }
    }

    void CreatePathPreview()
    {
        pathPreview = new GameObject("PathPreview");
        lineRenderer = pathPreview.AddComponent<LineRenderer>();
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;
        lineRenderer.material = new Material(Shader.Find("Sprites/Default")) { color = Color.yellow };
    }

    void UpdatePathPreview()
    {
        if (!adjustingTangents)
        {
            Vector3 instancePosition = GetMouseWorldPosition();
            pathPreview.transform.position = Vector3.Lerp(pathPreview.transform.position, instancePosition, movementSmoothness * Time.deltaTime);
        }
        UpdateLineRenderer();
    }

    Vector3 GetMouseWorldPosition()
    {
        Plane plane = new Plane(Vector3.up, Vector3.zero);
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        return plane.Raycast(ray, out float distance) ? ray.GetPoint(distance) : Vector3.zero;
    }

    void PlacePointAtObtainedPosition()
    {
        if (!pathPreview.activeSelf) return;

        GameObject newPoint = Instantiate(pointPrefab, pathPreview.transform.position, Quaternion.identity);
        currentPath.Add(newPoint);

        if (currentPath.Count > 1)
        {
            GameObject previousPoint = currentPath[currentPath.Count - 2];
            controlPoints[newPoint] = (previousPoint.transform.position + newPoint.transform.position) / 2;
        }

        UpdateLineRenderer();
    }

    void AdjustTangents()
    {
        if (currentPath.Count < 2) return;

        Vector3 mousePosition = GetMouseWorldPosition();
        GameObject lastPoint = currentPath[currentPath.Count - 1];
        controlPoints[lastPoint] = mousePosition;

        UpdateLineRenderer();
    }

    void RemoveLastPoint()
    {
        if (currentPath.Count == 0) return;

        GameObject lastPoint = currentPath[currentPath.Count - 1];
        currentPath.RemoveAt(currentPath.Count - 1);
        Destroy(lastPoint);
        controlPoints.Remove(lastPoint);

        UpdateLineRenderer();
    }

    void FinishCurrentPath()
    {
        if (currentPath.Count == 0) return;

        paths.Add(new List<GameObject>(currentPath));
        currentPath.Clear();
        controlPoints.Clear();
        UpdateLineRenderer();
    }

    void ClearCurrentPath()
    {
        foreach (GameObject point in currentPath)
        {
            Destroy(point);
        }
        currentPath.Clear();
        controlPoints.Clear();
        UpdateLineRenderer();
    }

    void UpdateLineRenderer()
    {
        if (lineRenderer == null) return;

        List<Vector3> interpolatedPoints = InterpolatePath(currentPath);
        lineRenderer.positionCount = interpolatedPoints.Count;
        for (int i = 0; i < interpolatedPoints.Count; i++)
        {
            lineRenderer.SetPosition(i, interpolatedPoints[i]);
        }
    }

    List<Vector3> InterpolatePath(List<GameObject> path)
    {
        List<Vector3> interpolatedPoints = new List<Vector3>();
        if (path.Count < 2) return interpolatedPoints;

        for (int i = 0; i < path.Count - 1; i++)
        {
            Vector3 p0 = path[i].transform.position;
            Vector3 p1 = controlPoints.ContainsKey(path[i + 1]) ? controlPoints[path[i + 1]] : (p0 + path[i + 1].transform.position) / 2;
            Vector3 p2 = path[i + 1].transform.position;

            for (int j = 0; j < interpolationSteps; j++)
            {
                float t = j / (float)interpolationSteps;
                interpolatedPoints.Add(Bezier(p0, p1, p2, t));
            }
        }

        interpolatedPoints.Add(path[path.Count - 1].transform.position);
        return interpolatedPoints;
    }

    Vector3 Bezier(Vector3 p0, Vector3 p1, Vector3 p2, float t)
    {
        float u = 1 - t;
        return u * u * p0 + 2 * u * t * p1 + t * t * p2;
    }
}
