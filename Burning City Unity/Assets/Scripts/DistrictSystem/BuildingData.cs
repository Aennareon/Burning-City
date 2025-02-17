using UnityEngine;
using static BuildingObject;

[CreateAssetMenu(fileName = "BuildingData", menuName = "City Data/Building Data")]
public class BuildingData : ScriptableObject
{
    public BuildingType buildingType;
    public DistrictZone districtZone;
    public CityRaces cityRaces;
    public Vector3 buildingPosition;
    public Vector3 buildingRotation;
    public Vector3 doorPosition;
}
