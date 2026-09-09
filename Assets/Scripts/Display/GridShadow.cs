using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
[RequireComponent(typeof(PolygonCollider2D))]
public class GridShadow : MonoBehaviour
{
    public GameObject shadowTile;
    private GameObject shadowObject;

    public int GridSize = 6;

    public List<Vector2Int> _activeTiles;

    private SpriteRenderer[,] _sprites;

    private PolygonCollider2D _collider;

    public Color _tileFreeColor = new(0.6f, 1f, 0.6f, 0.5f); // Light green
    public Color _tileBlockedColor = new(1f, 0.6f, 0.6f, 0.5f); // White with 10% opacity

    public void Awake()
    {
        shadowObject = null;
        _collider = GetComponent<PolygonCollider2D>();
        _sprites = new SpriteRenderer[GridSize, GridSize];
        CreateGridShadows();
        ClearActivePoints();
    }

    public void Update() => updateTileColor();

    private void updateTileColor()
    {
        Vector2Int currentGridPosition = Vector2Int.FloorToInt(GridUtils.WorldToGrid(transform.position));
        // active tiles are relative to currentGridPosition so we need to offset them to absolute
        List<bool> occupancyStatus = GameBootstrap.World.GridMap.getOccupancyStatus(_activeTiles.Select(tile => currentGridPosition + tile).ToList());
        for (int i = 0; i < occupancyStatus.Count; i++)
        {
            Vector2Int point = _activeTiles[i];
            if (!occupancyStatus[i])
            {
                _sprites[point.x, point.y].color = _tileFreeColor;
            }
            else
            {
                _sprites[point.x, point.y].color = _tileBlockedColor;
            }
        }
    }
    //public void update Check with the overal grid and the collider to see if there is anything blocking the grid and update the color of the shadows accordingly
    private void CreateGridShadows()
    {
        Vector2 currPos = new(transform.position.x, transform.position.y);
        for (int x = 0; x < GridSize; x++)
        {
            for (int y = 0; y < GridSize; y++)
            {
                Vector2 position = GridUtils.GridToWorld(new Vector2(x, y));
                GameObject shadow = Instantiate(shadowTile, currPos + position, Quaternion.identity, transform);
                shadow.name = $"Shadow_{x}_{y}";
                _sprites[x, y] = shadow.GetComponent<SpriteRenderer>();
                shadow.SetActive(false);
            }
        }
    }

    private void UpdateCollider()
    {
        Debug.Log("GridShadow: Creating PolygonCollider2D with active points: " + string.Join(", ", _activeTiles));
        List<Vector2> expanded = GridUtils.ExpandGridTilesToWorldPoints(_activeTiles);
        Debug.Log("GridShadow: Expanded points for collider: " + string.Join(", ", expanded));
        List<Vector2> points = MeshUtils.ConvexHull(expanded);
        Debug.Log("GridShadow: Creating PolygonCollider2D with points: " + string.Join(", ", points));
        _collider.SetPath(0, points);
        if (TryGetComponent<GridSnapper>(out GridSnapper snapper))
        {
            snapper.UpdateOffset();
        }
    }

    private void UpdateActivePoints()
    {
        // Load the active points from the serialized field
        foreach (Vector2Int point in _activeTiles)
        {
            if (point.x < 0 || point.x >= GridSize || point.y < 0 || point.y >= GridSize)
            {
                Debug.LogWarning($"Active point {point} is out of bounds for grid size {GridSize}. Skipping.");
                continue;
            }
            _sprites[point.x, point.y].gameObject.SetActive(true); // Activate the shadow for the active point
            _sprites[point.x, point.y].color = _tileFreeColor; // Set the color to indicate that the tile is free by default
        }
    }

    private void ClearActivePoints()
    {
        foreach (Vector2Int point in _activeTiles)
        {
            if (point.x < 0 || point.x >= GridSize || point.y < 0 || point.y >= GridSize)
            {
                Debug.LogWarning($"Active point {point} is out of bounds for grid size {GridSize}. Skipping.");
                continue;
            }
            _sprites[point.x, point.y].gameObject.SetActive(false); // Deactivate the shadow for the active point
        }
    }

    public void setShadowObject(GameObject shadowPrefab, Vector2Int[] gridOccupancy)
    {
        ClearShadowObject();
        _activeTiles = new List<Vector2Int>(gridOccupancy);
        shadowObject = Instantiate(shadowPrefab, transform);
        UpdateActivePoints();
        UpdateCollider();
    }

    public void ClearShadowObject()
    {
        if (shadowObject != null)
        {
            Destroy(shadowObject);
            shadowObject = null;
        }
        ClearActivePoints();
        UpdateCollider();
    }
}
