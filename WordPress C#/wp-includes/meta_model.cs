using System;
using System.Collections.Generic;

public class MetaData
{
    public int Id { get; set; }
    public string MetaKey { get; set; }
    public string MetaValue { get; set; }
    public string ObjectType { get; set; } // e.g., "post", "user", "comment"
    public int ObjectId { get; set; }
}

public class ObjectMeta
{
    public int Id { get; set; }
    public string ObjectType { get; set; }
    public int ObjectId { get; set; }
    public List<MetaData> Metadata { get; set; } = new List<MetaData>();
}