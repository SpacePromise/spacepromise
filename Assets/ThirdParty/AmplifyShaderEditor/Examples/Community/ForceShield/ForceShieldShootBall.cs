using UnityEngine;

namespace Assets.ThirdParty.AmplifyShaderEditor.Examples.Community.ForceShield
{
    public class ForceShieldShootBall : MonoBehaviour
    {

		// Shooting balls XD 

        public Rigidbody bullet;
        public Transform origshoot;
        public float speed = 1000.0f;
        private float distance = 10.0f;

        void Update()
        {
            if (Input.GetButtonDown("Fire1"))
            {
                Vector3 targetpoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, this.distance);
                targetpoint = Camera.main.ScreenToWorldPoint(targetpoint);
                Rigidbody bulletInstance = Instantiate(this.bullet, this.transform.position, Quaternion.identity) as Rigidbody;
                bulletInstance.transform.LookAt(targetpoint);
                bulletInstance.AddForce(bulletInstance.transform.forward * this.speed);
            }

        }
    }

}
