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
            var depthScale = 1f;//0.5f;
            var depthPositionOffset = depth <= 0 ? 0 : 0.5f;

            // TODO: Use pool
            this.instance = Object.Instantiate(meshRendererPrefab, parentTransform);
            this.instance.transform.localScale = new Vector3(depthScale, depthScale, depthScale);
            //this.instance.transform.localPosition = new Vector3(
            //    this.localUp.x != 0 ? depthPositionOffset * this.localUp.x : x - depthPositionOffset, 
            //    this.localUp.y != 0 ? depthPositionOffset * this.localUp.y : (this.localUp.z != 0 ? y : x) - depthPositionOffset, 
            //    this.localUp.z != 0 ? depthPositionOffset * this.localUp.z : y - depthPositionOffset);
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

                    //var res = faceResolution - 1;
                    //var xx = (((startX + x0)) / res);
                    //var yy = (((startY + y0)) / res);
                    //var up = centre;

                    var xx = x / (faceResolution - 1f);
                    var yy = y / (faceResolution - 1f);

                    var cubePoint = CalcVertexCubePosition(xx, yy, localUp, axisA, axisB);
                    //var cubePoint = CalcVertexCubePosition(x0, y0, resolutionLimit, localUp, axisA, axisB);

                    //vertices[vertexIndex] = cubePoint.normalized;
                    vertices[vertexIndex] = CalcVertexSpherePosition(cubePoint);

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

        private static Vector3 CalcVertexCubePosition(float x, float y, Vector3 localUp, Vector3 axisA, Vector3 axisB)
        {
            //Vector2 percent = new Vector2(x, y) / (resolutionLimit - 0.5f);
            //var centre = new Vector3(0.5f, 0.5f, 0.5f);
            //var halfSize = 0.5f;

            //return centre + ((percent.x - .5f) * 2 * axisA + (percent.y - .5f) * 2 * axisB) * halfSize;

            return localUp +
                   ((x - 0.5f) * 2 * axisA +
                   (y - 0.5f) * 2 * axisB);

            //// Calculate vertex position
            //var value = localUp +
            //            ((float) x / resolutionLimit - 0.5f) * 2 * axisA +
            //            ((float) y / resolutionLimit - 0.5f) * 2 * axisB;
            //return value;
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
