using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Utils;
using UnityEngine;

namespace Assets.Scripts.Astronomical
{
    public class AstronomicalObjectLodDebugger : MonoBehaviour
    {
        private AstronomicalObject astronomicalObject;

        public int Faces;
        public int Depth;
        public int SurfacesTotal;
        public int SurfacesActive;


        public void Start()
        {
            // Cache astronomical object component
            this.astronomicalObject = this.gameObject.GetComponent<AstronomicalObject>();
            if (this.astronomicalObject != null) 
                return;

            // Deactivate if astronomical object is not present
            this.Deactivate();
        }

        public void Update()
        {
            // Deactivate if astronomical object is not present
            if (this.astronomicalObject == null)
                this.Deactivate();

            this.Faces = this.astronomicalObject.Faces.Length;

            var surfaces = new Stack<QuadTreeNode<AstronomicalObjectSurface>>();
            foreach (var face in this.astronomicalObject.Faces) 
                surfaces.Push(face.Surfaces);

            this.SurfacesActive = 0;
            this.SurfacesTotal = 0;
            this.Depth = 0;
            while (surfaces.Any())
            {
                this.Depth++;
                var currentSurfaces = surfaces.ToList();
                this.SurfacesTotal += currentSurfaces.Count;
                surfaces.Clear();
                foreach (var surfaceNode in currentSurfaces)
                {
                    if (surfaceNode.IsLeaf)
                    {
                        this.SurfacesActive++;
                        continue;
                    }

                    foreach (var surfaceNodeChild in surfaceNode.Children) 
                        surfaces.Push(surfaceNodeChild);
                }
            }
        }

        private void Deactivate()
        {
            // Warn developer and deactivate
            Debug.LogWarning($"{typeof(AstronomicalObjectLodDebugger).Name} requires {typeof(AstronomicalObject).Name} on same GameObject. Deactivated.");
            this.gameObject.SetActive(false);
        }
    }
}