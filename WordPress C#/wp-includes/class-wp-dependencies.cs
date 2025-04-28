using System;
using System.Collections.Generic;
using System.Linq;

public class DependencyManager
{
    private readonly List<Dependency> _dependencies = new();

    // افزودن وابستگی جدید
    public void AddDependency(string handle, string src, List<string> dependencies, string version)
    {
        var dependency = new Dependency
        {
            Handle = handle,
            Src = src,
            Dependencies = dependencies,
            Version = version,
            Enqueued = false
        };

        _dependencies.Add(dependency);
    }

    // دریافت وابستگی بر اساس شناسه
    public Dependency GetDependency(string handle)
    {
        return _dependencies.FirstOrDefault(d => d.Handle == handle);
    }

    // اضافه کردن وابستگی به صف
    public void EnqueueDependency(string handle)
    {
        var dependency = GetDependency(handle);
        if (dependency != null && !dependency.Enqueued)
        {
            // ابتدا وابستگی‌های این وابستگی را به صف اضافه کن
            foreach (var depHandle in dependency.Dependencies)
            {
                EnqueueDependency(depHandle);
            }

            // سپس خود وابستگی را به صف اضافه کن
            dependency.Enqueued = true;
        }
    }

    // دریافت تمام وابستگی‌های موجود در صف
    public List<Dependency> GetEnqueuedDependencies()
    {
        return _dependencies.Where(d => d.Enqueued).ToList();
    }

    // حذف وابستگی
    public void RemoveDependency(string handle)
    {
        var dependency = GetDependency(handle);
        if (dependency != null)
        {
            _dependencies.Remove(dependency);
        }
    }
}