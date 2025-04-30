using System;
using System.Collections.Generic;
using System.Linq;

public class ErrorManager
{
    private readonly Dictionary<string, List<string>> _errors = new();
    private readonly Dictionary<string, List<object>> _errorData = new();
    private readonly Dictionary<string, List<List<object>>> _additionalData = new();

    // افزودن خطای جدید
    public void Add(string code, string message, object data = null)
    {
        if (string.IsNullOrEmpty(code))
        {
            return;
        }

        if (!_errors.ContainsKey(code))
        {
            _errors[code] = new List<string>();
        }

        _errors[code].Add(message);

        if (data != null)
        {
            AddData(data, code);
        }

        // رویداد اضافه کردن خطا
        OnErrorAdded?.Invoke(code, message, data);
    }

    // دریافت تمام کدهای خطا
    public List<string> GetErrorCodes()
    {
        return _errors.Keys.ToList();
    }

    // دریافت اولین کد خطا
    public string GetErrorCode()
    {
        var codes = GetErrorCodes();
        return codes.Any() ? codes[0] : string.Empty;
    }

    // دریافت تمام پیام‌های خطا برای یک کد خاص
    public List<string> GetErrorMessages(string code = "")
    {
        if (string.IsNullOrEmpty(code))
        {
            return _errors.Values.SelectMany(messages => messages).ToList();
        }

        return _errors.ContainsKey(code) ? _errors[code] : new List<string>();
    }

    // دریافت اولین پیام خطا
    public string GetErrorMessage(string code = "")
    {
        if (string.IsNullOrEmpty(code))
        {
            code = GetErrorCode();
        }

        var messages = GetErrorMessages(code);
        return messages.Any() ? messages[0] : string.Empty;
    }

    // دریافت آخرین داده مرتبط با خطا
    public object GetErrorData(string code = "")
    {
        if (string.IsNullOrEmpty(code))
        {
            code = GetErrorCode();
        }

        return _errorData.ContainsKey(code) ? _errorData[code].LastOrDefault() : null;
    }

    // اضافه کردن داده به خطا
    public void AddData(object data, string code)
    {
        if (string.IsNullOrEmpty(code))
        {
            code = GetErrorCode();
        }

        if (!_errorData.ContainsKey(code))
        {
            _errorData[code] = new List<object>();
        }

        if (!_additionalData.ContainsKey(code))
        {
            _additionalData[code] = new List<List<object>>();
        }

        _additionalData[code].Add(_errorData[code]);
        _errorData[code].Add(data);
    }

    // دریافت تمام داده‌های مرتبط با خطا
    public List<object> GetAllErrorData(string code = "")
    {
        if (string.IsNullOrEmpty(code))
        {
            code = GetErrorCode();
        }

        var data = new List<object>();

        if (_additionalData.ContainsKey(code))
        {
            data.AddRange(_additionalData[code].SelectMany(d => d));
        }

        if (_errorData.ContainsKey(code))
        {
            data.Add(_errorData[code].LastOrDefault());
        }

        return data;
    }

    // حذف خطای خاص
    public void Remove(string code)
    {
        _errors.Remove(code);
        _errorData.Remove(code);
        _additionalData.Remove(code);
    }

    // ادغام خطاهای دو شیء
    public void MergeFrom(ErrorManager other)
    {
        foreach (var code in other.GetErrorCodes())
        {
            foreach (var message in other.GetErrorMessages(code))
            {
                Add(code, message);
            }

            foreach (var data in other.GetAllErrorData(code))
            {
                AddData(data, code);
            }
        }
    }

    // انتقال خطاهای این شیء به شیء دیگر
    public void ExportTo(ErrorManager other)
    {
        other.MergeFrom(this);
    }

    // رویداد اضافه کردن خطا
    public event Action<string, string, object> OnErrorAdded;
}