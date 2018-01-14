﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundChunk : MonoBehaviour
{
    public GroundSharedRenderData SharedRenderData;
    
    private readonly List<GroundSurface> groundSurfaces = new List<GroundSurface>();


    private void Start()
    {
        this.GenerateGroundSurfaces();
    }

    private void Update()
    {
        foreach (var groundSurface in this.groundSurfaces)
        {
            if (groundSurface.renderer != null)
                groundSurface.renderer.RequestRefresh();
        }
    }

    private void GenerateGroundSurfaces()
    {
        // Clear existing ground surfaces
        this.DestroyChildrenWithComponent<GroundSurface>();

        // Create ground surfaces
        var surfaceSize = (float) this.SharedRenderData.ChunkSize / this.SharedRenderData.SurfacesPerChunk;
        for (var verticalIndex = 0; verticalIndex < this.SharedRenderData.SurfacesPerChunk; verticalIndex++)
        {
            for (var horizontalIndex = 0; horizontalIndex < this.SharedRenderData.SurfacesPerChunk; horizontalIndex++)
            {
                var groundSurfaceObject = this.CreateChildGameObjectFromComponent<GroundSurface>();
                groundSurfaceObject.name += (horizontalIndex + verticalIndex * this.SharedRenderData.SurfacesPerChunk).ToString();
                groundSurfaceObject.transform.localPosition =
                    new Vector3(
                        horizontalIndex * surfaceSize + surfaceSize / 2,
                        0,
                        verticalIndex * surfaceSize + surfaceSize / 2);

                var groundSurface = groundSurfaceObject.GetComponent<GroundSurface>();
                groundSurface.SharedRenderData = this.SharedRenderData;
                this.groundSurfaces.Add(groundSurface);
            }
        }
    }
}
