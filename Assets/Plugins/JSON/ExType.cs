using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;
using JetBrains.Annotations;
using UnityEngine;

public static class ExType
{
    /// <summary>
    /// Создает новый объект указанного класса, но только не для UnityEngine.Object
    /// </summary>
    public static object CreateDefaultSerialization( this Type _type )
    {
        if (_type == typeof(string))
            return string.Empty;

        if (typeof(UnityEngine.Object).IsAssignableFrom(_type))
            return null;

        return Activator.CreateInstance(_type);
    }
    
    public static T CreateDefaultSerialization<T>()
    {
        return (T) CreateDefaultSerialization(typeof(T));
    }
    
    /// <summary>
    /// The binding flags for instance fields and properties.
    /// </summary>
    public const BindingFlags instanceBindingFlags =
        BindingFlags.FlattenHierarchy | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;

    /// <summary>
    /// The binding flags for static fields and properties.
    /// </summary>
    public const BindingFlags staticBindingFlags =
        BindingFlags.FlattenHierarchy | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public;

    /// <summary>
    /// A regular expression to match an instance's prefab name
    /// </summary>
    private static readonly Regex matchPrefabName =
        new Regex(@".+(?=\s*\(Clone\))|.+");

    /// <summary>
    /// Gets the custom attributes.
    /// </summary>
    /// <remarks>
    /// http://msdn.microsoft.com/en-us/library/dwc6ew1d(v=vs.90).aspx
    /// </remarks>
    /// <returns>The custom attributes on the provider.</returns>
    /// <param name='provider'>Provider.</param>
    /// <param name='inherit'>
    /// <c>true</c> if inherited attributes should be included; otherwise, <c>false</c>.
    /// </param>
    /// <typeparam name='T'>The type of the custom attributes desired.</typeparam>
    /// <exception cref='System.ArgumentNullException'>
    /// Is thrown if the provider is <see langword="null" />.
    /// </exception>
    public static T[] GetCustomAttributes<T>(
        this ICustomAttributeProvider provider, bool inherit = false
    ) where T : Attribute
    {
        if (provider == null)
        {
            throw new ArgumentNullException("provider");
        }
        T[] attributes = provider.GetCustomAttributes(typeof(T), inherit) as T[];
        if (attributes == null)
        {   // WORKAROUND: Due to a bug in the code for retrieving attributes from a dynamic generated parameter,
            // GetCustomAttributes can return an instance of an object[] instead of T[], and hence the cast above
            // will return null.
            return new T[0];
        }
        return attributes;
    }

    /// <summary>
    /// Gets the field value on an instance.
    /// </summary>
    /// <returns>The field value on an instance.</returns>
    /// <param name="provider">Provider.</param>
    /// <param name="fieldName">Field name.</param>
    /// <param name="bindingAttr">Binding flags.</param>
    /// <typeparam name="T">The type of the field.</typeparam>
    public static T GetFieldValue<T>(
        this object provider, string fieldName, BindingFlags bindingAttr = instanceBindingFlags
        )
    {
        return GetFieldValue<T>(provider.GetType(), provider, fieldName, bindingAttr);
    }

    /// <summary>
    /// Gets the field value on a provider
    /// </summary>
    /// <param name="type">Type.</param>
    /// <param name="provider">Provider.</param>
    /// <param name="fieldName">Field name.</param>
    /// <param name="bindingAttr">Binding attr.</param>
    /// <typeparam name="T">The type of the field.</typeparam>
    /// <exception cref="System.ArgumentNullException">
    /// Is thrown if the type or fieldName is <see langword="null" />.
    /// </exception>
    /// <exception cref="System.ArgumentException">Is thrown if fieldName is empty.</exception>
    private static T GetFieldValue<T>(
        Type type, object provider, string fieldName, BindingFlags bindingAttr
        )
    {
        if (type == null)
        {
            throw new ArgumentNullException("type");
        }
        else if (fieldName == null)
        {
            throw new ArgumentNullException("fieldName");
        }
        else if (fieldName.Length == 0)
        {
            throw new ArgumentException("No field name specified.", "fieldName");
        }
        return (T)type.GetField(fieldName, bindingAttr).GetValue(provider);
    }

    /// <summary>
    /// Gets the name of the prefab associated with the supplied instance.
    /// </summary>
    /// <returns>The prefab name.</returns>
    /// <param name="instance">Instance.</param>
    public static string GetPrefabName(this UnityEngine.Object instance)
    {
        return matchPrefabName.Match(instance.name).Value;
    }

