using System;
using System.Collections.Generic;
using System.Linq;

namespace Jay
{
    public class ModifiableValue<T> where T : struct
    {
        public T BaseValue { get; private set; }
        public T Value => GetValue();
    
        public IEnumerable<IValueModifier<T>> GetAllModifiers() => modifiers;
    
        private readonly List<IValueModifier<T>> modifiers = new();
    
        private T cachedValue;
        private bool isDirty;
    
        public ModifiableValue(T baseValue)
        {
            BaseValue = baseValue;
            isDirty = true;
        }
    
        public void SetBase(T newValue)
        {
            if (EqualityComparer<T>.Default.Equals(BaseValue, newValue)) return;
            
            BaseValue = newValue;
            isDirty = true;
        }
    
        private T GetValue()
        {
            if (isDirty == false) return cachedValue;
    
            cachedValue = Recalculate();
            isDirty = false;
            return cachedValue;
        }
    
        private T Recalculate()
        {
            return modifiers.Aggregate(BaseValue, (current, modifier) => modifier.Apply(current));
        }
    
        public void AddModifier(IValueModifier<T> modifier)
        {
            modifiers.Add(modifier);
            isDirty = true;
        }
    
        public void RemoveModifier(IValueModifier<T> modifier)
        {
            modifiers.Remove(modifier);
            isDirty = true;
        }
    
        public void RemoveModifiersBySource(object source)
        {
            modifiers.RemoveAll(m => m.Source.Equals(source));
            isDirty = true;
        }
    
        public void ClearModifiers()
        {
            modifiers.Clear();
            isDirty = true;
        }
    }
}

