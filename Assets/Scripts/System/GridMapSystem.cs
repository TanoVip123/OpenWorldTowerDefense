using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GridMapSystem : IUpdatableSystem, IGameSystem
{
    private World _world;
    private GridMap _gridMap;
    public void Initialize(World world)
    {
        _world = world;
        _world.AddSystem(this);
        _gridMap = _world.GridMap;
        Debug.Log("GridMapSystem initialized");
    }

    public void Shutdown() => Debug.Log("GridMapSystem shutdown");

    public void Update(float deltaTime)
    {
        IEnumerable<EntityID> entities = _world.GetEntitiesWithComponent<GridOccupancyRequest>();
        foreach (EntityID entityId in entities)
        {
            if (_world.TryGetComponentFromEntity<GridOccupancyComponent>(entityId, out GridOccupancyComponent gridOccupancyComponent))
            {
                // Snap the entity's position to the grid using the GridSnapper component
                _gridMap.addOccupiedTiles(entityId, gridOccupancyComponent.occupiedTiles);
                _world.RemoveComponentFromEntity<GridOccupancyRequest>(entityId);
            }
        }
    }
}

