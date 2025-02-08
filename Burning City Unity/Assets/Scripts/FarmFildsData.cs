using NUnit.Framework;
using System;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "FieldData", menuName = "CityZones/FieldData")]
public class FarmFildsData : CityZoneData
{
    public enum fildType
    {
        cowField, grainField, 
    }

    [Header("Fild Data")]
    public fildType[] productionTipes;
    public subZonePoints[] subZoneLimits;
}
