using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public class WP_Text_Diff_Renderer_Inline : Text_Diff_Renderer_Inline
{
    /// <summary>
    /// Splits the input string into words based on non-word characters.
    /// </summary>
    /// <param name="input">The input string to split.</param>
    /// <param name="newlineEscape">The escape sequence for newline characters.</param>
    /// <returns>A list of words and delimiters.</returns>
    public List<string> SplitOnWords(string input, string newlineEscape = "\n")
    {
        if (string.IsNullOrEmpty(input))
        {
            return new List<string>();
        }

        // Remove null characters
        input = input.Replace("\0", "");

        // Split the string into words and delimiters using a regex pattern
        var words = Regex.Split(input, @"([^\w])");

        // Replace newline characters with the specified escape sequence
        for (int i = 0; i < words.Length; i++)
        {
            words[i] = words[i].Replace("\n", newlineEscape);
        }

        return new List<string>(words);
    }
}