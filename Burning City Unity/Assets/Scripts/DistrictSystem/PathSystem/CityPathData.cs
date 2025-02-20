using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CustomCityPathData", menuName = "City Data/Custom City Path Data")]
public class CustomCityPathData : ScriptableObject
{
    public List<Vector3> points = new List<Vector3>();
    public List<Vector3> controlPoints = new List<Vector3>();
    public bool useCurves;
}
