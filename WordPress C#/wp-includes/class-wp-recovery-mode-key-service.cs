using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

public class RecoveryModeKeyService
{
    /// <summary>
    /// نام Option برای ذخیره کلیدها
    /// </summary>
    private const string OptionName = "recovery_keys";

    /// <summary>
    /// Database Context
    /// </summary>
    private readonly AppDbContext _context;

    /// <summary>
    /// لاگ‌گیری
    /// </summary>
    private readonly ILogger<RecoveryModeKeyService> _logger;

    /// <summary>
    /// Cache
    /// </summary>
    private readonly IMemoryCache _cache;

    /// <summary>
    /// سازنده کلاس
    /// </summary>
    public RecoveryModeKeyService(AppDbContext context, ILogger<RecoveryModeKeyService> logger, IMemoryCache cache)
    {
        _context = context;
        _logger = logger;
        _cache = cache;
    }

    /// <summary>
    /// تولید توکن حالت بازیابی
    /// </summary>
    public string GenerateRecoveryModeToken()
    {
        return GenerateRandomString(22);
    }

    /// <summary>
    /// تولید و ذخیره کلید حالت بازیابی
    /// </summary>
    public string GenerateAndStoreRecoveryModeKey(string token)
    {
        var key = GenerateRandomString(22);

        var records = GetKeys();
        records[token] = new KeyRecord
        {
            HashedKey = ComputeFastHash(key),
            CreatedAt = DateTime.UtcNow
        };

        UpdateKeys(records);

        // اجرای رویداد
        OnGenerateRecoveryModeKey(token, key);

        return key;
    }

    /// <summary>
    /// اعتبارسنجی کلید حالت بازیابی
    /// </summary>
    public Result ValidateRecoveryModeKey(string token, string key, int ttl)
    {
        var records = GetKeys();

        if (!records.ContainsKey(token))
        {
            return Result.Failure("Recovery Mode not initialized.");
        }

        var record = records[token];
        records.Remove(token);
        UpdateKeys(records);

        if (record == null || string.IsNullOrEmpty(record.HashedKey) || record.CreatedAt == default)
        {
            return Result.Failure("Invalid recovery key format.");
        }

        if (!VerifyFastHash(key, record.HashedKey))
        {
            return Result.Failure("Invalid recovery key.");
        }

        if (DateTime.UtcNow > record.CreatedAt.AddSeconds(ttl))
        {
            return Result.Failure("Recovery key expired.");
        }

        return Result.Success();
    }

    /// <summary>
    /// پاک کردن کلیدهای منقضی
    /// </summary>
    public void CleanExpiredKeys(int ttl)
    {
        var records = GetKeys();
        var expiredKeys = records.Where(kvp => DateTime.UtcNow > kvp.Value.CreatedAt.AddSeconds(ttl)).ToList();

        foreach (var key in expiredKeys)
        {
            records.Remove(key.Key);
        }

        UpdateKeys(records);
    }

    /// <summary>
    /// حذف کلید استفاده‌شده
    /// </summary>
    private void RemoveKey(string token)
    {
        var records = GetKeys();
        records.Remove(token);
        UpdateKeys(records);
    }

    /// <summary>
    /// دریافت کلیدها
    /// </summary>
    private Dictionary<string, KeyRecord> GetKeys()
    {
        return _context.Options.FirstOrDefault(o => o.Key == OptionName)?.Value
            .Deserialize<Dictionary<string, KeyRecord>>() ?? new Dictionary<string, KeyRecord>();
    }

    /// <summary>
    /// به‌روزرسانی کلیدها
    /// </summary>
    private bool UpdateKeys(Dictionary<string, KeyRecord> keys)
    {
        var option = new Option
        {
            Key = OptionName,
            Value = keys.Serialize()
        };

        _context.Options.Update(option);
        return _context.SaveChanges() > 0;
    }

    /// <summary>
    /// رویداد تولید کلید
    /// </summary>
    private void OnGenerateRecoveryModeKey(string token, string key)
    {
        _logger.LogInformation($"Recovery mode key generated. Token: {token}");
    }

    /// <summary>
    /// محاسبه هش سریع
    /// </summary>
    private string ComputeFastHash(string input)
    {
        using var sha256 = SHA256.Create();
        var hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));
        return Convert.ToBase64String(hashBytes);
    }

    /// <summary>
    /// اعتبارسنجی هش سریع
    /// </summary>
    private bool VerifyFastHash(string input, string hashedInput)
    {
        var inputHash = ComputeFastHash(input);
        return string.Equals(inputHash, hashedInput, StringComparison.Ordinal);
    }

    /// <summary>
    /// تولید رشته تصادفی
    /// </summary>
    private string GenerateRandomString(int length)
    {
        var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        var random = new Random();
        return new string(Enumerable.Repeat(chars, length)
            .Select(s => s[random.Next(s.Length)]).ToArray());
    }
}

/// <summary>
/// مدل داده‌ای کلید
/// </summary>
public class KeyRecord
{
    public string HashedKey { get; set; }
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// نتیجه عملیات
/// </summary>
public class Result
{
    public bool IsSuccess { get; }
    public string Error { get; }

    public static Result Success() => new Result(true, null);
    public static Result Failure(string error) => new Result(false, error);

    private Result(bool isSuccess, string error)
    {
        IsSuccess = isSuccess;
        Error = error;
    }
}