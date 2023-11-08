using GalleonLibrary;

Console.Write("Input: ");
var input = Console.ReadLine();
Console.WriteLine(input);

var tokens = LengthTokenizer.Tokenize(input!);
var parsed = LengthParser.ParseTokens(tokens);

if (!parsed.IsValid())
{
    foreach (var error in parsed.Errors)
    {
        Console.WriteLine(error);
    }
    Console.WriteLine();
    goto endProgram;
}

Console.WriteLine($"Meters: {LengthDisplay.DisplayMeters(parsed.LengthValues)}");
Console.WriteLine($"Millimeters: {LengthDisplay.DisplayMillimeters(parsed.LengthValues)}");
Console.WriteLine($"Imperial: {LengthDisplay.DisplayImperial(parsed.LengthValues)}");

endProgram:
Console.WriteLine("\nPress any key to exit...");
Console.ReadKey();
