using System;

public interface IModifierInfo
{
    object Source { get; }
    string Tag { get; }
    string Identifier { get; }
}