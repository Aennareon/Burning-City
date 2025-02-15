using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RaceEventsTimeline", menuName = "Scriptable Objects/RaceEventsTimeline")]
public class RaceEventsTimeline : ScriptableObject
{
    public RaceDatabase raceData;
    public RaceArrivalDatabase raceArrival;
    public RaceIntermediateEventsDatabase intermediateEventsDatabase;
    public List<BaseHistoryEvent> Timeline = new List<BaseHistoryEvent>();

    [Header("Year Range")]
    public int startYear;
    public int endYear;
    public int eventInterval = 150;

    public void PopulateTimeline()
    {
        Dictionary<HistoryEventsData.CityRaces, int> raceArrivalYears = new Dictionary<HistoryEventsData.CityRaces, int>();

        foreach (var arrival in raceArrival.raceArrivals)
        {
            // Evento de llegada
            arrival.nameOfEvent = "Arrival of the " + arrival.raceName;
            arrival.yearOfEvent = Random.Range(startYear, endYear + 1);
            arrival.raceAligmentOnArrival = Random.Range(0, 101);
            Timeline.Add(arrival);
            raceArrivalYears[arrival.raceName] = arrival.yearOfEvent;
        }

        foreach (var race in raceArrivalYears.Keys)
        {
            int currentYear = raceArrivalYears[race];

            while (currentYear < endYear)
            {
                currentYear += eventInterval;

                // Evento de cambio
                var raceChangeEvent = CreateRaceChangeEvent(race, currentYear);
                if (raceChangeEvent != null)
                {
                    intermediateEventsDatabase.changeEvents.Add(raceChangeEvent);
                    Timeline.Add(raceChangeEvent);
                }

                // Evento de inicio de conflicto
                if (raceArrivalYears.Count > 1)
                {
                    var otherRace = GetRandomRaceExcluding(race, raceArrivalYears.Keys);
                    if (otherRace != null)
                    {
                        var conflictStartEvent = CreateConflictStartEvent(race, otherRace.Value, currentYear);
                        if (conflictStartEvent != null)
                        {
                            intermediateEventsDatabase.conflictStartEvents.Add(conflictStartEvent);
                            Timeline.Add(conflictStartEvent);

                            // Evento de fin de conflicto
                            var conflictEndEvent = CreateConflictEndEvent(conflictStartEvent);
                            if (conflictEndEvent != null)
                            {
                                intermediateEventsDatabase.conflictEndEvents.Add(conflictEndEvent);
                                Timeline.Add(conflictEndEvent);

                                // No generar más conflictos para estas razas en el rango de tiempo del conflicto
                                currentYear = conflictEndEvent.yearOfEvent + Random.Range(5, 25);
                            }
                        }
                    }
                }
            }
        }
    }

    private RaceConflictStartEvent CreateConflictStartEvent(HistoryEventsData.CityRaces race1, HistoryEventsData.CityRaces race2, int year)
    {
        var conflictStartEvent = ScriptableObject.CreateInstance<RaceConflictStartEvent>();
        conflictStartEvent.yearOfEvent = year;
        conflictStartEvent.race1 = race1;
        conflictStartEvent.race2 = race2;
        conflictStartEvent.conflictReason = "Territorial dispute";
        conflictStartEvent.isWar = true;
        conflictStartEvent.nameOfEvent = "Conflict Start: " + conflictStartEvent.race1 + " vs " + conflictStartEvent.race2;
        return conflictStartEvent;
    }

    private RaceConflictEndEvent CreateConflictEndEvent(RaceConflictStartEvent conflictStartEvent)
    {
        var conflictEndEvent = ScriptableObject.CreateInstance<RaceConflictEndEvent>();
        conflictEndEvent.yearOfEvent = conflictStartEvent.yearOfEvent + Random.Range(5, 25); // Año aleatorio después del inicio del conflicto
        conflictEndEvent.race1 = conflictStartEvent.race1;
        conflictEndEvent.race2 = conflictStartEvent.race2;
        conflictEndEvent.resolutionReason = "Peace treaty";
        conflictEndEvent.isPeaceTreaty = true;
        conflictEndEvent.nameOfEvent = "Conflict End: " + conflictEndEvent.race1 + " vs " + conflictEndEvent.race2;
        return conflictEndEvent;
    }

    private RaceChangeEvent CreateRaceChangeEvent(HistoryEventsData.CityRaces race, int year)
    {
        var raceChangeEvent = ScriptableObject.CreateInstance<RaceChangeEvent>();
        raceChangeEvent.yearOfEvent = year;
        raceChangeEvent.raceName = race;
        raceChangeEvent.changeReason = "Cultural assimilation";
        raceChangeEvent.changeAlignment = true;
        raceChangeEvent.newAlignment = Random.Range(0, 101);
        raceChangeEvent.nameOfEvent = "Race Change: " + raceChangeEvent.raceName;
        return raceChangeEvent;
    }

    private HistoryEventsData.CityRaces? GetRandomRaceExcluding(HistoryEventsData.CityRaces excludeRace, ICollection<HistoryEventsData.CityRaces> availableRaces)
    {
        var races = new List<HistoryEventsData.CityRaces>(availableRaces);
        races.Remove(excludeRace);
        if (races.Count == 0) return null;
        return races[Random.Range(0, races.Count)];
    }
}
