// This allow the use of StringEnumConverter to convert enum values to string values in the inspector
using System;

[AttributeUsage(AttributeTargets.Field)]
public class StringValueAttribute : Attribute
{
    public string Value { get; }
    public StringValueAttribute(string value) => Value = value;
}
