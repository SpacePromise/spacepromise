using System.Linq;
using Assets.Scripts.Utils;
using UnityEngine;

namespace Assets.Scripts.Astronomical
{
    public class AstronomicalObjectFace
    {
        public GameObject faceGameObject;

        private readonly QuadTreeNode<AstronomicalObjectSurface> surfaces = new QuadTreeNode<AstronomicalObjectSurface>();

        public void Load()
        {
            this.surfaces.Data = CreateSurface(this.faceGameObject.transform, 0, 0);
        }

        public void ApplyLod(int lod)
        {
            if (lod > 0 && this.surfaces.IsLeaf)
            {
                var subSurfaces = new[]
                {
                    CreateSurface(this.surfaces.Data.instance.transform, 0, 0),
                    CreateSurface(this.surfaces.Data.instance.transform, 1, 0),
                    CreateSurface(this.surfaces.Data.instance.transform, 0, 1),
                    CreateSurface(this.surfaces.Data.instance.transform, 1, 1)
                };
                this.surfaces.Expand(subSurfaces);
                this.surfaces.Data.DeactivateRendering();
            }
            else if (lod <= 0 && !this.surfaces.IsLeaf)
            {
                var collapsedSurfaces = this.surfaces.Collapse();
                var collapsedSurfacesCount = collapsedSurfaces.Length;
                for (var surfaceIndex = 0; surfaceIndex < collapsedSurfacesCount; surfaceIndex++)
                    collapsedSurfaces[surfaceIndex].Destroy();

                this.surfaces.Data.ActivateRendering();
            }
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