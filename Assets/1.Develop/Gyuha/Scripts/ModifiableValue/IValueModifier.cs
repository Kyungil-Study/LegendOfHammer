using System;

namespace Jay
{
    public interface IValueModifier<T> : IModifierInfo
    {
        T Value { get; }
        T Apply(T baseValue);
    }

    public class FloatModifier : IValueModifier<float>
    {
        public object Source { get; set; }
        public string Tag { get; set; }
        public string Identifier { get; set; }
        public float Value { get; set; }

        public virtual float Apply(float baseValue)
        {
            return baseValue + Value;
        }

        public FloatModifier(object source, string tag, string identifier, float value)
        {
            Source = source;
            Tag = tag;
            Identifier = identifier;
            Value = value;
        }
    }
    
    public class AdditiveFloat : FloatModifier
    {
        public AdditiveFloat(object source, string tag, string identifier, float value) : base(source, tag, identifier, value)
        {
            
        }
    }
    
    public class PercentBonusModifier : FloatModifier
    {
        public PercentBonusModifier(object source, string tag, string identifier, float value) : base(source, tag, identifier, value)
        {
            
        }
    }
}

