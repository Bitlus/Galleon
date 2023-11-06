namespace GalleonLibrary;

public static class LengthParser
{
    private static readonly List<double> ValidDenominators = new() { 2, 4, 8, 16, 32, 64 };
    private static readonly Dictionary<string, LengthUnit> LengthUnitMap = new Dictionary<string, LengthUnit>
    {
        { "\"", LengthUnit.Inches },
        { "in", LengthUnit.Inches },
        { "inches", LengthUnit.Inches },
        { "'", LengthUnit.Feet },
        { "feet", LengthUnit.Feet },
        { "ft", LengthUnit.Feet },
        { "mm", LengthUnit.Millimetres },
        { "millimetres", LengthUnit.Millimetres },
        { "millimeters", LengthUnit.Millimetres },
        { "cm", LengthUnit.Centimetres },
        { "centimeters", LengthUnit.Centimetres },
        { "m", LengthUnit.Metres },
        { "metres", LengthUnit.Metres },
        { "meters", LengthUnit.Metres },
    };
    
    private static readonly Dictionary<LengthUnit, UnitSystem> UnitSystemMap = new Dictionary<LengthUnit, UnitSystem>
    {
        { LengthUnit.Inches, UnitSystem.Imperial },
        { LengthUnit.Feet, UnitSystem.Imperial },
        { LengthUnit.Millimetres, UnitSystem.Metric },
        { LengthUnit.Centimetres, UnitSystem.Metric },
        { LengthUnit.Metres, UnitSystem.Metric },
    };

    private static bool IsFractionalToken(string token)
    {
       return token.Contains('/') || token.Contains('\\');
    }

    private static List<(string, string)> GenerateTokenPairs(List<string> tokens)
    {
        var tokenPairs = new List<(string, string)>();
        for (int i = 0; i < tokens.Count; i+=2)
        {
            tokenPairs.Add((tokens[i], tokens[i + 1]));
        }
        return tokenPairs;
    }

    private static bool IsAllSameUnitSystem(List<NonFractionalParseResult> results)
    {
        if (results.Count == 0)
        {
            return true;
        }

        var firstSystem = results.First().LengthValue.System;
        foreach(var result in results)
        {
            if (result.LengthValue.System != firstSystem)
            {
                return false;
            }
        }

        return true;
    }

    private static FractionalParseResult ParseFractionalToken(string token)
    {
        var splitter = token.Contains('/') ? "/" : "\\";
        var tokens = token.Split(splitter);
        if (tokens.Length != 2)
        {
            return new FractionalParseResult(0, 0, false);
        }

        var numeratorResult = Double.TryParse(tokens[0], out var numerator);
        var denominatorResult = Double.TryParse(tokens[1], out var denominator);

        if (!numeratorResult || !denominatorResult)
        {
            return new FractionalParseResult(0, 0, false);
        }

        if (!ValidDenominators.Contains(denominator)) 
        {
            return new FractionalParseResult(numerator, denominator, false);
        }

        return new FractionalParseResult(numerator, denominator, true);
    }

    private static NonFractionalParseResult ParseNonFractionalToken((string, string) tokenPair)
    {
        var valueResult = Double.TryParse(tokenPair.Item1, out var value);
        
        var unitString = tokenPair.Item2;
        var containsKey = LengthUnitMap.ContainsKey(unitString);

        if (!containsKey || !valueResult)
        {
            return new NonFractionalParseResult(new LengthValue(UnitSystem.Imperial, LengthUnit.Inches, 0.0), false);
        }

        var unit = LengthUnitMap[unitString];
        var system = UnitSystemMap[unit];

        return new NonFractionalParseResult(new LengthValue(system, unit, value), true);
    }

    private static LengthValue MapFractionalParseResultToLengthValue(FractionalParseResult result)
    {
        var multiplier = result.Denominator switch
        {
            2 => 32,
            4 => 16,
            8 => 8,
            16 => 4,
            32 => 2,
            64 => 1,
            _ => 0,
        };

        var newNumerator = result.Numerator * multiplier;

        return new LengthValue(UnitSystem.Imperial, LengthUnit.FractionOfInch, newNumerator);
    }
        
    public static LengthParserResult ParseTokens(List<string> tokens)
    {
        var errors = new List<string>();
        var lengthValues = new List<LengthValue>();

        var fractionalTokens = tokens.Where(IsFractionalToken).ToList();
        var nonFractionalTokens = tokens.Where(token => !IsFractionalToken(token)).ToList();

        if (nonFractionalTokens.Count() % 2 != 0)
        {
            errors.Add("Not all units/values have an associated unit/value");
            return new LengthParserResult
            {
                LengthValues = lengthValues,
                Errors = errors,
            };
        }

        var parsedFractionalTokens = fractionalTokens.Select(ParseFractionalToken);

        var tokenPairs = GenerateTokenPairs(nonFractionalTokens);
        var parsedNonFractionalTokens = tokenPairs.Select(ParseNonFractionalToken);

        var validFractionalResults = parsedFractionalTokens.Where(result => result.IsValid).ToList();
        var validNonFractionalResults = parsedNonFractionalTokens.Where(result => result.IsValid).ToList();

        if (!IsAllSameUnitSystem(validNonFractionalResults))
        {
            errors.Add("There is a mix of imperial and metric units");
        }

        var nonFractionalLengthValues = validNonFractionalResults.Select(result => result.LengthValue).ToList();
        var fractionalLengthValues = validFractionalResults.Select(MapFractionalParseResultToLengthValue).ToList();

        lengthValues.AddRange(nonFractionalLengthValues);
        lengthValues.AddRange(fractionalLengthValues);

        return new LengthParserResult
        {
            LengthValues = lengthValues,
            Errors = errors,
        };
    }
}

internal readonly record struct FractionalParseResult(double Numerator, double Denominator, bool IsValid);
internal readonly record struct NonFractionalParseResult(LengthValue LengthValue, bool IsValid);
