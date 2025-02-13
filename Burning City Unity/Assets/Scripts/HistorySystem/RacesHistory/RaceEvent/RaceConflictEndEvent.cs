using UnityEngine;

[CreateAssetMenu(fileName = "RaceConflictEndEvent", menuName = "History System/Race Conflict End Event")]
public class RaceConflictEndEvent : BaseHistoryEvent
{
    [Header("Race Conflict End Info")]
    public HistoryEventsData.CityRaces race1;
    public HistoryEventsData.CityRaces race2;
    [TextArea(10, 20)]
    public string resolutionReason;
    public bool isExtinction;
    public bool isExpulsion;
    public bool isPeaceTreaty;
    public bool isSurrender;

    [Header("Race conflict resolution tale")]
    [TextArea(20, 40)]
    public string raceConflictResolutionText;

    private void OnValidate()
    {
        raceConflictResolutionText = GenerateNarrative() + "\n\n" + GenerateRaceConflictResolutionNarrative();
    }

    private string GenerateRaceConflictResolutionNarrative()
    {
        string text = $"In the year {yearOfEvent}, the conflict between the {race1} and the {race2} finally came to an end. The resolution was brought about by: {resolutionReason}. ";

        if (isExtinction)
        {
            text += $"Tragically, the conflict resulted in the extinction of the {race2}, leaving a void in the city's population. ";
        }
        if (isExpulsion)
        {
            text += $"The {race2} were expelled from the city, forced to find a new home elsewhere. ";
        }
        if (isPeaceTreaty)
        {
            text += $"A peace treaty was signed, bringing an end to the hostilities and paving the way for a fragile peace. ";
        }
        if (isSurrender)
        {
            text += $"The {race2} surrendered, accepting the terms imposed by the {race1}. ";
        }

        text += $"This event marked the conclusion of a dark chapter in the history of both the {race1} and the {race2}, as they began the long process of healing and rebuilding.";

        return text;
    }
}




