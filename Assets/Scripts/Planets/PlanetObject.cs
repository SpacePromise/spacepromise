using UnityEngine;

namespace Assets.Scripts.Planets
{
    public class PlanetObject : MonoBehaviour
    {
        private const int NumberOfFaces = 1;

        [Range(2, 256)]
        public int Resolution = 10;

        private static readonly Vector3[] PlanetFaceDirections =
            {Vector3.up, Vector3.down, Vector3.left, Vector3.right, Vector3.forward, Vector3.back};

        [SerializeField, HideInInspector]
        private MeshFilter[] meshFilters;
        private PlanetFace[] planetFaces;

        private void OnValidate()
        {
            this.Initialize();
            this.GenerateMeshes();
        }

        private void Initialize()
        {
            this.RenderMeshFilters();

            this.planetFaces = new PlanetFace[NumberOfFaces];

            for (var i = 0; i < NumberOfFaces; i++)
            {
                // Instantiate new planet face
                this.planetFaces[i] = new PlanetFace(
                    this.meshFilters[i].sharedMesh, 
                    this.Resolution,
                    PlanetFaceDirections[i]);
            }
        }

        private void RenderMeshFilters()
        {
            if (this.meshFilters != null && this.meshFilters.Length > 0) return;

            this.meshFilters = new MeshFilter[NumberOfFaces];

            for (var i = 0; i < NumberOfFaces; i++)
            {
                // Create child object
                var meshObject = new GameObject("PlanetFaceMesh");
                meshObject.transform.parent = this.transform;

                // Add mesh renderer and assign material
                var meshRenderer = meshObject.AddComponent<MeshRenderer>();
                meshRenderer.sharedMaterial = new Material(Shader.Find("Standard"));

                // Add mesh filter and assign new mesh
                this.meshFilters[i] = meshObject.AddComponent<MeshFilter>();
                this.meshFilters[i].sharedMesh = new Mesh();
            }
        }

        private void GenerateMeshes()
        {
            foreach (var planetFace in this.planetFaces)
                planetFace.ConstructMesh();
        }
    }
}
