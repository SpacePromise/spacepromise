using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Assets.Scripts.Astronomical
{
    public class AstronomicalObjectSurface
    {
        private static GameObject meshRendererPrefab;
        private static Material meshRendererPrefabMaterial;
        public GameObject instance;

        private MeshData meshData;
        private MeshRenderer renderer;


        public void Load()
        {
            this.meshData = PlaneMeshFactory.Instance.Plane(1, 1, 32, 32);
        }

        public void Instantiate(Transform parentTransform, int x, int y)
        {
            if (this.instance != null)
                return;

            if (meshRendererPrefab == null)
                meshRendererPrefab = Resources.Load<GameObject>("Prefabs/MeshRendererPrefab");
            if (meshRendererPrefabMaterial == null)
                meshRendererPrefabMaterial = new Material(Shader.Find("Standard"));

            // TODO: Use pool
            this.instance = Object.Instantiate(meshRendererPrefab, parentTransform);
            this.instance.transform.localScale = new Vector3(0.5f, 1, 0.5f);
            this.instance.transform.localPosition = new Vector3(x * 0.5f - 0.25f, 0, y * 0.5f - 0.25f);
            this.renderer = instance.GetComponent<MeshRenderer>();
            var filter = instance.GetComponent<MeshFilter>();

            renderer.sharedMaterial = meshRendererPrefabMaterial;
            filter.sharedMesh = this.meshData.ApplyToMesh(new Mesh());
        }

        public void DeactivateRendering()
        {
            this.renderer.enabled = false;
        }

        public void ActivateRendering()
        {
            this.renderer.enabled = true;
        }

        public void Destroy()
        {
            if (this.instance == null)
                return;

            // TODO: Use pool
            Object.Destroy(this.instance);
            this.instance = null;
        }
    }
}
