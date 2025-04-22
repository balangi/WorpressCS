using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IXR
{
    /// <summary>
    /// Represents a value in XML-RPC format.
    /// </summary>
    public class IXR_Value
    {
        // Properties
        public object Data { get; private set; }
        public string Type { get; private set; }

        /// <summary>
        /// Constructor to initialize the IXR_Value object.
        /// </summary>
        /// <param name="data">The data to serialize.</param>
        /// <param name="type">Optional type of the data (auto-detected if not provided).</param>
        public IXR_Value(object data, string type = null)
        {
            Data = data;
            Type = string.IsNullOrEmpty(type) ? CalculateType() : type;

            // If the type is 'struct' or 'array', recursively convert all values to IXR_Value objects
            if (Type == "struct")
            {
                var dictionary = (Data as Dictionary<string, object>) ?? new Dictionary<string, object>();
                foreach (var key in dictionary.Keys.ToList())
                {
                    dictionary[key] = new IXR_Value(dictionary[key]);
                }
                Data = dictionary;
            }
            else if (Type == "array")
            {
                var list = (Data as List<object>) ?? new List<object>();
                for (int i = 0; i < list.Count; i++)
                {
                    list[i] = new IXR_Value(list[i]);
                }
                Data = list;
            }
        }

        /// <summary>
        /// Calculates the type of the data.
        /// </summary>
        /// <returns>The detected type.</returns>
        private string CalculateType()
        {
            if (Data is bool)
                return "boolean";
            if (Data is int)
                return "int";
            if (Data is double)
                return "double";
            if (Data is IXR_Date)
                return "date";
            if (Data is IXR_Base64)
                return "base64";
            if (Data is Dictionary<string, object>)
                return "struct";
            if (Data is List<object> list && IsStruct(list))
                return "struct";
            if (Data is List<object>)
                return "array";
            return "string";
        }

        /// <summary>
        /// Gets the XML representation of the value.
        /// </summary>
        /// <returns>The XML string for the value.</returns>
        public string GetXml()
        {
            var xmlBuilder = new StringBuilder();

            switch (Type)
            {
                case "boolean":
                    xmlBuilder.Append($"<boolean>{((bool)Data ? 1 : 0)}</boolean>");
                    break;

                case "int":
                    xmlBuilder.Append($"<int>{Data}</int>");
                    break;

                case "double":
                    xmlBuilder.Append($"<double>{Data}</double>");
                    break;

                case "string":
                    xmlBuilder.Append($"<string>{EscapeXml((string)Data)}</string>");
                    break;

                case "array":
                    xmlBuilder.AppendLine("<array><data>");
                    foreach (var item in (List<object>)Data)
                    {
                        xmlBuilder.AppendLine($"  <value>{item.GetXml()}</value>");
                    }
                    xmlBuilder.AppendLine("</data></array>");
                    break;

                case "struct":
                    xmlBuilder.AppendLine("<struct>");
                    foreach (var kvp in (Dictionary<string, object>)Data)
                    {
                        var key = EscapeXml(kvp.Key);
                        xmlBuilder.AppendLine($"  <member><name>{key}</name><value>{kvp.Value.GetXml()}</value></member>");
                    }
                    xmlBuilder.AppendLine("</struct>");
                    break;

                case "date":
                    xmlBuilder.Append(((IXR_Date)Data).GetXml());
                    break;

                case "base64":
                    xmlBuilder.Append(((IXR_Base64)Data).GetXml());
                    break;

                default:
                    throw new InvalidOperationException($"Unsupported type: {Type}");
            }

            return xmlBuilder.ToString();
        }

        /// <summary>
        /// Checks whether the given list is a struct (associative array) or not.
        /// </summary>
        /// <param name="list">The list to check.</param>
        /// <returns>True if the list is a struct; otherwise, false.</returns>
        private bool IsStruct(List<object> list)
        {
            return list.Select((value, index) => new { Index = index, Value = value })
                       .Any(item => item.Index != (int)item.Value);
        }

        /// <summary>
        /// Escapes special characters in XML.
        /// </summary>
        /// <param name="input">The input string.</param>
        /// <returns>The escaped string.</returns>
        private string EscapeXml(string input)
        {
            return input.Replace("&", "&amp;")
                        .Replace("<", "<")
                        .Replace(">", ">")
                        .Replace("\"", "&quot;")
                        .Replace("'", "&apos;");
        }
    }
}