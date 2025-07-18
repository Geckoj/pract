public static class StringExtensions
{
    public static bool IsPalindrome(this string input)
    {
        if (input == null)
            return false;

        string normalized = new string(
            input.ToLower()
            .Where(c => !char.IsWhiteSpace(c) && !char.IsPunctuation(c))
            .ToArray()
        );
        if (normalized.Length == 0)
            return false;

        string reversed = new string(normalized.Reverse().ToArray());

        return normalized == reversed;
    }
}