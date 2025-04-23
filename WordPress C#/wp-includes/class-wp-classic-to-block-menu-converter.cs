using System;
using System.Collections.Generic;
using System.Linq;
using WordPress.Core.Data;
using WordPress.Core.Models;

namespace WordPress.Core.MenuConverters
{
    public class ClassicToBlockMenuConverter
    {
        /// <summary>
        /// Converts a classic menu to block format.
        /// </summary>
        public static object Convert(MenuItem menu, AppDbContext dbContext)
        {
            if (menu == null || string.IsNullOrEmpty(menu.Title))
            {
                throw new ArgumentException("The menu provided is not a valid menu.");
            }

            var menuItems = dbContext.MenuItems.Where(mi => mi.ParentId == null).ToList();

            if (menuItems == null || !menuItems.Any())
            {
                return string.Empty;
            }

            // Group menu items by parent ID
            var menuItemsByParentId = GroupByParentId(menuItems);

            // Get the first level of menu items
            var firstLevelMenuItems = menuItemsByParentId.ContainsKey(0)
                ? menuItemsByParentId[0]
                : new List<MenuItem>();

            // Convert to blocks
            var innerBlocks = ToBlocks(firstLevelMenuItems, menuItemsByParentId);

            return SerializeBlocks(innerBlocks);
        }

        /// <summary>
        /// Groups menu items by their parent ID.
        /// </summary>
        private static Dictionary<int, List<MenuItem>> GroupByParentId(List<MenuItem> menuItems)
        {
            var grouped = new Dictionary<int, List<MenuItem>>();

            foreach (var menuItem in menuItems)
            {
                var parentId = menuItem.ParentId ?? 0;

                if (!grouped.ContainsKey(parentId))
                {
                    grouped[parentId] = new List<MenuItem>();
                }

                grouped[parentId].Add(menuItem);
            }

            return grouped;
        }

        /// <summary>
        /// Converts menu items to nested blocks.
        /// </summary>
        private static List<Dictionary<string, object>> ToBlocks(
            List<MenuItem> menuItems,
            Dictionary<int, List<MenuItem>> menuItemsByParentId)
        {
            if (menuItems == null || !menuItems.Any())
            {
                return new List<Dictionary<string, object>>();
            }

            var blocks = new List<Dictionary<string, object>>();

            foreach (var menuItem in menuItems)
            {
                var className = menuItem.Classes != null && menuItem.Classes.Any()
                    ? string.Join(" ", menuItem.Classes)
                    : null;

                var block = new Dictionary<string, object>
                {
                    ["blockName"] = menuItemsByParentId.ContainsKey(menuItem.Id)
                        ? "core/navigation-submenu"
                        : "core/navigation-link",
                    ["attrs"] = new Dictionary<string, object>
                    {
                        ["className"] = className,
                        ["description"] = menuItem.Description,
                        ["id"] = menuItem.Id,
                        ["kind"] = menuItem.Kind?.Replace("_", "-"),
                        ["label"] = menuItem.Title,
                        ["opensInNewTab"] = menuItem.OpensInNewTab,
                        ["rel"] = menuItem.Rel,
                        ["title"] = menuItem.Title,
                        ["type"] = menuItem.Type,
                        ["url"] = menuItem.Url
                    }
                };

                // Recursively process child menu items
                var innerBlocks = menuItemsByParentId.ContainsKey(menuItem.Id)
                    ? ToBlocks(menuItemsByParentId[menuItem.Id], menuItemsByParentId)
                    : new List<Dictionary<string, object>>();

                block["innerBlocks"] = innerBlocks;
                block["innerContent"] = innerBlocks.Select(SerializeBlock).ToList();

                blocks.Add(block);
            }

            return blocks;
        }

        /// <summary>
        /// Serializes blocks into a string format.
        /// </summary>
        private static string SerializeBlocks(List<Dictionary<string, object>> blocks)
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(blocks);
        }

        /// <summary>
        /// Serializes a single block.
        /// </summary>
        private static string SerializeBlock(Dictionary<string, object> block)
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(block);
        }
    }
}