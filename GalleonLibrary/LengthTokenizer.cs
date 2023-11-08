namespace GalleonLibrary;
public static class LengthTokenizer
{
    private static bool IsNumberOrSlash(char c)
    {
        return char.IsDigit(c) || c == '/' || c == '\\' || c == '.';
    }

    private static bool IsSameType(char c1, char c2)
    {
        var c1IsNum = IsNumberOrSlash(c1);
        var c2IsNum = IsNumberOrSlash(c2);

        return c1IsNum == c2IsNum;
    }

    private static bool IsCurrentCharAndLastCharSame(char c, string currentToken)
    {
        if (string.IsNullOrEmpty(currentToken))
        {
            return false;
        }

        return IsSameType(c, currentToken.Last());
    }

    public static List<string> Tokenize(string input)
    {
        input = input.ToLower().Trim();
        var tokens = new List<string>();
        string currentToken = "";

        foreach (var c in input)
        {
            if (char.IsWhiteSpace(c) || !IsCurrentCharAndLastCharSame(c, currentToken))
            {
                if (!string.IsNullOrEmpty(currentToken))
                {
                    tokens.Add(currentToken);
                    currentToken = string.Empty;
                }

                if (char.IsWhiteSpace(c))
                {
                    continue;
                }
            }



            if (char.IsDigit(c) || c == '/' || c == '\\' || char.IsLetter(c) || c == '"' || c == '\'' || c == '.') 
            {
                currentToken += c;
            }
        }

        if (!string.IsNullOrEmpty(currentToken))
        {
            tokens.Add(currentToken);
        }

        return tokens;
    }
}
