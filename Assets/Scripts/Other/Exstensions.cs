using System;

/// <summary>
/// This class will hold all extensions for this project
/// </summary>
public static class Extensions
{
    /// <summary>
    /// Method to try and parse a string to a float
    /// </summary>
    /// <param name="val"></param>
    /// <param name="parsedVal"></param>
    /// <returns>If it is succesfull</returns>
    public static bool TryParseFloat(this string val, out float parsedVal)
    {
        try
        {
            parsedVal = Convert.ToSingle(val);

            return true;
        }
        catch (Exception)
        {
            parsedVal = 0;
            return false;
        }
    }

    /// <summary>
    /// Method to try and parse a string to an int
    /// </summary>
    /// <param name="val"></param>
    /// <param name="parsedVal"></param>
    /// <returns>If it is succesfull</returns>
    public static bool TryParseInt(this string val, out int parsedVal)
    {
        try
        {
            parsedVal = Convert.ToInt32(val);

            return true;
        }
        catch (Exception)
        {
            parsedVal = 0;
            return false;
        }
    }
}