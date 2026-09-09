using System;
using UnityEngine;

[Serializable]
public struct BuildingId
{
    public string Value;

    public BuildingId(string value) => Value = value;

    public static bool operator ==(BuildingId a, BuildingId b) => a.Value == b.Value;
    public static bool operator !=(BuildingId a, BuildingId b) => a.Value != b.Value;

    public override readonly bool Equals(object obj)
        => obj is BuildingId other && this == other;

    public override readonly int GetHashCode()
        => Value.GetHashCode();

    public override readonly string ToString()
        => Value;
}

[CreateAssetMenu(fileName = "BuildingDefinition", menuName = "Scriptable Objects/BuildingDefinition")]
public class BuildingDefinition : ScriptableObject
{
    public BuildingId Id;
    public BuildingType BuildingType;
    public GameObject BuildingPrefab;
    public GameObject BuildingShadow;
    public Sprite BuildingIcon;
    public Vector2Int[] GridOccupancy;
}
