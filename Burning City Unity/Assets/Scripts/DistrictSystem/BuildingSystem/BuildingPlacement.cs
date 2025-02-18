using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingPlacement : MonoBehaviour
{
    public List<GameObject> buildingPrefabs;
    public int currentPrefabIndex = 0;

    private GameObject buildingPreview;

    public float movementSmoothness = 5f;
    public float rotationSpeed = 120f; // Adjust rotation speed as needed
    private float continuousRotation = 0f;

    void Start()
    {
        CreateBuildingPreview();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))  // Left click
        {
            PlaceBuildingAtObtainedPosition();
        }

        UpdateBuildingPreview();
        RotatePreviewContinuously();
        ChangePrefabWithKeys();
    }

    void CreateBuildingPreview()
    {
        buildingPreview = Instantiate(buildingPrefabs[currentPrefabIndex], Vector3.zero, Quaternion.identity);
        DisableComponents(buildingPreview);
        buildingPreview.SetActive(false);
    }

    void UpdateBuildingPreview()
    {
        Plane plane = new Plane(Vector3.up, Vector3.zero);
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        float distance;

        if (plane.Raycast(ray, out distance))
        {
            Vector3 instancePosition = ray.GetPoint(distance);
            instancePosition.y = 0; // Set Y position to 0

            // Only smooth movement if the preview was active
            if (buildingPreview.activeSelf)
            {
                SmoothMove(buildingPreview.transform, new Vector3(instancePosition.x, 0, instancePosition.z));
            }
            else
            {
                buildingPreview.transform.position = new Vector3(instancePosition.x, 0, instancePosition.z);
            }

            buildingPreview.SetActive(true);
        }
        else
        {
            buildingPreview.SetActive(false);
        }
    }

    void RotatePreviewContinuously()
    {
        if (buildingPreview.activeSelf)
        {
            // Rotate continuously when 'R' is pressed
            if (Input.GetKey(KeyCode.R))
            {
                continuousRotation += rotationSpeed * Time.deltaTime;
                buildingPreview.transform.rotation = Quaternion.Euler(0f, continuousRotation, 0f);
            }
            // Rotate continuously in the opposite direction when 'E' is pressed
            else if (Input.GetKey(KeyCode.E))
            {
                continuousRotation -= rotationSpeed * Time.deltaTime;
                buildingPreview.transform.rotation = Quaternion.Euler(0f, continuousRotation, 0f);
            }
        }
    }

    void SmoothMove(Transform target, Vector3 destination)
    {
        target.position = Vector3.Lerp(target.position, destination, movementSmoothness * Time.deltaTime);
    }

    void PlaceBuildingAtObtainedPosition()
    {
        if (buildingPreview.activeSelf)
        {
            InstantiateBuilding(buildingPreview.transform.position, buildingPreview.transform.rotation);
        }
    }

    void InstantiateBuilding(Vector3 position, Quaternion rotation)
    {
        if (buildingPrefabs.Count > 0 && currentPrefabIndex < buildingPrefabs.Count)
        {
            GameObject selectedPrefab = buildingPrefabs[currentPrefabIndex];
            GameObject newBuilding = Instantiate(selectedPrefab, position, rotation);
            BuildingObject buildingObject = newBuilding.GetComponent<BuildingObject>();
            if (buildingObject != null)
            {
                buildingObject.SetOriginalPrefab(selectedPrefab);
            }
        }
        else
        {
            Debug.LogWarning("No building prefabs available.");
        }
    }

    void ChangePrefabWithKeys()
    {
        for (int i = 0; i < buildingPrefabs.Count; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i))
            {
                ChangePrefab(i);
                break;
            }
        }
    }

    void ChangePrefab(int index)
    {
        if (index >= 0 && index < buildingPrefabs.Count)
        {
            currentPrefabIndex = index;
            Debug.Log("Current prefab: " + buildingPrefabs[currentPrefabIndex].name);
            Destroy(buildingPreview);
            CreateBuildingPreview();
        }
    }

    void DisableComponents(GameObject obj)
    {
        // Get all components in the object and disable them
        Component[] components = obj.GetComponentsInChildren<Component>();

        foreach (var component in components)
        {
            // Make sure not to disable the Transform, as it is essential
            if (!(component is Transform))
            {
                // Disable the component if it is a Behaviour
                if (component is Behaviour)
                {
                    (component as Behaviour).enabled = false;
                }
            }
        }
    }
}
