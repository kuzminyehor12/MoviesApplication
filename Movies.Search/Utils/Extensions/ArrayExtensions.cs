namespace Movies.Search.Utils.Extensions;

internal static class ArrayExtensions
{
    internal static (float Frequency, int[] Positions) FrequencyWithPositions<T>(this T[] array, T element)
    {
        if (array is null)
        {
            throw new ArgumentNullException(nameof(array));
        }

        if (element is null)
        {
            throw new ArgumentNullException(nameof(element));
        }
        
        List<int> indices = new List<int>();
        float frequency = 0;
        
        for (int i = 0; i < array.Length; i++)
        {
            if (array[i]?.Equals(element) ?? false) 
            {
                indices.Add(i);
                frequency++;
            }    
        }
        
        return (frequency, indices.ToArray());
    }
}