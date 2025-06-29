using System;
using System.Collections.Generic;

public class FontFace
{
    public string FontFamily { get; set; }
    public List<string> Src { get; set; } = new List<string>();
    public string FontStyle { get; set; } = "normal";
    public string FontWeight { get; set; } = "400";
    public string FontDisplay { get; set; } = "fallback";
    public string AscentOverride { get; set; }
    public string DescentOverride { get; set; }
    public string FontStretch { get; set; }
    public string FontVariant { get; set; }
    public string FontFeatureSettings { get; set; }
    public string FontVariationSettings { get; set; }
    public string LineGapOverride { get; set; }
    public string SizeAdjust { get; set; }
    public string UnicodeRange { get; set; }
}

public class FontCollection
{
    public string Slug { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public List<FontFamily> FontFamilies { get; set; } = new List<FontFamily>();
    public List<FontCategory> Categories { get; set; } = new List<FontCategory>();
}

public class FontFamily
{
    public string Name { get; set; }
    public List<FontFace> FontFaces { get; set; } = new List<FontFace>();
}

public class FontCategory
{
    public string Name { get; set; }
    public string Slug { get; set; }
}