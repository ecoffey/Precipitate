namespace Precipitate.Parsing
{
    public interface ISolutionParserFactory
    {
        ISolutionParser ForFile(string filePath);
    }
}