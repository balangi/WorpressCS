using System;
using System.Collections.Generic;
using System.Linq;

namespace WordPressThemeJson
{
    /// <summary>
    /// Class to provide access to update a theme.json structure.
    /// </summary>
    public class WP_Theme_JSON_Data
    {
        /// <summary>
        /// Container of the data to update.
        /// </summary>
        private WP_Theme_JSON ThemeJson { get; set; }

        /// <summary>
        /// The origin of the data: default, theme, user, etc.
        /// </summary>
        private string Origin { get; set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="data">Array following the theme.json specification.</param>
        /// <param name="origin">The origin of the data: default, theme, user.</param>
        public WP_Theme_JSON_Data(Dictionary<string, object> data = null, string origin = "theme")
        {
            // Initialize with default schema if no data is provided
            data ??= new Dictionary<string, object>
            {
                { "version", WP_Theme_JSON.LatestSchema }
            };

            this.Origin = origin;
            this.ThemeJson = new WP_Theme_JSON(data, this.Origin);
        }

        /// <summary>
        /// Updates the theme.json with the given data.
        /// </summary>
        /// <param name="newData">Array following the theme.json specification.</param>
        /// <returns>The own instance with access to the modified data.</returns>
        public WP_Theme_JSON_Data UpdateWith(Dictionary<string, object> newData)
        {
            var newThemeJson = new WP_Theme_JSON(newData, this.Origin);
            this.ThemeJson.Merge(newThemeJson);
            return this;
        }

        /// <summary>
        /// Returns an array containing the underlying data following the theme.json specification.
        /// </summary>
        /// <returns>Dictionary representing the raw data.</returns>
        public Dictionary<string, object> GetData()
        {
            return this.ThemeJson.GetRawData();
        }

        /// <summary>
        /// Returns theme JSON object.
        /// </summary>
        /// <returns>The theme JSON structure stored in this data object.</returns>
        public WP_Theme_JSON GetThemeJson()
        {
            return this.ThemeJson;
        }
    }

    /// <summary>
    /// Class that encapsulates the processing of structures that adhere to the theme.json spec.
    /// </summary>
    public class WP_Theme_JSON
    {
        /// <summary>
        /// The latest version of the schema in use.
        /// </summary>
        public const int LatestSchema = 3;

        /// <summary>
        /// Container of data in theme.json format.
        /// </summary>
        private Dictionary<string, object> ThemeJson { get; set; }

        /// <summary>
        /// The origin of the data: default, theme, user, etc.
        /// </summary>
        private string Origin { get; set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="data">Array following the theme.json specification.</param>
        /// <param name="origin">The origin of the data: default, theme, user.</param>
        public WP_Theme_JSON(Dictionary<string, object> data, string origin)
        {
            this.ThemeJson = Migrate(data, origin);
            this.Origin = origin;
        }

        /// <summary>
        /// Migrates the theme.json data to the latest schema.
        /// </summary>
        /// <param name="data">The input data.</param>
        /// <param name="origin">The origin of the data.</param>
        /// <returns>Migrated data.</returns>
        private static Dictionary<string, object> Migrate(Dictionary<string, object> data, string origin)
        {
            // Placeholder for migration logic
            return data;
        }

        /// <summary>
        /// Merges the current theme.json with another theme.json.
        /// </summary>
        /// <param name="other">Another theme.json object to merge.</param>
        public void Merge(WP_Theme_JSON other)
        {
            // Placeholder for merge logic
            foreach (var key in other.ThemeJson.Keys)
            {
                if (this.ThemeJson.ContainsKey(key))
                {
                    this.ThemeJson[key] = other.ThemeJson[key];
                }
                else
                {
                    this.ThemeJson.Add(key, other.ThemeJson[key]);
                }
            }
        }

        /// <summary>
        /// Returns the raw data of the theme.json.
        /// </summary>
        /// <returns>Dictionary representing the raw data.</returns>
        public Dictionary<string, object> GetRawData()
        {
            return this.ThemeJson;
        }
    }
}