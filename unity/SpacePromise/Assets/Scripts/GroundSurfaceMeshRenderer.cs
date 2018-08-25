using UnityEngine;

namespace Assets.Scripts
{
    public class GroundSurfaceMeshRenderer : MonoBehaviour
    {
        public int Lod = 40;
        public int LodBase = 200;

        private MeshFilter meshFilter;
        private MeshRenderer meshRenderer;
        private Material terrainMaterial;

        private Mesh mesh;

        private int currentLod;
        private bool isStarted = false;
        private bool isRefreshWaiting;
        private int lodSize;
        private float renderScale;

        private void Awake()
        {

        }

        private void Start()
        {
            this.meshRenderer = this.gameObject.GetOrAddComponent<MeshRenderer>();
            this.meshFilter = this.gameObject.GetOrAddComponent<MeshFilter>();
            this.terrainMaterial = Resources.Load<Material>("Materials/Terrain/TerrainMaterial");
            this.isStarted = true;
        }

        private void Update()
        {
            if (this.isRefreshWaiting && this.isStarted)
                this.Refresh();
        }

        public void RequestRefresh()
        {
            this.isRefreshWaiting = true;
        }

        private void ApplyMeshData(Mesh mesh)
        {
            var vertices = this.mesh.vertices;
            var verticesCount = this.mesh.vertices.Length;
            var transformX = this.transform.position.x;
            var transformZ = this.transform.position.z;

            for (var index = 0; index < verticesCount; index++)
            {
                vertices[index].y = Mathf.PerlinNoise(
                                        (vertices[index].x * this.renderScale + transformX) / 25f,
                                        (vertices[index].z * this.renderScale + transformZ) / 25f)
                                    * 80;
            }

            this.mesh.vertices = vertices;
        }

        private void Refresh()
        {
            this.RefreshLod();

            this.isRefreshWaiting = false;
        }
    
        private void RefreshLod()
        {
            // Ignore if we don't need to update
            if (this.currentLod == this.Lod)
                return;

            {
                // Generate new mesh
                var newLodSize = this.LodBase / this.currentLod;
                this.mesh = PlaneMeshPool.Instance.Exchange(this.mesh, this.lodSize, this.lodSize, newLodSize, newLodSize);

                // Assign new LOD
                this.lodSize = newLodSize;
                this.currentLod = this.Lod;

                // Set new mesh
                this.meshFilter.sharedMesh = this.mesh;
                this.meshRenderer.sharedMaterial = this.terrainMaterial;
                this.MeshRecalculate();

                this.renderScale = (float)this.LodBase / this.lodSize;
                this.transform.localScale = new Vector3(this.renderScale, 1, this.renderScale);
            }

            {
                this.ApplyMeshData(this.mesh);
                this.MeshRecalculate();
            }
        }

        private void MeshRecalculate()
        {
            this.mesh.RecalculateBounds();
            this.mesh.RecalculateNormals();
        }
    }
}
