namespace MultipleApiRequester.UnitTests;

public static class Extensions
{
    public static bool ContainsAll(this string source, params string[] parts) => parts.All(x => source.Contains(x));
}