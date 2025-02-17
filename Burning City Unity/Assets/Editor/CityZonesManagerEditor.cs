using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CityZonesManager))]
public class CityZonesManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        CityZonesManager manager = (CityZonesManager)target;

        if (GUILayout.Button("Generate Zones Procedurally"))
        {
            Rect area = new Rect(0, 0, 100, 100); // Define el área donde se generarán las zonas
            int numberOfZones = 5; // Define el número de zonas a generar
            manager.GenerateZonesProcedurally(area, numberOfZones);
        }
    }
}
