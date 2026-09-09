using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GridSnapSystem : IUpdatableSystem, IGameSystem
{
    private World _world;
    public void Initialize(World world)
    {
        _world = world;
        _world.AddSystem(this);
        Debug.Log("GridSnapSystem initialized");
    }

    public void Shutdown() => Debug.Log("GridSnapSystem shutdown");

    public void Update(float deltaTime)
    {
        // get current mouse position and convert it to grid position
        // GridSnappableComponent is a general snapping utillity, we just happen to snap to mouse in this case. In the future we may want to snap to other things like a grid or a specific point in the world.
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        worldPosition.z = 0f;

        // Debug.Log($"GridSnapSystem: Mouse world position: {worldPosition}");
        IEnumerable<EntityID> entities = _world.GetEntitiesWithComponent<GridSnappableComponent>();
        foreach (EntityID entityId in entities)
        {
            if (_world.TryGetComponentFromEntity<GridSnappableComponent>(entityId, out GridSnappableComponent gridSnappableComponent))
            {
                // Snap the entity's position to the grid using the GridSnapper component
                gridSnappableComponent.Snapper.SnapToGrid(worldPosition);
            }
        }
    }
}

