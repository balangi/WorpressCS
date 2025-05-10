using System;
using System.Collections.Generic;
using System.Linq;

public class ScriptModuleService
{
    private readonly AppDbContext _context;

    public ScriptModuleService(AppDbContext context)
    {
        _context = context;
    }

    // ثبت یک ماژول جدید
    public void Register(string moduleId, string src, List<ScriptModuleDependency> dependencies, string version = null)
    {
        if (!_context.ScriptModules.Any(m => m.ModuleId == moduleId))
        {
            var scriptModule = new ScriptModule
            {
                ModuleId = moduleId,
                Src = src,
                Version = version ?? "default",
                Enqueued = false,
                Dependencies = dependencies
            };
            _context.ScriptModules.Add(scriptModule);
            _context.SaveChanges();
        }
    }

    // علامت‌گذاری یک ماژول برای اجرا (enqueue)
    public void Enqueue(string moduleId)
    {
        var scriptModule = _context.ScriptModules.FirstOrDefault(m => m.ModuleId == moduleId);
        if (scriptModule != null)
        {
            scriptModule.Enqueued = true;
            _context.SaveChanges();
        }
    }

    // حذف علامت‌گذاری برای اجرا (dequeue)
    public void Dequeue(string moduleId)
    {
        var scriptModule = _context.ScriptModules.FirstOrDefault(m => m.ModuleId == moduleId);
        if (scriptModule != null)
        {
            scriptModule.Enqueued = false;
            _context.SaveChanges();
        }
    }

    // حذف یک ماژول
    public void Deregister(string moduleId)
    {
        var scriptModule = _context.ScriptModules.FirstOrDefault(m => m.ModuleId == moduleId);
        if (scriptModule != null)
        {
            _context.ScriptModules.Remove(scriptModule);
            _context.SaveChanges();
        }
    }

    // دریافت لیست ماژول‌های علامت‌گذاری‌شده برای اجرا
    public List<ScriptModule> GetEnqueuedModules()
    {
        return _context.ScriptModules
            .Include(m => m.Dependencies)
            .Where(m => m.Enqueued)
            .ToList();
    }

    // دریافت تمام وابستگی‌های یک ماژول
    public List<ScriptModuleDependency> GetDependencies(string moduleId)
    {
        var scriptModule = _context.ScriptModules
            .Include(m => m.Dependencies)
            .FirstOrDefault(m => m.ModuleId == moduleId);

        return scriptModule?.Dependencies.ToList() ?? new List<ScriptModuleDependency>();
    }

    // چاپ لیست ماژول‌های علامت‌گذاری‌شده برای اجرا
    public void PrintEnqueuedModules()
    {
        var enqueuedModules = GetEnqueuedModules();
        foreach (var module in enqueuedModules)
        {
            Console.WriteLine($"Module: {module.ModuleId}, Src: {module.Src}, Version: {module.Version}");
        }
    }
}