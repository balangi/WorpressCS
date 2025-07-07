using System;
using System.Collections.Generic;

public class NetworkSettings
{
    public string CookiePath { get; set; }
    public string SiteCookiePath { get; set; }
    public string AdminCookiePath { get; set; }
    public string CookieDomain { get; set; }
}

public class UploadSettings
{
    public string UploadBlogsDir { get; set; }
    public string UploadsDir { get; set; }
    public string BlogUploadDir { get; set; }
}

public class FileConstants
{
    public bool WpmuSendFile { get; set; }
    public bool WpmuAccelRedirect { get; set; }
}