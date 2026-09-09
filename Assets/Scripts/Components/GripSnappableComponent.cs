using UnityEngine;

public struct GridSnappableComponent : IComponent
{
    public GridSnapper Snapper { get; set; } // The object that is snapped to the grid
    public GridSnappableComponent(GridSnapper snapper) => Snapper = snapper;
}
