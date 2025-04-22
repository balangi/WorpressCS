public enum AvifParseStatus
{
    Found,
    NotFound,
    Truncated,
    Aborted,
    Invalid
}

public class AvifFeatures
{
    public bool HasPrimaryItem { get; set; }
    public bool HasAlpha { get; set; }
    public uint PrimaryItemId { get; set; }
    public uint Width { get; set; }
    public uint Height { get; set; }
    public uint BitDepth { get; set; }
    public uint NumChannels { get; set; }
}

public class AvifBox
{
    public uint Size { get; set; }
    public string Type { get; set; }
    public byte Version { get; set; }
    public uint Flags { get; set; }
    public long ContentSize { get; set; }
}