namespace Jay
{
    public abstract class StatValue<T> where T : struct
    {
        protected T baseValue;

        protected ModifiableValue<T> additive;
        protected ModifiableValue<float> multiplicative;

        public T BaseValue => baseValue;
        public ModifiableValue<T> Additive => additive;
        public ModifiableValue<float> Multiplicative => multiplicative;

        public abstract T Value { get; }
        public abstract T MultiplicativeAmount { get; }
        
        protected StatValue(T initBase)
        {
            baseValue = initBase;
            additive = new ModifiableValue<T>(default);
            multiplicative = new ModifiableValue<float>(0f);
        }

    }

    public class StatValueInt : StatValue<int>
    {
        public override int Value => (int)((BaseValue + Additive.Value) * (1f + Multiplicative.Value));
        public override int MultiplicativeAmount => (int)((BaseValue + Additive.Value) * Multiplicative.Value);

        public StatValueInt(int initValue) : base(initValue) { }
    }

    public class StatValueFloat : StatValue<float>
    {
        public override float Value => (BaseValue + Additive.Value) * (1 + Multiplicative.Value);
        public override float MultiplicativeAmount => (BaseValue + Additive.Value) * (Multiplicative.Value);
        
        public StatValueFloat(float initValue) : base(initValue) { }
    }
}