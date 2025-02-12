using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using Unity.VisualScripting;

[ExecuteAlways]
public class DrawZone : MonoBehaviour
{
    [Header("DATA")]
    public ZoneData zoneData;  // Debemos arrastrar aquí la referencia de FarmFildData o ZoneData en el editor.

    [Header("Zones Limits")]
    public List<Vector3> zoneLimits;

    private void OnValidate()
    {
        if (zoneData != null)
        {
            // Si los límites han cambiado, actualizamos las zonas
            zoneData.zoneLimits = zoneLimits;
            zoneData.CalculateZones(zoneData.zoneSubdivisions, 1);  // Calculamos las zonas
        }
    }

    void OnDrawGizmos()
    {
        if (zoneData != null)
        {
            // Dibujar la zona principal
            Gizmos.color = zoneData.zoneColor;
            DrawPolygonGizmo(zoneData.zoneLimits);

            // Dibujar las subzonas y sus centroides
            Gizmos.color = zoneData.zoneColor;
            for (int i = 0; i < zoneData.subZones.Count; i++)
            {
                var subZone = zoneData.subZones[i];
                DrawPolygonGizmo(subZone.limits);

                // Dibujar el centroide de la subzona
                Vector3 subZoneCentroid = CalculateCentroid(subZone.limits);

                // Mostrar el nombre de la producción sobre el centroide
                if (zoneData is FarmFildData farmData && i < farmData.listOfProductions.Count)
                {
                    string label = farmData.listOfProductions[i].ToString();
                    Handles.Label(subZoneCentroid + Vector3.up, label); // Usa Handles para etiquetar sobre el gizmo
                }

                if (zoneData is MiningZoneData miningData && i < miningData.listOfProductions.Count)
                {
                    string label = miningData.listOfProductions[i].ToString();
                    Handles.Label(subZoneCentroid + Vector3.up, label); // Usa Handles para etiquetar sobre el gizmo
                }

                if (zoneData is WoodlandWorkcamp woodsData && i < woodsData.listOfProductions.Count)
                {
                    string label = woodsData.listOfProductions[i].ToString();
                    Handles.Label(subZoneCentroid + Vector3.up, label); // Usa Handles para etiquetar sobre el gizmo
                }

                if (zoneData is WaterWorkZoneData waterData && i < waterData.listOfProductions.Count)
                {
                    string label = waterData.listOfProductions[i].ToString();
                    Handles.Label(subZoneCentroid + Vector3.up, label); // Usa Handles para etiquetar sobre el gizmo
                }
            }

            // Dibujar los puntos de spawn
            Gizmos.color = Color.blue;
            foreach (var point in zoneData.spawnPoints)
            {
                Gizmos.DrawSphere(point, 0.2f);
            }
        }
    }

    private void DrawPolygonGizmo(List<Vector3> points)
    {
        if (points == null || points.Count < 3) return;

        for (int i = 0; i < points.Count; i++)
        {
            Gizmos.DrawLine(points[i], points[(i + 1) % points.Count]);
            Gizmos.DrawSphere(points[i], 0.2f);
        }
    }

    // Calcula el centroide de una lista de puntos
    private Vector3 CalculateCentroid(List<Vector3> points)
    {
        return points.Aggregate(Vector3.zero, (acc, p) => acc + p) / points.Count;
    }
}
