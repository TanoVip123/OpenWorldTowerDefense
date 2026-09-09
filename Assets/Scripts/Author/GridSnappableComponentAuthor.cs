using System.Collections.Generic;
using UnityEngine;

public class GridSnappableComponentAuthor : MonoBehaviour, IComponentAuthor
{
    public GridSnappableComponent GridSnappableComponent { get; private set; }

    // Register the GridSnappableComponent with the world. This allows entities to be marked as grid-snappable.
    public void RegisterToWorld(World world, EntityID entityId)
    {
        if (TryGetComponent(out GridSnapper snapper))
        {
            GridSnappableComponent = new GridSnappableComponent(snapper);
            Debug.Log("GridSnapper component attached successfully. GridSnappableComponent initialized.");
            world.AddComponentToEntity<GridSnappableComponent>(entityId, GridSnappableComponent);
        }
        else
        {
            Debug.LogError("GridSnapper component is missing. Please add a GridSnapper component to this GameObject.");
        }

    }
}
