using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BaseHistoryEvent", menuName = "History System/History Event")]
public class BaseHistoryEvent : HistoryEventsData
{
    [Header("Event Info")]
    public int yearOfEvent;
    public string nameOfEvent;
    public EventType eventType;
    [TextArea(0, 20)]
    public string descriptionOfEvent;
    [TextArea(0, 40)]
    public string narrative;

    private void OnValidate()
    {
        var eventInfo = GetRandomEventInfo(eventType);
        nameOfEvent = eventInfo.title;
        descriptionOfEvent = eventInfo.description;
        narrative = GenerateNarrative();
    }

    public string GenerateNarrative()
    {
        string text = $"{nameOfEvent} " + "- Year " + $"{yearOfEvent} -" + "\n\n" + $"{descriptionOfEvent}.";
        return text;
    }

    public EventInfo GetRandomEventInfo(EventType eventType)
    {
        if (eventInfos.ContainsKey(eventType))
        {
            List<EventInfo> infos = eventInfos[eventType];
            if (infos.Count > 0)
            {
                System.Random random = new System.Random();
                int index = random.Next(infos.Count);
                return infos[index];
            }
        }
        return new EventInfo(string.Empty, string.Empty);
    }
}
