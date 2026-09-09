using UnityEngine;

public struct BuildingComponent : IComponent
{
    public BuildingId buildingId { get; set; }

    public BuildingComponent(BuildingId buildingId) => this.buildingId = buildingId;
}
