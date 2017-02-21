using UnityEngine;

namespace Assets.Engine.Pathfinding
{
    public class PathfindingStaticObstacle : MonoBehaviour
    {
        public Rect Bounds;
        private Collider obstacleCollider;


        public void Start()
        {
            // Cache collider component
            this.obstacleCollider = this.GetComponent<Collider>();
        }

        public void Update()
        {
            if (this.transform.hasChanged)
            {
                this.transform.hasChanged = false;

                // Retrieve new values
                var size = this.obstacleCollider.bounds.size;
                var position = this.transform.localPosition;

                // Update bounds
                this.Bounds = new Rect(
                    position.x,
                    position.z,
                    size.x, size.z);
            }
        }
    }
}