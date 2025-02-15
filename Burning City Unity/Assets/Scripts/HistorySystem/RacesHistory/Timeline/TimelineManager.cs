using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.IO;

public class TimelineManager : MonoBehaviour
{
    private static string raceDataPath = "Assets/Data/CityData/HistoryData/RacesData/RaceData";
    private static string raceArrivalPath = "Assets/Data/CityData/HistoryData/RacesData/RaceArrivals";
    private static string intermediateEventsPath = "Assets/Data/CityData/HistoryData/RacesData/IntermediateEvents";
    private static string raceDatabasePath = "Assets/Data/CityData/HistoryData/RacesData/RaceDatabase.asset";
    private static string raceArrivalDatabasePath = "Assets/Data/CityData/HistoryData/RacesData/RaceArrivalDatabase.asset";
    private static string intermediateEventsDatabasePath = "Assets/Data/CityData/HistoryData/RacesData/RaceIntermediateEventsDatabase.asset";
    private static string timelinePath = "Assets/Data/CityData/HistoryData/RacesData/RaceEventsTimeline.asset";

    [MenuItem("Tools/Generate Timeline")]
    public static void GenerateTimeline()
    {
        // Paso 1: Crear los archivos de datos de las razas
        RaceDataManager.UpdateRaceData();

        // Paso 2: Crear los archivos de llegada de las razas
        RaceArrivalManager.UpdateRaceArrivals();

        // Paso 3: Crear la línea de tiempo y los eventos intermedios
        CreateTimelineAndEvents();
    }

    private static void CreateTimelineAndEvents()
    {
        // Cargar o crear la base de datos de eventos intermedios
        RaceIntermediateEventsDatabase intermediateEventsDatabase = AssetDatabase.LoadAssetAtPath<RaceIntermediateEventsDatabase>(intermediateEventsDatabasePath);
        if (intermediateEventsDatabase == null)
        {
            intermediateEventsDatabase = ScriptableObject.CreateInstance<RaceIntermediateEventsDatabase>();
            AssetDatabase.CreateAsset(intermediateEventsDatabase, intermediateEventsDatabasePath);
            Debug.Log("Created new Race Intermediate Events Database");
        }

        // Limpiar las listas de la base de datos
        intermediateEventsDatabase.conflictStartEvents.Clear();
        intermediateEventsDatabase.conflictEndEvents.Clear();
        intermediateEventsDatabase.changeEvents.Clear();

        // Cargar la base de datos de llegadas
        RaceArrivalDatabase raceArrivalDatabase = AssetDatabase.LoadAssetAtPath<RaceArrivalDatabase>(raceArrivalDatabasePath);
        if (raceArrivalDatabase == null)
        {
            Debug.LogError("Race Arrival Database not found!");
            return;
        }

        // Crear la línea de tiempo
        RaceEventsTimeline timeline = AssetDatabase.LoadAssetAtPath<RaceEventsTimeline>(timelinePath);
        if (timeline == null)
        {
            timeline = ScriptableObject.CreateInstance<RaceEventsTimeline>();
            AssetDatabase.CreateAsset(timeline, timelinePath);
            Debug.Log("Created new Race Events Timeline");
        }

        timeline.raceArrival = raceArrivalDatabase;
        timeline.intermediateEventsDatabase = intermediateEventsDatabase;
        timeline.startYear = 0; // Ajusta según sea necesario
        timeline.endYear = 1000; // Ajusta según sea necesario
        timeline.eventInterval = 150; // Ajusta según sea necesario

        timeline.PopulateTimeline();

        // Verificar y eliminar elementos vacíos en la base de datos
        intermediateEventsDatabase.RemoveNullElements();

        // Guardar los eventos intermedios en archivos
        SaveIntermediateEvents(intermediateEventsDatabase);

        // Marcar la base de datos y la línea de tiempo como sucias para asegurar que se guarden los cambios
        EditorUtility.SetDirty(intermediateEventsDatabase);
        EditorUtility.SetDirty(timeline);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    private static void SaveIntermediateEvents(RaceIntermediateEventsDatabase intermediateEventsDatabase)
    {
        // Asegúrate de que las carpetas existen
        string conflictStartPath = Path.Combine(intermediateEventsPath, "ConflictStart");
        string conflictEndPath = Path.Combine(intermediateEventsPath, "ConflictEnd");
        string changeEventPath = Path.Combine(intermediateEventsPath, "ChangeEvent");

        if (!Directory.Exists(conflictStartPath))
        {
            Directory.CreateDirectory(conflictStartPath);
        }
        if (!Directory.Exists(conflictEndPath))
        {
            Directory.CreateDirectory(conflictEndPath);
        }
        if (!Directory.Exists(changeEventPath))
        {
            Directory.CreateDirectory(changeEventPath);
        }

        // Eliminar archivos existentes
        DeleteExistingFiles(conflictStartPath);
        DeleteExistingFiles(conflictEndPath);
        DeleteExistingFiles(changeEventPath);

        // Guardar eventos de inicio de conflicto
        foreach (var conflictStartEvent in intermediateEventsDatabase.conflictStartEvents)
        {
            string assetPath = $"{conflictStartPath}/{SanitizeFileName(conflictStartEvent.nameOfEvent)}_Start.asset";
            SaveOrUpdateAsset(conflictStartEvent, assetPath);
        }

        // Guardar eventos de fin de conflicto
        foreach (var conflictEndEvent in intermediateEventsDatabase.conflictEndEvents)
        {
            string assetPath = $"{conflictEndPath}/{SanitizeFileName(conflictEndEvent.nameOfEvent)}_End.asset";
            SaveOrUpdateAsset(conflictEndEvent, assetPath);
        }

        // Guardar eventos de cambio de raza
        foreach (var changeEvent in intermediateEventsDatabase.changeEvents)
        {
            string assetPath = $"{changeEventPath}/{SanitizeFileName(changeEvent.nameOfEvent)}_Change.asset";
            SaveOrUpdateAsset(changeEvent, assetPath);
        }
    }

    private static void DeleteExistingFiles(string path)
    {
        var files = Directory.GetFiles(path, "*.asset");
        foreach (var file in files)
        {
            AssetDatabase.DeleteAsset(file);
        }
    }

    private static void SaveOrUpdateAsset(Object asset, string path)
    {
        if (File.Exists(path))
        {
            AssetDatabase.DeleteAsset(path);
        }
        AssetDatabase.CreateAsset(asset, path);
    }

    private static string SanitizeFileName(string fileName)
    {
        foreach (char c in Path.GetInvalidFileNameChars())
        {
            fileName = fileName.Replace(c, '_');
        }
        return fileName;
    }
}
