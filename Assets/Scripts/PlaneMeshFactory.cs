using System;
using UnityEngine;

namespace Assets.Scripts
{
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
 
            var normals = new Vector3[ vertices.Length ];
            for( var n = 0; n < normals.Length; n++ )
                normals[n] = Vector3.up;
 
            var uvs = new Vector2[ vertices.Length ];
            for(var v = 0; v < resZ; v++)
            {
                for(var u = 0; u < resX; u++)
                {
                    uvs[ u + v * resX ] = new Vector2( (float)u / (resX - 1), (float)v / (resZ - 1) );
                }
            }
 
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

            return new MeshData
            {
                Uv = uvs,
                Vertices = vertices,
                Triangles = triangles,
                Normals = normals
            };
        }
    }
}