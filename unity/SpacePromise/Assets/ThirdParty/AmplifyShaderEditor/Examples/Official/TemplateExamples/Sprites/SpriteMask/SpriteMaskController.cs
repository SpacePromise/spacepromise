// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using UnityEngine;
using UnityEngine.Sprites;

namespace Assets.ThirdParty.AmplifyShaderEditor.Examples.Official.TemplateExamples.Sprites.SpriteMask
{
    [ExecuteInEditMode]
    public class SpriteMaskController : MonoBehaviour
    {
        private SpriteRenderer m_spriteRenderer;
        private Vector4 m_uvs;

        void OnEnable ()
        {
            this.m_spriteRenderer = this.GetComponent<SpriteRenderer>();
            this.m_uvs = DataUtility.GetInnerUV( this.m_spriteRenderer.sprite );
            this.m_spriteRenderer.sharedMaterial.SetVector( "_CustomUVS", this.m_uvs );
        }
    }
}
