using System.Collections.Generic;
using UnityEngine;

public struct GridOccupancyComponent : IComponent
{
    public List<Vector2Int> occupiedTiles;

    public GridOccupancyComponent(List<Vector2Int> occupiedTiles) => this.occupiedTiles = occupiedTiles;
}
