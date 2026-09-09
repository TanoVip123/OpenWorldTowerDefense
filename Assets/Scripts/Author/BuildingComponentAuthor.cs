using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BuildingComponentAuthor : MonoBehaviour, IComponentAuthor
{
    public BuildingId buildingId;

    public void RegisterToWorld(World world, EntityID entityId)
    {
        BuildingDefinition buildingDefinition = GameBootstrap.DefinitionDatabase.GetBuildingDefinition(buildingId);
        Vector2 gridPosition = GridUtils.WorldToGrid(gameObject.transform.position);
        List<Vector2Int> occupiedTiles = GridUtils.offsetGridPoints(buildingDefinition.GridOccupancy.ToList(), new Vector2Int(Mathf.FloorToInt(gridPosition.x), Mathf.FloorToInt(gridPosition.y)));
        GridOccupancyComponent gridOccupancyComponent = new(occupiedTiles);

        world.AddComponentToEntity<BuildingComponent>(entityId, new BuildingComponent(buildingId));
        world.AddComponentToEntity<GridOccupancyComponent>(entityId, gridOccupancyComponent);
        world.AddComponentToEntity<GridOccupancyRequest>(entityId, new GridOccupancyRequest());
        world.AddComponentToEntity<PhysicColliderRequest>(entityId, new PhysicColliderRequest { ShapeType = PhysicShapeType.Grid });
    }
}
