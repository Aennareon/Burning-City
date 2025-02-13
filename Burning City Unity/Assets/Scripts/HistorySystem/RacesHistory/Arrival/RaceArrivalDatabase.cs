using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "RaceArrivalDatabase", menuName = "History System/Race Arrival Database")]
public class RaceArrivalDatabase : ScriptableObject
{
    public List<NewRaceHV> raceArrivals = new List<NewRaceHV>();
}
