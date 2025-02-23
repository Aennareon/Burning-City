using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CustomCityPathDatabase", menuName = "City Data/Custom City Path Database")]
public class CustomCityPathDatabase : ScriptableObject
{
    public List<CustomCityPathData> paths = new List<CustomCityPathData>();

#if UNITY_EDITOR
    public void UpdateDatabase()
    {
        // Lógica para actualizar la base de datos
        paths.Clear();
        string[] guids = UnityEditor.AssetDatabase.FindAssets("t:CustomCityPathData", new[] { "Assets/Data/CityData/Paths" });
        foreach (string guid in guids)
        {
            string path = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
            CustomCityPathData pathData = UnityEditor.AssetDatabase.LoadAssetAtPath<CustomCityPathData>(path);
            if (pathData != null)
            {
                paths.Add(pathData);
            }
        }

        // Limpiar elementos vacíos
        paths.RemoveAll(item => item == null);
    }
#endif
}
