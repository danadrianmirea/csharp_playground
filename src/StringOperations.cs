using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace StringOperationsDemo
{
    public class StringOperations
    {
        public void Demo()
        {
            Console.WriteLine("=== C# String Operations Demo ===\n");
            
            // Sample strings for demonstration
            string originalText = "   Hello, World! Welcome to C# String Operations.   ";
            string mixedCaseText = "Hello World CSharp DotNet";
            string csvData = "John,Doe,30,New York";
            string htmlText = "<div>Hello <b>World</b></div>";
            string multilineText = "Line 1\nLine 2\nLine 3";
            string phoneNumber = "(123) 456-7890";
            string email = "user@example.com";
            string sentence = "The quick brown fox jumps over the lazy dog";
            
            // 1. Basic string operations
            Console.WriteLine("1. Basic String Operations:");
            Console.WriteLine($"Original: '{originalText}'");
            Console.WriteLine($"Trimmed: '{originalText.Trim()}'");
            Console.WriteLine($"Trim start: '{originalText.TrimStart()}'");
            Console.WriteLine($"Trim end: '{originalText.TrimEnd()}'");
            Console.WriteLine($"Length: {originalText.Length}");
            Console.WriteLine($"Upper case: {originalText.ToUpper()}");
            Console.WriteLine($"Lower case: {originalText.ToLower()}");
            
            // 2. String searching
            Console.WriteLine("\n2. String Searching:");
            Console.WriteLine($"Contains 'World': {originalText.Contains("World")}");
            Console.WriteLine($"Contains 'world' (case-insensitive): {originalText.Contains("world", StringComparison.OrdinalIgnoreCase)}");
            Console.WriteLine($"StartsWith '   Hello': {originalText.StartsWith("   Hello")}");
            Console.WriteLine($"EndsWith 'Operations.   ': {originalText.EndsWith("Operations.   ")}");
            Console.WriteLine($"IndexOf 'World': {originalText.IndexOf("World")}");
            Console.WriteLine($"LastIndexOf 'o': {originalText.LastIndexOf('o')}");
            
            // 3. String extraction
            Console.WriteLine("\n3. String Extraction:");
            Console.WriteLine($"Substring(3, 5): '{originalText.Substring(3, 5)}'");
            Console.WriteLine($"Remove(0, 3): '{originalText.Remove(0, 3)}'");
            
            // Find and extract "World"
            int worldIndex = originalText.IndexOf("World");
            if (worldIndex >= 0)
            {
                Console.WriteLine($"Extracted 'World' context: '{originalText.Substring(Math.Max(0, worldIndex - 5), Math.Min(15, originalText.Length - worldIndex + 5))}'");
            }
            
            // 4. String replacement
            Console.WriteLine("\n4. String Replacement:");
            Console.WriteLine($"Replace 'World' with 'Universe': {originalText.Replace("World", "Universe")}");
            Console.WriteLine($"Replace spaces with underscores: {originalText.Replace(" ", "_")}");
            Console.WriteLine($"Remove all spaces: {originalText.Replace(" ", "")}");
            
            // 5. String splitting and joining
            Console.WriteLine("\n5. String Splitting and Joining:");
            Console.WriteLine($"CSV data: {csvData}");
            string[] csvParts = csvData.Split(',');
            Console.WriteLine($"Split by comma: {string.Join(" | ", csvParts)}");
            
            string[] words = sentence.Split(' ');
            Console.WriteLine($"Sentence words: {string.Join(", ", words)}");
            Console.WriteLine($"Number of words: {words.Length}");
            
            // Join with different separator
            string joined = string.Join("-", words);
            Console.WriteLine($"Joined with hyphens: {joined}");
            
            // 6. String formatting
            Console.WriteLine("\n6. String Formatting:");
            string name = "John";
            int age = 30;
            double salary = 75000.50;
            
            // Traditional string.Format
            string formatted1 = string.Format("Name: {0}, Age: {1}, Salary: {2:C}", name, age, salary);
            Console.WriteLine($"string.Format: {formatted1}");
            
            // String interpolation (C# 6+)
            string formatted2 = $"Name: {name}, Age: {age}, Salary: {salary:C}";
            Console.WriteLine($"String interpolation: {formatted2}");
            
            // Composite formatting with alignment
            Console.WriteLine("\nFormatted table:");
            Console.WriteLine(string.Format("{0,-10} {1,5} {2,10:C}", "Name", "Age", "Salary"));
            Console.WriteLine(string.Format("{0,-10} {1,5} {2,10:C}", "John", 30, 75000.50));
            Console.WriteLine(string.Format("{0,-10} {1,5} {2,10:C}", "Jane", 28, 68000.75));
            Console.WriteLine(string.Format("{0,-10} {1,5} {2,10:C}", "Bob", 35, 82000.00));
            
            // 7. StringBuilder for efficient string manipulation
            Console.WriteLine("\n7. StringBuilder (for efficient string building):");
            StringBuilder sb = new StringBuilder();
            sb.Append("Hello");
            sb.Append(" ");
            sb.Append("World");
            sb.AppendLine("!");
            sb.AppendFormat("Today is {0:yyyy-MM-dd}", DateTime.Now);
            sb.AppendLine();
            sb.Append("This is line 3.");
            
            Console.WriteLine("StringBuilder content:");
            Console.WriteLine(sb.ToString());
            Console.WriteLine($"StringBuilder length: {sb.Length}");
            Console.WriteLine($"StringBuilder capacity: {sb.Capacity}");
            
            // Modify StringBuilder
            sb.Insert(0, "START: ");
            sb.Replace("World", "Universe");
            sb.Remove(0, 7); // Remove "START: "
            Console.WriteLine($"Modified: {sb.ToString()}");
            
            // 8. String comparison
            Console.WriteLine("\n8. String Comparison:");
            string str1 = "hello";
            string str2 = "HELLO";
            string str3 = "hello";
            
            Console.WriteLine($"str1 = '{str1}', str2 = '{str2}', str3 = '{str3}'");
            Console.WriteLine($"str1 == str2: {str1 == str2}");
            Console.WriteLine($"str1.Equals(str2): {str1.Equals(str2)}");
            Console.WriteLine($"str1.Equals(str2, StringComparison.OrdinalIgnoreCase): {str1.Equals(str2, StringComparison.OrdinalIgnoreCase)}");
            Console.WriteLine($"str1.CompareTo(str2): {str1.CompareTo(str2)}");
            Console.WriteLine($"string.Compare(str1, str2, StringComparison.OrdinalIgnoreCase): {string.Compare(str1, str2, StringComparison.OrdinalIgnoreCase)}");
            
            // 9. Character operations
            Console.WriteLine("\n9. Character Operations:");
            string text = "Hello123";
            Console.WriteLine($"Text: {text}");
            
            foreach (char c in text)
            {
                Console.WriteLine($"  '{c}' - IsLetter: {char.IsLetter(c)}, IsDigit: {char.IsDigit(c)}, IsWhiteSpace: {char.IsWhiteSpace(c)}, Upper: {char.ToUpper(c)}");
            }
            
            // Convert string to char array and back
            char[] charArray = text.ToCharArray();
            Array.Reverse(charArray);
            string reversed = new string(charArray);
            Console.WriteLine($"Reversed: {reversed}");
            
            // 10. Regular expressions
            Console.WriteLine("\n10. Regular Expressions:");
            
            // Email validation
            string emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            bool isValidEmail = Regex.IsMatch(email, emailPattern);
            Console.WriteLine($"Email '{email}' is valid: {isValidEmail}");
            
            // Extract numbers
            string textWithNumbers = "Order 12345 with price $99.99 and tax $9.99";
            string numberPattern = @"\d+\.?\d*";
            MatchCollection numberMatches = Regex.Matches(textWithNumbers, numberPattern);
            Console.WriteLine($"Text: {textWithNumbers}");
            Console.WriteLine("Numbers found:");
            foreach (Match match in numberMatches)
            {
                Console.WriteLine($"  {match.Value}");
            }
            
            // Phone number formatting
            string phonePattern = @"\((\d{3})\)\s*(\d{3})-(\d{4})";
            string phoneFormat = "$1-$2-$3";
            string formattedPhone = Regex.Replace(phoneNumber, phonePattern, phoneFormat);
            Console.WriteLine($"Phone '{phoneNumber}' formatted: {formattedPhone}");
            
            // HTML tag removal
            string htmlPattern = @"<[^>]*>";
            string plainText = Regex.Replace(htmlText, htmlPattern, "");
            Console.WriteLine($"HTML: {htmlText}");
            Console.WriteLine($"Plain text: {plainText}");
            
            // 11. String encoding
            Console.WriteLine("\n11. String Encoding:");
            string unicodeText = "Hello 世界";
            Console.WriteLine($"Original: {unicodeText}");
            
            byte[] utf8Bytes = Encoding.UTF8.GetBytes(unicodeText);
            Console.WriteLine($"UTF-8 bytes: {BitConverter.ToString(utf8Bytes)}");
            Console.WriteLine($"UTF-8 string from bytes: {Encoding.UTF8.GetString(utf8Bytes)}");
            
            // Different encodings
            byte[] asciiBytes = Encoding.ASCII.GetBytes("Hello");
            byte[] unicodeBytes = Encoding.Unicode.GetBytes("Hello");
            Console.WriteLine($"ASCII bytes for 'Hello': {BitConverter.ToString(asciiBytes)}");
            Console.WriteLine($"Unicode bytes for 'Hello': {BitConverter.ToString(unicodeBytes)}");
            
            // 12. Multiline and verbatim strings
            Console.WriteLine("\n12. Multiline and Verbatim Strings:");
            string multiline = "Line 1\nLine 2\nLine 3";
            Console.WriteLine("Regular multiline string:");
            Console.WriteLine(multiline);
            
            string verbatim = @"Line 1
Line 2
Line 3
Path: C:\Users\Name\Documents";
            Console.WriteLine("\nVerbatim string (preserves line breaks and doesn't escape):");
            Console.WriteLine(verbatim);
            
            // 13. String performance demonstration
            Console.WriteLine("\n13. String Performance Comparison:");
            Console.WriteLine("Building a string with 10,000 'a' characters:");
            
            // Inefficient way (creates new string each time)
            DateTime start = DateTime.Now;
            string inefficient = "";
            for (int i = 0; i < 10000; i++)
            {
                inefficient += "a";
            }
            TimeSpan inefficientTime = DateTime.Now - start;
            Console.WriteLine($"  String concatenation: {inefficientTime.TotalMilliseconds:F2} ms");
            
            // Efficient way with StringBuilder
            start = DateTime.Now;
            StringBuilder efficientBuilder = new StringBuilder();
            for (int i = 0; i < 10000; i++)
            {
                efficientBuilder.Append("a");
            }
            string efficient = efficientBuilder.ToString();
            TimeSpan efficientTime = DateTime.Now - start;
            Console.WriteLine($"  StringBuilder: {efficientTime.TotalMilliseconds:F2} ms");
            Console.WriteLine($"  StringBuilder is {inefficientTime.TotalMilliseconds / efficientTime.TotalMilliseconds:F1}x faster");
            
            // 14. Common string utility methods
            Console.WriteLine("\n14. Common String Utility Methods:");
            
            // IsNullOrEmpty and IsNullOrWhiteSpace
            string emptyString = "";
            string whitespaceString = "   ";
            string nullString = null;
            
            Console.WriteLine($"emptyString is null or empty: {string.IsNullOrEmpty(emptyString)}");
            Console.WriteLine($"whitespaceString is null or whitespace: {string.IsNullOrWhiteSpace(whitespaceString)}");
            Console.WriteLine($"nullString is null or empty: {string.IsNullOrEmpty(nullString)}");
            
            // Padding
            string paddedLeft = "42".PadLeft(5, '0');
            string paddedRight = "42".PadRight(5, '*');
            Console.WriteLine($"PadLeft: '{paddedLeft}'");
            Console.WriteLine($"PadRight: '{paddedRight}'");
            
            // 15. Custom string operations
            Console.WriteLine("\n15. Custom String Operations:");
            
            string result = ReverseString("Hello World");
            Console.WriteLine($"ReverseString('Hello World'): {result}");
            
            int vowelCount = CountVowels("Hello World");
            Console.WriteLine($"CountVowels('Hello World'): {vowelCount}");
            
            bool isPalindrome = IsPalindrome("racecar");
            Console.WriteLine($"IsPalindrome('racecar'): {isPalindrome}");
            Console.WriteLine($"IsPalindrome('hello'): {IsPalindrome("hello")}");
            
            string initials = GetInitials("John Fitzgerald Kennedy");
            Console.WriteLine($"GetInitials('John Fitzgerald Kennedy'): {initials}");
            
            Console.WriteLine("\n=== String Operations Demo Complete ===");
        }
        
        // Custom string utility methods
        
        public string ReverseString(string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;
                
            char[] charArray = input.ToCharArray();
            Array.Reverse(charArray);
            return new string(charArray);
        }
        
        public int CountVowels(string input)
        {
            if (string.IsNullOrEmpty(input))
                return 0;
                
            int count = 0;
            string vowels = "aeiouAEIOU";
            
            foreach (char c in input)
            {
                if (vowels.IndexOf(c) >= 0)
                    count++;
            }
            
            return count;
        }
        
        public bool IsPalindrome(string input)
        {
            if (string.IsNullOrEmpty(input))
                return true;
                
            // Remove non-alphanumeric characters and convert to lowercase
            string cleaned = Regex.Replace(input, @"[^a-zA-Z0-9]", "").ToLower();
            
            // Compare with reverse
            char[] charArray = cleaned.ToCharArray();
            Array.Reverse(charArray);
            string reversed = new string(charArray);
            
            return cleaned == reversed;
        }
        
        public string GetInitials(string fullName)
        {
            if (string.IsNullOrWhiteSpace(fullName))
                return "";
                
            string[] names = fullName.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            StringBuilder initials = new StringBuilder();
            
            foreach (string name in names)
            {
                if (name.Length > 0)
                    initials.Append(name[0]);
            }
            
            return initials.ToString().ToUpper();
        }
        
        public string TruncateWithEllipsis(string input, int maxLength)
        {
            if (string.IsNullOrEmpty(input) || input.Length <= maxLength)
                return input;
                
            if (maxLength <= 3)
                return input.Substring(0, maxLength);
                
            return input.Substring(0, maxLength - 3) + "...";
        }
        
        public Dictionary<char, int> CharacterFrequency(string input)
        {
            Dictionary<char, int> frequency = new Dictionary<char, int>();
            
            foreach (char c in input)
            {
                if (frequency.ContainsKey(c))
                    frequency[c]++;
                else
                    frequency[c] = 1;
            }
            
            return frequency;
        }
        
        public string RemoveDuplicateCharacters(string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;
                
            HashSet<char> seen = new HashSet<char>();
            StringBuilder result = new StringBuilder();
            
            foreach (char c in input)
            {
                if (!seen.Contains(c))
                {
                    seen.Add(c);
                    result.Append(c);
                }
            }
            
            return result.ToString();
        }
    }
}