using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Dapper;

public class DatabaseManager
{
    private readonly string _connectionString;
    public string LastError { get; private set; }

    public DatabaseManager(string connectionString)
    {
        _connectionString = connectionString;
    }

    // اجرای پرس‌وجوی دستی
    public int Query(string sql, object parameters = null)
    {
        try
        {
            using var connection = new SqlConnection(_connectionString);
            connection.Open();
            return connection.Execute(sql, parameters);
        }
        catch (Exception ex)
        {
            LastError = ex.Message;
            throw;
        }
    }

    // دریافت تمام ردیف‌ها
    public IEnumerable<T> GetResults<T>(string sql, object parameters = null)
    {
        try
        {
            using var connection = new SqlConnection(_connectionString);
            connection.Open();
            return connection.Query<T>(sql, parameters);
        }
        catch (Exception ex)
        {
            LastError = ex.Message;
            throw;
        }
    }

    // دریافت یک ردیف
    public T GetRow<T>(string sql, object parameters = null)
    {
        try
        {
            using var connection = new SqlConnection(_connectionString);
            connection.Open();
            return connection.QueryFirstOrDefault<T>(sql, parameters);
        }
        catch (Exception ex)
        {
            LastError = ex.Message;
            throw;
        }
    }

    // دریافت یک مقدار
    public T GetVar<T>(string sql, object parameters = null)
    {
        try
        {
            using var connection = new SqlConnection(_connectionString);
            connection.Open();
            return connection.ExecuteScalar<T>(sql, parameters);
        }
        catch (Exception ex)
        {
            LastError = ex.Message;
            throw;
        }
    }

    // دریافت آخرین خطا
    public string GetLastError()
    {
        return LastError;
    }
}