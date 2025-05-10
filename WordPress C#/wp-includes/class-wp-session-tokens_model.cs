using System;
using System.Collections.Generic;

public class SessionToken
{
    public int Id { get; set; }
    public int UserId { get; set; } // شناسه کاربر
    public string TokenHash { get; set; } // هش توکن
    public DateTime Expiration { get; set; } // زمان انقضا
    public string IpAddress { get; set; } // آدرس IP
    public string UserAgent { get; set; } // عامل کاربر
    public DateTime LoginTime { get; set; } // زمان لاگین
}