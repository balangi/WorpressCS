

        public CustomSettingService(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Retrieves all active custom settings.
        /// </summary>
        public IEnumerable<CustomSetting> GetAllActiveSettings()
        {
            return _context.CustomSettings
                .Include(s => s.Options)
                .Where(s => s.IsActive)
                .ToList();
        }

        /// <summary>
        /// Retrieves a custom setting by key.
        /// </summary>
        public CustomSetting? GetSettingByKey(string key)
        {
            return _context.CustomSettings
                .Include(s => s.Options)
                .FirstOrDefault(s => s.Key == key);
        }

        /// <summary>
        /// Adds or updates a custom setting.
        /// </summary>
        public void AddOrUpdateSetting(CustomSetting setting)
        {
            var existingSetting = _context.CustomSettings.FirstOrDefault(s => s.Key == setting.Key);

            if (existingSetting != null)
            {
                // Update existing setting
                existingSetting.Value = setting.Value;
                existingSetting.Type = setting.Type;
                existingSetting.Section = setting.Section;
                existingSetting.Label = setting.Label;
                existingSetting.Description = setting.Description;
                existingSetting.IsActive = setting.IsActive;

                _context.CustomSettings.Update(existingSetting);
            }
            else
            {
                // Add new setting
                _context.CustomSettings.Add(setting);
            }

            _context.SaveChanges();
        }

        /// <summary>
        /// Deletes a custom setting by key.
        /// </summary>
        public void DeleteSettingByKey(string key)
        {
            var setting = _context.CustomSettings.FirstOrDefault(s => s.Key == key);

            if (setting != null)
            {
                _context.CustomSettings.Remove(setting);
                _context.SaveChanges();
            }
        }