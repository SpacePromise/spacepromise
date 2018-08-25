using UnityEngine;

namespace Assets.ThirdParty.AmplifyShaderEditor.Examples.Community.ForceShield
{

    public class ForceShieldImpactDetection : MonoBehaviour
    {


        private float hitTime;
        private Material mat;

        void Start()
        {

			// Store material reference
            this.mat = this.GetComponent<Renderer>().material;

        }

        void Update()
        {

			// Animate the _hitTime shader property for impact effect
            if (this.hitTime > 0)
            {
                this.hitTime -= Time.deltaTime * 1000;
                if (this.hitTime < 0)
                {
                    this.hitTime = 0;
                }
                this.mat.SetFloat("_HitTime", this.hitTime);
            }

        }

        void OnCollisionEnter(Collision collision)
        {
			// On colission set shader vector property _HitPosition with the impact point and
			// set _hittime shader property to start the impact effect
            foreach (ContactPoint contact in collision.contacts)
            {
                this.mat.SetVector("_HitPosition", this.transform.InverseTransformPoint(contact.point));
                this.hitTime = 500;
				this.mat.SetFloat("_HitTime", this.hitTime);
            }
        }
    }

}
