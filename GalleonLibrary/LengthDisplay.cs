namespace GalleonLibrary;

public static class LengthDisplay
{
    private static readonly Dictionary<LengthUnit, double> UnitMultipliers = new Dictionary<LengthUnit, double>
    {
        { LengthUnit.SixtyFourthOfInch, 1 },
        { LengthUnit.Feet, 768 },
        { LengthUnit.Inches, 64 },
        { LengthUnit.Metres, 2_519.6864 },
        { LengthUnit.Centimetres, 25.196864 },
        { LengthUnit.Millimetres, 2.5196854 },
    };

    public static double ConvertToBaseUnit(List<LengthValue> lengthValues)
    {
        var sum = lengthValues.Aggregate(0.0, (sum, next) => sum + (UnitMultipliers[next.Unit] * next.Value));
        return sum;
    }

    public static string DisplayMeters(List<LengthValue> lengthValues)
    {
        var baseValue = ConvertToBaseUnit(lengthValues);

        var meters = baseValue / UnitMultipliers[LengthUnit.Metres];
        return $"{meters:0.000}m";
    }

    public static string DisplayMillimeters(List<LengthValue> lengthValues)
    {
        var baseValue = ConvertToBaseUnit(lengthValues);
        var millimeters = baseValue / UnitMultipliers[LengthUnit.Millimetres];
        return $"{millimeters:0.000}mm";
    }

    public static string DisplayImperial(List<LengthValue> lengthValues)
    {
        var baseValue = ConvertToBaseUnit(lengthValues);

        var feet = Math.Floor(baseValue / UnitMultipliers[LengthUnit.Feet]);
        var remaining = baseValue - (feet * UnitMultipliers[LengthUnit.Feet]);

        var inches = Math.Floor(remaining / UnitMultipliers[LengthUnit.Inches]);
        var remainingFractional = remaining - (inches * UnitMultipliers[LengthUnit.Inches]);

        var displayString = "";
        if (feet > 0)
        {
            displayString += $"{feet}' ";
        }

        if (inches > 0)
        {
            displayString += $"{inches} ";
        }

        (int, int) fractionOfInch = SimplifyToPowerOfTwo((int)remainingFractional, 64);

        if (fractionOfInch.Item1 > 0 && fractionOfInch.Item2 > 0)
        {
            displayString += $"{fractionOfInch.Item1}/{fractionOfInch.Item2}";
        }

        if (inches > 0 || (fractionOfInch.Item1 > 0 && fractionOfInch.Item2 > 0))
        {
            displayString += "\"";
        }
        
        return displayString.Trim();
    }

    internal static int FindGreatestCommonDenominator(int a, int b)
    {
        while (b != 0)
        {
            int temp = b;
            b = a % b;
            a = temp;
        }
        return a;
    }
    
    // Simplify the fraction and ensure the denominator is 2, 4, 8, 16, 32, or 64
    public static (int numerator, int denominator) SimplifyToPowerOfTwo(int numerator, int denominator)
    {
        // Find the GCD
        int gcd = FindGreatestCommonDenominator(numerator, denominator);

        // Simplify the fraction
        numerator /= gcd;
        denominator /= gcd;

        // Ensure the denominator is a power of two and does not exceed 64
        while (denominator > 64 || (denominator & (denominator - 1)) != 0)
        {
            gcd = FindGreatestCommonDenominator(numerator, denominator);
            if (gcd == 1) break; // Can't simplify further

            numerator /= gcd;
            denominator /= gcd;
        }

        return (numerator, denominator);
    }
}