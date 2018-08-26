using UnityEngine;
using UnityEngine.Experimental.UIElements;

namespace Assets.Scripts.Astronomical
{
    public class AstronomicalObject : MonoBehaviour
    {
        private const int NumberOfFaces = 1;

        private readonly AstronomicalObjectFace[] faces = new AstronomicalObjectFace[NumberOfFaces];

        public void Start()
        {
            for (var faceIndex = 0; faceIndex < NumberOfFaces; faceIndex++)
            {
                var faceGameObject = new GameObject("AstronomicalObjectFace");
                faceGameObject.transform.parent = this.transform;

                var face = new AstronomicalObjectFace
                {
                    FaceGameObject = faceGameObject
                };

                face.Load();

                this.faces[faceIndex] = face;
            }
        }

        public void OnValidate()
        {
            this.ApplyLod();
        }

        [Range(0, 8)]
        public int currentLod = 0;

        public void ApplyLod()
        {
            for (int faceIndex = 0; faceIndex < NumberOfFaces; faceIndex++)
            {
                this.faces[faceIndex].ApplyLod(this.currentLod);
            }
        }
    }
}