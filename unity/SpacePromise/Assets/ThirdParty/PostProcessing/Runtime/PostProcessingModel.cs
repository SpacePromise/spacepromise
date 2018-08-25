using System;
using Assets.ThirdParty.PostProcessing.Runtime.Attributes;
using UnityEngine;

namespace Assets.ThirdParty.PostProcessing.Runtime
{
    [Serializable]
    public abstract class PostProcessingModel
    {
        [SerializeField, GetSet("enabled")]
        bool m_Enabled;
        public bool enabled
        {
            get { return this.m_Enabled; }
            set
            {
                this.m_Enabled = value;

                if (value)
                    this.OnValidate();
            }
        }

        public abstract void Reset();

        public virtual void OnValidate()
        {}
    }
}
