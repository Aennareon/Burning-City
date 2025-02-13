using UnityEditor;
using UnityEngine;
using System.IO;
using System.Collections.Generic;

public class RaceDataManager : MonoBehaviour
{
    private static string raceDataPath = "Assets/Data/CityData/HistoryData/RacesData/RaceData";
    private static string raceArrivalPath = "Assets/Data/CityData/HistoryData/RacesData/RaceArrivals";
    private static string databasePath = "Assets/Data/CityData/HistoryData/RacesData/RaceDatabase.asset";

    [MenuItem("Tools/Update Race Data")]
    public static void UpdateRaceData()
    {
        // Asegúrate de que las carpetas existen
        if (!Directory.Exists(raceDataPath))
        {
            Directory.CreateDirectory(raceDataPath);
        }

        // Cargar o crear la base de datos
        RaceDatabase database = AssetDatabase.LoadAssetAtPath<RaceDatabase>(databasePath);
        if (database == null)
        {
            database = ScriptableObject.CreateInstance<RaceDatabase>();
            AssetDatabase.CreateAsset(database, databasePath);
            Debug.Log("Created new Race Database");
        }

        // Limpiar la lista de la base de datos
        database.races.Clear();

        // Obtén todas las razas
        var raceNames = System.Enum.GetValues(typeof(HistoryEventsData.CityRaces));

        // Recorre cada raza y crea/actualiza el objeto correspondiente
        foreach (HistoryEventsData.CityRaces race in raceNames)
        {
            string raceDataAssetPath = $"{raceDataPath}/{race.ToString()}_Data.asset";
            RaceData raceData = AssetDatabase.LoadAssetAtPath<RaceData>(raceDataAssetPath);

            if (raceData == null)
            {
                // Crear un nuevo objeto si no existe
                raceData = ScriptableObject.CreateInstance<RaceData>();
                raceData.raceName = race;
                AssetDatabase.CreateAsset(raceData, raceDataAssetPath);
                Debug.Log($"Created new race data for {race}");
            }

            // Actualizar la información del objeto RaceData
            string raceArrivalAssetPath = $"{raceArrivalPath}/{race.ToString()}_Arrival.asset";
            NewRaceHV raceArrival = AssetDatabase.LoadAssetAtPath<NewRaceHV>(raceArrivalAssetPath);

            if (raceArrival != null)
            {
                raceData.raceDescription = raceArrival.raceDescription;
                raceData.raceAlignment = raceArrival.raceAligmentOnArrival;
                raceData.religion = raceArrival.religionOnArrival;
                raceData.residenceLocation = raceArrival.moveInLocation;
                raceData.cityObjective = raceArrival.attitudeCityState;
                EditorUtility.SetDirty(raceData);
                Debug.Log($"Updated race data for {race}");
            }

            // Añadir el objeto a la base de datos
            database.races.Add(raceData);
        }

        // Marcar la base de datos como sucia para asegurar que se guarden los cambios
        EditorUtility.SetDirty(database);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    public static List<NewRaceHV> GetRaceEvents(HistoryEventsData.CityRaces race)
    {
        List<NewRaceHV> raceEvents = new List<NewRaceHV>();
        string[] allAssets = AssetDatabase.FindAssets("t:NewRaceHV", new[] { raceArrivalPath });

        foreach (string guid in allAssets)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            NewRaceHV raceEvent = AssetDatabase.LoadAssetAtPath<NewRaceHV>(assetPath);

            if (raceEvent != null && raceEvent.raceName == race)
            {
                raceEvents.Add(raceEvent);
            }
        }

        return raceEvents;
    }
}

