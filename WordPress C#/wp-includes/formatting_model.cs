using System;
using System.Collections.Generic;

public class FormattingOptions
{
    public bool RichText { get; set; } = false;
    public int Threshold { get; set; } = 4;
}

public class EmailValidationResult
{
    public string Email { get; set; }
    public bool IsValid { get; set; }
    public string Reason { get; set; }
}