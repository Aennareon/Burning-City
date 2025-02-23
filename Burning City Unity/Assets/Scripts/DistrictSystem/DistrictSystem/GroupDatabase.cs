using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GroupDatabase", menuName = "City Data/Group Database")]
public class GroupDatabase : ScriptableObject
{
    [System.Serializable]
    public class DistrictGroup
    {
        public BuildingObject.DistrictZone districtZone;
        public List<PositionGroup> groups = new List<PositionGroup>();
    }

    [System.Serializable]
    public class RaceGroup
    {
        public BuildingObject.CityRaces cityRace;
        public List<PositionGroup> groups = new List<PositionGroup>();
    }

    [System.Serializable]
    public class PositionGroup
    {
        public List<Vector3> positions = new List<Vector3>();
    }

    [SerializeField]
    public List<DistrictGroup> districtGroups = new List<DistrictGroup>();

    [SerializeField]
    public List<RaceGroup> raceGroups = new List<RaceGroup>();

    [SerializeField]
    public List<PositionGroup> zonasMixtas = new List<PositionGroup>();

    [SerializeField]
    public List<PositionGroup> zonasMulticulturales = new List<PositionGroup>();
}


