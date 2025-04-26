
    /// <summary>
    /// Represents a custom setting in the system.
    /// </summary>
    public class CustomSetting
    {
        // Properties
        public int Id { get; set; }
        public string Key { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
        public string Type { get; set; } = "text"; // e.g., text, checkbox, select
        public string Section { get; set; } = string.Empty;
        public string Label { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;

        // Navigation property
        public virtual ICollection<CustomSettingOption> Options { get; set; } = new List<CustomSettingOption>();
    }

    /// <summary>
    /// Represents options for a custom setting (e.g., dropdown options).
    /// </summary>
    public class CustomSettingOption
    {
        public int Id { get; set; }
        public int SettingId { get; set; } // Foreign key to CustomSetting
        public string Label { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;

        // Navigation property
        public virtual CustomSetting Setting { get; set; }
    }
