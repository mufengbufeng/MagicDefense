using System;
using UnityEngine;

public static class TransformExtended
{
    public static Type GetOrAddComponent(this Transform transform, Type type)
    {
        var component = transform.GetComponent(type);
        if (component == null)
        {
            component = transform.gameObject.AddComponent(type);
        }
        return component.GetType();
    }

    public static T GetOrAddComponent<T>(this Transform transform) where T : Component
    {
        var component = transform.GetComponent<T>();
        if (component == null)
        {
            component = transform.gameObject.AddComponent<T>();
        }
        return component;
    }

}