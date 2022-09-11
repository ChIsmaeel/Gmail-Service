namespace Task1;
using System;
using System.Text;

internal class DummyContentGenerator
{
    private static Random _random = new Random();
    public static string Generate()
    {
        var sb = new StringBuilder();
        sb.Append("Name: ");

        // First Letter Capital
        sb.Append(RandomString(1));

        // Add remaining Letter in lower case
        sb.Append(RandomString(RandomInt(2, 20), true));

        sb.Append(" ");

        // First Letter Capital
        sb.Append(RandomString(1));

        // Add remaining Letter in lower case
        sb.AppendLine(RandomString(RandomInt(2, 20), true));

        sb.Append("Age: ");

        sb.Append(RandomInt(20, 100));


        return sb.ToString();

    }

    // Generates a random string with a given size.    
    public static string RandomString(int size, bool lowerCase = false)
    {
        var builder = new StringBuilder(size);

        // Unicode/ASCII Letters are divided into two blocks
        // (Letters 65–90 / 97–122):   
        // The first group containing the uppercase letters and
        // the second group containing the lowercase.  

        // char is a single Unicode character  
        char offset = lowerCase ? 'a' : 'A';
        const int lettersOffset = 26; // A...Z or a..z: length = 26  

        for (var i = 0; i < size; i++)
        {
            var @char = (char)_random.Next(offset, offset + lettersOffset);
            builder.Append(@char);
        }

        return lowerCase ? builder.ToString().ToLower() : builder.ToString();
    }

    public static int RandomInt(int min, int max)
    {
        return _random.Next(min, max);
    }
}
