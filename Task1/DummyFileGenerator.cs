namespace Task1;
public record InMemoryFileInfo(string FileName, string Content);

internal class DummyFileGenerator
{
    public static InMemoryFileInfo Generate()
    {
        var currentTimeStamp = DateTime.Now;

        var fileName = currentTimeStamp.ToString("MMddyyyy_hhmmss");

        return new(fileName + ".txt", DummyContentGenerator.Generate());
    }
}

