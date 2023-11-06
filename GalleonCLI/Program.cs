using GalleonLibrary;

var test = "2Feet 4 1/64\"";
Console.WriteLine("Input: " + test);

var tokens = LengthTokenizer.Tokenize(test);
foreach (var token in tokens)
{
    Console.WriteLine("Token:" + token);
}