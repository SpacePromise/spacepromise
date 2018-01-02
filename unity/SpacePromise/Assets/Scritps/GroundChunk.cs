using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class UnityExtensions
{
    public static void DestroyChildrenWithComponent<T>(this MonoBehaviour component, bool destroyImmediate = true)
        where T : Component
    {
        // Retrieve game objects
        var existingObjects = component.GetComponentsInChildren<T>().Select(comp => comp.gameObject).ToList();
        
        // Destroy
        foreach (var obj in existingObjects)
            if (destroyImmediate)
                Object.DestroyImmediate(obj);
            else
                Object.Destroy(obj);
        
        // Log
        if (existingObjects.Any())
            Debug.Log($"Destroyed {existingObjects.Count} {typeof(T).Name} objects.");
    }

    public static GameObject CreateChildGameObjectFromComponent<T>(this MonoBehaviour component)
        where T : Component
    {
        var obj = new GameObject
        {
            name = typeof(T).Name
        };
        obj.transform.parent = component.transform;
        obj.transform.localPosition = Vector3.zero;
        obj.AddComponent<T>();

        return obj;
    }
}

[ExecuteInEditMode]
[DisallowMultipleComponent]
public class GroundChunk : MonoBehaviour
{
    private int size = 500;
    private int groundSurfacesSize = 5;
    
    private void Start()
    {
        this.GenerateGroundSurfaces();
    }

    private void GenerateGroundSurfaces()
    {
        // Clear existing ground surfaces
        this.DestroyChildrenWithComponent<GroundSurface>();

        // Create ground surfaces
        var surfaceSize = (float) this.size / this.groundSurfacesSize;
        for (var verticalIndex = 0; verticalIndex < this.groundSurfacesSize; verticalIndex++)
        {
            for (var horizontalIndex = 0; horizontalIndex < this.groundSurfacesSize; horizontalIndex++)
            {
                var groundSurface = this.CreateChildGameObjectFromComponent<GroundSurface>();
                groundSurface.name += (horizontalIndex + verticalIndex * this.groundSurfacesSize).ToString();
                groundSurface.transform.localPosition =
                    new Vector3(
                        horizontalIndex * surfaceSize + surfaceSize / 2,
                        0,
                        verticalIndex * surfaceSize + surfaceSize / 2);
            }
        }
    }
}
