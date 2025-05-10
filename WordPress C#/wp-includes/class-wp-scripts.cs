using System;
using System.Collections.Generic;
using System.Linq;

public class ScriptService
{
    private readonly AppDbContext _context;

    public ScriptService(AppDbContext context)
    {
        _context = context;
    }

    // ثبت یک اسکریپت جدید
    public void Register(string handle, string src, List<ScriptDependency> dependencies, string version = null)
    {
        if (!_context.Scripts.Any(s => s.Handle == handle))
        {
            var script = new Script
            {
                Handle = handle,
                Src = src,
                Version = version ?? "default",
                Enqueued = false,
                Dependencies = dependencies
            };
            _context.Scripts.Add(script);
            _context.SaveChanges();
        }
    }

    // علامت‌گذاری اسکریپت برای اجرا (enqueue)
    public void Enqueue(string handle)
    {
        var script = _context.Scripts.FirstOrDefault(s => s.Handle == handle);
        if (script != null)
        {
            script.Enqueued = true;
            _context.SaveChanges();
        }
    }

    // حذف علامت‌گذاری برای اجرا (dequeue)
    public void Dequeue(string handle)
    {
        var script = _context.Scripts.FirstOrDefault(s => s.Handle == handle);
        if (script != null)
        {
            script.Enqueued = false;
            _context.SaveChanges();
        }
    }

    // حذف یک اسکریپت
    public void Deregister(string handle)
    {
        var script = _context.Scripts.FirstOrDefault(s => s.Handle == handle);
        if (script != null)
        {
            _context.Scripts.Remove(script);
            _context.SaveChanges();
        }
    }

    // دریافت لیست اسکریپت‌های علامت‌گذاری‌شده برای اجرا
    public List<Script> GetEnqueuedScripts()
    {
        return _context.Scripts
            .Include(s => s.Dependencies)
            .Where(s => s.Enqueued)
            .ToList();
    }

    // دریافت تمام وابستگی‌های یک اسکریپت
    public List<ScriptDependency> GetDependencies(string handle)
    {
        var script = _context.Scripts
            .Include(s => s.Dependencies)
            .FirstOrDefault(s => s.Handle == handle);

        return script?.Dependencies.ToList() ?? new List<ScriptDependency>();
    }

    // چاپ لیست اسکریپت‌های علامت‌گذاری‌شده
    public void PrintEnqueuedScripts()
    {
        var enqueuedScripts = GetEnqueuedScripts();
        foreach (var script in enqueuedScripts)
        {
            Console.WriteLine($"Script: {script.Handle}, Src: {script.Src}, Version: {script.Version}");
        }
    }
}