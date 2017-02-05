using UnityEngine;

namespace Assets.Environment.Trees.Scripts
{
    public class TreeGrowthScale : MonoBehaviour
    {
        public float GrowthRatio = 0.001111111F;
        [Range(0.1f, 1f)] public float InitialScale = 0.1f;
        [Range(0.1f, 1f)] public float CurrentScale = 0.1f;
        public bool IsFullyGrown;

        public void Start()
        {
            this.CurrentScale = this.InitialScale;
        }

        public void FixedUpdate()
        {
            if (this.IsFullyGrown)
                return;

            this.CurrentScale += Time.deltaTime * this.GrowthRatio;
        
            if (this.CurrentScale >= 1f)
            {
                this.CurrentScale = 1f;
                this.IsFullyGrown = true;
            }

            this.gameObject.transform.localScale = new Vector3(this.CurrentScale, this.CurrentScale, this.CurrentScale);
        }
    }
}
