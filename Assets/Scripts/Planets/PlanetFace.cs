using UnityEngine;

namespace Assets.Scripts.Planets
{
    

    public class PlanetFace
    {
        private readonly Mesh mesh;
        private readonly int resolution;
        private readonly Vector3 localUp;
        private readonly Vector3 axisA;
        private readonly Vector3 axisB;

        public PlanetFace(Mesh mesh, int resolution, Vector3 localUp)
        {
            this.mesh = mesh;
            this.resolution = resolution;
            this.localUp = localUp;

            this.axisA = new Vector3(this.localUp.y, this.localUp.z, this.localUp.x);
            this.axisB = Vector3.Cross(this.localUp, this.axisA);
        }

        public void ConstructMesh()
        {
            var resolutionLimit = this.resolution - 1;
            var vertices = new Vector3[this.resolution * this.resolution];
            var indices = new int[resolutionLimit * resolutionLimit * 6];

            var indiceIndex = 0;

            for (var y = 0; y < this.resolution; y++)
            {
                for (var x = 0; x < this.resolution; x++)
                {
                    // Calculate vertex position
                    var pointOnUnitCube = this.localUp +
                                          ((float) x / resolutionLimit - 0.5f) * 2 * this.axisA +
                                          ((float) y / resolutionLimit - 0.5f) * 2 * this.axisB;

                    //var x2 = pointOnUnitCube.x * pointOnUnitCube.x;
                    //var y2 = pointOnUnitCube.y * pointOnUnitCube.y;
                    //var z2 = pointOnUnitCube.z * pointOnUnitCube.z;
                    //var fixedPointOnSphere = new Vector3(
                    //    pointOnUnitCube.x * Mathf.Sqrt(1f - y2 / 2f - z2 / 2f + y2 * z2 / 3f),
                    //    pointOnUnitCube.y * Mathf.Sqrt(1f - x2 / 2f - z2 / 2f + x2 * z2 / 3f),
                    //    pointOnUnitCube.z * Mathf.Sqrt(1f - x2 / 2f - y2 / 2f + x2 * y2 / 3f));

                    // Set vertex position
                    var vertexIndex = x + y * this.resolution;
                    vertices[vertexIndex] = pointOnUnitCube;

                    // Skip indices for right and bottom edges
                    if (x == resolutionLimit || y == resolutionLimit) continue;

                    // Connect triangle 1
                    indices[indiceIndex] = vertexIndex;
                    indices[indiceIndex + 1] = vertexIndex + this.resolution + 1;
                    indices[indiceIndex + 2] = vertexIndex + this.resolution;

                    // Connect triangle 2
                    indices[indiceIndex + 3] = vertexIndex;
                    indices[indiceIndex + 4] = vertexIndex + 1;
                    indices[indiceIndex + 5] = vertexIndex + this.resolution + 1;

                    indiceIndex += 6;
                }
            }

            // Assign vertices and indices to the mesh
            this.mesh.Clear();
            this.mesh.vertices = vertices;
            this.mesh.triangles = indices;
            this.mesh.normals = vertices;
        }
    }
}
