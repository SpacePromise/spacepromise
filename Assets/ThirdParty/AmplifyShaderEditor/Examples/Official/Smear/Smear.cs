using System.Collections.Generic;
using UnityEngine;

// You can execute this in the editor as long as you don't instantiate the material
//[ExecuteInEditMode]
namespace Assets.ThirdParty.AmplifyShaderEditor.Examples.Official.Smear
{
    public class Smear : MonoBehaviour
    {
        Queue<Vector3> m_recentPositions = new Queue<Vector3>();

        public int FramesBufferSize = 0;

        public Renderer Renderer = null;

        private Material m_instancedMaterial;
        private Material InstancedMaterial
        {
            get { return this.m_instancedMaterial; }
            set { this.m_instancedMaterial = value; }
        }

        private void Start()
        {
            // Instantiate the material so every object has a unique smear effect
            this.InstancedMaterial = this.Renderer.material;

            // Use this instead if you want to affect all objects at the same time or if you want to run in the editor
            //InstancedMaterial = Renderer.sharedMaterial;
        }

        private void LateUpdate()
        {
            // Feed the previous position in the queue to the shader
            if ( this.m_recentPositions.Count > this.FramesBufferSize )
                this.InstancedMaterial.SetVector( "_PrevPosition", this.m_recentPositions.Dequeue() );

            // Feed the current anchor position to the shader
            this.InstancedMaterial.SetVector( "_Position", this.transform.position );
            this.m_recentPositions.Enqueue( this.transform.position );
        }
    }
}
