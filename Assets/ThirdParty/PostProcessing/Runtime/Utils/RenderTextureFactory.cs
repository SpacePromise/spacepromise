using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.ThirdParty.PostProcessing.Runtime.Utils
{
    public sealed class RenderTextureFactory : IDisposable
    {
        HashSet<RenderTexture> m_TemporaryRTs;

        public RenderTextureFactory()
        {
            this.m_TemporaryRTs = new HashSet<RenderTexture>();
        }

        public RenderTexture Get(RenderTexture baseRenderTexture)
        {
            return this.Get(
                baseRenderTexture.width,
                baseRenderTexture.height,
                baseRenderTexture.depth,
                baseRenderTexture.format,
                baseRenderTexture.sRGB ? RenderTextureReadWrite.sRGB : RenderTextureReadWrite.Linear,
                baseRenderTexture.filterMode,
                baseRenderTexture.wrapMode
                );
        }

        public RenderTexture Get(int width, int height, int depthBuffer = 0, RenderTextureFormat format = RenderTextureFormat.ARGBHalf, RenderTextureReadWrite rw = RenderTextureReadWrite.Default, FilterMode filterMode = FilterMode.Bilinear, TextureWrapMode wrapMode = TextureWrapMode.Clamp, string name = "FactoryTempTexture")
        {
            var rt = RenderTexture.GetTemporary(width, height, depthBuffer, format, rw); // add forgotten param rw
            rt.filterMode = filterMode;
            rt.wrapMode = wrapMode;
            rt.name = name;
            this.m_TemporaryRTs.Add(rt);
            return rt;
        }

        public void Release(RenderTexture rt)
        {
            if (rt == null)
                return;

            if (!this.m_TemporaryRTs.Contains(rt))
                throw new ArgumentException(string.Format("Attempting to remove a RenderTexture that was not allocated: {0}", rt));

            this.m_TemporaryRTs.Remove(rt);
            RenderTexture.ReleaseTemporary(rt);
        }

        public void ReleaseAll()
        {
            var enumerator = this.m_TemporaryRTs.GetEnumerator();
            while (enumerator.MoveNext())
                RenderTexture.ReleaseTemporary(enumerator.Current);

            this.m_TemporaryRTs.Clear();
        }

        public void Dispose()
        {
            this.ReleaseAll();
        }
    }
}
