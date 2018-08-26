using Assets.Scripts.Utils;
using UnityEngine;

namespace Assets.Scripts.Astronomical
{
    public class AstronomicalObjectFace
    {
        public GameObject FaceGameObject;

        private readonly QuadTreeNode<AstronomicalObjectSurface> surfaces = new QuadTreeNode<AstronomicalObjectSurface>();

        public void Load()
        {
            this.surfaces.Data = CreateSurface(this.FaceGameObject.transform, 0, 0);
        }

        public void ApplyLod(int lod)
        {
            if (lod > 0 && this.surfaces.IsLeaf)
            {
                ExpandNode(this.surfaces);
            }
            else if (lod <= 0 && !this.surfaces.IsLeaf)
            {
                CollapseNode(this.surfaces);
            }
        }

        private static void ExpandNode(QuadTreeNode<AstronomicalObjectSurface> node)
        {
            var subSurfaces = new[]
            {
                CreateSurface(node.Data.instance.transform, 0, 0),
                CreateSurface(node.Data.instance.transform, 1, 0),
                CreateSurface(node.Data.instance.transform, 0, 1),
                CreateSurface(node.Data.instance.transform, 1, 1)
            };
            node.Expand(subSurfaces);
            node.Data.DeactivateRendering();
        }

        private static void CollapseNode(QuadTreeNode<AstronomicalObjectSurface> node) 
        {
            var collapsedSurfaces = node.Collapse();
            var collapsedSurfacesCount = collapsedSurfaces.Length;
            for (var surfaceIndex = 0; surfaceIndex < collapsedSurfacesCount; surfaceIndex++)
                collapsedSurfaces[surfaceIndex].Destroy();

            node.Data.ActivateRendering();
        }

        private static AstronomicalObjectSurface CreateSurface(Transform parentTransform, int x, int y)
        {
            var surface = new AstronomicalObjectSurface();

            surface.Load();
            surface.Instantiate(parentTransform, x, y);

            return surface;
        }
    }
}