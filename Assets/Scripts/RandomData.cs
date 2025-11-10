using UnityEngine;

public static class RandomData
{
    /// <summary>
    /// Returns TRUE a % of the time.
    /// Example: TestChance(10) = roughly 1 in 10 chance (10%).
    /// </summary>
    public static bool TestChance(int percent)
    {
        percent = Mathf.Clamp(percent, 0, 100);
        return Random.Range(0, 100) < percent;
    }

    /// <summary>
    /// Returns a random float between min and max.
    /// </summary>
    public static float Range(float min, float max)
    {
        return Random.Range(min, max);
    }

    /// <summary>
    /// Returns a random int between min (inclusive) and max (exclusive).
    /// </summary>
    public static int Range(int min, int max)
    {
        return Random.Range(min, max);
    }

    /// <summary>
    /// Returns +1 or -1 randomly.
    /// </summary>
    public static int PlusOrMinus()
    {
        return Random.Range(0, 2) == 0 ? -1 : 1;
    }
}
