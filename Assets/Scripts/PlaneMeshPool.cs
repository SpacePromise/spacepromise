using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    public class PlaneMeshPool : IPlaneMeshPool
    {
        private static readonly Lazy<IPlaneMeshPool> LazyInstance = 
            new Lazy<IPlaneMeshPool>(() => new PlaneMeshPool());
        public static IPlaneMeshPool Instance => LazyInstance.Value;

        private readonly Lazy<PlaneMeshFactory> factory = 
            new Lazy<PlaneMeshFactory>(() => PlaneMeshFactory.Instance);

        private readonly object dictionaryLock = new object();
        private readonly Dictionary<Vector2, Queue<Mesh>> availableMeshes 
            = new Dictionary<Vector2, Queue<Mesh>>();

        public Mesh Exchange(Mesh returnMesh, int returnWidth, int returnHeight, int takeWidth, int takeHeight)
        {
            if (returnMesh != null)
                this.Return(returnMesh, returnWidth, returnHeight);
            return this.Take(takeWidth, takeHeight);
        }

        public Mesh Take(int width, int height)
        {
            if (width <= 0) throw new ArgumentOutOfRangeException(nameof(width));
            if (height <= 0) throw new ArgumentOutOfRangeException(nameof(height));
		
            var size = new Vector2(width, height);
            lock (this.dictionaryLock)
            {
                return this.GetOne(size);
            }
        }

        public void Return(Mesh mesh, int width, int height)
        {
            if (width <= 0) throw new ArgumentOutOfRangeException(nameof(width));
            if (height <= 0) throw new ArgumentOutOfRangeException(nameof(height));
		
            var size = new Vector2(width, height);
            lock (this.dictionaryLock)
            {
                this.GetCollection(size).Enqueue(mesh);
            }
        }

        private Mesh GetOne(Vector2 size)
        {
            var collection = this.GetCollection(size);
            if (collection.Count <= 0)
                this.FillOne(collection, size);

            return collection.Dequeue();
        }
	
        private Queue<Mesh> GetCollection(Vector2 size)
        {
            // Create collection if not available already
            if (!this.availableMeshes.ContainsKey(size))
                this.availableMeshes.Add(size, new Queue<Mesh>());
		
            return this.availableMeshes[size];
        }
	
        private void FillOne(Queue<Mesh> collection, Vector2 size)
        {
            // Generate mesh data
            var meshData = this.factory.Value.Plane(
                (int)size.x, (int)size.y, 
                (int)size.x + 1, (int)size.y + 1);

            // Create mesh
            var mesh = new Mesh
            {
                vertices = meshData.Vertices,
                uv = meshData.Uv,
                triangles = meshData.Triangles,
                normals = meshData.Normals
            };
		
            collection.Enqueue(mesh);
        }
    }
}