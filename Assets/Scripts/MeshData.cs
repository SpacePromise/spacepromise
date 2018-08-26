using UnityEngine;

namespace Assets.Scripts
{
    public class MeshData
    {
        public Vector3[] Vertices;

        public Vector2[] Uv;

        public Vector3[] Normals;

        public int[] Triangles;

        public Mesh ApplyToMesh(Mesh mesh)
        {
            return ApplyToMesh(this, mesh);
        }

        public static Mesh ApplyToMesh(MeshData data, Mesh mesh)
        {
            mesh.vertices = data.Vertices;
            mesh.uv = data.Uv;
            mesh.triangles = data.Triangles;
            mesh.normals = data.Normals;
            return mesh;
        }
    }
}