using System.Collections.Generic;
using UnityEngine;

public static class GridUtils
{
    public static float tileWidth = 1.0f; // Width of a single tile in world units
    public static float tileHeight = 0.5f; // Height of a single tile in world units. 1:2 to width ratio for isometric tiles
    public static Vector2 WorldToGrid(Vector2 worldPosition, bool roundToNearest = true)
    {
        float x = (worldPosition.x / tileWidth) + (worldPosition.y / tileHeight);
        float y = (worldPosition.y / tileHeight) - (worldPosition.x / tileWidth);
        if (roundToNearest)
        {
            x = Mathf.Floor(x);
            y = Mathf.Floor(y);
        }
        return new Vector2(x, y);
    }

    public static Vector2 GridToWorld(Vector2 gridPosition)
    {
        float x = (gridPosition.x - gridPosition.y) * tileWidth / 2;
        float y = (gridPosition.x + gridPosition.y) * tileHeight / 2;
        return new Vector2(x, y);
    }

    public static List<Vector2> ExpandGridTilesToWorldPoints(List<Vector2Int> gridPoints)
    {
        List<Vector2> worldpoints = new();
        foreach (Vector2 p in gridPoints)
        {
            worldpoints.Add(new Vector2(p.x, p.y));
            worldpoints.Add(new Vector2(p.x + 1f, p.y));
            worldpoints.Add(new Vector2(p.x + 1f, p.y + 1f));
            worldpoints.Add(new Vector2(p.x, p.y + 1f));
        }
        worldpoints = worldpoints.ConvertAll(p => GridUtils.GridToWorld(new Vector2(p.x, p.y)));
        return worldpoints;
    }

    public static List<Vector2Int> offsetGridPoints(List<Vector2Int> gridPoints, Vector2Int offset)
    {
        List<Vector2Int> offsetPoints = new();
        foreach (Vector2Int p in gridPoints)
        {
            offsetPoints.Add(new Vector2Int(p.x + offset.x, p.y + offset.y));
        }
        return offsetPoints;
    }
}
