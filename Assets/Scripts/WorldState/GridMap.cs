using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GridMap
{
    private HashSet<Vector2Int> occupiedTiles;
    private Dictionary<EntityID, List<Vector2Int>> entityOccupiedTiles;

    public void initialize()
    {
        occupiedTiles = new HashSet<Vector2Int>();
        entityOccupiedTiles = new Dictionary<EntityID, List<Vector2Int>>();
    }

    // This method doesn't check for duplicates, it just adds the Tiles to the entity's list. You might want to add checks if you want to avoid duplicates.
    public void addOccupiedTiles(EntityID entityId, List<Vector2Int> Tiles)
    {
        entityOccupiedTiles[entityId] = new List<Vector2Int>(Tiles);
        Tiles.ForEach(Tile => occupiedTiles.Add(Tile));
    }

    public void removeOccupiedTiles(EntityID entityId)
    {
        if (entityOccupiedTiles.TryGetValue(entityId, out List<Vector2Int> Tiles))
        {
            Tiles.ForEach(Tile => occupiedTiles.Remove(Tile));
            entityOccupiedTiles.Remove(entityId);
        }
    }

    public bool isOccupied(List<Vector2Int> Tiles)
    {
        Debug.Log($"Checking occupancy for tiles: {string.Join(", ", Tiles.Select(t => $"({t.x}, {t.y})"))}");
        return Tiles.Any(Tile => occupiedTiles.Contains(Tile));
    }
    // Return a list of booleans indicating whether each tile in the input list is occupied or not.
    public List<bool> getOccupancyStatus(List<Vector2Int> Tiles) => Tiles.Select(Tile => occupiedTiles.Contains(Tile)).ToList();
}
