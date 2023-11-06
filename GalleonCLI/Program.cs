using GalleonLibrary;

var test = "2Feet 4 1/64\"";
Console.WriteLine("Input: " + test);

var tokens = LengthTokenizer.Tokenize(test);

Console.Write("Tokens: ");
foreach (var token in tokens)
{
    Console.Write(token + " ");
}
Console.WriteLine("\n");

var parsedResult = LengthParser.ParseTokens(tokens);

foreach(var lengthValue in parsedResult.LengthValues)
{
    Console.WriteLine(lengthValue.System);
    Console.WriteLine(lengthValue.Unit);
    Console.WriteLine(lengthValue.Value);
    Console.WriteLine();
}