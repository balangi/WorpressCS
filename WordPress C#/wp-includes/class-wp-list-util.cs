using System;
using System.Collections.Generic;
using System.Linq;

public class ListUtil<T>
{
    /// <summary>
    /// ورودی اصلی
    /// </summary>
    private List<T> Input { get; set; }

    /// <summary>
    /// خروجی
    /// </summary>
    private List<T> Output { get; set; }

    /// <summary>
    /// پارامترهای موقت برای مرتب‌سازی
    /// </summary>
    private Dictionary<string, string> OrderBy { get; set; } = new();

    /// <summary>
    /// سازنده کلاس
    /// </summary>
    public ListUtil(List<T> input)
    {
        Input = input;
        Output = new List<T>(input);
    }

    /// <summary>
    /// دریافت ورودی اصلی
    /// </summary>
    public List<T> GetInput()
    {
        return Input;
    }

    /// <summary>
    /// دریافت خروجی
    /// </summary>
    public List<T> GetOutput()
    {
        return Output;
    }

    /// <summary>
    /// فیلتر کردن لیست بر اساس شرایط
    /// </summary>
    public List<T> Filter(Dictionary<string, object> args, string @operator = "AND")
    {
        if (args == null || args.Count == 0)
        {
            return Output;
        }

        @operator = @operator.ToUpper();
        if (!new[] { "AND", "OR", "NOT" }.Contains(@operator))
        {
            Output = new List<T>();
            return Output;
        }

        var filtered = Output.Where(item =>
        {
            int matched = 0;
            foreach (var arg in args)
            {
                var propertyValue = GetPropertyValue(item, arg.Key);
                if (propertyValue != null && propertyValue.Equals(arg.Value))
                {
                    matched++;
                }
            }

            return (@operator == "AND" && matched == args.Count) ||
                   (@operator == "OR" && matched > 0) ||
                   (@operator == "NOT" && matched == 0);
        }).ToList();

        Output = filtered;
        return Output;
    }

    /// <summary>
    /// استخراج فیلد خاص از لیست
    /// </summary>
    public List<object> Pluck(string field, string indexKey = null)
    {
        var newList = new List<object>();

        if (string.IsNullOrEmpty(indexKey))
        {
            foreach (var item in Output)
            {
                var value = GetPropertyValue(item, field);
                if (value != null)
                {
                    newList.Add(value);
                }
            }
        }
        else
        {
            var dictionary = new Dictionary<object, object>();
            foreach (var item in Output)
            {
                var fieldValue = GetPropertyValue(item, field);
                var key = GetPropertyValue(item, indexKey);

                if (fieldValue != null)
                {
                    if (key != null)
                    {
                        dictionary[key] = fieldValue;
                    }
                    else
                    {
                        newList.Add(fieldValue);
                    }
                }
            }

            newList.AddRange(dictionary.Values);
        }

        Output = newList.Cast<T>().ToList();
        return newList;
    }

    /// <summary>
    /// مرتب‌سازی لیست بر اساس فیلدهای مشخص
    /// </summary>
    public List<T> Sort(Dictionary<string, string> orderBy = null, string order = "ASC", bool preserveKeys = false)
    {
        if (orderBy == null || orderBy.Count == 0)
        {
            return Output;
        }

        foreach (var kvp in orderBy)
        {
            orderBy[kvp.Key] = kvp.Value.ToUpper() == "DESC" ? "DESC" : "ASC";
        }

        OrderBy = orderBy;

        if (preserveKeys)
        {
            Output = Output.OrderBy(x => x, new CustomComparer(this)).ToList();
        }
        else
        {
            Output = Output.OrderBy(x => x, new CustomComparer(this)).ToList();
        }

        OrderBy.Clear();
        return Output;
    }

    /// <summary>
    /// دریافت مقدار خاصیت از شیء یا آرایه
    /// </summary>
    private object GetPropertyValue(object obj, string propertyName)
    {
        if (obj is IDictionary<string, object> dict && dict.ContainsKey(propertyName))
        {
            return dict[propertyName];
        }
        else if (obj is T typedObj)
        {
            var property = typeof(T).GetProperty(propertyName);
            return property?.GetValue(typedObj);
        }

        return null;
    }

    /// <summary>
    /// کلاس مقایسه‌کننده سفارشی برای مرتب‌سازی
    /// </summary>
    private class CustomComparer : IComparer<T>
    {
        private readonly ListUtil<T> _listUtil;

        public CustomComparer(ListUtil<T> listUtil)
        {
            _listUtil = listUtil;
        }

        public int Compare(T x, T y)
        {
            foreach (var kvp in _listUtil.OrderBy)
            {
                var field = kvp.Key;
                var direction = kvp.Value;

                var xValue = _listUtil.GetPropertyValue(x, field);
                var yValue = _listUtil.GetPropertyValue(y, field);

                if (xValue == null || yValue == null)
                {
                    continue;
                }

                if (xValue.Equals(yValue))
                {
                    continue;
                }

                var result = direction == "DESC" ? -1 : 1;

                if (xValue is IComparable comparableX && yValue is IComparable comparableY)
                {
                    return comparableX.CompareTo(comparableY) * result;
                }

                return string.Compare(xValue.ToString(), yValue.ToString(), StringComparison.Ordinal) * result;
            }

            return 0;
        }
    }
}