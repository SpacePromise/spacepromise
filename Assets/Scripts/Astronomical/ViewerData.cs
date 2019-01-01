using UnityEngine;

namespace Assets.Scripts.Astronomical
{
    public class ViewerData
    {
        public Vector3 ViewerPosition;

        public bool IsViewerLodPositionLocked;
        public Vector3 ViewerLodPosition;

        public void SetViewerPosition(Vector3 newViewerPosition)
        {
            this.ViewerPosition = newViewerPosition;

            // Don't update LOD position if locked
            if (!this.IsViewerLodPositionLocked)
                this.ViewerLodPosition = newViewerPosition;
        }

        public void LockLodPosition()
        {
            this.IsViewerLodPositionLocked = true;
        }

        public void UnlockLodPosition()
        {
            this.IsViewerLodPositionLocked = false;
            this.ViewerLodPosition = this.ViewerPosition;
        }
    }
}