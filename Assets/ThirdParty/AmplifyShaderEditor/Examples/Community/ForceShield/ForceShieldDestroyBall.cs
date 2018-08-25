using UnityEngine;

namespace Assets.ThirdParty.AmplifyShaderEditor.Examples.Community.ForceShield
{
    public class ForceShieldDestroyBall : MonoBehaviour
    {
		// Destroy the gameObject after lifetime
        public float lifetime = 5f;

        void Start()
        {
            Destroy(this.gameObject, this.lifetime);
        }
    }
}
