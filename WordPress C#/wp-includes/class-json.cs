using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Services
{
    /// <summary>
    /// Represents a JSON encoder and decoder.
    /// </summary>
    public class JsonService
    {
        private readonly JsonSerializerOptions _options;

        /// <summary>
        /// Initializes a new instance of the JsonService class.
        /// </summary>
        public JsonService()
        {
            // Configure JSON serialization options
            _options = new JsonSerializerOptions
            {
                WriteIndented = true, // Pretty-print JSON
                PropertyNameCaseInsensitive = true,
                AllowTrailingCommas = true,
                ReadCommentHandling = JsonCommentHandling.Skip
            };
        }

        /// <summary>
        /// Encodes an object into a JSON string.
        /// </summary>
        /// <param name="value">The object to encode.</param>
        /// <returns>A JSON string representation of the object.</returns>
        public string Encode(object value)
        {
            try
            {
                return JsonSerializer.Serialize(value, _options);
            }
            catch (Exception ex)
            {
                throw new JsonException("Failed to encode object to JSON.", ex);
            }
        }

        /// <summary>
        /// Decodes a JSON string into an object of the specified type.
        /// </summary>
        /// <typeparam name="T">The type of the object to decode into.</typeparam>
        /// <param name="json">The JSON string to decode.</param>
        /// <returns>An object of the specified type.</returns>
        public T Decode<T>(string json)
        {
            try
            {
                return JsonSerializer.Deserialize<T>(json, _options);
            }
            catch (Exception ex)
            {
                throw new JsonException("Failed to decode JSON string.", ex);
            }
        }

        /// <summary>
        /// Decodes a JSON string into a dynamic object.
        /// </summary>
        /// <param name="json">The JSON string to decode.</param>
        /// <returns>A dynamic object representing the JSON data.</returns>
        public dynamic Decode(string json)
        {
            try
            {
                return JsonSerializer.Deserialize<JsonElement>(json, _options);
            }
            catch (Exception ex)
            {
                throw new JsonException("Failed to decode JSON string.", ex);
            }
        }
    }
}