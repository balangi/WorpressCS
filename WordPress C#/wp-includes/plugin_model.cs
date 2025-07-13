using System;
using System.Collections.Generic;

public class Hook
{
    public string Name { get; set; }
    public List<HookCallback> Callbacks { get; set; } = new List<HookCallback>();
}

public class HookCallback
{
    public string Id { get; set; }
    public Delegate Callback { get; set; }
    public int Priority { get; set; }
}

public class Filter : Hook
{
    // Inherits from Hook, as Filters are a type of Hook.
}