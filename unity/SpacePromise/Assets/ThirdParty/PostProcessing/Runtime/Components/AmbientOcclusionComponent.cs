using Assets.ThirdParty.PostProcessing.Runtime.Models;
using Assets.ThirdParty.PostProcessing.Runtime.Utils;
using UnityEngine;
using UnityEngine.Rendering;

namespace Assets.ThirdParty.PostProcessing.Runtime.Components
{
    using DebugMode = BuiltinDebugViewsModel.Mode;

    public sealed class AmbientOcclusionComponent : PostProcessingComponentCommandBuffer<AmbientOcclusionModel>
    {
        static class Uniforms
        {
            internal static readonly int _Intensity         = Shader.PropertyToID("_Intensity");
            internal static readonly int _Radius            = Shader.PropertyToID("_Radius");
            internal static readonly int _FogParams         = Shader.PropertyToID("_FogParams");
            internal static readonly int _Downsample        = Shader.PropertyToID("_Downsample");
            internal static readonly int _SampleCount       = Shader.PropertyToID("_SampleCount");
            internal static readonly int _OcclusionTexture1 = Shader.PropertyToID("_OcclusionTexture1");
            internal static readonly int _OcclusionTexture2 = Shader.PropertyToID("_OcclusionTexture2");
            internal static readonly int _OcclusionTexture  = Shader.PropertyToID("_OcclusionTexture");
            internal static readonly int _MainTex           = Shader.PropertyToID("_MainTex");
            internal static readonly int _TempRT            = Shader.PropertyToID("_TempRT");
        }

        const string k_BlitShaderString = "Hidden/Post FX/Blit";
        const string k_ShaderString = "Hidden/Post FX/Ambient Occlusion";

        readonly RenderTargetIdentifier[] m_MRT =
        {
            BuiltinRenderTextureType.GBuffer0, // Albedo, Occ
            BuiltinRenderTextureType.CameraTarget // Ambient
        };

        enum OcclusionSource
        {
            DepthTexture,
            DepthNormalsTexture,
            GBuffer
        }

        OcclusionSource occlusionSource
        {
            get
            {
                if (this.context.isGBufferAvailable && !this.model.settings.forceForwardCompatibility)
                    return OcclusionSource.GBuffer;

                if (this.model.settings.highPrecision && (!this.context.isGBufferAvailable || this.model.settings.forceForwardCompatibility))
                    return OcclusionSource.DepthTexture;

                return OcclusionSource.DepthNormalsTexture;
            }
        }

        bool ambientOnlySupported
        {
            get { return this.context.isHdr && this.model.settings.ambientOnly && this.context.isGBufferAvailable && !this.model.settings.forceForwardCompatibility; }
        }

        public override bool active
        {
            get
            {
                return this.model.enabled
                       && this.model.settings.intensity > 0f
                       && !this.context.interrupted;
            }
        }

        public override DepthTextureMode GetCameraFlags()
        {
            var flags = DepthTextureMode.None;

            if (this.occlusionSource == OcclusionSource.DepthTexture)
                flags |= DepthTextureMode.Depth;

            if (this.occlusionSource != OcclusionSource.GBuffer)
                flags |= DepthTextureMode.DepthNormals;

            return flags;
        }

        public override string GetName()
        {
            return "Ambient Occlusion";
        }

        public override CameraEvent GetCameraEvent()
        {
            return this.ambientOnlySupported && !this.context.profile.debugViews.IsModeActive(DebugMode.AmbientOcclusion)
                   ? CameraEvent.BeforeReflections
                   : CameraEvent.BeforeImageEffectsOpaque;
        }

