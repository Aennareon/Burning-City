using System.IO;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class CityDataManager : MonoBehaviour
{
    public string dataPath = "Assets/Data/CityData";
    public OutlineDatabase sharedOutlineDatabase; // Referencia a la instancia compartida de OutlineDatabase

    void Awake()
    {
        // Verificar si existe algún CityDataObject
        CityDataObject[] cityDataObjects = Resources.FindObjectsOfTypeAll<CityDataObject>();
        if (cityDataObjects.Length == 0)
        {
            // Si no existe ningún CityDataObject, crear uno nuevo
            CreateNewCityDataObject(true);
        }
        else
        {
            // Si existe al menos uno, inicializar el que esté activo
            InitializeActiveCityDataObject();
        }
    }

    public void CreateNewCityDataObject(bool setActive = false)
    {
        string cityName = GetNextCityName();

        // Crear un nuevo CityDataObject
        CityDataObject newCityDataObject = ScriptableObject.CreateInstance<CityDataObject>();
        newCityDataObject.cityName = cityName;
        newCityDataObject.DataPath = Path.Combine(dataPath, cityName);

        // Crear las carpetas de datos
        string cityFolderPath = newCityDataObject.DataPath;
        string buildingsFolderPath = Path.Combine(cityFolderPath, "Buildings");
        string pathsFolderPath = Path.Combine(cityFolderPath, "Paths");

        if (!Directory.Exists(cityFolderPath))
        {
            Directory.CreateDirectory(cityFolderPath);
        }

        if (!Directory.Exists(buildingsFolderPath))
        {
            Directory.CreateDirectory(buildingsFolderPath);
        }

        if (!Directory.Exists(pathsFolderPath))
        {
            Directory.CreateDirectory(pathsFolderPath);
        }

        // Crear y asignar las bases de datos
        BuildingDatabase buildingDatabase = CreateAndSaveScriptableObject<BuildingDatabase>(cityFolderPath, "BuildingDatabase.asset");
        CityPathDatabase cityPathDatabase = CreateAndSaveScriptableObject<CityPathDatabase>(cityFolderPath, "CityPathDatabase.asset");
        GroupDatabase groupDatabase = CreateAndSaveScriptableObject<GroupDatabase>(cityFolderPath, "GroupDatabase.asset");

        // Inicializar las bases de datos en el CityDataObject
        newCityDataObject.InitializeDatabases(buildingDatabase, cityPathDatabase, sharedOutlineDatabase, groupDatabase);

        // Guardar el CityDataObject en el directorio de datos
        string assetPath = Path.Combine(dataPath, "CityDataObjects", cityName + ".asset");
        string cityDataObjectsFolderPath = Path.Combine(dataPath, "CityDataObjects");
        if (!Directory.Exists(cityDataObjectsFolderPath))
        {
            Directory.CreateDirectory(cityDataObjectsFolderPath);
        }

#if UNITY_EDITOR
        AssetDatabase.CreateAsset(newCityDataObject, assetPath);
        AssetDatabase.SaveAssets();
#endif

        // Si no hay ningún CityDataObject activo, hacer que este sea el activo
        if (setActive || !AnyCityDataObjectActive())
        {
            SetActiveCity(newCityDataObject);
        }

        Debug.Log("Nuevo CityDataObject creado: " + cityName);
    }

    private string GetNextCityName()
    {
        int maxCityNumber = 0;
        string[] cityDataObjectPaths = Directory.GetFiles(Path.Combine(dataPath, "CityDataObjects"), "*.asset");

        foreach (string path in cityDataObjectPaths)
        {
            string fileName = Path.GetFileNameWithoutExtension(path);
            if (fileName.StartsWith("City"))
            {
                string numberPart = fileName.Substring(4);
                if (int.TryParse(numberPart, out int cityNumber))
                {
                    if (cityNumber > maxCityNumber)
                    {
                        maxCityNumber = cityNumber;
                    }
                }
            }
        }

        return "City" + (maxCityNumber + 1);
    }

    public void DestroyCityDataObject(CityDataObject cityDataObject)
    {
        if (cityDataObject != null)
        {
            string cityFolderPath = cityDataObject.DataPath;

            // Eliminar el CityDataObject
#if UNITY_EDITOR
            AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(cityDataObject));
            AssetDatabase.SaveAssets();
#endif

            // Eliminar el directorio asociado
            if (Directory.Exists(cityFolderPath))
            {
                Directory.Delete(cityFolderPath, true);
            }

            Debug.Log("CityDataObject y su directorio asociado han sido eliminados: " + cityDataObject.cityName);
        }
        else
        {
            Debug.LogError("CityDataObject no está asignado.");
        }
    }

    public void InitializeActiveCityDataObject()
    {
        CityDataObject[] cityDataObjects = Resources.FindObjectsOfTypeAll<CityDataObject>();
        foreach (CityDataObject cityDataObject in cityDataObjects)
        {
            if (cityDataObject.ActiveCity)
            {
                cityDataObject.UpdateCityManagers();
                Debug.Log("CityDataObject activo inicializado: " + cityDataObject.cityName);
                return;
            }
        }

        Debug.LogError("No se encontró ningún CityDataObject activo.");
    }

    private bool AnyCityDataObjectActive()
    {
        CityDataObject[] cityDataObjects = Resources.FindObjectsOfTypeAll<CityDataObject>();
        foreach (CityDataObject cityDataObject in cityDataObjects)
        {
            if (cityDataObject.ActiveCity)
            {
                return true;
            }
        }
        return false;
    }

    private void SetActiveCity(CityDataObject cityDataObject)
    {
        CityDataObject[] cityDataObjects = Resources.FindObjectsOfTypeAll<CityDataObject>();
        foreach (CityDataObject obj in cityDataObjects)
        {
            obj.ActiveCity = false;
        }
        cityDataObject.ActiveCity = true;
        Debug.Log("CityDataObject activo establecido: " + cityDataObject.cityName);
    }

    private T CreateAndSaveScriptableObject<T>(string folderPath, string fileName) where T : ScriptableObject
    {
        T scriptableObject = ScriptableObject.CreateInstance<T>();
        string assetPath = Path.Combine(folderPath, fileName);
#if UNITY_EDITOR
        AssetDatabase.CreateAsset(scriptableObject, assetPath);
        AssetDatabase.SaveAssets();
#endif
        return scriptableObject;
    }
}
