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
    public List<Vector3> AllSpawnPoints;

    private void OnValidate()
    {
        cityZones = FindAllScriptableObjects<ZoneData>();
        UpdateZonesToPrefabZones();

        AllSpawnPoints.Clear(); // Limpiar la lista antes de copiar los valores

        foreach (var zone in cityZones)
        {
            if (zone.spawnPoints != null)
            {
                AllSpawnPoints.AddRange(zone.spawnPoints);
            }
        }
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

    public void GenerateZonesProcedurally(Rect area, int numberOfZones)
    {
        cityZones.Clear();
        AllSpawnPoints.Clear();

        for (int i = 0; i < numberOfZones; i++)
        {
            ZoneData newZone = ScriptableObject.CreateInstance<ZoneData>();
            newZone.name = $"Zone_{i + 1}";
            newZone.zoneColor = GetRandomZoneType().zoneTypeColor;
            newZone.spawnPoints = GenerateRandomSpawnPoints(area, 10); // Generar 10 puntos de spawn aleatorios
            cityZones.Add(newZone);
            AllSpawnPoints.AddRange(newZone.spawnPoints);

            SaveZoneData(newZone);
        }

        UpdateZonesToPrefabZones();
    }

    private ZoneType GetRandomZoneType()
    {
        return zoneTypes[Random.Range(0, zoneTypes.Length)];
    }

    private List<Vector3> GenerateRandomSpawnPoints(Rect area, int count)
    {
        List<Vector3> spawnPoints = new List<Vector3>();

        for (int i = 0; i < count; i++)
        {
            float x = Random.Range(area.xMin, area.xMax);
            float y = Random.Range(area.yMin, area.yMax);
            spawnPoints.Add(new Vector3(x, y, 0));
        }

        return spawnPoints;
    }

    private void SaveZoneData(ZoneData zoneData)
    {
        string path = $"Assets/CityZones/{zoneData.name}.asset";
        AssetDatabase.CreateAsset(zoneData, path);
        AssetDatabase.SaveAssets();
    }
}
