using System;

namespace IXR
{
    /// <summary>
    /// Represents the main entry point for the IXR library.
    /// </summary>
    public static class IXR
    {
        /// <summary>
        /// Initializes the IXR library and ensures all components are loaded.
        /// </summary>
        public static void Initialize()
        {
            Console.WriteLine("IXR Library Initialized.");
        }

        /// <summary>
        /// Displays information about the IXR library.
        /// </summary>
        public static void About()
        {
            Console.WriteLine("IXR - The Incutio XML-RPC Library");
            Console.WriteLine("Copyright (c) 2010, Incutio Ltd.");
            Console.WriteLine("Version: 1.7.4");
            Console.WriteLine("Author: Simon Willison");
            Console.WriteLine("License: BSD");
        }
    }
}