        public override void PopulateCommandBuffer(CommandBuffer cb)
        {
            var settings = this.model.settings;

            // Material setup
            var blitMaterial = this.context.materialFactory.Get(k_BlitShaderString);

            var material = this.context.materialFactory.Get(k_ShaderString);
            material.shaderKeywords = null;
            material.SetFloat(Uniforms._Intensity, settings.intensity);
            material.SetFloat(Uniforms._Radius, settings.radius);
            material.SetFloat(Uniforms._Downsample, settings.downsampling ? 0.5f : 1f);
            material.SetInt(Uniforms._SampleCount, (int)settings.sampleCount);

            if (!this.context.isGBufferAvailable && RenderSettings.fog)
            {
                material.SetVector(Uniforms._FogParams, new Vector3(RenderSettings.fogDensity, RenderSettings.fogStartDistance, RenderSettings.fogEndDistance));

                switch (RenderSettings.fogMode)
                {
                    case FogMode.Linear:
                        material.EnableKeyword("FOG_LINEAR");
                        break;
                    case FogMode.Exponential:
                        material.EnableKeyword("FOG_EXP");
                        break;
                    case FogMode.ExponentialSquared:
                        material.EnableKeyword("FOG_EXP2");
                        break;
                }
            }
            else
            {
                material.EnableKeyword("FOG_OFF");
            }

            int tw = this.context.width;
            int th = this.context.height;
            int ts = settings.downsampling ? 2 : 1;
            const RenderTextureFormat kFormat = RenderTextureFormat.ARGB32;
            const RenderTextureReadWrite kRWMode = RenderTextureReadWrite.Linear;
            const FilterMode kFilter = FilterMode.Bilinear;

            // AO buffer
            var rtMask = Uniforms._OcclusionTexture1;
            cb.GetTemporaryRT(rtMask, tw / ts, th / ts, 0, kFilter, kFormat, kRWMode);

            // AO estimation
            cb.Blit((Texture)null, rtMask, material, (int)this.occlusionSource);

            // Blur buffer
            var rtBlur = Uniforms._OcclusionTexture2;

            // Separable blur (horizontal pass)
            cb.GetTemporaryRT(rtBlur, tw, th, 0, kFilter, kFormat, kRWMode);
            cb.SetGlobalTexture(Uniforms._MainTex, rtMask);
            cb.Blit(rtMask, rtBlur, material, this.occlusionSource == OcclusionSource.GBuffer ? 4 : 3);
            cb.ReleaseTemporaryRT(rtMask);

            // Separable blur (vertical pass)
            rtMask = Uniforms._OcclusionTexture;
            cb.GetTemporaryRT(rtMask, tw, th, 0, kFilter, kFormat, kRWMode);
            cb.SetGlobalTexture(Uniforms._MainTex, rtBlur);
            cb.Blit(rtBlur, rtMask, material, 5);
            cb.ReleaseTemporaryRT(rtBlur);

            if (this.context.profile.debugViews.IsModeActive(DebugMode.AmbientOcclusion))
            {
                cb.SetGlobalTexture(Uniforms._MainTex, rtMask);
                cb.Blit(rtMask, BuiltinRenderTextureType.CameraTarget, material, 8);
                this.context.Interrupt();
            }
            else if (this.ambientOnlySupported)
            {
                cb.SetRenderTarget(this.m_MRT, BuiltinRenderTextureType.CameraTarget);
                cb.DrawMesh(GraphicsUtils.quad, Matrix4x4.identity, material, 0, 7);
            }
            else
            {
                var fbFormat = this.context.isHdr ? RenderTextureFormat.DefaultHDR : RenderTextureFormat.Default;

                int tempRT = Uniforms._TempRT;
                cb.GetTemporaryRT(tempRT, this.context.width, this.context.height, 0, FilterMode.Bilinear, fbFormat);
                cb.Blit(BuiltinRenderTextureType.CameraTarget, tempRT, blitMaterial, 0);
                cb.SetGlobalTexture(Uniforms._MainTex, tempRT);
                cb.Blit(tempRT, BuiltinRenderTextureType.CameraTarget, material, 6);
                cb.ReleaseTemporaryRT(tempRT);
            }

            cb.ReleaseTemporaryRT(rtMask);
        }
    }
}
