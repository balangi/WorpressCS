using System;
using System.Collections.Generic;

public class HttpResponseData
{
    public int Id { get; set; }
    public string Url { get; set; } // URL درخواست
    public int StatusCode { get; set; } // کد وضعیت پاسخ
    public string Body { get; set; } // بدنه پاسخ
    public Dictionary<string, string> Headers { get; set; } // هدرهای پاسخ
    public DateTime RequestTime { get; set; } // زمان درخواست
}