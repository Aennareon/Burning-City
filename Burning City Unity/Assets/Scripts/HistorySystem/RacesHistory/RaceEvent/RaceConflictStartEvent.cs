using UnityEngine;

[CreateAssetMenu(fileName = "RaceConflictStartEvent", menuName = "History System/Race Conflict Start Event")]
public class RaceConflictStartEvent : BaseHistoryEvent
{
    [Header("Race Conflict Info")]
    public HistoryEventsData.CityRaces race1;
    public HistoryEventsData.CityRaces race2;
    [TextArea(10, 20)]
    public string conflictReason;
    public bool isWar;
    public bool isDiplomatic;
    public bool isPurge;
    public bool isDisagreement;

    [Header("Race conflict tale")]
    [TextArea(20, 40)]
    public string raceConflictText;

    private void OnValidate()
    {
        raceConflictText = GenerateNarrative() + "\n\n" + GenerateRaceConflictNarrative();
    }

    private string GenerateRaceConflictNarrative()
    {
        string text = $"In the year {yearOfEvent}, tensions between the {race1} and the {race2} reached a boiling point. The spark that ignited this conflict was: {conflictReason}. ";

        if (isWar)
        {
            text += $"The situation quickly escalated into a fierce war, with both sides engaging in brutal battles. ";
        }
        if (isDiplomatic)
        {
            text += $"Despite attempts at diplomacy, the conflict remained unresolved and continued to simmer. ";
        }
        if (isPurge)
        {
            text += $"A violent purge ensued, causing widespread panic and destruction. ";
        }
        if (isDisagreement)
        {
            text += $"The conflict was characterized by sharp disagreements and intense arguments. ";
        }

        text += $"This event marked the beginning of a turbulent period for both the {race1} and the {race2}, as they grappled with the fallout of their escalating hostilities.";

        return text;
    }
}




