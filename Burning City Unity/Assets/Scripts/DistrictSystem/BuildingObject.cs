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

    public float gizmoSize = 1;

    private BuildingData buildingData;

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

    private void Start()
    {
        buildingData = BuildingDataManager.CreateBuildingData(this);
    }

    private void OnValidate()
    {
        BuildingDataManager.UpdateBuildingData(this, buildingData);
    }

    private void OnDestroy()
    {
#if UNITY_EDITOR
        if (Application.isPlaying && !isExitingPlayMode)
        {
            BuildingDataManager.DeleteBuildingData(buildingData);
        }
#else
        BuildingDataManager.DeleteBuildingData(buildingData);
#endif
    }

    public void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        DrawGizmoCircle(gameObject.transform.position, gizmoSize);
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(gameObject.transform.position + doorPosition, 0.3f);
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
