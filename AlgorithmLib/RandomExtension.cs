namespace AlgorithmLib;

public static class RandomExtension
{
    public static double GenerateDouble(this Random random, double min, double max, int accuracy = 2) =>
        Math.Round(random.NextDouble() * (max - min) + min, accuracy);
}