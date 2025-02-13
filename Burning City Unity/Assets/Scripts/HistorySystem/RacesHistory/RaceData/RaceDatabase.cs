using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "RaceDatabase", menuName = "History System/Race Database")]
public class RaceDatabase : ScriptableObject
{
    public List<RaceData> races = new List<RaceData>();
}

