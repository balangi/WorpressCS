using System;
using System.Linq;
using Microsoft.AspNetCore.Http;

public class LoadService
{
    private readonly ApplicationDbContext _context;

    public LoadService(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Fixes server variables to ensure compatibility with IIS and other environments.
    /// </summary>
    public void FixServerVariables(HttpRequest request)
    {
        var serverSettings = GetServerSettings();

        // Set default values for missing server variables
        serverSettings.RequestUri ??= request.Path.ToString();
        serverSettings.ScriptFilename ??= request.PathBase.ToString();

        // Handle IIS-specific cases
        if (string.IsNullOrEmpty(serverSettings.RequestUri))
        {
            if (!string.IsNullOrEmpty(request.Headers["HTTP_X_ORIGINAL_URL"]))
            {
                serverSettings.RequestUri = request.Headers["HTTP_X_ORIGINAL_URL"];
            }
            else if (!string.IsNullOrEmpty(request.Headers["HTTP_X_REWRITE_URL"]))
            {
                serverSettings.RequestUri = request.Headers["HTTP_X_REWRITE_URL"];
            }
            else
            {
                serverSettings.RequestUri = request.Path.ToString();
            }
        }

        // Append query string if it exists
        if (!string.IsNullOrEmpty(request.QueryString.Value))
        {
            serverSettings.RequestUri += "?" + request.QueryString.Value;
        }

        UpdateServerSettings(serverSettings);
    }

    /// <summary>
    /// Validates the PHP version and required extensions.
    /// </summary>
    public void ValidatePhpVersion(string currentPhpVersion, string requiredPhpVersion, List<string> requiredExtensions)
    {
        if (string.Compare(requiredPhpVersion, currentPhpVersion, StringComparison.Ordinal) > 0)
        {
            throw new InvalidOperationException(
                $"Your server is running PHP version {currentPhpVersion} but WordPress requires at least {requiredPhpVersion}."
            );
        }

        var missingExtensions = requiredExtensions.Where(ext => !IsExtensionLoaded(ext)).ToList();
        if (missingExtensions.Any())
        {
            throw new InvalidOperationException(
                $"The following PHP extensions are missing: {string.Join(", ", missingExtensions)}"
            );
        }
    }

    /// <summary>
    /// Checks if a PHP extension is loaded.
    /// </summary>
    private bool IsExtensionLoaded(string extensionName)
    {
        // Placeholder for checking if an extension is loaded
        return true;
    }

    /// <summary>
    /// Retrieves the server settings.
    /// </summary>
    private ServerSettings GetServerSettings()
    {
        return _context.ServerSettings.FirstOrDefault() ?? new ServerSettings();
    }

    /// <summary>
    /// Updates the server settings in the database.
    /// </summary>
    private void UpdateServerSettings(ServerSettings settings)
    {
        _context.Update(settings);
        _context.SaveChanges();
    }
}