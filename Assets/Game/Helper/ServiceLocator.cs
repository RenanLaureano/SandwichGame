using UnityEngine;
using System.Collections.Generic;
using System;

public class ServiceLocator : Singleton<ServiceLocator>
{
    private Dictionary<Type, System.Object> mObjects;

    protected ServiceLocator()
    {
        mObjects = new Dictionary<Type, object>();
    }
    public bool IsRegistered<T>()
    {
        return mObjects.ContainsKey(typeof(T));
    }
    public void Register<T>(T thing)
    {
        if (thing != null)
        {
            mObjects[typeof(T)] = thing;
        }
    }
    public T GetComponentRegistered<T>() where T : Component
    {
        var type = typeof(T);
        if (!mObjects.ContainsKey(type))
        {
            return null;
        }

        return (T)mObjects[typeof(T)];
    }
}
