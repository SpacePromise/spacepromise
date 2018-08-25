using System;
using UnityEngine;

namespace Assets.ThirdParty.PostProcessing.Runtime.Utils
{
    // Small wrapper on top of AnimationCurve to handle zero-key curves and keyframe looping

    [Serializable]
    public sealed class ColorGradingCurve
    {
        public AnimationCurve curve;

        [SerializeField]
        bool m_Loop;

        [SerializeField]
        float m_ZeroValue;

        [SerializeField]
        float m_Range;

        AnimationCurve m_InternalLoopingCurve;

        public ColorGradingCurve(AnimationCurve curve, float zeroValue, bool loop, Vector2 bounds)
        {
            this.curve = curve;
            this.m_ZeroValue = zeroValue;
            this.m_Loop = loop;
            this.m_Range = bounds.magnitude;
        }

        public void Cache()
        {
            if (!this.m_Loop)
                return;

            var length = this.curve.length;

            if (length < 2)
                return;

            if (this.m_InternalLoopingCurve == null)
                this.m_InternalLoopingCurve = new AnimationCurve();

            var prev = this.curve[length - 1];
            prev.time -= this.m_Range;
            var next = this.curve[0];
            next.time += this.m_Range;
            this.m_InternalLoopingCurve.keys = this.curve.keys;
            this.m_InternalLoopingCurve.AddKey(prev);
            this.m_InternalLoopingCurve.AddKey(next);
        }

        public float Evaluate(float t)
        {
            if (this.curve.length == 0)
                return this.m_ZeroValue;

            if (!this.m_Loop || this.curve.length == 1)
                return this.curve.Evaluate(t);

            return this.m_InternalLoopingCurve.Evaluate(t);
        }
    }
}
