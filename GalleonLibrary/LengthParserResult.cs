namespace GalleonLibrary;

public class LengthParserResult
{
    public List<LengthValue> LengthValues { get; init; } = null!;
    public List<string> Errors { get; init; } = null!;

    public bool IsValid()
    {
        return Errors.Count == 0;
    }
}
