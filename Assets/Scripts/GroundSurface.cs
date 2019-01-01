using UnityEngine;

namespace Assets.Scripts
{
    public class GroundSurface : MonoBehaviour
    {
        public GroundSurfaceMeshRenderer MeshRenderer;
        public GroundSharedRenderData SharedRenderData;


        private void Start()
        {
            this.MeshRenderer = this.gameObject.GetOrAddComponent<GroundSurfaceMeshRenderer>();
        }

        public void Update()
        {
            var lodDistance = Vector3.Distance(this.SharedRenderData.LodViewerPosition, this.transform.position) / 160;
            this.MeshRenderer.LodBase = this.SharedRenderData.ChunkSize / this.SharedRenderData.SurfacesPerChunk;
            this.MeshRenderer.Lod = Mathf.Clamp((int)lodDistance, 1, 22);
        }
    }
}
