using System;

namespace task01;

public static class StringExtensions
{
    public static bool IsPalindrome(this string input)
    {
        if (string.IsNullOrWhiteSpace(input)) return false;

        ReadOnlySpan<char> span = input.AsSpan();
        Span<char> cleaned = stackalloc char[span.Length];
        int cleanedLength = 0;

        foreach (char c in span)
        {
            if (char.IsLetterOrDigit(c))
            {
                cleaned[cleanedLength++] = char.ToLowerInvariant(c);
            }
        }

        if (cleanedLength == 0) return false;

        int left = 0;
        int right = cleanedLength - 1;

        while (left < right)
        {
            if (cleaned[left] != cleaned[right]) return false;
            left++;
            right--;
        }

        return true;
    }
}
