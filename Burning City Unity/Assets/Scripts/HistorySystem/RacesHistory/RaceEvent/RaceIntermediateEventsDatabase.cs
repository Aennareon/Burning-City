using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RaceIntermediateEventsDatabase", menuName = "History System/Race Intermediate Events Database")]
public class RaceIntermediateEventsDatabase : ScriptableObject
{
    public List<RaceConflictStartEvent> conflictStartEvents = new List<RaceConflictStartEvent>();
    public List<RaceConflictEndEvent> conflictEndEvents = new List<RaceConflictEndEvent>();
    public List<RaceChangeEvent> changeEvents = new List<RaceChangeEvent>();

    public void RemoveNullElements()
    {
        conflictStartEvents.RemoveAll(e => e == null);
        conflictEndEvents.RemoveAll(e => e == null);
        changeEvents.RemoveAll(e => e == null);
    }
}
