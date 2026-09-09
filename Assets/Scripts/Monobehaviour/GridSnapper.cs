using UnityEngine;

public class GridSnapper : MonoBehaviour
{
    private PolygonCollider2D _collider2D;
    private Vector2 offset = Vector2.zero;
    public void Start()
    {
        if (!TryGetComponent<PolygonCollider2D>(out _collider2D))
        {
            Debug.Log("GridSnapper doesn't have a PolygonCollider2D component.");
        }
        else
        {
            // Calculate the offset based on the collider's bounds
            offset = _collider2D.bounds.center - transform.localPosition;
        }
    }
    public void SnapToGrid(Vector2 worldPosition)
    {
        Vector2 gridPosition = GridUtils.WorldToGrid(worldPosition - offset);
        Debug.Log("GridSnapper: Snapping to grid position: " + gridPosition + " with offset: " + offset);
        Vector2 snappedPosition = GridUtils.GridToWorld(gridPosition);
        transform.position = new Vector3(snappedPosition.x, snappedPosition.y, transform.position.z);
    }

    public void UpdateOffset()
    {
        if (_collider2D != null)
        {
            offset = _collider2D.bounds.center - transform.localPosition;
            Debug.Log("GridSnapper: Updated offset to: " + offset);
        }
    }
}
