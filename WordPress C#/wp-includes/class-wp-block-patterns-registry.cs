using System;
using System.Collections.Generic;
using System.Linq;
using WordPress.Core.Data;
using WordPress.Core.Models;

namespace WordPress.Core.BlockPatterns
{
    public class BlockPatternsRegistry
    {
        /// <summary>
        /// Registered block patterns dictionary.
        /// </summary>
        private readonly Dictionary<string, BlockPattern> _registeredPatterns = new Dictionary<string, BlockPattern>();

        /// <summary>
        /// Patterns registered outside the `init` action.
        /// </summary>
        private readonly Dictionary<string, BlockPattern> _registeredPatternsOutsideInit = new Dictionary<string, BlockPattern>();

        /// <summary>
        /// Container for the main instance of the class.
        /// </summary>
        private static BlockPatternsRegistry _instance;

        /// <summary>
        /// Private constructor to enforce singleton pattern.
        /// </summary>
        private BlockPatternsRegistry() { }

        /// <summary>
        /// Registers a block pattern.
        /// </summary>
        public bool Register(string patternName, Dictionary<string, object> patternProperties, AppDbContext dbContext)
        {
            if (string.IsNullOrEmpty(patternName))
            {
                throw new ArgumentException("Pattern name must be a string.");
            }

            if (!patternProperties.ContainsKey("title") || !(patternProperties["title"] is string title))
            {
                throw new ArgumentException("Pattern title must be a string.");
            }

            var content = patternProperties.ContainsKey("content") ? patternProperties["content"]?.ToString() : null;
            var filePath = patternProperties.ContainsKey("filePath") ? patternProperties["filePath"]?.ToString() : null;

            if (string.IsNullOrEmpty(content) && !string.IsNullOrEmpty(filePath))
            {
                content = ReadContentFromFile(filePath);
            }

            if (string.IsNullOrEmpty(content))
            {
                throw new ArgumentException("Pattern content must be provided either directly or via a file path.");
            }

            var pattern = new BlockPattern
            {
                Name = patternName,
                Title = title,
                Description = patternProperties.ContainsKey("description") ? patternProperties["description"]?.ToString() : null,
                Content = content,
                Keywords = patternProperties.ContainsKey("keywords") && patternProperties["keywords"] is List<string> keywords ? keywords : new List<string>(),
                BlockTypes = patternProperties.ContainsKey("blockTypes") && patternProperties["blockTypes"] is List<string> blockTypes ? blockTypes : new List<string>(),
                PostTypes = patternProperties.ContainsKey("postTypes") && patternProperties["postTypes"] is List<string> postTypes ? postTypes : new List<string>(),
                TemplateTypes = patternProperties.ContainsKey("templateTypes") && patternProperties["templateTypes"] is List<string> templateTypes ? templateTypes : new List<string>(),
                FilePath = filePath,
                IsRegisteredOutsideInit = !IsInInitAction()
            };

            _registeredPatterns[patternName] = pattern;

            if (pattern.IsRegisteredOutsideInit)
            {
                _registeredPatternsOutsideInit[patternName] = pattern;
            }

            // Save to database
            dbContext.BlockPatterns.Add(pattern);
            dbContext.SaveChanges();

            return true;
        }

        /// <summary>
        /// Unregisters a block pattern.
        /// </summary>
        public bool Unregister(string patternName, AppDbContext dbContext)
        {
            if (!_registeredPatterns.ContainsKey(patternName))
            {
                throw new KeyNotFoundException($"Pattern \"{patternName}\" not found.");
            }

            _registeredPatterns.Remove(patternName);
            _registeredPatternsOutsideInit.Remove(patternName);

            // Remove from database
            var pattern = dbContext.BlockPatterns.FirstOrDefault(p => p.Name == patternName);
            if (pattern != null)
            {
                dbContext.BlockPatterns.Remove(pattern);
                dbContext.SaveChanges();
            }

            return true;
        }

        /// <summary>
        /// Retrieves an array containing the properties of a registered block pattern.
        /// </summary>
        public BlockPattern GetRegistered(string patternName)
        {
            return _registeredPatterns.ContainsKey(patternName) ? _registeredPatterns[patternName] : null;
        }

        /// <summary>
        /// Retrieves all registered block patterns.
        /// </summary>
        public IEnumerable<BlockPattern> GetAllRegistered(bool outsideInitOnly = false)
        {
            return outsideInitOnly
                ? _registeredPatternsOutsideInit.Values
                : _registeredPatterns.Values;
        }

        /// <summary>
        /// Checks if a block pattern is registered.
        /// </summary>
        public bool IsRegistered(string patternName)
        {
            return _registeredPatterns.ContainsKey(patternName);
        }

        /// <summary>
        /// Utility method to retrieve the main instance of the class.
        /// </summary>
        public static BlockPatternsRegistry GetInstance()
        {
            return _instance ??= new BlockPatternsRegistry();
        }

        /// <summary>
        /// Reads content from a file.
        /// </summary>
        private string ReadContentFromFile(string filePath)
        {
            if (System.IO.File.Exists(filePath))
            {
                return System.IO.File.ReadAllText(filePath);
            }

            throw new FileNotFoundException("File not found.", filePath);
        }

        /// <summary>
        /// Simulates checking if the current action is `init`.
        /// </summary>
        private bool IsInInitAction()
        {
            // In a real scenario, this could check the current action context.
            return true;
        }
    }
}