    /// <summary>
    /// Gets the property value on an instance.
    /// </summary>
    /// <returns>The property value on an instance.</returns>
    /// <param name="provider">Provider.</param>
    /// <param name="propertyName">Property name.</param>
    /// <param name="bindingAttr">Binding flags.</param>
    /// <param name="index">Index.</param>
    /// <typeparam name="T">The type of the property.</typeparam>
    public static T GetPropertyValue<T>(
        this object provider,
        string propertyName,
        BindingFlags bindingAttr = instanceBindingFlags,
        object[] index = null
        )
    {
        return GetPropertyValue<T>(provider.GetType(), provider, propertyName, bindingAttr, index);
    }

    /// <summary>
    /// Gets the property value on a provider.
    /// </summary>
    /// <returns>The property value on an instance.</returns>
    /// <param name="provider">Provider.</param>
    /// <param name="propertyName">Property name.</param>
    /// <param name="bindingAttr">Binding flags.</param>
    /// <param name="index">Index.</param>
    /// <typeparam name="T">The type of the property.</typeparam>
    /// <exception cref="System.ArgumentNullException">
    /// Is thrown if the provider or propertyName is <see langword="null" />.
    /// </exception>
    /// <exception cref="System.ArgumentException">Is thrown if propertyName is empty.</exception>
    private static T GetPropertyValue<T>(
        Type type, object provider, string propertyName, BindingFlags bindingAttr, object[] index
        )
    {
        if (provider == null)
        {
            throw new ArgumentNullException("provider");
        }
        else if (propertyName == null)
        {
            throw new ArgumentNullException("propertyName");
        }
        else if (propertyName.Length == 0)
        {
            throw new ArgumentException("No property name specified.", "propertyName");
        }
        return (T)provider.GetType().GetProperty(propertyName, bindingAttr).GetValue(provider, index);
    }

    /// <summary>
    /// Gets the field value on a class.
    /// </summary>
    /// <returns>The field value on a class.</returns>
    /// <param name="type">Type.</param>
    /// <param name="fieldName">Field name.</param>
    /// <param name="bindingAttr">Binding flags.</param>
    /// <typeparam name="T">The type of the field.</typeparam>
    public static T GetStaticFieldValue<T>(
        this Type type, string fieldName, BindingFlags bindingAttr = staticBindingFlags
        )
    {
        return GetFieldValue<T>(type, null, fieldName, bindingAttr);
    }

    /// <summary>
    /// Gets the property value on a class.
    /// </summary>
    /// <returns>The property value on a class.</returns>
    /// <param name="provider">Provider.</param>
    /// <param name="propertyName">Property name.</param>
    /// <param name="bindingAttr">Binding flags.</param>
    /// <param name="index">Index.</param>
    /// <typeparam name="T">The type of the property.</typeparam>
    public static T GetStaticPropertyValue<T>(
        this Type type,
        string propertyName,
        BindingFlags bindingAttr = staticBindingFlags,
        object[] index = null
    )
    {
        if (propertyName == null)
        {
            throw new ArgumentNullException("propertyName");
        }
        else if (propertyName.Length == 0)
        {
            throw new ArgumentException("No property name specified.", "propertyName");
        }
        return (T)type.GetProperty(propertyName, bindingAttr).GetValue(null, index);
    }
    

    /// <summary>
    /// Возвращает FullName классы, заменяя + на .
    /// </summary>
    public static string FullNameDots( this Type _type )
    {
        return _type?.FullName?.Replace('+', '.');
    }

    #region Type Searcher

    private static readonly Dictionary<(Type, Type), List<string>> TypeSearcherHash = new Dictionary<(Type, Type), List<string>>();
    private static readonly Dictionary<(Type, Type), bool> TypeSearcherHashBool = new Dictionary<(Type, Type), bool>();
    private static Dictionary<(Assembly, Assembly), bool> isAssemblyDependants = new Dictionary<(Assembly, Assembly), bool>();
    
    private static List<string> defaultList = new List<string>();

    /// <summary>
    /// Returns type of element is this type is a collection. Returns null otherwise
    /// </summary>
    private static Type GetElementType(Type _collectionType)
    {
        if (!typeof(ICollection).IsAssignableFrom(_collectionType)) 
            return null;
        
        Type GetElementType( Type _current )
        {
            if (_current == null || _current == typeof(object))
                return null;
                
            if (_current.IsArray)
                return _current.GetElementType();
                
            if (_current.IsGenericType)
            {
                Type[] arguments = _current.GetGenericArguments();
                if (arguments.Length == 1)
                    return arguments[0];
                return null;
            }

            return GetElementType(_current.BaseType);
        }

        return GetElementType(_collectionType);
    }
    
    private static bool IsType<T>( Type _containerType )
    {
        return typeof(T).IsAssignableFrom(_containerType) || typeof(ICollection<T>).IsAssignableFrom(_containerType);
    }

    #endregion
}
