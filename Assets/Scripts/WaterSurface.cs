using UnityEngine;
using UnityEngine.Rendering;

namespace Assets.Scripts
{
    public class WaterSurface : MonoBehaviour 
    {
        private void Start()
        {
            // Instantiate game object
            var waterCube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            DestroyImmediate(waterCube.GetComponent<BoxCollider>());
            waterCube.name = "WaterCube";
            waterCube.transform.parent = this.transform;
            waterCube.transform.localPosition = new Vector3(0, 10, 0);
            waterCube.transform.localScale = new Vector3(10000, 20, 10000);
        
            // Set material
            var waterRenderer = waterCube.GetComponent<MeshRenderer>();
            waterRenderer.receiveShadows = false;
            waterRenderer.shadowCastingMode = ShadowCastingMode.Off;
            waterRenderer.sharedMaterial = Resources.Load<Material>("Materials/Water/OceanStatic/OceanStaticMaterial");
        }
    }
}
