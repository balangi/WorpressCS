using System;
using System.Collections.Generic;
using System.Linq;

namespace WordPress.Users
{
    /// <summary>
    /// Core class used to implement the WP_User object.
    /// </summary>
    public class WP_User
    {
        /// <summary>
        /// User data container.
        /// </summary>
        public dynamic Data { get; private set; }

        /// <summary>
        /// The user's ID.
        /// </summary>
        public int ID { get; private set; }

        /// <summary>
        /// Array of key/value pairs where keys represent a capability name and boolean values represent whether the user has that capability.
        /// </summary>
        public Dictionary<string, bool> AllCaps { get; private set; } = new();

        /// <summary>
        /// The filter context applied to user data fields.
        /// </summary>
        public string Filter { get; private set; }

        /// <summary>
        /// The site ID the capabilities of this user are initialized for.
        /// </summary>
        public int SiteID { get; private set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="id">User's ID.</param>
        /// <param name="name">Optional. User's username.</param>
        /// <param name="siteID">Optional. Site ID, defaults to current site.</param>
        public WP_User(int id = 0, string name = "", int siteID = 0)
        {
            ID = id;
            SiteID = siteID;
            InitializeUserData(id, name);
        }

        /// <summary>
        /// Initializes user data.
        /// </summary>
        /// <param name="id">User's ID.</param>
        /// <param name="name">User's username.</param>
        private void InitializeUserData(int id, string name)
        {
            // Fetch user data from database or cache
            var user = GetUserFromDatabase(id, name);

            if (user == null)
            {
                throw new ArgumentException("Invalid user ID or username.");
            }

            Data = user;
            ID = Convert.ToInt32(Data.ID);
            AllCaps = GetUserCapabilities(ID);
        }

        /// <summary>
        /// Fetches user data from the database.
        /// </summary>
        /// <param name="id">User's ID.</param>
        /// <param name="name">User's username.</param>
        /// <returns>User data as a dynamic object.</returns>
        private dynamic GetUserFromDatabase(int id, string name)
        {
            // Simulate fetching user data from database
            var users = new List<dynamic>
            {
                new { ID = 1, Username = "admin", Email = "admin@example.com", Role = "administrator" },
                new { ID = 2, Username = "editor", Email = "editor@example.com", Role = "editor" }
            };

            if (id > 0)
            {
                return users.FirstOrDefault(u => u.ID == id);
            }
            else if (!string.IsNullOrEmpty(name))
            {
                return users.FirstOrDefault(u => u.Username == name);
            }

            return null;
        }

        /// <summary>
        /// Gets user capabilities.
        /// </summary>
        /// <param name="userID">User's ID.</param>
        /// <returns>Dictionary of capabilities.</returns>
        private Dictionary<string, bool> GetUserCapabilities(int userID)
        {
            // Simulate fetching user capabilities from database
            var capabilities = new Dictionary<string, bool>
            {
                { "edit_posts", true },
                { "delete_posts", false },
                { "manage_options", userID == 1 } // Only admin can manage options
            };

            return capabilities;
        }

        /// <summary>
        /// Magic method for getting a certain custom field.
        /// </summary>
        /// <param name="key">User meta key to get.</param>
        /// <returns>Value of the custom field.</returns>
        public object Get(string key)
        {
            if (Data == null || !Data.ContainsKey(key))
            {
                return null;
            }

            return Data[key];
        }

        /// <summary>
        /// Magic method for setting a certain custom field.
        /// </summary>
        /// <param name="key">User meta key to set.</param>
        /// <param name="value">Value to set.</param>
        public void Set(string key, object value)
        {
            if (Data == null)
            {
                Data = new Dictionary<string, object>();
            }

            Data[key] = value;
        }

        /// <summary>
        /// Magic method for checking the existence of a certain custom field.
        /// </summary>
        /// <param name="key">User meta key to check if set.</param>
        /// <returns>Whether the given user meta key is set.</returns>
        public bool IsSet(string key)
        {
            return Data != null && Data.ContainsKey(key);
        }

        /// <summary>
        /// Updates user capabilities based on roles.
        /// </summary>
        public void UpdateUserLevelFromCaps()
        {
            // Logic to update user level based on capabilities
            Console.WriteLine("Updating user level from capabilities...");
        }
    }
}