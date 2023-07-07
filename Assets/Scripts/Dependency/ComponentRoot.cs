using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComponentRoot
{
    private static Dictionary<Type, object> _binds = new Dictionary<Type, object>();

    public static void Bind<T>(T @object)
    {
        var type = typeof(T);

        if (!_binds.ContainsKey(type))
        {
            _binds.Add(type, null);
        }

        _binds[type] = @object;
    }

    public static T Resolve<T>()
    {
        var type = typeof(T);

        if (!_binds.ContainsKey(type))
        {
            throw new Exception($"{type} Dont Exist");
        }

        return (T)_binds[type];
    }
}
