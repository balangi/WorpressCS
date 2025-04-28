using System;
using System.Collections.Generic;
using System.Linq;

public class DependencyManager
{
    private readonly List<Dependency> _dependencies = new();

    // افزودن وابستگی جدید
    public void AddDependency(Dependency dependency)
    {
        _dependencies.Add(dependency);
    }

    // دریافت وابستگی بر اساس شناسه
    public Dependency GetDependency(string handle)
    {
        return _dependencies.FirstOrDefault(d => d.Handle == handle);
    }

    // حذف وابستگی
    public void RemoveDependency(string handle)
    {
        var dependency = _dependencies.FirstOrDefault(d => d.Handle == handle);
        if (dependency != null)
        {
            _dependencies.Remove(dependency);
        }
    }

    // دریافت تمام وابستگی‌ها
    public List<Dependency> GetAllDependencies()
    {
        return _dependencies;
    }
}