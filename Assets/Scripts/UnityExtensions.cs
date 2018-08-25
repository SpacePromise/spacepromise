using System.Linq;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace Assets.Scripts
{
    public static class UnityExtensions
    {
        public static void DestroyChildrenWithComponent<T>(this MonoBehaviour component)
            where T : Component
        {
            // Retrieve game objects
            var existingObjects = component.GetComponentsInChildren<T>().Select(comp => comp.gameObject).ToList();
        
            // Destroy
            foreach (var obj in existingObjects)
                obj.DestroyChecked();
        
            // Log
            if (existingObjects.Any())
                Debug.Log($"Destroyed {existingObjects.Count} {typeof(T).Name} objects.");
        }

        public static void DestroyChecked(this UnityObject obj)
        {
#if UNITY_EDITOR
            if (Application.isPlaying)
                UnityObject.Destroy(obj);
            else
                UnityObject.DestroyImmediate(obj);
#else
        UnityObject.Destroy(obj);
#endif
        }

        public static GameObject CreateChildGameObjectFromComponent<T>(this MonoBehaviour component)
            where T : Component
        {
            var obj = new GameObject
            {
                name = typeof(T).Name
            };
            obj.transform.parent = component.transform;
            obj.transform.localPosition = Vector3.zero;
            obj.AddComponent<T>();

            return obj;
        }
    }
}