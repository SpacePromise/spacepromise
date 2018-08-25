using UnityEngine;

namespace Assets.ThirdParty.PostProcessing.Runtime.Attributes
{
    public sealed class MinAttribute : PropertyAttribute
    {
        public readonly float min;

        public MinAttribute(float min)
        {
            this.min = min;
        }
    }
}
