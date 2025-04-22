using System;

namespace IXR
{
    /// <summary>
    /// Represents a Base64 encoded data object.
    /// </summary>
    public class IXR_Base64
    {
        // Property to hold the data
        public byte[] Data { get; private set; }

        /// <summary>
        /// Constructor to initialize the IXR_Base64 object with data.
        /// </summary>
        /// <param name="data">The raw data to be encoded in Base64.</param>
        public IXR_Base64(byte[] data)
        {
            Data = data ?? throw new ArgumentNullException(nameof(data), "Data cannot be null.");
        }

        /// <summary>
        /// Converts the data to an XML representation with Base64 encoding.
        /// </summary>
        /// <returns>A string containing the XML representation of the Base64 encoded data.</returns>
        public string GetXml()
        {
            string base64String = Convert.ToBase64String(Data);
            return $"<base64>{base64String}</base64>";
        }
    }
}