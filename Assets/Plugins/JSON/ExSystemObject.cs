using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using Object = UnityEngine.Object;

public static class ExSystemObject
{
    public static IFormatProvider Format = new NumberFormatInfo();

    public static TResult Cast<TResult>(this object _obj, TResult _default = default(TResult))
    {
        if (_obj == null)
            return _default;
        if (_obj is TResult)
            return (TResult)_obj;

        try
        {
            IConvertible res = _obj as IConvertible;
            if (res != null)
                return (TResult)res.ToType(typeof(TResult), Format);
            throw new Exception("Inconvertable type");
        }
        catch (Exception _ex)
        {
            if (typeof(TResult).IsEnum)
            {
                try
                {
                    string str = _obj as string;
                    if (str != null)
                        return (TResult)Enum.Parse(typeof(TResult), str);
                    return (TResult)Enum.ToObject(typeof(TResult), _obj);
                }
                catch (Exception _ex2)
                {
                    _ex = _ex2;
                }
            }
#if UNITY_EDITOR || DEBUG
            Debug.LogWarning("Unable to convert from " + _obj.GetType().Name + " to " + typeof(TResult).Name + ": " + _ex);
#endif
        }
        return _default;
    }

    public static object Cast(this object _obj, Type _castType, object _default)
    {
        if (_obj == null)
            return _default;
        if (_castType.IsInstanceOfType(_obj))
            return _obj;

        try
        {
            IConvertible res = _obj as IConvertible;
            if (res != null)
                return res.ToType(_castType, Format);
            throw new Exception("Inconvertable type");
        }
        catch (Exception _ex)
        {
            if (_castType.IsEnum)
            {
                try
                {
                    return Enum.ToObject(_castType, _obj);
                }
                catch (Exception _ex2)
                {
                    _ex = _ex2;
                }
            }
#if UNITY_EDITOR || DEBUG
            Debug.LogWarning("Unable to convert from " + _obj.GetType().Name + " to " + _castType.Name + ": " + _ex);
#endif
        }
        return _default;
    }

    public static TResult CastNew<TResult>(this object _obj)
        where TResult : new()
    {
        if (_obj == null)
            return new TResult();
        if (_obj is TResult)
            return (TResult)_obj;

        IConvertible res = _obj as IConvertible;
        if (res != null)
        {
            try
            {
                return (TResult)res.ToType(typeof(TResult), Format);
            }
            catch (Exception _ex)
            {
#if UNITY_EDITOR || DEBUG
                Debug.LogWarning("Unable to convert from " + _obj.GetType().Name + " to " + typeof(TResult).Name + ": " + _ex);
#endif
                return new TResult();
            }
        }

        Debug.LogWarning("Unable to convert from" + _obj.GetType().Name + " to " + typeof(TResult).Name);
        return new TResult();
    }

    public static T DeepClone<T>(this T a)
    {
        using (MemoryStream stream = new MemoryStream())
        {
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(stream, a);
            stream.Position = 0;
            return (T)formatter.Deserialize(stream);
        }
    }

    #region Deep Clone Advanced

    /// <summary>
    /// The Clone Method that will be recursively used for the deep clone.
    /// </summary>
    private static readonly MethodInfo CloneMethod = typeof(object).GetMethod("MemberwiseClone", BindingFlags.NonPublic | BindingFlags.Instance);

    /// <summary>
    /// Returns TRUE if the type is a primitive one, FALSE otherwise.
    /// </summary>
    private static bool IsPrimitive(Type type)
    {
        if (type == typeof(String)) return true;
        return (type.IsValueType & type.IsPrimitive);
    }

    /// <summary>
    /// Returns a Deep Clone / Deep Copy of an object using a recursive call to the CloneMethod specified above.
    /// </summary>
    public static void DeepCloneAdvanced( this object _from, object _to)
    {
        DeepClone_Internal(_from, new Dictionary<object, object>(new ReferenceEqualityComparer()), _to);
    }

