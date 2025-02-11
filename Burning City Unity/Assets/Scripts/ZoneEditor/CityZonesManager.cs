using NUnit.Framework;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;


[CreateAssetMenu(fileName = "CityZonesManager", menuName = "CityData/Managers/City Zones Manager")]
[ExecuteInEditMode]
public class CityZonesManager : ScriptableObject
{
    public enum ZoneTypes { FarmFild, MiningArea, WoodlandWorkcamp, WaterWorkzone };

    [System.Serializable]
    public class ZoneType
    {
        public ZoneTypes name;
        [TextArea]
        public string description;

        public Color zoneTypeColor;
    }

    [Header("City Zones Types")]
    public ZoneType[] zoneTypes;

    [Header("City Zones")]
    public List<ZoneData> cityZones;

    private void OnValidate()
    {
        cityZones = FindAllScriptableObjects<ZoneData>();
        UpdateZonesToPrefabZones();
    }

    public static List<T> FindAllScriptableObjects<T>() where T : ScriptableObject
    {
        List<T> results = new List<T>();
        string[] guids = AssetDatabase.FindAssets($"t:{typeof(T).Name}");

        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            T asset = AssetDatabase.LoadAssetAtPath<T>(path);
            if (asset != null)
            {
                results.Add(asset);
            }
        }

        return results;
    }

    public void UpdateZonesToPrefabZones()
    {
        if (zoneTypes.Length != 0 && cityZones.Count != 0)
        {
            foreach (ZoneType type in zoneTypes)
            {
                foreach (ZoneData zone in cityZones)
                {
                    if (type.name == ZoneTypes.FarmFild && zone is FarmFildData)
                    {
                        zone.zoneColor = type.zoneTypeColor;
                    }

                    if (type.name == ZoneTypes.WoodlandWorkcamp && zone is WoodlandWorkcamp)
                    {
                        zone.zoneColor = type.zoneTypeColor;
                    }

                    if (type.name == ZoneTypes.MiningArea && zone is MiningZoneData)
                    {
                        zone.zoneColor = type.zoneTypeColor;
                    }

                    if (type.name == ZoneTypes.WaterWorkzone && zone is WaterWorkZoneData)
                    {
                        zone.zoneColor = type.zoneTypeColor;
                    }
                }
            }
        }
        else
        {
            Debug.Log("No data transfer to do");
            return;
        }
    }

}
