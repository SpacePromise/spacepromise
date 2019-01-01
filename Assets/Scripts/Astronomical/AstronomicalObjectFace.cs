using System;
using System.Runtime.CompilerServices;
using Assets.Scripts.Utils;
using UnityEngine;

namespace Assets.Scripts.Astronomical
{
    public class AstronomicalObjectFace
    {
        private const int MaxSubdivisions = 6;

        [NonSerialized]
        public GameObject FaceGameObject;

        [NonSerialized]
        public readonly QuadTreeNode<AstronomicalObjectSurface> Surfaces = new QuadTreeNode<AstronomicalObjectSurface>();

        private readonly Vector3 localUp;
        private readonly Vector3 axisA;
        private readonly Vector3 axisB;


        public AstronomicalObjectFace(Vector3 localUp)
        {
            this.localUp = localUp;

            this.axisA = new Vector3(this.localUp.y, this.localUp.z, this.localUp.x);
            this.axisB = Vector3.Cross(this.localUp, this.axisA);
        }


        public void Load()
        {
            this.Surfaces.Data = CreateSurface(this.FaceGameObject.transform, 0, 0, true);
        }

        public void ApplyLod()
        {
            ApplySurfaceLod(0, this.Surfaces);
        }

        private void ApplySurfaceLod(int currentDepth, QuadTreeNode<AstronomicalObjectSurface> lodSurface)
        {
            if (currentDepth > MaxSubdivisions)
            {
                return;
            }

            // Depth first - collapse children
            ApplyLodToChildren(currentDepth, lodSurface);

            // TODO: Remove dependency on absolute Unity position (lodSurface.Data.instance.transform.position)
            var lodThreshold = ((float)MaxSubdivisions - currentDepth) / currentDepth; 
            var lodPosition = ViewerDataProvider.Instance.Data.ViewerLodPosition;
            var viewerDistance = Vector3.Distance(lodSurface.Data.instance.transform.position, lodPosition);
            var needsLodIncrease = currentDepth < 1; //viewerDistance <= lodThreshold;
            var needsLodDecrease = false;//viewerDistance > lodThreshold;

            // Process current face
            if (needsLodIncrease && lodSurface.IsLeaf)
            {
                ExpandNode(lodSurface);
            }
            else if (needsLodDecrease && !lodSurface.IsLeaf)
            {
                CollapseNode(lodSurface);
            }
            
            // Breadth first - expand children
            ApplyLodToChildren(currentDepth, lodSurface);
        }

        private void ApplyLodToChildren(int currentDepth, QuadTreeNode<AstronomicalObjectSurface> lodSurface)
        {
            // Ignore if there is no children
            if (lodSurface.IsLeaf) return;

            // Apply surface LOD to each child
            foreach (var childSurface in lodSurface.Children) 
                ApplySurfaceLod(currentDepth + 1, childSurface);
        }

        private void ExpandNode(QuadTreeNode<AstronomicalObjectSurface> node)
        {
            // Create child nodes
            var subSurfaces = new[]
            {
                CreateSurface(node.Data.instance.transform, 0, 0),
                CreateSurface(node.Data.instance.transform, 1, 0),
                CreateSurface(node.Data.instance.transform, 0, 1),
                CreateSurface(node.Data.instance.transform, 1, 1)
            };
            node.Expand(subSurfaces);

            // Deactivate parent node
            node.Data.DeactivateRendering();
        }

        private static void CollapseNode(QuadTreeNode<AstronomicalObjectSurface> node) 
        {
            // Deactivate child nodes
            var collapsedSurfaces = node.Collapse();
            var collapsedSurfacesCount = collapsedSurfaces.Length;
            for (var surfaceIndex = 0; surfaceIndex < collapsedSurfacesCount; surfaceIndex++)
                collapsedSurfaces[surfaceIndex].Destroy();

            // Activate parent node
            node.Data.ActivateRendering();
        }

        private AstronomicalObjectSurface CreateSurface(Transform parentTransform, int x, int y, bool isRoot = false)
        {
            var surface = new AstronomicalObjectSurface(x, y, this.localUp, this.axisA, this.axisB);

            surface.Load();
            surface.Instantiate(parentTransform, x, y, isRoot ? 0 : 1);

            return surface;
        }
    }
}