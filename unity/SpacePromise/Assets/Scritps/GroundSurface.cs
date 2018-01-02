using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[DisallowMultipleComponent]
public class GroundSurface : MonoBehaviour 
{
    private void Start()
    {
        this.gameObject.EnsureSingleComponent<GroundSurfaceMeshRenderer>();
    }
}
