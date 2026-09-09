using System;

public static class ComponentRegistry
{
    // This array holds the types of all components in our game.
    // The World.cs script reads this array on startup to efficiently allocate memory for each component type.
    public static readonly Type[] Types =
    {
        typeof(MovementComponent),
        typeof(SelectableComponent),
        typeof(MovementTargetComponent),
        typeof(PathComponent),
        typeof(HealthComponent),
        typeof(GridSnappableComponent),
        typeof(PhysicColliderRequest),
        typeof(GridOccupancyComponent),
        typeof(GridOccupancyRequest),
        typeof(BuildingComponent),
        typeof(StatComponent)
    };
}
