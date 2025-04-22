using System;
using System.Collections.Generic;
using System.Linq;
using WordPress.Core.Data;
using WordPress.Core.Models;

namespace WordPress.Core.BlockPatterns
{
    public class BlockPatternCategoriesRegistry
    {
        /// <summary>
        /// Registered block pattern categories dictionary.
        /// </summary>
        private readonly Dictionary<string, BlockPatternCategory> _registeredCategories = new Dictionary<string, BlockPatternCategory>();

        /// <summary>
        /// Pattern categories registered outside the `init` action.
        /// </summary>
        private readonly Dictionary<string, BlockPatternCategory> _registeredCategoriesOutsideInit = new Dictionary<string, BlockPatternCategory>();

        /// <summary>
        /// Container for the main instance of the class.
        /// </summary>
        private static BlockPatternCategoriesRegistry _instance;

        /// <summary>
        /// Private constructor to enforce singleton pattern.
        /// </summary>
        private BlockPatternCategoriesRegistry() { }

        /// <summary>
        /// Registers a pattern category.
        /// </summary>
        public bool Register(string categoryName, Dictionary<string, object> categoryProperties, AppDbContext dbContext)
        {
            if (string.IsNullOrEmpty(categoryName))
            {
                throw new ArgumentException("Block pattern category name must be a string.");
            }

            var category = new BlockPatternCategory
            {
                Name = categoryName,
                Label = categoryProperties.ContainsKey("label") ? categoryProperties["label"].ToString() : null,
                IsRegisteredOutsideInit = !IsInInitAction()
            };

            _registeredCategories[categoryName] = category;

            if (!IsInInitAction())
            {
                _registeredCategoriesOutsideInit[categoryName] = category;
            }

            // Save to database
            dbContext.BlockPatternCategories.Add(category);
            dbContext.SaveChanges();

            return true;
        }

        /// <summary>
        /// Unregisters a pattern category.
        /// </summary>
        public bool Unregister(string categoryName, AppDbContext dbContext)
        {
            if (!_registeredCategories.ContainsKey(categoryName))
            {
                throw new KeyNotFoundException($"Block pattern category \"{categoryName}\" not found.");
            }

            _registeredCategories.Remove(categoryName);
            _registeredCategoriesOutsideInit.Remove(categoryName);

            // Remove from database
            var category = dbContext.BlockPatternCategories.FirstOrDefault(c => c.Name == categoryName);
            if (category != null)
            {
                dbContext.BlockPatternCategories.Remove(category);
                dbContext.SaveChanges();
            }

            return true;
        }

        /// <summary>
        /// Retrieves an array containing the properties of a registered pattern category.
        /// </summary>
        public BlockPatternCategory GetRegistered(string categoryName)
        {
            return _registeredCategories.ContainsKey(categoryName) ? _registeredCategories[categoryName] : null;
        }

        /// <summary>
        /// Retrieves all registered pattern categories.
        /// </summary>
        public IEnumerable<BlockPatternCategory> GetAllRegistered(bool outsideInitOnly = false)
        {
            return outsideInitOnly
                ? _registeredCategoriesOutsideInit.Values
                : _registeredCategories.Values;
        }

        /// <summary>
        /// Checks if a pattern category is registered.
        /// </summary>
        public bool IsRegistered(string categoryName)
        {
            return _registeredCategories.ContainsKey(categoryName);
        }

        /// <summary>
        /// Utility method to retrieve the main instance of the class.
        /// </summary>
        public static BlockPatternCategoriesRegistry GetInstance()
        {
            return _instance ??= new BlockPatternCategoriesRegistry();
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