public static class WordPressConstants
{
    // Human-readable data sizes in bytes
    public const long KB_IN_BYTES = 1024;
    public const long MB_IN_BYTES = 1024 * KB_IN_BYTES;
    public const long GB_IN_BYTES = 1024 * MB_IN_BYTES;
    public const long TB_IN_BYTES = 1024 * GB_IN_BYTES;
    public const long PB_IN_BYTES = 1024 * TB_IN_BYTES;
    public const long EB_IN_BYTES = 1024 * PB_IN_BYTES;
    public const long ZB_IN_BYTES = 1024 * EB_IN_BYTES;
    public const long YB_IN_BYTES = 1024 * ZB_IN_BYTES;

    // Time constants in seconds
    public const int MINUTE_IN_SECONDS = 60;
    public const int HOUR_IN_SECONDS = 60 * MINUTE_IN_SECONDS;
    public const int DAY_IN_SECONDS = 24 * HOUR_IN_SECONDS;
    public const int WEEK_IN_SECONDS = 7 * DAY_IN_SECONDS;
    public const int MONTH_IN_SECONDS = 30 * DAY_IN_SECONDS; // Approximate
    public const int YEAR_IN_SECONDS = 365 * DAY_IN_SECONDS; // Approximate

    // Cron lock timeout
    public const int WP_CRON_LOCK_TIMEOUT = MINUTE_IN_SECONDS;

    // Revision settings
    public const bool WP_POST_REVISIONS = true;
    public const int EMPTY_TRASH_DAYS = 30;
    public const int AUTOSAVE_INTERVAL = MINUTE_IN_SECONDS;

    // Development and debugging
    public const bool WP_DEBUG = false;
    public const bool WP_DEBUG_DISPLAY = true;
    public const bool WP_DEBUG_LOG = false;
    public const bool WP_CACHE = false;
    public const bool SCRIPT_DEBUG = false;
    public const bool MEDIA_TRASH = false;
    public const bool SHORTINIT = false;

    // Security & cookies
    public const string WP_DEFAULT_THEME = "twentytwentyfive";
    public const string COOKIE_DOMAIN = "";
    public const string RECOVERY_MODE_COOKIE_PREFIX = "wordpress_rec_";

    // Feature flags
    public const bool WP_FEATURE_BETTER_PASSWORDS = true;

    // Paths - These should be set dynamically at runtime
    public static string ABSPATH => AppDomain.CurrentDomain.BaseDirectory;
    public static string WP_CONTENT_DIR => Path.Combine(ABSPATH, "wp-content");
    public static string WP_PLUGIN_DIR => Path.Combine(WP_CONTENT_DIR, "plugins");
    public static string WP_CONTENT_URL => "/wp-content"; // Example for URL path
}