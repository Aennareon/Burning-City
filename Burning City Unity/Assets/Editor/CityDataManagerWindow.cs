using UnityEditor;
using UnityEngine;

public class CityDataManagerWindow : EditorWindow
{
    private CityDataManager cityDataManager;

    [MenuItem("Window/City Data Manager")]
    public static void ShowWindow()
    {
        GetWindow<CityDataManagerWindow>("City Data Manager");
    }

    private void OnGUI()
    {
        GUILayout.Label("City Data Manager", EditorStyles.boldLabel);

        cityDataManager = (CityDataManager)EditorGUILayout.ObjectField("City Data Manager", cityDataManager, typeof(CityDataManager), true);

        if (cityDataManager == null)
        {
            EditorGUILayout.HelpBox("Please assign a City Data Manager.", MessageType.Warning);
            return;
        }

        if (GUILayout.Button("Create New City Data Object"))
        {
            CreateNewCityDataObject();
        }

        if (GUILayout.Button("Delete Active City Data Object"))
        {
            DeleteActiveCityDataObject();
        }
    }

    private void CreateNewCityDataObject()
    {
        if (cityDataManager != null)
        {
            cityDataManager.CreateNewCityDataObject(true);
            Debug.Log("New City Data Object created.");
        }
        else
        {
            Debug.LogError("City Data Manager is not assigned.");
        }
    }

    private void DeleteActiveCityDataObject()
    {
        if (cityDataManager != null)
        {
            CityDataObject activeCity = GetActiveCityDataObject();
            if (activeCity != null)
            {
                cityDataManager.DestroyCityDataObject(activeCity);
                Debug.Log("Active City Data Object deleted.");
            }
            else
            {
                Debug.LogError("No active City Data Object found.");
            }
        }
        else
        {
            Debug.LogError("City Data Manager is not assigned.");
        }
    }

    private CityDataObject GetActiveCityDataObject()
    {
        CityDataObject[] cityDataObjects = Resources.FindObjectsOfTypeAll<CityDataObject>();
        foreach (CityDataObject cityDataObject in cityDataObjects)
        {
            if (cityDataObject.ActiveCity)
            {
                return cityDataObject;
            }
        }
        return null;
    }
}

