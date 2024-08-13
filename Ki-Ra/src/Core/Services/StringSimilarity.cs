namespace Ki_Ra.src.Core.Services
{
    public static class StringSimilarity
    {
        public static double CalculateSimilarity(string string1, string string2)
        {
            int distance = ComputeLevenshteinDistance(string1, string2);
            int maxLength = Math.Max(string1.Length, string2.Length);
            return (1.0 - ((double)distance / maxLength)) * 100;
        }
        private static int ComputeLevenshteinDistance(string string1, string string2)
        {
            int[,] distance = new int[string1.Length + 1, string2.Length + 1];
            for (int i = 0; i <= string1.Length; i++)
                distance[i, 0] = i;
            for (int j = 0; j <= string2.Length; j++)
                distance[0, j] = j;
            for (int i = 1; i <= string1.Length; i++)
            {
                for (int j = 1; j <= string2.Length; j++)
                {
                    int cost = (string2[j - 1] == string1[i - 1]) ? 0 : 1;
                    distance[i, j] = Math.Min(
                        Math.Min(distance[i - 1, j] + 1, distance[i, j - 1] + 1),
                        distance[i - 1, j - 1] + cost);
                }
            }
            return distance[string1.Length, string2.Length];
        }
    }
}
