using UnityEngine;

[CreateAssetMenu(fileName = "BaseHistoryEvent", menuName = "History System/History Event")]
public class BaseHistoryEvent : HistoryEventsData
{
    [Header("Event Info")]
    public int yearOfEvent;
    public string nameOfEvent;
    [TextArea(5, 10)]
    public string descriptionOfEvent;


    public string GenerateNarrative()
    {
        string text = $"{nameOfEvent} " + "- Year " + $"{yearOfEvent} -" + "\n\n" + $"{descriptionOfEvent}.";
        return text;
    }
}
