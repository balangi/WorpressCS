using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace WordPress.SpeculativeLoading
{
    /// <summary>
    /// Class for prefixing URL patterns.
    /// This class is intended primarily for use as part of the speculative loading feature.
    /// </summary>
    public class WPUrlPatternPrefixer
    {
        /// <summary>
        /// Map of context string to base path pairs.
        /// </summary>
        private Dictionary<string, string> Contexts { get; set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="contexts">Optional. Map of context string to base path pairs. Default is the contexts returned by GetDefaultContexts method.</param>
        public WPUrlPatternPrefixer(Dictionary<string, string> contexts = null)
        {
            if (contexts != null && contexts.Any())
            {
                Contexts = contexts.ToDictionary(
                    pair => pair.Key,
                    pair => EscapePatternString(AddTrailingSlash(pair.Value))
                );
            }
            else
            {
                Contexts = GetDefaultContexts();
            }
        }

        /// <summary>
        /// Prefixes the given URL path pattern with the base path for the given context.
        /// </summary>
        /// <param name="pathPattern">URL pattern starting with the path segment.</param>
        /// <param name="context">Optional. Context to use for prefixing the path pattern. Default is 'home'.</param>
        /// <returns>URL pattern, prefixed as necessary.</returns>
        public string PrefixPathPattern(string pathPattern, string context = "home")
        {
            // If context path does not exist, the context is invalid.
            if (!Contexts.ContainsKey(context))
            {
                Console.WriteLine($"Invalid URL pattern context {context}.");
                return pathPattern;
            }

            var contextPath = Contexts[context];
            var escapedContextPath = contextPath;

            // Escape context path if it contains special characters.
            if (Regex.IsMatch(contextPath, @"[:?#]"))
            {
                escapedContextPath = "{" + contextPath.Substring(0, contextPath.Length - 1) + "}/";
            }

            // If the path already starts with the context path, remove it first.
            if (pathPattern.StartsWith(contextPath))
            {
                pathPattern = pathPattern.Substring(contextPath.Length);
            }

            return escapedContextPath + pathPattern.TrimStart('/');
        }

        /// <summary>
        /// Returns the default contexts used by the class.
        /// </summary>
        /// <returns>Map of context string to base path pairs.</returns>
        public static Dictionary<string, string> GetDefaultContexts()
        {
            return new Dictionary<string, string>
            {
                { "home", EscapePatternString(AddTrailingSlash(new Uri("http://example.com/").AbsolutePath)) },
                { "site", EscapePatternString(AddTrailingSlash(new Uri("http://example.com/site/").AbsolutePath)) },
                { "uploads", EscapePatternString(AddTrailingSlash("/wp-content/uploads/")) },
                { "content", EscapePatternString(AddTrailingSlash("/wp-content/")) },
                { "plugins", EscapePatternString(AddTrailingSlash("/wp-content/plugins/")) },
                { "template", EscapePatternString(AddTrailingSlash("/wp-content/themes/template/")) },
                { "stylesheet", EscapePatternString(AddTrailingSlash("/wp-content/themes/stylesheet/")) }
            };
        }

        /// <summary>
        /// Escapes a string for use in a URL pattern component.
        /// </summary>
        /// <param name="str">String to be escaped.</param>
        /// <returns>String with backslashes added where required.</returns>
        private static string EscapePatternString(string str)
        {
            var escapeChars = "+*?:{}()\\";
            foreach (var c in escapeChars)
            {
                str = str.Replace(c.ToString(), "\\" + c);
            }
            return str;
        }

        /// <summary>
        /// Adds a trailing slash to the given string if it doesn't already have one.
        /// </summary>
        /// <param name="str">The string to add a trailing slash to.</param>
        /// <returns>The string with a trailing slash.</returns>
        private static string AddTrailingSlash(string str)
        {
            return str.EndsWith("/") ? str : str + "/";
        }
    }
}