using UnityEngine;


public class CityZoneData : ScriptableObject
{
    [System.Serializable]
    public class subZonePoints
    {
        public Vector3[] subZoneLimitsPoints;
    }
    [Header("Zone Limits")]
    public Vector3[] zoneLimits;
    public int numberOfDivisions;
}
