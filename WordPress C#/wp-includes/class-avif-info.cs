using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

public class AvifParser : IDisposable
{
    private const uint MaxSize = 4294967295;
    private const int MaxNumBoxes = 4096;
    private const int MaxTiles = 16;
    private const int MaxProps = 32;
    
    private readonly BinaryReader _reader;
    private int _parsedBoxesCount;
    private bool _dataWasSkipped;

    public AvifFeatures Features { get; } = new AvifFeatures();

    public AvifParser(Stream stream)
    {
        _reader = new BinaryReader(stream, Encoding.UTF8, true);
    }

    public bool Parse()
    {
        try
        {
            if (!ParseFtyp()) return false;
            return ParseMeta();
        }
        catch (EndOfStreamException)
        {
            return false;
        }
    }

    private bool ParseFtyp()
    {
        var box = ParseBoxHeader();
        if (box.Type != "ftyp") return false;

        var brandFound = false;
        var majorBrand = ReadString(4);
        ReadUInt32BigEndian(); // Minor version

        var remaining = box.ContentSize - 8;
        while (remaining > 0)
        {
            var brand = ReadString(4);
            if (brand is "avif" or "avis")
                brandFound = true;
            
            remaining -= 4;
        }

        return brandFound;
    }

    private bool ParseMeta()
    {
        while (ParseBoxHeader() is { } box)
        {
            if (box.Type == "meta")
            {
                return ParseMetaContent(box.ContentSize);
            }
            
            Skip(box.ContentSize);
        }
        return false;
    }

    private bool ParseMetaContent(long contentSize)
    {
        while (contentSize > 0)
        {
            var box = ParseBoxHeader();
            contentSize -= (int)box.Size;

            switch (box.Type)
            {
                case "pitm":
                    ParsePitm(box);
                    break;
                case "iprp":
                    ParseIprp(box.ContentSize);
                    break;
                case "iref":
                    ParseIref(box.ContentSize);
                    break;
                default:
                    Skip(box.ContentSize);
                    break;
            }
        }

        return Features.HasPrimaryItem &&
               Features.Width > 0 &&
               Features.Height > 0;
    }

    private AvifBox ParseBoxHeader()
    {
        if (_reader.BaseStream.Position + 8 > _reader.BaseStream.Length)
            throw new EndOfStreamException();

        var size = ReadUInt32BigEndian();
        var type = ReadString(4);

        var headerSize = 8L;
        if (size == 1)
        {
            size = (uint)ReadUInt64BigEndian();
            headerSize += 8;
        }

        var box = new AvifBox
        {
            Size = size,
            Type = type,
            ContentSize = size - headerSize
        };

        if (IsFullBox(type))
        {
            var versionFlags = ReadUInt32BigEndian();
            box.Version = (byte)(versionFlags >> 24);
            box.Flags = versionFlags & 0x00FFFFFF;
            headerSize += 4;
        }

        box.ContentSize = size - headerSize;
        _parsedBoxesCount++;

        if (_parsedBoxesCount > MaxNumBoxes)
            throw new InvalidOperationException("Too many boxes");

        return box;
    }

    private void ParsePitm(AvifBox box)
    {
        var itemId = box.Version == 0 ? 
            ReadUInt16BigEndian() : 
            ReadUInt32BigEndian();
        
        Features.HasPrimaryItem = true;
        Features.PrimaryItemId = itemId;
    }

    // سایر متدهای ParseIprp، ParseIref و...

    #region Helper Methods
    
    private uint ReadUInt32BigEndian()
    {
        return BitConverter.ToUInt32(ReadBytes(4).Reverse().ToArray());
    }

    private ushort ReadUInt16BigEndian()
    {
        return BitConverter.ToUInt16(ReadBytes(2).Reverse().ToArray());
    }

    private string ReadString(int length)
    {
        return Encoding.ASCII.GetString(ReadBytes(length));
    }

    private byte[] ReadBytes(int count)
    {
        var bytes = _reader.ReadBytes(count);
        if (bytes.Length != count)
            throw new EndOfStreamException();
        return bytes;
    }

    private void Skip(long bytes)
    {
        _reader.BaseStream.Seek(bytes, SeekOrigin.Current);
    }

    private static bool IsFullBox(string type)
    {
        return type is "meta" or "pitm" or "ipma" or "ispe" 
            or "pixi" or "iref" or "auxC";
    }

    public void Dispose()
    {
        _reader.Dispose();
    }
    
    #endregion
}