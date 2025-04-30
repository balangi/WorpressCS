using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class HookManager : IEnumerable, IEnumerator
{
    private readonly Dictionary<int, Dictionary<string, Action>> _callbacks = new();
    private readonly List<int> _priorities = new();
    private readonly Stack<List<int>> _iterations = new();

    private int _currentPriorityIndex = -1;

    // افزودن هک
    public void AddFilter(string hookName, Action callback, int priority)
    {
        if (!_callbacks.ContainsKey(priority))
        {
            _callbacks[priority] = new Dictionary<string, Action>();
            _priorities.Add(priority);
            _priorities.Sort();
        }

        var functionKey = GenerateUniqueFunctionKey(hookName, callback, priority);
        _callbacks[priority][functionKey] = callback;
    }

    // حذف هک
    public bool RemoveFilter(string hookName, Action callback, int priority)
    {
        var functionKey = GenerateUniqueFunctionKey(hookName, callback, priority);

        if (_callbacks.ContainsKey(priority) && _callbacks[priority].ContainsKey(functionKey))
        {
            _callbacks[priority].Remove(functionKey);

            if (_callbacks[priority].Count == 0)
            {
                _callbacks.Remove(priority);
                _priorities.Remove(priority);
            }

            return true;
        }

        return false;
    }

    // اجرای همه هک‌ها
    public void DoAction()
    {
        foreach (var priority in _priorities)
        {
            if (_callbacks.ContainsKey(priority))
            {
                foreach (var callback in _callbacks[priority].Values)
                {
                    callback?.Invoke();
                }
            }
        }
    }

    // ایجاد کلید منحصر به فرد برای توابع
    private string GenerateUniqueFunctionKey(string hookName, Action callback, int priority)
    {
        return $"{hookName}_{callback.Method.Name}_{priority}";
    }

    // پیاده‌سازی واسط Iterator
    public IEnumerator GetEnumerator()
    {
        _currentPriorityIndex = -1;
        return this;
    }

    public object Current => _priorities[_currentPriorityIndex];

    public bool MoveNext()
    {
        _currentPriorityIndex++;
        return _currentPriorityIndex < _priorities.Count;
    }

    public void Reset()
    {
        _currentPriorityIndex = -1;
    }
}