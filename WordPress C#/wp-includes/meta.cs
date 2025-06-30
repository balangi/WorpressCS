using System;
using System.Collections.Generic;
using System.Linq;

public class MetaService
{
    private readonly ApplicationDbContext _context;

    public MetaService(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Adds metadata for the specified object.
    /// </summary>
    public void AddMetaData(string objectType, int objectId, string metaKey, string metaValue)
    {
        var metaData = new MetaData
        {
            ObjectType = objectType,
            ObjectId = objectId,
            MetaKey = metaKey,
            MetaValue = metaValue
        };

        _context.Metadata.Add(metaData);
        _context.SaveChanges();
    }

    /// <summary>
    /// Retrieves metadata for the specified object by meta key.
    /// </summary>
    public List<MetaData> GetMetaData(string objectType, int objectId, string metaKey = null)
    {
        var query = _context.Metadata
            .Where(m => m.ObjectType == objectType && m.ObjectId == objectId);

        if (!string.IsNullOrEmpty(metaKey))
        {
            query = query.Where(m => m.MetaKey == metaKey);
        }

        return query.ToList();
    }

    /// <summary>
    /// Updates metadata for the specified object.
    /// </summary>
    public void UpdateMetaData(int metaDataId, string newMetaValue)
    {
        var metaData = _context.Metadata.FirstOrDefault(m => m.Id == metaDataId);
        if (metaData == null)
        {
            throw new ArgumentException("Metadata not found.");
        }

        metaData.MetaValue = newMetaValue;
        _context.SaveChanges();
    }

    /// <summary>
    /// Deletes metadata for the specified object.
    /// </summary>
    public void DeleteMetaData(int metaDataId)
    {
        var metaData = _context.Metadata.FirstOrDefault(m => m.Id == metaDataId);
        if (metaData == null)
        {
            throw new ArgumentException("Metadata not found.");
        }

        _context.Metadata.Remove(metaData);
        _context.SaveChanges();
    }

    /// <summary>
    /// Checks if metadata exists for the specified object and key.
    /// </summary>
    public bool HasMetaData(string objectType, int objectId, string metaKey)
    {
        return _context.Metadata
            .Any(m => m.ObjectType == objectType && m.ObjectId == objectId && m.MetaKey == metaKey);
    }

    /// <summary>
    /// Retrieves metadata by its ID.
    /// </summary>
    public MetaData GetMetaDataById(int metaDataId)
    {
        return _context.Metadata.FirstOrDefault(m => m.Id == metaDataId);
    }
}