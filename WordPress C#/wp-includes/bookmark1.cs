using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text.RegularExpressions;

public static class BookmarkApi
{
    /// <summary>
    /// Retrieves bookmark data
    /// </summary>
    public static dynamic GetBookmark(dynamic bookmark, string output = "OBJECT", string filter = "raw")
    {
        var db = DbHelper.GetDatabase();
        
        dynamic _bookmark;
        
        if (bookmark == null)
        {
            _bookmark = Globals.Link ?? null;
        }
        else if (bookmark is IDictionary<string, object>)
        {
            CacheHelper.AddToCache(bookmark.link_id.ToString(), bookmark, "bookmark");
            _bookmark = bookmark;
        }
        else
        {
            if (Globals.Link != null && Globals.Link.link_id == bookmark)
            {
                _bookmark = Globals.Link;
            }
            else
            {
                _bookmark = CacheHelper.GetFromCache(bookmark.ToString(), "bookmark");
                if (_bookmark == null)
                {
                    var query = $"SELECT * FROM {db.Prefix}links WHERE link_id = @link_id LIMIT 1";
                    _bookmark = db.QueryFirstOrDefault(query, new { link_id = bookmark });
                    
                    if (_bookmark != null)
                    {
                        _bookmark.link_category = TermHelper.GetObjectTerms(_bookmark.link_id, "link_category", new { fields = "ids" })
                            .Distinct().ToArray();
                        CacheHelper.AddToCache(_bookmark.link_id.ToString(), _bookmark, "bookmark");
                    }
                }
            }
        }

        if (_bookmark == null)
        {
            return null;
        }

        _bookmark = SanitizeBookmark(_bookmark, filter);

        switch (output)
        {
            case "OBJECT":
                return _bookmark;
            case "ARRAY_A":
                return ObjectHelper.ToDictionary(_bookmark);
            case "ARRAY_N":
                return ObjectHelper.ToDictionary(_bookmark).Values.ToArray();
            default