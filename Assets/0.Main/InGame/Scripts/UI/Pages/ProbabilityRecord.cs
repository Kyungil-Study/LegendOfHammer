public struct ProbabilityRecord<T>
{
    public T ID;
    public int minProbability; // 확률
    public int maxProbability;
        
    public ProbabilityRecord(T id, int min, int max)
    {
        ID = id;
        minProbability = min;
        maxProbability = max;
    }
        
    public bool IsInRange(int value)
    {
        // Check if the value is within the range of minProbability and maxProbability
        return value >= minProbability && value <= maxProbability;
    }
}

