using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "OutlineDatabase", menuName = "City Data/Outline Database")]
public class OutlineDatabase : ScriptableObject
{
    [System.Serializable]
    public class OutlineDistrictType
    {
        public BuildingObject.DistrictZone districtZone;
        public GameObject outlinePrefab;
    }

    [System.Serializable]
    public class OutlineRaceType
    {
        public BuildingObject.CityRaces cityRace;
        public GameObject outlinePrefab;
    }

    public List<OutlineDistrictType> districtOutlines = new List<OutlineDistrictType>();
    public List<OutlineRaceType> raceOutlines = new List<OutlineRaceType>();
    public GameObject mixZoneOutline;
    public GameObject mixRaceOutline;

}

