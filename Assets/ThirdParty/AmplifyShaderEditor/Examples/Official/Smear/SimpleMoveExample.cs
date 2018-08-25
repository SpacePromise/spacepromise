using UnityEngine;

namespace Assets.ThirdParty.AmplifyShaderEditor.Examples.Official.Smear
{
    /// <summary>
    /// This is just a simple movement example in which the object is moved
    /// to a random location inside a bounding box using smooth lerp
    /// </summary>
    public class SimpleMoveExample : MonoBehaviour
    {
        private Vector3 m_previous;
        private Vector3 m_target;
        private Vector3 m_originalPosition;
        public Vector3 BoundingVolume = new Vector3( 3, 1, 3 );
        public float Speed = 10;

        private void Start()
        {
            this.m_originalPosition = this.transform.position;
            this.m_previous = this.transform.position;
            this.m_target = this.transform.position;
        }

        private void Update()
        {
            this.transform.position = Vector3.Slerp( this.m_previous, this.m_target, Time.deltaTime * this.Speed );
            this.m_previous = this.transform.position;
            if ( Vector3.Distance( this.m_target, this.transform.position ) < 0.1f )
            {
                this.m_target = this.transform.position + Random.onUnitSphere * Random.Range( 0.7f, 4f );
                this.m_target.Set( Mathf.Clamp( this.m_target.x, this.m_originalPosition.x - this.BoundingVolume.x, this.m_originalPosition.x + this.BoundingVolume.x ), Mathf.Clamp( this.m_target.y, this.m_originalPosition.y - this.BoundingVolume.y, this.m_originalPosition.y + this.BoundingVolume.y ), Mathf.Clamp( this.m_target.z, this.m_originalPosition.z - this.BoundingVolume.z, this.m_originalPosition.z + this.BoundingVolume.z ) );
            }
        }
    }
}
