using System;
using System.Collections.Generic;

public class Option
{
    public int Id { get; set; }
    public string OptionName { get; set; }
    public string OptionValue { get; set; }
    public bool Autoload { get; set; }
}

public class NetworkOption
{
    public int Id { get; set; }
    public int NetworkId { get; set; }
    public string MetaKey { get; set; }
    public string MetaValue { get; set; }
}