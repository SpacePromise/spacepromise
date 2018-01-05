using System;
using System.Collections.Generic;
using UnityEngine;

public static class GameObjectExtensions
{
	public static T EnsureSingleComponent<T>(this GameObject @this)
		where T : Component
	{
		var component = @this.GetComponent<T>();
		return component != null
			? component
			: @this.AddComponent<T>();
	}
}

public class MeshData
{
	public Vector3[] Vertices;

	public Vector2[] Uv;

	public Vector3[] Normals;

	public int[] Triangles;
}

public class PlaneMeshFactory
{
	private static readonly Lazy<PlaneMeshFactory> LazyInstance = 
		new Lazy<PlaneMeshFactory>(() => new PlaneMeshFactory());
	public static PlaneMeshFactory Instance => LazyInstance.Value;

	public MeshData Plane(int width, int height, int resolutionX, int resolutionZ)
	{
		if (width <= 0) throw new ArgumentOutOfRangeException(nameof(width));
		if (height <= 0) throw new ArgumentOutOfRangeException(nameof(height));

		var resX = resolutionX; // 2 minimum
		var resZ = resolutionZ;
 
		#region Vertices		
		var vertices = new Vector3[ resX * resZ ];
		for(var z = 0; z < resZ; z++)
		{
			// [ -length / 2, length / 2 ]
			var zPos = ((float)z / (resZ - 1) - .5f) * height;
			for(var x = 0; x < resX; x++)
			{
				// [ -width / 2, width / 2 ]
				var xPos = ((float)x / (resX - 1) - .5f) * width;
				vertices[ x + z * resX ] = new Vector3( xPos, 0f, zPos );
			}
		}
		#endregion
 
		#region Normales
		var normals = new Vector3[ vertices.Length ];
		for( var n = 0; n < normals.Length; n++ )
			normals[n] = Vector3.up;
		#endregion
 
		#region UVs		
		var uvs = new Vector2[ vertices.Length ];
		for(var v = 0; v < resZ; v++)
		{
			for(var u = 0; u < resX; u++)
			{
				uvs[ u + v * resX ] = new Vector2( (float)u / (resX - 1), (float)v / (resZ - 1) );
			}
		}
		#endregion
 
		#region Triangles
		var nbFaces = (resX - 1) * (resZ - 1);
		var triangles = new int[ nbFaces * 6 ];
		var t = 0;
		for(var face = 0; face < nbFaces; face++ )
		{
			// Retrieve lower left corner from face ind
			var i = face % (resX - 1) + (face / (resZ - 1) * resX);
 
			triangles[t++] = i + resX;
			triangles[t++] = i + 1;
			triangles[t++] = i;
 
			triangles[t++] = i + resX;	
			triangles[t++] = i + resX + 1;
			triangles[t++] = i + 1; 
		}
		#endregion

		return new MeshData
		{
			Uv = uvs,
			Vertices = vertices,
			Triangles = triangles,
			Normals = normals
		};
	}
}

public class PlaneMeshPool
{
	private static readonly Lazy<PlaneMeshPool> LazyInstance = 
		new Lazy<PlaneMeshPool>(() => new PlaneMeshPool());
	public static PlaneMeshPool Instance => LazyInstance.Value;

	private readonly Lazy<PlaneMeshFactory> factory = 
		new Lazy<PlaneMeshFactory>(() => PlaneMeshFactory.Instance);

	private readonly object dictionaryLock = new object();
	private readonly Dictionary<Vector2, Queue<Mesh>> availableMeshes 
		= new Dictionary<Vector2, Queue<Mesh>>();

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

[ExecuteInEditMode]
[DisallowMultipleComponent]
public class GroundSurfaceMeshRenderer : MonoBehaviour {
	
	private void Awake()
	{

	}

	private void Start()
	{
		{
			var meshRenderer = this.gameObject.EnsureSingleComponent<MeshRenderer>();
			var meshFilter = this.gameObject.EnsureSingleComponent<MeshFilter>();

			var mesh = PlaneMeshPool.Instance.Take(100, 100);
			mesh.RecalculateBounds();
			mesh.RecalculateNormals();
			meshFilter.sharedMesh = mesh;

			meshRenderer.sharedMaterial = Resources.Load<Material>("Materials/Terrain/TerrainMaterial");
		}

		{
			var mesh = this.GetComponent<MeshFilter>().sharedMesh;
			var vertices = mesh.vertices;

			for (var index = 0; index < vertices.Length; index++)
			{
				vertices[index].y = Mathf.PerlinNoise(
					                    (vertices[index].x + this.transform.position.x) / 25f,
					                    (vertices[index].z + this.transform.position.z) / 25f)
				                    * 30;
			}

			mesh.vertices = vertices;
			mesh.RecalculateBounds();
			mesh.RecalculateNormals();
		}
	}
}
