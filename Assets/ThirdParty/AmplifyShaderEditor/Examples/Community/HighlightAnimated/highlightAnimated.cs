using UnityEngine;

namespace Assets.ThirdParty.AmplifyShaderEditor.Examples.Community.HighlightAnimated
{

	public class highlightAnimated : MonoBehaviour
    {

        private Material mat;

        void Start()
        {
            this.mat = this.GetComponent<Renderer>().material;
        }

        void OnMouseEnter()
        {
            this.switchhighlighted(true);
		}

        void OnMouseExit()
        {
            this.switchhighlighted(false);
        }

        void switchhighlighted(bool highlighted)
        {
            this.mat.SetFloat("_Highlighted", (highlighted ? 1.0f : 0.0f));
        }

    }

}