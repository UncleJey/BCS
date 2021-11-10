using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public static class ExList
{
    public static int IndexOf<T>(this IList<T> _array, T _value)
    {
        if (_array == null)
            return -1;
        for (int i = _array.Count - 1; i >= 0; i--)
        {
            if (Equals(_array[i], _value))
                return i;
        }
        return -1;
    }

    public static int IndexOf<T>(this IList<T> _array, Func<T, bool> _value)
    {
        if (_array == null)
            return -1;
        for (int i = _array.Count - 1; i >= 0; i--)
        {
            if (_value(_array[i]))
                return i;
        }
        return -1;
    }

    public static bool Contains<T>(this IList<T> _array, T _value)
    {
        return IndexOf(_array, _value) >= 0;
    }

    public static bool Contains<T>(this IList<T> _array, IList<T> _values)
    {
        for (int i = _values.Count - 1; i >= 0; i--)
        {
            if (IndexOf(_array, _values[i]) >= 0)
                return true;
        }
        return false;
    }

    public static bool Contains<T>(this IList<T> _array, Func<T, bool> _value)
    {
        if (_array == null)
            return false;
        for (int i = _array.Count - 1; i >= 0; i--)
        {
            if (_value(_array[i]))
                return true;
        }
        return false;
    }

    public static void AddOnce<T>(this IList<T> _list, T _value)
    {
        if (!_list.Contains(_value))
            _list.Add(_value);
    }

    public static void AddOnce(this IList _list, object _value)
    {
        if (!_list.Contains(_value))
            _list.Add(_value);
    }

    public static void AddNotNull(this IList _list, object _value)
    {
        if (_value != null)
            _list.Add(_value);
    }

    public static void AddCast<T>(this IList<T> _list, object _value)
    {
        _list.Add((T)_value);
    }

    public static T[] Remove<T>(this T[] _array, T _value)
    {
        if (_array == null)
            return null;
        return RemoveAt(_array, _array.IndexOf(_value));
    }
    
    public static T[] RemoveAt<T>(this T[] _array, int _index )
    {
        if (_array == null || _index < 0)
            return _array;
        T[] result = new T[_array.Length - 1];
        for (int i = 0; i < _index; i++) result[i] = _array[i];
        for (int i = _index + 1; i < _array.Length; i++) result[i - 1] = _array[i];

        return result;
    }

    public static T GetRandom<T>(this IList<T> _list, T _onEmpty = default(T))
    {
        if (_list == null || _list.Count == 0)
            return _onEmpty;
        var index = Random.Range(0, _list.Count);
        return _list[index];
    }

    public static object GetRandom(this IList _list, object _onEmpty)
    {
        if (_list.Count == 0)
            return _onEmpty;
        var index = Random.Range(0, _list.Count);
        return _list[index];
    }

    public static T GetOrInsert<T>(this IList<T> _list, int _index, T _def = default(T))
    {
        if ((uint)_index >= (uint)_list.Count)
            _list.Insert(_index, _def);
        return _list[_index];
    }

    public static T Get<T>(this IList<T> _list, int _index, T _def = default(T))
    {
        if (_list == null || (uint)_index >= (uint)_list.Count)
            return _def;
        return _list[_index];
    }

    public static T Get<T>(this T[,] _array, int _pos1, int _pos2, T _def = default(T))
    {
        if (_array != null && _array.IsInRange(_pos1, _pos2))
            return _array[_pos1, _pos2];
        return _def;
    }

    public static T GetClamped<T>(this IList<T> _list, int _index, T _def = default(T))
    {
        return Get(_list, Mathf.Clamp(_index, 0, _list.Count - 1), _def);
    }

    public static T Last<T>(this IList<T> _list, T _def = default(T))
    {
        return _list.Get(_list.Count - 1, _def);
    }

    public static T First<T>(this IList<T> _list, T _def = default(T))
    {
        return _list.Get(0, _def);
    }

    /// <summary>
    /// Возвращает первое значение, отвечающее условию сортировки
    /// </summary>
    public static T First<T>(this IList<T> _list, Func<T, T, bool> _condition, T _default = default(T))
    {
        if (_list == null || _list.Count == 0)
            return _default;
        List<T> lst = new List<T>(_list);
        lst.Sort((_x, _y) => _condition(_x, _y) ? -1 : 1);
        return lst[0];
    }

    public static void Set<T>(this IList<T> _list, int _index, T _value)
    {
        if ((uint)_index >= (uint)_list.Count)
            _list.Insert(_index, _value);
        else
            _list[_index] = _value;
    }

    public static void Replace<T>(this IList<T> _list, T _valueSource, T _valueNew)
    {
        int index = _list.IndexOf(_valueSource);
        if (index >= 0)
            _list[index] = _valueNew;
    }


    /// <summary>
    /// Меняет два элемента из позициями в списке
    /// </summary>
    public static void Exchange<T>(this IList<T> _list, T _first, T _second)
    {
        if (_list == null)
            return;
        int index1 = _list.IndexOf(_first);
        int index2 = _list.IndexOf(_second);

        if (index1 < 0 || index2 < 0)
            return;

        _list[index1] = _second;
        _list[index2] = _first;
    }

    /// <summary>
    /// Меняет два элемента из позициями в списке
    /// </summary>
    public static void Exchange<T>(this IList<T> _list, int _index1, int _index2)
    {
        if (_list == null)
            return;
        if (_index1 < 0 || _index2 < 0)
            return;

        T e1 = _list[_index1];
        _list[_index1] = _list[_index2];
        _list[_index2] = e1;
    }

    /// <summary>
    /// Перемешивает элементы списка в случайном порядке. Возвращает тот же список.
    /// </summary>
    public static IList<T> Shuffle<T>(this IList<T> _list)
    {
        List<T> tlist = new List<T>(_list);
        _list.Clear();
        while (tlist.Count > 0)
        {
            int pos = Random.Range(0, tlist.Count);
            _list.Add(tlist[pos]);
            tlist.RemoveAt(pos);
        }
        return _list;
    }

    public static int CountSafe(this IList _list)
    {
        return _list != null ? _list.Count : 0;
    }

    public static bool IsInRange(this IList _list, int _index)
    {
        return _list != null && ((uint)_index) < _list.Count;
    }

    public static bool IsInRange<T>(this T[,] _array, int _pos1, int _pos2)
    {
        return _array != null && ((uint)_pos1) < _array.GetLength(0) && ((uint)_pos2) < _array.GetLength(1);
    }

    public static TResult[] Select<TSource, TResult>(this IList<TSource> source, Func<TSource, TResult> selector)
    {
        TResult[] res = new TResult[source.Count];
        for (int i = source.Count - 1; i >= 0; i--)
            res[i] = selector(source[i]);
        return res;
    }

    public static TSource Aggregate<TSource>(this IList<TSource> _source, Func<TSource, TSource, TSource> _func)
    {
        if (_source.Count == 0)
            return default(TSource);

        TSource source1 = _source[0];
        for (int i = 1; i < _source.Count; i++)
            source1 = _func(source1, _source[i]);

        return source1;
    }

    /// <summary>
    /// Выполнить функция с объектом. Просто для удобства - чтобы можно было в одну строчку всё вписать.
    /// </summary>
    public static TResult DoWith<TSource, TResult>(this TSource _source, Func<TSource, TResult> _operator)
    {
        return _operator(_source);
    }

    public static T GetPrev<T>(this IList<T> _list, T _current)
    {
        return _list.IndexOf(_current) < 1 ? _list.Last() : _list[_list.IndexOf(_current) - 1];
    }

    public static T GetNext<T>(this IList<T> _list, T _current)
    {
        int idx = _list.IndexOf(_current);
        return (idx < 0 || idx == _list.Count - 1) ? _list.First() : _list[idx + 1];
    }
}
