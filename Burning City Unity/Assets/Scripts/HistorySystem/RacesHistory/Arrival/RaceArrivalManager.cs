using UnityEditor;
using UnityEngine;
using System.IO;

public class RaceArrivalManager : MonoBehaviour
{
    private static string dataPath = "Assets/Data/CityData/HistoryData/RacesData/RaceArrivals";
    private static string databasePath = "Assets/Data/CityData/HistoryData/RacesData/RaceArrivalDatabase.asset";

    [MenuItem("Tools/Update Race Arrivals")]
    public static void UpdateRaceArrivals()
    {
        // Asegúrate de que la carpeta existe
        if (!Directory.Exists(dataPath))
        {
            Directory.CreateDirectory(dataPath);
        }

        // Cargar o crear la base de datos
        RaceArrivalDatabase database = AssetDatabase.LoadAssetAtPath<RaceArrivalDatabase>(databasePath);
        if (database == null)
        {
            database = ScriptableObject.CreateInstance<RaceArrivalDatabase>();
            AssetDatabase.CreateAsset(database, databasePath);
            Debug.Log("Created new Race Arrival Database");
        }

        // Limpiar la lista de la base de datos
        database.raceArrivals.Clear();

        // Obtén todas las razas
        var raceNames = System.Enum.GetValues(typeof(HistoryEventsData.CityRaces));

        // Recorre cada raza y crea/actualiza el objeto correspondiente
        foreach (HistoryEventsData.CityRaces race in raceNames)
        {
            string assetPath = $"{dataPath}/{race.ToString()}_Arrival.asset";
            NewRaceHV raceArrival = AssetDatabase.LoadAssetAtPath<NewRaceHV>(assetPath);

            if (raceArrival == null)
            {
                // Crear un nuevo objeto si no existe
                raceArrival = ScriptableObject.CreateInstance<NewRaceHV>();
                raceArrival.raceName = race;
                AssetDatabase.CreateAsset(raceArrival, assetPath);
                Debug.Log($"Created new race arrival event for {race}");
            }
            else
            {
                // Actualizar el objeto existente
                raceArrival.raceName = race;
                EditorUtility.SetDirty(raceArrival);
                Debug.Log($"Updated race arrival event for {race}");
            }

            // Añadir el objeto a la base de datos
            database.raceArrivals.Add(raceArrival);
        }

        // Eliminar objetos que ya no corresponden a ninguna raza
        var existingAssets = Directory.GetFiles(dataPath, "*.asset");
        foreach (var asset in existingAssets)
        {
            string fileName = Path.GetFileNameWithoutExtension(asset);
            string raceName = fileName.Replace("_Arrival", "");
            if (!System.Enum.IsDefined(typeof(HistoryEventsData.CityRaces), raceName))
            {
                AssetDatabase.DeleteAsset(asset);
                Debug.Log($"Deleted obsolete race arrival event for {raceName}");
            }
        }

        // Marcar la base de datos como sucia para asegurar que se guarden los cambios
        EditorUtility.SetDirty(database);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
}
