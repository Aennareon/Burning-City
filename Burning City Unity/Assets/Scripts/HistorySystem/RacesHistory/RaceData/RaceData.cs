using UnityEngine;

[CreateAssetMenu(fileName = "RaceData", menuName = "History System/Race Data")]
public class RaceData : ScriptableObject
{
    public HistoryEventsData.CityRaces raceName;
    public string raceDescription;
    [Range(0, 100)]
    public int raceAlignment;
    public HistoryEventsData.WorldGods religion;
    public HistoryEventsData.CityZones residenceLocation;
    public HistoryEventsData.AttitudeCityState cityObjective;
}
