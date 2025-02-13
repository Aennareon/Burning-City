using UnityEngine;

[CreateAssetMenu(fileName = "RaceChangeEvent", menuName = "History System/Race Change Event")]
public class RaceChangeEvent : BaseHistoryEvent
{
    [Header("Race Change Info")]
    public HistoryEventsData.CityRaces raceName;
    [TextArea(10, 20)]
    public string changeReason;
    public bool changeResidenceLocation;
    public HistoryEventsData.CityZones newResidenceLocation;
    public bool changeAlignment;
    [Range(0, 100)]
    public int newAlignment;
    public bool changeReligion;
    public HistoryEventsData.WorldGods newReligion;
    public bool changeCityObjective;
    public HistoryEventsData.AttitudeCityState newCityObjective;

    [Header("Race change tale")]
    [TextArea(20, 40)]
    public string raceChangeText;

    private void OnValidate()
    {
        raceChangeText = GenerateNarrative() + "\n\n" + GenerateRaceChangeNarrative();
    }

    private string GenerateRaceChangeNarrative()
    {
        string text = $"In the year {yearOfEvent}, the {raceName} underwent a profound transformation. The catalyst for this change was: {changeReason}. ";

        if (changeResidenceLocation)
        {
            text += $"They relocated to a new area within the city, settling in the {newResidenceLocation}. ";
        }
        if (changeReligion)
        {
            text += $"They abandoned their old beliefs and began to worship the god of {newReligion}, seeking new spiritual guidance. ";
        }
        if (changeCityObjective)
        {
            text += $"Their ambitions within the city evolved, now focusing on {newCityObjective}. ";
        }

        text += $"This period marked a significant chapter in the history of the {raceName}, as they adapted to new circumstances and redefined their role and influence within the city.";

        return text;
    }
}


