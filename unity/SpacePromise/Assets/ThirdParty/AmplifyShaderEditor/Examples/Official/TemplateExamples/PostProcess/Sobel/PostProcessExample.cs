using UnityEngine;

namespace Assets.ThirdParty.AmplifyShaderEditor.Examples.Official.TemplateExamples.PostProcess.Sobel
{
    [ExecuteInEditMode]
    public class PostProcessExample : MonoBehaviour
    {
        public Material PostProcessMat;
        private void Awake()
        {
            if( this.PostProcessMat == null )
            {
                this.enabled = false;
            }
            else
            {
                // This is on purpose ... it prevents the know bug
                // https://issuetracker.unity3d.com/issues/calling-graphics-dot-blit-destination-null-crashes-the-editor
                // from happening
                this.PostProcessMat.mainTexture = this.PostProcessMat.mainTexture;
            }

        }

        void OnRenderImage( RenderTexture src, RenderTexture dest )
        {
            Graphics.Blit( src, dest, this.PostProcessMat );
        }
    }
}
