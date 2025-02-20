using System.Collections.Generic;
using UnityEngine;
using System.IO;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class CustomCityPathManager : MonoBehaviour
{
    public CustomCityPathDatabase pathDatabase;
    public string pathDataDirectory = "Assets/Data/CityData/Paths"; // Directorio para guardar los PathData

    public void SavePath(List<GameObject> pathPoints, Dictionary<GameObject, Vector3> controlPoints, bool useCurves)
    {
        CustomCityPathData newPathData = CreatePathData(pathPoints, controlPoints, useCurves);
        SavePathDataToFile(newPathData);
        pathDatabase.paths.Add(newPathData);
#if UNITY_EDITOR
        pathDatabase.UpdateDatabase();
#endif
    }

    public void LoadPath(CustomCityPathData pathData, out List<Vector3> points, out List<Vector3> controlPoints, out bool useCurves)
    {
        points = new List<Vector3>(pathData.points);
        controlPoints = new List<Vector3>(pathData.controlPoints);
        useCurves = pathData.useCurves;
    }

    private CustomCityPathData CreatePathData(List<GameObject> pathPoints, Dictionary<GameObject, Vector3> controlPoints, bool useCurves)
    {
        CustomCityPathData newPathData = ScriptableObject.CreateInstance<CustomCityPathData>();
        foreach (var point in pathPoints)
        {
            newPathData.points.Add(point.transform.position);
        }
        foreach (var controlPoint in controlPoints.Values)
        {
            newPathData.controlPoints.Add(controlPoint);
        }
        newPathData.useCurves = useCurves;
        return newPathData;
    }

    private void SavePathDataToFile(CustomCityPathData pathData)
    {
#if UNITY_EDITOR
        if (!Directory.Exists(pathDataDirectory))
        {
            Directory.CreateDirectory(pathDataDirectory);
        }

        string uniqueFileName = $"{pathDataDirectory}/PathData_{System.Guid.NewGuid()}.asset";
        AssetDatabase.CreateAsset(pathData, uniqueFileName);
        AssetDatabase.SaveAssets();
#endif
    }
}
