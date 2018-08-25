using UnityEngine;

namespace Assets.Scripts.SceneElements.Camera
{
    public class RtsCamera : MonoBehaviour
    {
        public float ScrollWidth = 20f;
        public float ScrollSpeed = 200f;
        public float MaxCameraHeight = 1000f;
        public float MinCameraHeight = 160f;
        public float RotateSpeed = 100f;
        public float RotateAmount = 1f;

        private UnityEngine.Camera camera;

        // Use this for initialization
        void Start()
        {
            this.camera = this.GetComponent<UnityEngine.Camera>();
        }

        // Update is called once per frame
        void Update()
        {
                this.MoveCamera();
                this.RotateCamera();
        }

        private void MoveCamera()
        {
            float xpos = Input.mousePosition.x;
            float ypos = Input.mousePosition.y;
            Vector3 movement = new Vector3(0, 0, 0);
            bool mouseScroll = false;

            //horizontal camera movement
            if (xpos >= 0 && xpos < this.ScrollWidth)
            {
                movement.x -= this.ScrollSpeed;
                mouseScroll = true;
            }
            else if (xpos <= Screen.width && xpos > Screen.width - this.ScrollWidth)
            {
                movement.x += this.ScrollSpeed;
                mouseScroll = true;
            }

            //vertical camera movement
            if (ypos >= 0 && ypos < this.ScrollWidth)
            {
                movement.z -= this.ScrollSpeed;
                mouseScroll = true;
            }
            else if (ypos <= Screen.height && ypos > Screen.height - this.ScrollWidth)
            {
                movement.z += this.ScrollSpeed;
                mouseScroll = true;
            }

            //make sure movement is in the direction the camera is pointing
            //but ignore the vertical tilt of the camera to get sensible scrolling
            movement = this.camera.transform.TransformDirection(movement);
            movement.y = 0;

            //away from ground movement
            movement.y -= this.ScrollSpeed * Input.GetAxis("Mouse ScrollWheel");

            //calculate desired camera position based on received input
            Vector3 origin = this.camera.transform.position;
            Vector3 destination = origin;
            destination.x += movement.x;
            destination.y += movement.y;
            destination.z += movement.z;

            //limit away from ground movement to be between a minimum and maximum distance
            if (destination.y > this.MaxCameraHeight)
            {
                destination.y = this.MaxCameraHeight;
            }
            else if (destination.y < this.MinCameraHeight)
            {
                destination.y = this.MinCameraHeight;
            }

            //if a change in position is detected perform the necessary update
            if (destination != origin)
            {
                this.camera.transform.position = Vector3.MoveTowards(origin, destination, Time.deltaTime * this.ScrollSpeed);
            }
        }

        private void RotateCamera()
        {
            Vector3 origin = this.camera.transform.eulerAngles;
            Vector3 destination = origin;

            //detect rotation amount if ALT is being held and the Right mouse button is down
            if ((Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt)) && Input.GetMouseButton(1))
            {
                destination.x -= Input.GetAxis("Mouse Y") * this.RotateAmount;
                destination.y += Input.GetAxis("Mouse X") * this.RotateAmount;
            }

            //if a change in position is detected perform the necessary update
            if (destination != origin)
            {
                this.camera.transform.eulerAngles = Vector3.MoveTowards(origin, destination, Time.deltaTime * this.RotateSpeed);
            }
        }
    }
}
