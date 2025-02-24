using JetBrains.Annotations;
using UnityEngine;
using System.IO; // Importar para Path y Directory
#if UNITY_EDITOR
using UnityEditor; // Importar para AssetDatabase
#endif

[CreateAssetMenu(fileName = "CityDataObject", menuName = "City Data/City Data Object")]
public class CityDataObject : ScriptableObject
{
    [Header("City Settings")]
    public string cityName;
    public bool ActiveCity;
    public string DataPath;

    [Header("City Data")]
    public BuildingDatabase buildingDatabase;
    public CityPathDatabase CityPathDatabase;
    public GroupDatabase groupDatabase;
    //------------------------------------------
    private BuildingDataManager buildingDataManager;
    private BuildingInstantiator buildingInstantiator;
    private BuildingGroupManager buildingGroupManager;
    private OutlineManager outlineManager;
    private PathPlacement pathPlacement;
    private CityPathManager cityPathManager;
    private CityPathLoader cityPathLoader;

    public void InitializeDatabases(BuildingDatabase buildingDb, CityPathDatabase cityPathDb, OutlineDatabase outlineDb, GroupDatabase groupDb)
    {
        buildingDatabase = buildingDb;
        CityPathDatabase = cityPathDb;
        groupDatabase = groupDb;

        // Configurar el directorio de la base de datos de edificios
        buildingDatabase.directoryPath = Path.Combine(DataPath, cityName, "Buildings");
        buildingDatabase.UpdateDatabase();

        // Configurar el directorio de la base de datos de caminos
        CityPathDatabase.directoryPath = Path.Combine(DataPath, cityName, "Paths");
        CityPathDatabase.UpdateDatabase();
    }

    public void UpdateCityManagers()
    {
        if (ActiveCity)
        {
            if (!GetManagersFromScene())
            {
                Debug.LogError("No se pudieron encontrar todos los managers necesarios en la escena.");
                return;
            }

            buildingDataManager.buildingDatabase = buildingDatabase;
            buildingInstantiator.buildingDatabase = buildingDatabase;
            buildingDatabase.directoryPath = DataPath + "/Buildings";

            buildingGroupManager.buildingDatabase = buildingDatabase;
            buildingGroupManager.groupDatabase = groupDatabase;
            outlineManager.groupDatabase = groupDatabase;

            pathPlacement.buildingDatabase = buildingDatabase;
            pathPlacement.pathManager = cityPathManager;
            cityPathManager.pathDatabase = CityPathDatabase;
            cityPathManager.pathDataDirectory = DataPath + "/Paths";
            cityPathLoader.pathDatabase = CityPathDatabase;
            CityPathDatabase.directoryPath = DataPath + "/Paths";
        }
        else
        {
            return;
        }
    }

    public bool GetManagersFromScene()
    {
        buildingDataManager = Object.FindFirstObjectByType<BuildingDataManager>();
        buildingInstantiator = Object.FindFirstObjectByType<BuildingInstantiator>();
        buildingGroupManager = Object.FindFirstObjectByType<BuildingGroupManager>();
        outlineManager = Object.FindFirstObjectByType<OutlineManager>();
        pathPlacement = Object.FindFirstObjectByType<PathPlacement>();
        cityPathManager = Object.FindFirstObjectByType<CityPathManager>();
        cityPathLoader = Object.FindFirstObjectByType<CityPathLoader>();

        return buildingDataManager != null && buildingInstantiator != null && buildingGroupManager != null &&
               outlineManager != null && pathPlacement != null && cityPathManager != null && cityPathLoader != null;
    }

    public void CreateCityDataFolders()
    {
        string cityFolderPath = Path.Combine(DataPath, cityName);
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

        SaveScriptableObject(buildingDatabase, cityFolderPath, "BuildingDatabase.asset");
        SaveScriptableObject(CityPathDatabase, cityFolderPath, "CityPathDatabase.asset");
        SaveScriptableObject(groupDatabase, cityFolderPath, "GroupDatabase.asset");

        Debug.Log("Carpetas y archivos de datos creados para la ciudad: " + cityName);
    }

    private void SaveScriptableObject(ScriptableObject scriptableObject, string folderPath, string fileName)
    {
        string assetPath = Path.Combine(folderPath, fileName);
        AssetDatabase.CreateAsset(scriptableObject, assetPath);
        AssetDatabase.SaveAssets();
    }
}
