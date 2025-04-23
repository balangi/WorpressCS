using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using WordPress.Core.Data;
using WordPress.Core.Models;

namespace WordPress.Core.BlockTemplates
{
    public class BlockTemplatesRegistry
    {
        /// <summary>
        /// Registered templates dictionary.
        /// </summary>
        private readonly Dictionary<string, BlockTemplate> _registeredTemplates = new Dictionary<string, BlockTemplate>();

        /// <summary>
        /// Container for the main instance of the class.
        /// </summary>
        private static BlockTemplatesRegistry _instance;

        /// <summary>
        /// Private constructor to enforce singleton pattern.
        /// </summary>
        private BlockTemplatesRegistry() { }

        /// <summary>
        /// Registers a template.
        /// </summary>
        public BlockTemplate Register(string templateName, Dictionary<string, object> args, AppDbContext dbContext)
        {
            if (string.IsNullOrEmpty(templateName))
            {
                throw new ArgumentException("Template names must be strings.");
            }

            if (templateName.Any(char.IsUpper))
            {
                throw new ArgumentException("Template names must not contain uppercase characters.");
            }

            if (!templateName.Contains("//"))
            {
                throw new ArgumentException("Template names must contain a namespace prefix. Example: my-plugin//my-custom-template");
            }

            if (_registeredTemplates.ContainsKey(templateName))
            {
                throw new InvalidOperationException($"Template \"{templateName}\" is already registered.");
            }

            var themeName = "default-theme"; // Replace with actual theme logic
            var parts = templateName.Split(new[] { "//" }, StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length != 2)
            {
                throw new ArgumentException("Invalid template name format.");
            }

            var plugin = parts[0];
            var slug = parts[1];

            var template = new BlockTemplate
            {
                Name = templateName,
                Theme = themeName,
                Plugin = plugin,
                Content = args.ContainsKey("content") ? args["content"]?.ToString() : string.Empty,
                Source = "plugin",
                Slug = slug,
                Type = "wp_template",
                Title = args.ContainsKey("title") ? args["title"]?.ToString() : templateName,
                Description = args.ContainsKey("description") ? args["description"]?.ToString() : string.Empty,
                Status = "publish",
                Origin = "plugin",
                IsCustom = true,
                PostTypes = args.ContainsKey("post_types") && args["post_types"] is List<string> postTypes ? postTypes : new List<string>()
            };

            _registeredTemplates[templateName] = template;

            // Save to database
            dbContext.BlockTemplates.Add(template);
            dbContext.SaveChanges();

            return template;
        }

        /// <summary>
        /// Retrieves all registered templates.
        /// </summary>
        public IEnumerable<BlockTemplate> GetAllRegistered()
        {
            return _registeredTemplates.Values;
        }

        /// <summary>
        /// Retrieves a registered template by its name.
        /// </summary>
        public BlockTemplate GetRegistered(string templateName)
        {
            return _registeredTemplates.ContainsKey(templateName) ? _registeredTemplates[templateName] : null;
        }

        /// <summary>
        /// Retrieves a registered template by its slug.
        /// </summary>
        public BlockTemplate GetBySlug(string templateSlug)
        {
            return _registeredTemplates.Values.FirstOrDefault(t => t.Slug == templateSlug);
        }

        /// <summary>
        /// Retrieves registered templates matching a query.
        /// </summary>
        public IEnumerable<BlockTemplate> GetByQuery(Dictionary<string, object> query)
        {
            var slugsToInclude = query.ContainsKey("slug__in") && query["slug__in"] is List<string> slugsIn ? slugsIn : new List<string>();
            var slugsToSkip = query.ContainsKey("slug__not_in") && query["slug__not_in"] is List<string> slugsNotIn ? slugsNotIn : new List<string>();
            var postType = query.ContainsKey("post_type") ? query["post_type"]?.ToString() : null;

            return _registeredTemplates.Values.Where(t =>
                (slugsToInclude.Count == 0 || slugsToInclude.Contains(t.Slug)) &&
                (slugsToSkip.Count == 0 || !slugsToSkip.Contains(t.Slug)) &&
                (string.IsNullOrEmpty(postType) || t.PostTypes.Contains(postType))
            );
        }

        /// <summary>
        /// Checks if a template is registered.
        /// </summary>
        public bool IsRegistered(string templateName)
        {
            return _registeredTemplates.ContainsKey(templateName);
        }

        /// <summary>
        /// Unregisters a template.
        /// </summary>
        public BlockTemplate Unregister(string templateName, AppDbContext dbContext)
        {
            if (!_registeredTemplates.ContainsKey(templateName))
            {
                throw new InvalidOperationException($"Template \"{templateName}\" is not registered.");
            }

            var unregisteredTemplate = _registeredTemplates[templateName];
            _registeredTemplates.Remove(templateName);

            // Remove from database
            var dbTemplate = dbContext.BlockTemplates.FirstOrDefault(t => t.Name == templateName);
            if (dbTemplate != null)
            {
                dbContext.BlockTemplates.Remove(dbTemplate);
                dbContext.SaveChanges();
            }

            return unregisteredTemplate;
        }

        /// <summary>
        /// Utility method to retrieve the main instance of the class.
        /// </summary>
        public static BlockTemplatesRegistry GetInstance()
        {
            return _instance ??= new BlockTemplatesRegistry();
        }
    }
}