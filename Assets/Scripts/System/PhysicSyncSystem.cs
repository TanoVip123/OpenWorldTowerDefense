using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PhysicSyncSystem : IGameSystem, IFixedUpdatableSystem
{
    private World _world;
    public void Initialize(World world)
    {
        _world = world;
        _world.AddSystem(this);
        Debug.Log("PhysicSyncSystem initialized");
    }

    public void Shutdown() => Debug.Log("PhysicSyncSystem shutdown");
    public void FixedUpdate(float fixedDeltaTime)
    {
        // Only object that is movable (aka have MovementComponent) need to sync with Unity RigidBody
        IEnumerable<EntityID> entities = _world.GetEntitiesWithComponent<MovementComponent>();
        Debug.Log($"PhysicSyncSystem: {entities.Count()} entities");
        foreach (EntityID entityId in entities)
        {
            if (_world.GetEntityObject(entityId, out GameObject entityObject) && entityObject.TryGetComponent(out Rigidbody2D rb) && _world.TryGetComponentFromEntity(entityId, out MovementComponent movementComponent))
            {
                rb.linearVelocity = new Vector2(movementComponent.CurrSpeed.x, movementComponent.CurrSpeed.y);
                Debug.Log($"PhysicSyncSystem: EntityID {entityId} set Rigidbody speed to {rb.linearVelocity} currSpeed is {movementComponent.CurrSpeed}");
            }
        }

        IEnumerable<EntityID> entitiesWithPhysicRequest = _world.GetEntitiesWithComponent<PhysicColliderRequest>();
        foreach (EntityID entityId in entitiesWithPhysicRequest)
        {
            if (_world.GetEntityObject(entityId, out GameObject entityObject) && _world.TryGetComponentFromEntity<PhysicColliderRequest>(entityId, out PhysicColliderRequest physicRequest))
            {
                switch (physicRequest.ShapeType)
                {
                    case PhysicShapeType.Grid:
                        if (_world.TryGetComponentFromEntity<GridOccupancyComponent>(entityId, out GridOccupancyComponent gridOccupancyComponent))
                        {
                            if (!entityObject.TryGetComponent<PolygonCollider2D>(out PolygonCollider2D polygonCollider))
                            {
                                polygonCollider = entityObject.AddComponent<PolygonCollider2D>();
                            }
                            Vector2Int entityGridPosition = Vector2Int.FloorToInt(GridUtils.WorldToGrid(entityObject.transform.position));
                            // Normalize the tile occupancy first because Collider point is relative to GameObject position but the GridOccupancyComponent is absolute grid position. We need to convert the absolute grid position to relative position based on the GameObject position.
                            List<Vector2> expanded = GridUtils.ExpandGridTilesToWorldPoints(gridOccupancyComponent.occupiedTiles.Select(tile => tile - entityGridPosition).ToList());
                            // Debug.Log($"BuildingSystem: GridOccupancyComponent tiles: {string.Join(", ", gridOccupancyComponent.occupiedTiles.Select(tile => $"({tile.x}, {tile.y})"))}");
                            // Debug.Log($"BuildingSystem: expanded point {string.Join(", ", expanded.Select(point => $"({point.x}, {point.y})"))}");
                            List<Vector2> points = MeshUtils.ConvexHull(expanded);
                            polygonCollider.SetPath(0, points);
                            // Debug.Log($"PhysicSyncSystem: EntityID {entityId} set PolygonCollider2D path with{string.Join(", ", points.Select(tile => $"({tile.x}, {tile.y})"))}");
                        }
                        break;
                    case PhysicShapeType.None:
                        break;
                    case PhysicShapeType.Circle:
                        break;
                    case PhysicShapeType.Box:
                        break;
                    case PhysicShapeType.Polygon:
                        break;
                    default:
                        Debug.LogWarning($"PhysicSyncSystem: EntityID {entityId} has unsupported PhysicShapeType {physicRequest.ShapeType}");
                        break;
                }
                _world.RemoveComponentFromEntity<PhysicColliderRequest>(entityId);
            }
            else
            {
                Debug.LogWarning($"PhysicSyncSystem: EntityID {entityId} has PhysicColliderRequest but no GameObject or missing PhysicColliderRequest component");
                _world.RemoveComponentFromEntity<PhysicColliderRequest>(entityId);
            }
        }
    }
}
