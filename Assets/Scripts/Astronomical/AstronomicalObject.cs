using System;
using System.ComponentModel;
using UnityEngine;

namespace Assets.Scripts.Astronomical
{
    public class AstronomicalObject : MonoBehaviour
    {
        private static readonly Vector3[] FaceDirections =
            {Vector3.up, Vector3.down, Vector3.left, Vector3.right, Vector3.forward, Vector3.back};

        private static readonly int NumberOfFaces = FaceDirections.Length;

        [NonSerialized]
        public readonly AstronomicalObjectFace[] Faces = new AstronomicalObjectFace[NumberOfFaces];

        public void Start()
        {
            for (var faceIndex = 0; faceIndex < NumberOfFaces; faceIndex++)
            {
                var faceGameObject = new GameObject($"AstronomicalObjectFace {FaceDirections[faceIndex].x}, {FaceDirections[faceIndex].y}, {FaceDirections[faceIndex].z}");
                faceGameObject.transform.parent = this.transform;
                faceGameObject.transform.localPosition = Vector3.zero;
                faceGameObject.transform.localScale = Vector3.one;

                var face = new AstronomicalObjectFace(FaceDirections[faceIndex])
                {
                    FaceGameObject = faceGameObject
                };

                face.Load();

                this.Faces[faceIndex] = face;
            }
        }

        public void Update()
        {
            ViewerDataProvider.Instance.Data.SetViewerPosition(Camera.main.transform.position);

            this.ApplyLod();
        }

        public void ApplyLod()
        {
            for (var faceIndex = 0; faceIndex < NumberOfFaces; faceIndex++)
            {
                this.Faces[faceIndex].ApplyLod();
            }
        }
    }
}