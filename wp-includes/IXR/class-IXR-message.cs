using System;
using System.Collections.Generic;
using System.Xml;

namespace IXR
{
    /// <summary>
    /// Represents a message in XML-RPC format.
    /// </summary>
    public class IXR_Message
    {
        // Properties
        public string Message { get; private set; }
        public string MessageType { get; private set; } // methodCall / methodResponse / fault
        public int FaultCode { get; private set; }
        public string FaultString { get; private set; }
        public string MethodName { get; private set; }
        public List<object> Params { get; private set; } = new List<object>();

        // Stacks for tracking arrays and structs
        private readonly Stack<List<object>> _arrayStructs = new Stack<List<object>>();
        private readonly Stack<string> _arrayStructTypes = new Stack<string>();
        private readonly Stack<string> _currentStructName = new Stack<string>();

        private string _currentTag;
        private string _currentTagContents;

        /// <summary>
        /// Constructor to initialize the IXR_Message object.
        /// </summary>
        /// <param name="message">The raw XML-RPC message.</param>
        public IXR_Message(string message)
        {
            if (string.IsNullOrEmpty(message))
                throw new ArgumentNullException(nameof(message), "Message cannot be null or empty.");

            Message = message;
        }

        /// <summary>
        /// Parses the XML-RPC message.
        /// </summary>
        /// <returns>True if parsing was successful; otherwise, false.</returns>
        public bool Parse()
        {
            try
            {
                using (var reader = XmlReader.Create(new System.IO.StringReader(Message)))
                {
                    while (reader.Read())
                    {
                        switch (reader.NodeType)
                        {
                            case XmlNodeType.Element:
                                HandleTagOpen(reader);
                                break;

                            case XmlNodeType.Text:
                                HandleCData(reader.Value);
                                break;

                            case XmlNodeType.EndElement:
                                HandleTagClose(reader.Name);
                                break;
                        }
                    }
                }

                // Check for fault messages
                if (MessageType == "fault" && Params.Count > 0)
                {
                    if (Params[0] is Dictionary<string, object> fault)
                    {
                        FaultCode = (int)(double)fault["faultCode"];
                        FaultString = (string)fault["faultString"];
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error parsing XML-RPC message: {ex.Message}");
                return false;
            }
        }

        private void HandleTagOpen(XmlReader reader)
        {
            _currentTag = reader.Name;
            _currentTagContents = string.Empty;

            switch (_currentTag)
            {
                case "methodCall":
                case "methodResponse":
                case "fault":
                    MessageType = _currentTag;
                    break;

                case "data":
                    _arrayStructTypes.Push("array");
                    _arrayStructs.Push(new List<object>());
                    break;

                case "struct":
                    _arrayStructTypes.Push("struct");
                    _arrayStructs.Push(new List<object>());
                    break;
            }
        }

        private void HandleCData(string cdata)
        {
            _currentTagContents += cdata;
        }

        private void HandleTagClose(string tag)
        {
            object value = null;
            bool valueFlag = false;

            switch (tag)
            {
                case "int":
                case "i4":
                    value = int.Parse(_currentTagContents.Trim());
                    valueFlag = true;
                    break;

                case "double":
                    value = double.Parse(_currentTagContents.Trim());
                    valueFlag = true;
                    break;

                case "string":
                    value = _currentTagContents.Trim();
                    valueFlag = true;
                    break;

                case "dateTime.iso8601":
                    value = new IXR_Date(_currentTagContents.Trim());
                    valueFlag = true;
                    break;

                case "value":
                    if (!string.IsNullOrWhiteSpace(_currentTagContents))
                    {
                        value = _currentTagContents.Trim();
                        valueFlag = true;
                    }
                    break;

                case "boolean":
                    value = _currentTagContents.Trim() == "1";
                    valueFlag = true;
                    break;

                case "base64":
                    value = Convert.FromBase64String(_currentTagContents.Trim());
                    valueFlag = true;
                    break;

                case "data":
                case "struct":
                    value = _arrayStructs.Pop();
                    _arrayStructTypes.Pop();
                    valueFlag = true;
                    break;

                case "member":
                    _currentStructName.Pop();
                    break;

                case "name":
                    _currentStructName.Push(_currentTagContents.Trim());
                    break;

                case "methodName":
                    MethodName = _currentTagContents.Trim();
                    break;
            }

            if (valueFlag)
            {
                if (_arrayStructs.Count > 0)
                {
                    var currentStruct = _arrayStructs.Peek();
                    var currentType = _arrayStructTypes.Peek();

                    if (currentType == "struct")
                    {
                        ((Dictionary<string, object>)currentStruct)[_currentStructName.Peek()] = value;
                    }
                    else
                    {
                        ((List<object>)currentStruct).Add(value);
                    }
                }
                else
                {
                    Params.Add(value);
                }
            }

            _currentTagContents = string.Empty;
        }
    }
}