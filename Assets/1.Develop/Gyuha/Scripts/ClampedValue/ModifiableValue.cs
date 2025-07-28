using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IModifiableValue<T> where T : struct
{
    T Value { get; set; }
    List<IModifier<T>> Modifiers { get; }
}

public interface IModifier<T> where T : struct
{
    T Value { get; }
    T Apply(T value);
}

public class ModifiableValue<T> : IModifiableValue<T> where T : struct 
{
    public T Value { get; set; }
    public List<IModifier<T>> Modifiers { get; } = new();
}
