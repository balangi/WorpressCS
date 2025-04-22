using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using WordPress.Core.Data;
using WordPress.Core.Models;

namespace WordPress.Core.BlockMetadata
{
    public static class BlockMetadataRegistry
    {
        /// <summary>
        /// Container for storing block metadata collections.
        /// </summary>
        private static readonly Dictionary<string, BlockMetadataCollection> _collections = new Dictionary<string, BlockMetadataCollection>();

        /// <summary>
        /// Default identifier function to determine the block identifier from a given path.
        /// </summary>
        public static string DefaultIdentifierCallback(string path)
        {
            if (string.IsNullOrEmpty(path))
                return string.Empty;

            var normalizedPath = NormalizePath(path);

            if (Path.GetFileName(normalizedPath).Equals("block.json", StringComparison.OrdinalIgnoreCase))
            {
                return Path.GetFileName(Path.GetDirectoryName(normalizedPath));
            }

            return Path.GetFileName(normalizedPath);
        }

        /// <summary>
        /// Normalizes a path.
        /// </summary>
        private static string NormalizePath(string path)
        {
            return Path.GetFullPath(new Uri(path).LocalPath).TrimEnd(Path.DirectorySeparatorChar);
        }

        /// <summary>
        /// Registers a collection of block metadata.
        /// </summary>
        public static bool RegisterCollection(string path, string manifest, AppDbContext dbContext)
        {
            var normalizedPath = NormalizePath(path);

            if (_collections.ContainsKey(normalizedPath))
            {
                throw new InvalidOperationException("A collection with the same path is already registered.");
            }

            if (!File.Exists(manifest))
            {
                throw new FileNotFoundException("The specified manifest file does not exist.", manifest);
            }

            var collection = new BlockMetadataCollection
            {
                Path = normalizedPath,
                Metadata = LoadManifest(manifest)
            };

            _collections[normalizedPath] = collection;

            // Save to database
            dbContext.BlockMetadataCollections.Add(collection);
            dbContext.SaveChanges();

            return true;
        }

        /// <summary>
        /// Loads metadata from a manifest file.
        /// </summary>
        private static Dictionary<string, object> LoadManifest(string manifest)
        {
            // Simulate loading metadata from a PHP-like manifest file
            // In a real scenario, this could involve parsing a JSON or XML file
            return new Dictionary<string, object>
            {
                { "example-block", new { title = "Example Block", category = "widgets", icon = "smiley" } },
                { "another-block", new { title = "Another Block", category = "formatting", icon = "star-filled" } }
            };
        }

        /// <summary>
        /// Retrieves block metadata for a given block within a specific collection.
        /// </summary>
        public static Dictionary<string, object> GetMetadata(string fileOrFolder)
        {
            var normalizedPath = NormalizePath(fileOrFolder);
            var collectionPath = FindCollectionPath(normalizedPath);

            if (collectionPath == null || !_collections.ContainsKey(collectionPath))
            {
                return null;
            }

            var collection = _collections[collectionPath];
            var blockName = DefaultIdentifierCallback(normalizedPath);

            return collection.Metadata.ContainsKey(blockName) ? collection.Metadata[blockName] as Dictionary<string, object> : null;
        }

        /// <summary>
        /// Finds the collection path for a given file or folder.
        /// </summary>
        private static string FindCollectionPath(string path)
        {
            return _collections.Keys.FirstOrDefault(collectionPath => path.StartsWith(collectionPath, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Checks if metadata exists for a given block name in a specific collection.
        /// </summary>
        public static bool HasMetadata(string fileOrFolder)
        {
            return GetMetadata(fileOrFolder) != null;
        }

        /// <summary>
        /// Gets the list of absolute paths to all block metadata files that are part of the given collection.
        /// </summary>
        public static List<string> GetCollectionBlockMetadataFiles(string path)
        {
            var normalizedPath = NormalizePath(path);

            if (!_collections.ContainsKey(normalizedPath))
            {
                throw new KeyNotFoundException("No registered block metadata collection was found for the provided path.");
            }

            var collection = _collections[normalizedPath];
            return collection.Metadata.Keys.Select(blockName => Path.Combine(normalizedPath, blockName, "block.json")).ToList();
        }
    }
}