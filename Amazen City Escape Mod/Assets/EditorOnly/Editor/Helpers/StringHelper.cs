using System.IO;

public class StringHelper
{
    public static string RemoveInvalidFileNameChars(string str)
        => string.Concat(str.Split(Path.GetInvalidFileNameChars()));
}