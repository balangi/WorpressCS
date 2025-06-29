using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

public class FormattingService
{
    private readonly ApplicationDbContext _context;

    public FormattingService(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Replaces common plain text characters with formatted entities.
    /// </summary>
    public string FormatText(string text)
    {
        if (string.IsNullOrEmpty(text))
            return text;

        // Replace quotes and other special characters.
        text = text.Replace("'", "&#8217;")
                   .Replace("\"", "&#8220;")
                   .Replace("...", "&#8230;")
                   .Replace("--", "&#8212;")
                   .Replace("(tm)", "&#8482;")
                   .Replace("(c)", "&#169;")
                   .Replace("(r)", "&#174;");

        return text;
    }

    /// <summary>
    /// Validates an email address.
    /// </summary>
    public EmailValidationResult ValidateEmail(string email)
    {
        var result = new EmailValidationResult { Email = email };

        if (string.IsNullOrEmpty(email))
        {
            result.IsValid = false;
            result.Reason = "Email is empty.";
            return result;
        }

        if (!email.Contains('@'))
        {
            result.IsValid = false;
            result.Reason = "Email does not contain '@'.";
            return result;
        }

        var parts = email.Split('@');
        if (parts.Length != 2 || string.IsNullOrEmpty(parts[0]) || string.IsNullOrEmpty(parts[1]))
        {
            result.IsValid = false;
            result.Reason = "Invalid email format.";
            return result;
        }

        var localPart = parts[0];
        var domainPart = parts[1];

        // Validate local part.
        if (!Regex.IsMatch(localPart, @"^[a-zA-Z0-9!#$%&'*+/=?^_`{|}~.-]+$"))
        {
            result.IsValid = false;
            result.Reason = "Invalid characters in local part.";
            return result;
        }

        // Validate domain part.
        if (domainPart.Contains("..") || domainPart.StartsWith(".") || domainPart.EndsWith("."))
        {
            result.IsValid = false;
            result.Reason = "Invalid domain format.";
            return result;
        }

        var domainParts = domainPart.Split('.');
        if (domainParts.Length < 2)
        {
            result.IsValid = false;
            result.Reason = "Domain must have at least two parts.";
            return result;
        }

        foreach (var part in domainParts)
        {
            if (!Regex.IsMatch(part, @"^[a-z0-9-]+$"))
            {
                result.IsValid = false;
                result.Reason = "Invalid characters in domain part.";
                return result;
            }
        }

        result.IsValid = true;
        result.Reason = "Email is valid.";
        return result;
    }

    /// <summary>
    /// Adds leading zeros to a number based on the threshold.
    /// </summary>
    public string AddLeadingZeros(int number, int threshold)
    {
        return number.ToString().PadLeft(threshold, '0');
    }

    /// <summary>
    /// Calculates the human-readable time difference between two timestamps.
    /// </summary>
    public string GetTimeDifference(DateTime from, DateTime? to = null)
    {
        to ??= DateTime.UtcNow;
        var diff = to.Value - from;

        if (diff.TotalSeconds < 60)
            return $"{Math.Floor(diff.TotalSeconds)} seconds";

        if (diff.TotalMinutes < 60)
            return $"{Math.Floor(diff.TotalMinutes)} minutes";

        if (diff.TotalHours < 24)
            return $"{Math.Floor(diff.TotalHours)} hours";

        return $"{Math.Floor(diff.TotalDays)} days";
    }
}