using UnityEngine;

public static class GameObjectExtensions
{
    public static T GetOrAddComponent<T>(this GameObject @this)
        where T : Component
    {
        var component = @this.GetComponent<T>();
        return component != null
            ? component
            : @this.AddComponent<T>();
    }
}