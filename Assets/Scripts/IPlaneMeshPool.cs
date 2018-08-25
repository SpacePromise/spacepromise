using UnityEngine;

namespace Assets.Scripts
{
    public interface IPlaneMeshPool
    {
        Mesh Exchange(Mesh returnMesh, int returnWidth, int returnHeight, int takeWidth, int takeHeight);
        void Return(Mesh mesh, int width, int height);
        Mesh Take(int width, int height);
    }
}