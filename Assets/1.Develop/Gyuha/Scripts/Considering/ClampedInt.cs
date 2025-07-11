using UnityEngine;

public class ClampedInt : ClampedValue<int>
{
    public ClampedInt(int min, int max, int initial)
        : base(min, max, initial) { }
    
    public override float Ratio => (float)(Current - Min) / (Max - Min);
    
    protected override int Add(int a, int b) => a + b;
    protected override int Subtract(int a, int b) => a - b;
    protected override int Clamp(int value, int min, int max) => Mathf.Clamp(value, min, max);
    
    public static ClampedInt operator +(ClampedInt a, int b)
    {
        return new ClampedInt(a.Min, a.Max, a.Current + b);
    }
    
    public static ClampedInt operator -(ClampedInt a, int b)
    {
        return new ClampedInt(a.Min, a.Max, a.Current - b);
    }
}