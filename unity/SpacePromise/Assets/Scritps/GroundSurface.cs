using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundSurface : MonoBehaviour
{
    public GroundSurfaceMeshRenderer renderer;
    public GroundSharedRenderData SharedRenderData;


    private void Start()
    {
        this.renderer = this.gameObject.GetOrAddComponent<GroundSurfaceMeshRenderer>();
    }

    public void Update()
    {
        var lodDistance = Vector3.Distance(this.SharedRenderData.LodViewerPosition, this.transform.position) / 160;
        this.renderer.LodBase = this.SharedRenderData.ChunkSize / this.SharedRenderData.SurfacesPerChunk;
        this.renderer.Lod = Mathf.Clamp((int)lodDistance, 1, 22);
    }
}
