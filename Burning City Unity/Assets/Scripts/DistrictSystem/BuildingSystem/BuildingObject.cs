using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class BuildingObject : MonoBehaviour
{
    public enum BuildingType { Palace, House, SawMill, Werehouse }
    public enum DistrictZone { Royal, Arcana, Production, Residential }
    public enum CityRaces { Humans, Elfs, Orcs }

    public BuildingType buildingType;
    public DistrictZone districtZone;
    public CityRaces cityRaces;

    public Vector3 doorPosition;
    public GameObject buildingPrefab;

    public float gizmoSize = 1;

    private BuildingData buildingData;
    private BuildingDataManager buildingDataManager;

#if UNITY_EDITOR
    private static bool isExitingPlayMode = false;

    [InitializeOnLoadMethod]
    private static void OnLoad()
    {
        EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
    }

    private static void OnPlayModeStateChanged(PlayModeStateChange state)
    {
        if (state == PlayModeStateChange.ExitingPlayMode)
        {
            isExitingPlayMode = true;
        }
        else if (state == PlayModeStateChange.EnteredEditMode)
        {
            isExitingPlayMode = false;
        }
    }
#endif

    public void SetOriginalPrefab(GameObject prefab)
    {
        buildingPrefab = prefab;
    }

    public void SetBuildingData(BuildingData data)
    {
        buildingData = data;
        UpdateBuildingFromData();
    }

    private void Start()
    {
        buildingDataManager = Object.FindFirstObjectByType<BuildingDataManager>();
        if (buildingDataManager != null && buildingData == null)
        {
            buildingData = buildingDataManager.CreateBuildingData(this);
        }
        else if (buildingData == null)
        {
            Debug.LogError("BuildingDataManager not found in the scene.");
        }
    }

    private void OnValidate()
    {
        if (buildingData != null)
        {
            BuildingDataManager.UpdateBuildingData(this, buildingData);
        }
    }

    private void OnDestroy()
    {
#if UNITY_EDITOR
        if (Application.isPlaying && !isExitingPlayMode)
        {
            if (buildingDataManager != null)
            {
                buildingDataManager.DeleteBuildingData(buildingData);
            }
        }
#else
        if (buildingDataManager != null)
        {
            buildingDataManager.DeleteBuildingData(buildingData);
        }
#endif
    }

    private void UpdateBuildingFromData()
    {
        if (buildingData != null)
        {
            buildingType = buildingData.buildingType;
            districtZone = buildingData.districtZone;
            cityRaces = buildingData.cityRaces;
            doorPosition = buildingData.doorPosition;
            buildingPrefab = buildingData.buildingPrefab;
        }
    }

    public void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        DrawGizmoCircle(gameObject.transform.position, gizmoSize);
        Gizmos.color = Color.red;

        // Calcular la posición de la puerta con la rotación del edificio
        Vector3 rotatedDoorPosition = Quaternion.Euler(0, transform.eulerAngles.y, 0) * doorPosition;
        Gizmos.DrawSphere(gameObject.transform.position + rotatedDoorPosition, 0.3f);
    }

    private void DrawGizmoCircle(Vector3 center, float radius)
    {
        int segments = 360;
        float angle = 0f;

        Vector3 lastPoint = center + new Vector3(Mathf.Cos(angle) * radius, 0, Mathf.Sin(angle) * radius);
        for (int i = 1; i <= segments; i++)
        {
            angle += 2 * Mathf.PI / segments;
            Vector3 nextPoint = center + new Vector3(Mathf.Cos(angle) * radius, 0, Mathf.Sin(angle) * radius);
            Gizmos.DrawLine(lastPoint, nextPoint);
            lastPoint = nextPoint;
        }
    }
}