    /// <summary>
    /// Returns a Deep Clone / Deep Copy of an object using a recursive call to the CloneMethod specified above.
    /// </summary>
    public static object DeepCloneAdvanced( this object obj)
    {
        return DeepClone_Internal(obj, new Dictionary<object, object>(new ReferenceEqualityComparer()));
    }

    /// <summary>
    /// Returns a Deep Clone / Deep Copy of an object of type T using a recursive call to the CloneMethod specified above.
    /// </summary>
    public static T DeepCloneAdvanced<T>(this T _obj)
    {
        return (T) DeepCloneAdvanced((object)_obj);
    }

    private static object DeepClone_Internal(object obj, IDictionary<object, object> visited, object _target = null)
    {
        if (obj == null) return null;
        if (obj is Object && _target == null) return obj;
        var typeToReflect = obj.GetType();
        if (IsPrimitive(typeToReflect)) return obj;
        if (visited.ContainsKey(obj)) return visited[obj];
        if (typeof(Delegate).IsAssignableFrom(typeToReflect)) return null;
        object cloneObject = _target == null ? CloneMethod?.Invoke(obj, null) : _target;
        if (typeToReflect.IsArray)
        {
            var arrayType = typeToReflect.GetElementType();
            if (IsPrimitive(arrayType) == false)
            {
                Array clonedArray = (Array)cloneObject;
                ForEach(clonedArray, (array, indices) => array.SetValue(DeepClone_Internal(clonedArray.GetValue(indices), visited), indices));
            }

        }
        visited.Add(obj, cloneObject);
        CopyFields(obj, visited, cloneObject, typeToReflect);
        RecursiveCopyBaseTypePrivateFields(obj, visited, cloneObject, typeToReflect);
        return cloneObject;
    }

    private static void RecursiveCopyBaseTypePrivateFields(object originalObject, IDictionary<object, object> visited, object cloneObject, Type typeToReflect)
    {
        if (typeToReflect.BaseType != null)
        {
            RecursiveCopyBaseTypePrivateFields(originalObject, visited, cloneObject, typeToReflect.BaseType);
            CopyFields(originalObject, visited, cloneObject, typeToReflect.BaseType, BindingFlags.Instance | BindingFlags.NonPublic, info => info.IsPrivate);
        }
    }

    private static void CopyFields(object originalObject, IDictionary<object, object> visited, object cloneObject, Type typeToReflect, BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.FlattenHierarchy, Func<FieldInfo, bool> filter = null)
    {
        foreach (FieldInfo fieldInfo in typeToReflect.GetFields(bindingFlags))
        {
            if (filter != null && filter(fieldInfo) == false) continue;
            if (IsPrimitive(fieldInfo.FieldType)) continue;
            var originalFieldValue = fieldInfo.GetValue(originalObject);
            var clonedFieldValue = DeepClone_Internal(originalFieldValue, visited);
            fieldInfo.SetValue(cloneObject, clonedFieldValue);
        }
    }

    private class ReferenceEqualityComparer : EqualityComparer<object>
    {
        public override bool Equals(object x, object y)
        {
            return ReferenceEquals(x, y);
        }
        public override int GetHashCode(object obj)
        {
            if (obj == null) return 0;
            return obj.GetHashCode();
        }
    }

    private static void ForEach(Array array, Action<Array, int[]> action)
    {
        if (array.LongLength == 0) return;
        ArrayTraverse walker = new ArrayTraverse(array);
        do action(array, walker.Position);
        while (walker.Step());
    }

    private class ArrayTraverse
    {
        public int[] Position;
        private int[] maxLengths;

        public ArrayTraverse(Array array)
        {
            maxLengths = new int[array.Rank];
            for (int i = 0; i < array.Rank; ++i)
            {
                maxLengths[i] = array.GetLength(i) - 1;
            }
            Position = new int[array.Rank];
        }

        public bool Step()
        {
            for (int i = 0; i < Position.Length; ++i)
            {
                if (Position[i] < maxLengths[i])
                {
                    Position[i]++;
                    for (int j = 0; j < i; j++)
                    {
                        Position[j] = 0;
                    }
                    return true;
                }
            }
            return false;
        }
    }

    #endregion

}
