using UnityEngine;
using Object = UnityEngine.Object;

namespace Assets.Scripts.Astronomical
{
    public class AstronomicalObjectSurface
    {
        private const int Resolution = 32;

        private static GameObject meshRendererPrefab;
        private static Material meshRendererPrefabMaterial;
        public GameObject instance;

        private MeshData meshData;
        private MeshRenderer renderer;

        private readonly Vector2Int LocalCords;
        private readonly Vector2Int FaceCords;
        private readonly Vector3 localUp;
        private readonly Vector3 axisA;
        private readonly Vector3 axisB;


        public AstronomicalObjectSurface(AstronomicalObjectSurface parent, int x, int y, Vector3 localUp, Vector3 axisA, Vector3 axisB)
        {
            this.LocalCords = new Vector2Int(x, y);

            if (parent == null)
            {
                this.FaceCords = new Vector2Int(x * Resolution, y * Resolution);
            }
            else
            {
                // TODO: Calculate real face cords using parent size/depth
                this.FaceCords = new Vector2Int(x * Resolution, y * Resolution);
            }

            this.localUp = localUp;
            this.axisA = axisA;
            this.axisB = axisB;
        }


        public void Load()
        {
            //this.meshData = PlaneMeshFactory.Instance.Plane(1, 1, Resolution, Resolution);
        }

        public void Instantiate(Transform parentTransform, int x, int y, int depth)
        {
            if (this.instance != null)
                return;

            this.meshData = ConstructMesh(x, y, Resolution * (depth + 1), this.localUp, this.axisA, this.axisB);

            if (meshRendererPrefab == null)
                meshRendererPrefab = Resources.Load<GameObject>("Prefabs/MeshRendererPrefab");
            if (meshRendererPrefabMaterial == null)
                meshRendererPrefabMaterial = new Material(Shader.Find("Standard"));

            // Only first surface needs to be full size
            // (all other surfaces are half the size on both axis)
            var depthScale = 0.5f;
            var depthPositionOffset = depth <= 0 ? 0 : 0.5f;

            // TODO: Use pool
            this.instance = Object.Instantiate(meshRendererPrefab, parentTransform);
            this.instance.transform.localScale = new Vector3(depthScale, depthScale, depthScale);
            this.instance.transform.localPosition = new Vector3(
                this.localUp.x != 0 ? depthPositionOffset * this.localUp.x : x - depthPositionOffset, 
                this.localUp.y != 0 ? depthPositionOffset * this.localUp.y : (this.localUp.z != 0 ? y : x) - depthPositionOffset, 
                this.localUp.z != 0 ? depthPositionOffset * this.localUp.z : y - depthPositionOffset);
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

        public static MeshData ConstructMesh(int surfaceX, int surfaceY, int faceResolution, Vector3 localUp, Vector3 axisA, Vector3 axisB)
        {
            var resolutionLimit = Resolution - 1;
            var vertices = new Vector3[Resolution * Resolution];
            var normals = new Vector3[Resolution * Resolution];
            var indices = new int[resolutionLimit * resolutionLimit * 6];

            var startX = surfaceX * Resolution;
            var startY = surfaceY * Resolution;
            var edgeLimitX = startX + Resolution - 1;
            var edgeLimitY = startY + Resolution - 1;

            var indiceIndex = 0;

            for (int y = startY, y0 = 0; y < startY + Resolution; y++, y0++)
            {
                for (int x = startX, x0 = 0; x < startX + Resolution; x++, x0++)
                {
                    var vertexIndex = x0 + y0 * Resolution;

                    var cubePoint = CalcVertexCubePosition(x0, y0, resolutionLimit, localUp, axisA, axisB);

                    var xx = x0 + surfaceX * Resolution;
                    var yy = y0 + -surfaceY * Resolution + Resolution;

                    vertices[vertexIndex] = cubePoint;//CalcVertexSpherePosition(cubePoint);
                    normals[vertexIndex] = CalcVertexCubePosition(xx, yy, Resolution * 2 - 1, localUp, axisA, axisB);
                    //vertices[vertexIndex] = CalcVertexSpherePosition(x, y, faceResolution - 1, localUp, axisA, axisB);

                    // Skip indices for right and bottom edges
                    if (x == edgeLimitX || y == edgeLimitY) continue;

                    // Connect triangle 1
                    indices[indiceIndex] = vertexIndex;
                    indices[indiceIndex + 1] = vertexIndex + Resolution + 1;
                    indices[indiceIndex + 2] = vertexIndex + Resolution;

                    // Connect triangle 2
                    indices[indiceIndex + 3] = vertexIndex;
                    indices[indiceIndex + 4] = vertexIndex + 1;
                    indices[indiceIndex + 5] = vertexIndex + Resolution + 1;

                    indiceIndex += 6;
                }
            }

            // Assign vertices and indices to the mesh
            return new MeshData
            {
                Normals = normals,
                Triangles = indices,
                Vertices = vertices
            };
        }

        private static Vector3 CalcVertexCubePosition(int x, int y, int resolutionLimit, Vector3 localUp, Vector3 axisA, Vector3 axisB)
        {
            // Calculate vertex position
            return localUp +
                   ((float) x / resolutionLimit - 0.5f) * 2 * axisA +
                   ((float) y / resolutionLimit - 0.5f) * 2 * axisB;
        }

        private static Vector3 CalcVertexSpherePosition(Vector3 pointOnUnitCube)
        {
            var x2 = pointOnUnitCube.x * pointOnUnitCube.x;
            var y2 = pointOnUnitCube.y * pointOnUnitCube.y;
            var z2 = pointOnUnitCube.z * pointOnUnitCube.z;
            var fixedPointOnSphere = new Vector3(
                pointOnUnitCube.x * Mathf.Sqrt(1f - y2 / 2f - z2 / 2f + y2 * z2 / 3f),
                pointOnUnitCube.y * Mathf.Sqrt(1f - x2 / 2f - z2 / 2f + x2 * z2 / 3f),
                pointOnUnitCube.z * Mathf.Sqrt(1f - x2 / 2f - y2 / 2f + x2 * y2 / 3f));

            return fixedPointOnSphere;
        }
    }
}
